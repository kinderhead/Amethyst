using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public class EntityComparison(Entity entity, TargetSelector comparison) : Conditional
    {
        public readonly Entity Entity = entity;
        public readonly TargetSelector Comparison = comparison;

        public override Execute Process(Execute cmd, int tmp = 0)
        {
            Execute.Conditional branch = If ? cmd.If : cmd.Unless;

            if (Entity.IsCurrentTarget()) branch.Entity(Comparison);
            else
            {
                var cur = Project.ActiveProject.EntityRef(TargetSelector.Self);
                Entity.As(cmd);
                branch.Entity(Comparison);
                cur.As(cmd);
            }

            return cmd;
        }
    }
}
