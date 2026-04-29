using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Amethyst.Cli
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    public class ProjectInitCommandOptions : CommandSettings
    {
        [CommandOption("-n|--name")]
        [Description("Name of the project.")]
        public FlagValue<string> Name { get; set; }

        [CommandOption("-d|--description")]
        [Description("Project description.")]
        public string Description { get; set; }

        [CommandOption("-o|--output")]
        [Description("Folder to generate the project.")]
        [DefaultValue(".")]
        public string Output { get; set; }
    }

    public class ProjectInitCommand
    {
        
    }
}
