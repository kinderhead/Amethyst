using Amethyst.Cli;
using Datapack.Net.Pack;
using Datapack.Net.Reader;
using Geode;
using Geode.Errors;
using Spectre.Console;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Tmds.Utils;

namespace Amethyst.Daemon
{
    public static partial class Runner
    {
        public static int RunDatapack(DaemonRunOptions settings, IFileHandler handler)
        {
            if (Server.GetConfig() is null)
            {
                return 1;
            }

            if (!Path.Exists(settings.Datapack))
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Could not find datapack \"{settings.Datapack}\"[/]");
                return 1;
            }

            // Bad windows :(
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Rcon.StopServer();
            }

            Server.RemoveDatapack();

            if (!Rcon.IsServerRunning())
            {
                if (!AnsiConsole.Status().Spinner(Spinner.Known.BouncingBar).SpinnerStyle(Style.Parse("gold1")).StartAsync("[gold1]Starting Minecraft server[/]", async ctx =>
                {
                    if (Environment.ProcessPath is not string path)
                    {
                        AnsiConsole.MarkupLine($"[red]Error spawning server daemon process[/]");
                        return false;
                    }

					if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					{
						var thread = new Thread(() => Server.StartServer(null))
						{
							IsBackground = true
						};

						thread.Start();

						AppDomain.CurrentDomain.ProcessExit += (s, e) => Rcon.StopServer();
					}
					else
					{
						ExecFunction.Start(() => Server.StartServer(null, timeout: true));
					}

                    int delay = 100; // Wait for 15 seconds
                    for (int i = 0; i < 15000 / delay; i++)
                    {
                        if (Rcon.IsServerRunning())
                        {
                            return true;
                        }

                        await Task.Delay(delay);
                    }

                    AnsiConsole.MarkupLine($"[red]Error spawning server daemon process[/]");
                    return false;
                }).GetAwaiter().GetResult())
                {
                    return 1;
                }
            }

            Directory.CreateDirectory(Path.Combine(Server.ServerFolder, "world", "datapacks"));

            if (File.Exists(settings.Datapack))
            {
                if (Path.GetExtension(settings.Datapack) != ".zip")
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]Invalid datapack \"{settings.Datapack}\"[/]");
                    return 1;
                }

                File.Copy(settings.Datapack, Server.DatapackLocation + ".zip", true);
            }
            else
            {
                foreach (var dir in Directory.GetDirectories(settings.Datapack, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dir.Replace(settings.Datapack, Server.DatapackLocation));
                }

                foreach (var file in Directory.GetFiles(settings.Datapack, "*", SearchOption.AllDirectories))
                {
                    File.Copy(file, file.Replace(settings.Datapack, Server.DatapackLocation));
                }
            }

            using var log = new FileStream(Server.LogLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            log.Seek(0, SeekOrigin.End);

            using var stream = new StreamReader(log);

            using var rcon = new Rcon("localhost", Rcon.GetPort());
            rcon.Login(Rcon.Password);
            rcon.SendCommand("reload");

            string lastFailedFunction = "";

            var datapack = new DatapackReaderHandler(settings.Datapack);

            while (true)
            {
                if (stream.ReadLine() is string msg)
                {
                    var logMatch = TellrawLoggerLookup().Match(msg);
                    if (logMatch.Success)
                    {
                        msg = logMatch.Groups[1].Value;
                        if (msg == "[exit]")
                        {
                            break;
                        }

                        AnsiConsole.MarkupLine(msg.EscapeMarkup());
                        continue;
                    }

                    var errorMatch = GenericErrorLookup().Match(msg);
                    if (errorMatch.Success)
                    {
                        var failedFunctionMatch = FailedFunctionLookup().Match(errorMatch.Groups[1].Value);
                        if (failedFunctionMatch.Success)
                        {
                            lastFailedFunction = failedFunctionMatch.Groups[1].Value;
                            continue;
                        }

                        AnsiConsole.MarkupLineInterpolated($"[red]Error: {errorMatch.Groups[1].Value}[/]");
                        continue;
                    }

                    var parseErrorMatch = ParseErrorLookup().Match(msg);
                    if (parseErrorMatch.Success)
                    {
                        try
                        {
                            var line = int.Parse(parseErrorMatch.Groups[1].Value);
                            var column = int.Parse(parseErrorMatch.Groups[2].Value) + 1;
                            var file = DatapackReader.PathFor<Functions>(lastFailedFunction);
                            var srcLoc = new LocationRange(new(file, line, column));

                            var mcfunction = datapack.Reader.ReadFile(file).Split('\n');
                            LocationRange? mapLoc = null;

                            for (int i = 0; i < line; i++)
                            {
                                if (mcfunction[i].StartsWith('#'))
                                {
                                    mapLoc = LocationRange.From(mcfunction[i][1..], handler);
                                }
                            }

                            if (mapLoc is null)
                            {
                                new CompilerMessager(Color.Red)
                                    .Header($"Error loading [yellow]{lastFailedFunction}[/] ([grey underline]{srcLoc}[/])")
                                    .AddCode(datapack, srcLoc, true)
                                    .AddContent("Syntax error: [turquoise2]Could not parse command[/]");
                            }
                            else
                            {
                                new CompilerMessager(Color.Red)
                                    .Header($"Error loading [yellow]{lastFailedFunction}[/] ([grey underline]{srcLoc}[/])")
                                    .AddCode(datapack, srcLoc)
                                    .AddContent("Syntax error: [turquoise2]Could not parse command[/]")
                                    .AddContent("")
                                    .AddContent($"[green]Source at [underline]{mapLoc.Value}[/][/]")
                                    .AddCode(handler, mapLoc.Value, true);
                            }
                        }
                        catch (Exception e)
                        {
                            AnsiConsole.MarkupLineInterpolated($"[orange1]Error getting sources for {lastFailedFunction}: {e.Message}[/]");
                            throw;
                        }
                    }
                }
            }

            return 0;
        }

		[GeneratedRegex(@"(?:\[\d+:\d+:\d+\] \[Server thread/INFO\] \(TellrawLogger\) )([^\n]*)")]
		private static partial Regex TellrawLoggerLookup();

		[GeneratedRegex(@"(?:\[\d+:\d+:\d+\] \[Server thread/ERROR\] \(Minecraft\) )([^\n]*)")]
		private static partial Regex GenericErrorLookup();

        [GeneratedRegex(@"java\.util\.concurrent\.CompletionException: java\.lang\.IllegalArgumentException:.*?line (\d+).*?position (\d+)")]
        private static partial Regex ParseErrorLookup();

        [GeneratedRegex(@"Failed to load function (.*)")]
        private static partial Regex FailedFunctionLookup();
    }
}
