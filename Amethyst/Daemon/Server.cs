using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Amethyst.Cli;
using Datapack.Net.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Tmds.Utils;

namespace Amethyst.Daemon
{
    public static class Server
    {
        public static async Task<int> Setup(DaemonSetupOptions settings)
        {
            if (!settings.EULA)
            {
                AnsiConsole.MarkupLine("[red]Accept the Minecraft EULA by adding --eula[/]");
                return 1;
            }

            if (settings.Timeout <= 0)
            {
                AnsiConsole.MarkupLine("[red]Timeout must be positive[/]");
                return 1;
            }

            Rcon.StopServer();

            var config = UpdateConfig(settings);

            File.Delete(EULALocation);

            if (!await GetMinecraftServer(settings.MinecraftVersion))
            {
                return 1;
            }

            var res = AnsiConsole.Status().Spinner(Spinner.Known.BouncingBar).SpinnerStyle(Style.Parse("gold1")).Start("[gold1]Setting up Minecraft server[/]", ctx =>
            {
                if (!StartServer(config))
                {
                    return 1;
                }

                ctx.Status("Updating server.properties");

                File.WriteAllText(EULALocation, "eula=true");

                UpdateServerProperties(new Dictionary<string, string>
                {
                    {"server-port", settings.Port.ToString()},
                    {"query.port", settings.Port.ToString()},
                    {"rcon.port", (settings.Port + 1).ToString()},
                    {"rcon.password", Rcon.Password},
                    {"enable-rcon", "true"},
                    {"broadcast-rcon-to-ops", "false"},
                    {"spawn-protection", "0"},
                    {"pause-when-empty-seconds", "-1"},
                    {"level-type", "minecraft\\:flat"}
                });

                return 0;
            });

            if (res == 0)
            {
                AnsiConsole.MarkupLine("[green]Complete[/]");
            }

            return res;
        }

        public static bool StartServer(Config? config = null, bool watchOutput = false, bool timeout = false)
        {
            config ??= GetConfig();

            if (config is null)
            {
                return false;
            }

            if (Rcon.IsServerRunning())
            {
                AnsiConsole.MarkupLine("[red]Minecraft server is already running[/]");
                return false;
            }

            if (!CheckJava(config))
            {
                return false;
            }

            RemoveDatapack();

            var proc = Run(config.Java, $"-Xmx{config.Memory} -jar server.jar nogui");

            // A bit silly but it works
            new Task(async () =>
            {
                var lastActiveTime = DateTime.Now;
                bool finished = false;

				// Not doing this on Windows causes a freeze for some reason.
				string? line;
				Task outputTask = Task.Run(async () =>
				{
					while ((line = await proc.StandardOutput.ReadLineAsync()) is not null)
					{
						if (line.Contains(MARKER_TEXT))
						{
							lastActiveTime = DateTime.Now;
						}

						if (watchOutput)
						{
							AnsiConsole.MarkupLine(line.EscapeMarkup());
						}
					}

					finished = true;
				});

                if (timeout)
                {
                    var timespan = TimeSpan.FromMinutes(config.Timeout);

                    while (lastActiveTime + timespan > DateTime.Now && !finished)
                    {
                        await Task.Delay(100);
                    }

                    Rcon.StopServer();
                }

                if (outputTask is not null)
                {
                    await outputTask;
                }
            }).RunSynchronously();

            proc.WaitForExit();

            return proc.ExitCode == 0;
        }

