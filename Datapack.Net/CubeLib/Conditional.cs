using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
	public abstract class Conditional
	{
		/// <summary>
		/// Execute operation, false for unless.
		/// </summary>
		public bool If = true;

		public abstract Execute Process(Execute cmd, int tmp = 0);

		public static Conditional operator !(Conditional op)
		{
			var inverse = (Conditional)op.MemberwiseClone();
			inverse.If = !inverse.If;
			return inverse;
		}

		public static bool operator false(Conditional _) => false;
		public static bool operator true(Conditional _) => true;

		public static ConditionalWithModifiers operator |(Conditional a, ConditionalModifier b)
		{
			var ret = new ConditionalWithModifiers(a);
			ret.PostModifiers.Add(b);
			return ret;
		}

		public static ConditionalWithModifiers operator |(ConditionalModifier a, Conditional b)
		{
			var ret = new ConditionalWithModifiers(b);
			ret.PreModifiers.Add(a);
			return ret;
		}
	}

	public abstract class ConditionalModifier
	{
		public abstract Execute Process(Execute cmd);
	}

	public class GenericConditionalModifier(Action<Execute> func) : ConditionalModifier
	{
		public readonly Action<Execute> Func = func;

		public override Execute Process(Execute cmd)
		{
			Func(cmd);
			return cmd;
		}
	}

	public class ConditionalWithModifiers(Conditional cond) : Conditional
	{
		public readonly Conditional Base = cond;
		public readonly List<ConditionalModifier> PostModifiers = [];
		public readonly List<ConditionalModifier> PreModifiers = [];

		public override Execute Process(Execute cmd, int tmp = 0)
		{
			foreach (var i in PreModifiers)
			{
				_ = i.Process(cmd);
			}

			_ = Base.Process(cmd, tmp);
			foreach (var i in PostModifiers)
			{
				_ = i.Process(cmd);
			}

			return cmd;
		}

		public static ConditionalWithModifiers operator |(ConditionalWithModifiers a, ConditionalModifier b)
		{
			a.PostModifiers.Add(b);
			return a;
		}

		public static ConditionalWithModifiers operator |(ConditionalModifier a, ConditionalWithModifiers b)
		{
			b.PreModifiers.Add(a);
			return b;
		}
	}
}
