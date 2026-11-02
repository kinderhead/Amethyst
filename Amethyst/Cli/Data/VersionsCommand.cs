using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Spectre.Console.Cli;

namespace Amethyst.Cli.Data
{
    public class VersionsCommand : Command<BaseDataSettings>
    {
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        public override int Execute(CommandContext context, BaseDataSettings settings, CancellationToken cancellationToken)
        {
            var data = JsonConvert.SerializeObject(SupportedVersions.Versions);
            if (settings.Output is { } file) File.WriteAllText(file, data);
            else Console.WriteLine(data);
            return 0;
        }
    }
}