        public static Config? GetConfig()
        {
            if (!File.Exists(ConfigLocation) || !File.Exists(MinecraftServerLocation))
            {
                AnsiConsole.MarkupLine("[red]Minecraft server is not set up yet. Run \"amethyst setup\" to get started[/]");
                return null;
            }

            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigLocation));
        }

        public static Config UpdateConfig(DaemonSetupOptions baseSettings)
        {
            Directory.CreateDirectory(ServerFolder);

            if (Path.Exists(ConfigLocation))
            {
                AnsiConsole.MarkupLine("[yellow]Overriding existing server configuration[/]");
            }

            var config = new Config
            {
                AmethystVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(),
                Timeout = baseSettings.Timeout,
                Memory = baseSettings.Memory,
                Java = baseSettings.Java
            };

            File.WriteAllText(ConfigLocation, JsonConvert.SerializeObject(config, Formatting.Indented));

            return config;
        }

        public static void UpdateServerProperties(IDictionary<string, string> props)
        {
            var data = File.ReadAllLines(ServerPropertiesLocation);

            for (int i = 0; i < data.Length; i++)
            {
                foreach (var (k, v) in props)
                {
                    if (data[i].StartsWith(k))
                    {
                        data[i] = $"{k}={v}";
                    }
                }
            }

            File.WriteAllLines(ServerPropertiesLocation, data);
        }

        public static async Task<bool> GetMinecraftServer(string version)
        {
            Directory.CreateDirectory(Path.Combine(ServerFolder, "mods"));

			var (success, tellraw) = await GetTellrawLoggerVersion(version);

			if (!success)
			{
				return false; 
			}

            if (!DownloadFiles(
                ($"Minecraft {version}", new($"https://meta.fabricmc.net/v2/versions/loader/{version}/0.17.3/1.1.0/server/jar"), MinecraftServerLocation),
                ($"Tellraw Logger", new(tellraw), Path.Combine(ServerFolder, "mods", "tellraw-logger.jar")),
                ($"Better Log4j Config", new("https://github.com/BigWingBeat/better_log4j_config/releases/download/1.2.0/better_log4j_config-1.2.0-fabric.jar"), Path.Combine(ServerFolder, "mods", "better-log4j-config.jar"))
            ))
            {
                return false;
            }

            return true;
        }

		public static async Task<(bool, string)> GetTellrawLoggerVersion(string mcversion)
		{
			using var client = new HttpClient();
			var versions = JArray.Parse(await client.GetStringAsync($"https://api.modrinth.com/v2/project/tellraw-logger/version?game_versions=[\"{mcversion}\"]"));
			
			if (versions.Count == 0)
			{
				AnsiConsole.MarkupLineInterpolated($"[red]Unsupported Daemon Minecraft version {mcversion}[/]");
				return (false, "");
			}

			return (true, versions[0]["files"]?[0]?["url"]?.ToString() ?? "");
		}

        public static bool DownloadFiles(params (string name, Uri url, string path)[] files)
        {
            bool success = true;

            AnsiConsole.Progress().Columns
            (
                new TaskDescriptionColumn { Alignment = Justify.Left },
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn(),
                new DownloadedColumn(),
                new TransferSpeedColumn()
            ).StartAsync(async ctx =>
            {
                // https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient
                foreach (var (name, url, path) in files)
                {
                    try
                    {
                        var task = ctx.AddTask($"Downloading {name}", autoStart: false).IsIndeterminate();
                        ctx.Refresh();

                        using var client = new HttpClient();
                        using var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                        using var res = await client.GetAsync(url).ConfigureAwait(false);

                        var length = res.Content.Headers.ContentLength ?? 0;

                        if (length <= 0)
                        {
                            throw new Exception($"Invalid content length {length}");
                        }
                        
                        task.MaxValue = length;
                        task.IsIndeterminate(false);
                        task.StartTask();
                        ctx.Refresh();

                        using var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);

                        var buffer = new byte[16384];
                        long total = 0;
                        int read;

                        while ((read = await stream.ReadAsync(buffer).ConfigureAwait(false)) != 0)
                        {
                            await file.WriteAsync(buffer.AsMemory(0, read)).ConfigureAwait(false);
                            total += read;
                            task.Increment(read);
                            ctx.Refresh();
                        }

                        // Just in case
                        task.Value = task.MaxValue;
                    }
                    catch (Exception e)
                    {
                        AnsiConsole.MarkupLineInterpolated($"[red]Error downloading file {url}: {e.Message}[/]");
                        success = false;
                        return;
                    }
                }
            }).Wait();

            return success;
        }

        public static bool CheckJava(Config config)
        {
            try
            {
                var proc = Run(config.Java, "--version");
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new Exception();
                }

                return true;
            }
            catch (Exception)
            {
                AnsiConsole.MarkupLine("[red]Could not find Java. Make sure that the correct version of Java for the Minecraft version is selected[/]");
                return false;
            }
        }

        public static void RemoveDatapack()
        {
            if (File.Exists(DatapackLocation + ".zip"))
            {
                File.Delete(DatapackLocation + ".zip");
            }

            if (Directory.Exists(DatapackLocation))
            {
                Directory.Delete(DatapackLocation, true);
            }
        }

        public static Process Run(string cmd, string args, bool detach = false)
        {
            var proc = new Process();
            proc.StartInfo.FileName = cmd;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WorkingDirectory = ServerFolder;

            proc.Start();

            if (!detach)
            {
                Console.CancelKeyPress += (sender, args) =>
                {
                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }

                    args.Cancel = true;
                };
            }

            return proc;
        }

        public const string MARKER_TEXT = "[Server thread/INFO] (TellrawLogger) ";

        public static readonly string ServerFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Amethyst", "server");
        public static readonly string ConfigLocation = Path.Combine(ServerFolder, "config.json");
        public static readonly string EULALocation = Path.Combine(ServerFolder, "eula.txt");
        public static readonly string ServerPropertiesLocation = Path.Combine(ServerFolder, "server.properties");
        public static readonly string MinecraftServerLocation = Path.Combine(ServerFolder, "server.jar");
        public static readonly string DatapackLocation = Path.Combine(ServerFolder, "world", "datapacks", "datapack");
        public static readonly string LogLocation = Path.Combine(ServerFolder, "logs", "latest.log");
    }
}
