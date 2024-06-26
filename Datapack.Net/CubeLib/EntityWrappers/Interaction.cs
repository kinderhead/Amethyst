using System;
using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Data;
using Datapack.Net.Data._1_20_4;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib.EntityWrappers
{
    public class Interaction(ScoreRef id) : EntityWrapper(id)
    {
        public FloatEntityProperty Width { get => GetAs<FloatEntityProperty>("width"); set => value.Set(this, "width"); }
        public FloatEntityProperty Height { get => GetAs<FloatEntityProperty>("height"); set => value.Set(this, "height"); }
        public BoolEntityProperty Response { get => GetAs<BoolEntityProperty>("response"); set => value.Set(this, "response"); }

        public override EntityType Type => Entities.Interaction;

        private Entity? _attacker = null;
        public Entity Attacker
        {
            get
            {
                if (_attacker is not null) return _attacker;
                var id = State.Local(-1);
                State.AddCommand(As(new Execute()).On(OnRelation.Attacker).Store(id).Run(new FunctionCommand(State.Std.GetEntityID_Function())));
                //State.If(id == -1, new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Text("Interaction attacker is null", new() { Color = Color.RED })));
                _attacker = new(id);
                return _attacker;
            }
        }

        private Entity? _interactor = null;
        public Entity Interactor
        {
            get
            {
                if (_interactor is not null) return _interactor;
                var id = State.Local(-1);
                State.AddCommand(As(new Execute()).On(OnRelation.Target).Store(id).Run(new FunctionCommand(State.Std.GetEntityID_Function())));
                //State.If(id == -1, new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Text("Interaction attacker is null", new() { Color = Color.RED })));
                _interactor = new(id);
                return _interactor;
            }
        }

        public void ClearAttacker() => RemoveNBT("attack");
        public void ClearInteractor() => RemoveNBT("interaction");

        public override void PlayerCheck()
        {

        }
    }
}
