using Amethyst.Daemon;
using Geode;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Amethyst.Cli
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    public class BuildOptions : CommandSettings, IOptions
    {
        [CommandOption("-o|--output")]
        [Description("Zipped datapack, defaults to first input file's name.")]
        public required string Output { get; set; }

        [CommandOption("-d|--debug")]
        [Description("Enable debug checks.")]
        public bool Debug { get; set; }

        [CommandOption("--dump-ir")]
        [Description("Dump Geode IR and don't compile to datapack.")]
        public bool DumpIR { get; set; }

        [CommandOption("-O")]
        [Description("Set the opimization level.")]
        [DefaultValue(1)]
        public int OptimizationLevel { get; set; }

        [CommandOption("-c|--dump-cmd")]
        [Description("Dump non-std functions.")]
        public bool DumpCommands { get; set; }

        [CommandOption("--run")]
        [Description("Run the datapack if built successfully.")]
        public bool Run { get; set; }

        [CommandArgument(0, "<inputs>")]
        [Description("Files to compile.")]
        public required string[] Inputs { get; set; }
	}
    
    public class BuildCommand : Command<BuildOptions>
	{
		public override int Execute(CommandContext context, BuildOptions settings)
        {
            AnsiConsole.MarkupLine("[yellow]Amethyst is currently in development. Report issues at [aqua underline]https://github.com/kinderhead/Amethyst/issues[/].[/]\n");
            
            settings.Output ??= Path.GetFileName(settings.Inputs[0]) + ".zip";
            var compiler = new Compiler(settings);

            if (!compiler.Compile())
            {
                return 1;
            }

            if (settings.Run)
            {
                Runner.RunDatapack(new DaemonRunOptions() { Datapack = settings.Output }, compiler);
            }

            return 0;
        }
	}
}
