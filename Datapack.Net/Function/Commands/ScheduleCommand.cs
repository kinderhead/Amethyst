namespace Datapack.Net.Function.Commands
{
	public class ScheduleCommand : Command
	{
		public readonly MCFunction Function;
		public bool Clear;
		public int Ticks;
		public bool Replace;

		/// <summary>
		/// <c><![CDATA[schedule function <function> <time> [append|replace]]]></c>
		/// </summary>
		/// <param name="function"></param>
		/// <param name="ticks"></param>
		/// <param name="replace"></param>
		/// <param name="macro"></param>
		public ScheduleCommand(MCFunction function, int ticks, bool replace = true, bool macro = false) : base(macro)
		{
			Function = function;
			Ticks = ticks;
			Replace = replace;
		}

		/// <summary>
		/// <c><![CDATA[schedule clear <function>]]></c>
		/// </summary>
		/// <param name="function"></param>
		/// <param name="macro"></param>
		public ScheduleCommand(MCFunction function, bool macro = false) : base(macro)
		{
			Function = function;
			Clear = true;
		}

		protected override string PreBuild()
		{
			if (Clear)
			{
				return $"schedule clear {Function.ID}";
			}

			return $"schedule function {Function.ID} {Ticks} {(Replace ? "replace" : "append")}";
		}
	}
}
