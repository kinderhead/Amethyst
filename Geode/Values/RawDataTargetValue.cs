using Datapack.Net.Function;

namespace Geode.Values
{
	public class RawDataTargetValue(string target, TypeSpecifier type) : DataTargetValue
	{
		public readonly string RawTarget = target;
		public override IDataTarget Target => new RawDataTarget(RawTarget);

		public override TypeSpecifier Type => type;

		public override bool Equals(object? obj) => obj is RawDataTargetValue r && r.RawTarget == RawTarget;
		public override int GetHashCode() => RawTarget.GetHashCode() * 17;
		public override DataTargetValue Index(int index, TypeSpecifier type) => new RawDataTargetValue($"{RawTarget}[{index}]", type);
		public override DataTargetValue Property(string member, TypeSpecifier type) => new RawDataTargetValue($"{RawTarget}.{member}", type);
	}
}
