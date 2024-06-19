using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public class Entity(ScoreRef id) : IRuntimeArgument
    {
        private static readonly Stack<Entity> AsStack = [];
        private static readonly Stack<Entity> AtStack = [];

        public readonly ScoreRef ID = id;

        public ScoreRef GetAsArg() => ID;

        public void As(Action func, bool at = true)
        {
            var proj = Project.ActiveProject;
            var cmd = As(new Execute()).Run(proj.Lambda(() =>
            {
                AsStack.Push(this);
                if (at) AtStack.Push(this);

                func();

                AsStack.Pop();
                if (at) AtStack.Pop();
            }));
            if (at) cmd.At(new TargetSelector(TargetType.s));
            proj.AddCommand(cmd);
        }

        public void Teleport(Position pos) => Project.ActiveProject.AddCommand(As(new Execute().Run(new TeleportCommand(pos))));

        public void Teleport(Entity dest) => Project.ActiveProject.AddCommand(As(dest.As(new Execute().Run(new TeleportCommand(new Position(new(0, CoordType.Relative), new(0, CoordType.Relative), new(0, CoordType.Relative))))).At(new TargetSelector(TargetType.s)), true));

        /// <summary>
        /// Sets the execute's target to the entity. If ran inside <see cref="As(Action, bool)"/>
        /// then the current entity will be remembered and <c>@s</c> will be used.
        /// Setting <c>force</c> to true will always recalculate the current entity.
        /// </summary>
        /// <param name="cmd">Execute command</param>
        /// <param name="force">Force recalculation</param>
        /// <returns></returns>
        public Execute As(Execute cmd, bool force = false)
        {
            if (force || AsStack.Count == 0 || (AsStack.TryPeek(out var cur) && cur != this)) return cmd.As(new TargetSelector(TargetType.e)).If.Score(new TargetSelector(TargetType.s), Project.EntityIDScore, Comparison.Equal, ID.Target, ID.Score);
            return cmd.As(new TargetSelector(TargetType.s));
        }
    }
}
