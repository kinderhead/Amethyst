using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class ReturnCommand : Command
    {
        public readonly int? Value;
        public readonly bool Fail = false;
        public readonly Command? Cmd;

        public ReturnCommand(int value, bool macro = false) : base(macro)
        {
            Value = value;
        }

        /// <summary>
        /// By default it fails the function.
        /// </summary>
        /// <param name="macro"></param>
        public ReturnCommand(bool macro = false) : base(macro)
        {
            Fail = true;
        }

        public ReturnCommand(Command cmd, bool macro = false) : base(macro)
        {
            Cmd = cmd;
            if (Cmd.Macro) Macro = true;
        }

        protected override string PreBuild()
        {
            if (Fail) return "return fail";
            if (Value is not null) return $"return {Value}";
            if (Cmd is not null) return $"return run {Cmd.Build()}";
            throw new ArgumentException("Invalid return command");
        }
    }
}
