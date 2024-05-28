using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public class IfHandler
    {
        public readonly Project Project;
        public readonly List<ScoreRefComparison> Comparisons = [];

        public IfHandler(Project project, ScoreRefComparison comp, Action func)
        {
            Project = project;
            Comparisons.Add(comp);

            var cmd = comp.Process(new Execute());
            Project.AddCommand(cmd.Run(new FunctionCommand(Project.Lambda(func))));
        }

        public IfHandler ElseIf(ScoreRefComparison comp, Action func)
        {
            Else(() =>
            {
                var cmd = comp.Process(new Execute());
                Project.AddCommand(cmd.Run(new FunctionCommand(Project.Lambda(func))));
            });
            Comparisons.Add(comp);
            return this;
        }

        public void Else(Action func)
        {
            var cmd = new Execute();
            cmd = (!Comparisons.Last()).Process(cmd);
            Project.AddCommand(cmd.Run(new FunctionCommand(Project.Lambda(func))));
        }
    }
}
