using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Geode
{
	public record class RenderContext(MCFunction MCFunction, IR.Block Block, GeodeBuilder Builder, FunctionContext Func)
	{
		public ScoreValue SuccessScore => Builder.Score("func_success");

		public virtual void Add(params IEnumerable<Command> cmds)
		{
			if (Func.IsMacroFunction)
			{
				MCFunction.Add(cmds.Select(i =>
				{
					i.Macro = true;
					return i;
				}));
			}
			else
			{
				MCFunction.Add(cmds);
			}
		}

		public void StoreCompound(DataTargetValue dest, Dictionary<string, ValueRef> dict, bool setEmpty = true)
		{
			var ret = StoreCompoundOrReturnConstant(dest, dict, setEmpty);
			if (ret is LiteralValue l)
			{
				dest.Store(l, this);
			}
		}

		public Value StoreCompoundOrReturnConstant(DataTargetValue dest, Dictionary<string, ValueRef> dict, bool setEmpty = true)
		{
			var nbt = new NBTCompound();
			var runtime = new Dictionary<string, Value>();
			var forceUseStorage = false;

			foreach (var (key, val) in dict.Select(i => (i.Key, i.Value.Expect())))
			{
				if (val is VoidValue)
				{
					forceUseStorage = true;
				}
				else if (val is MacroValue m && m.Type.EffectiveType == NBTType.String)
				{
					nbt[key] = new NBTString(m.GetMacro());
				}
				else if (val is IConstantValue l)
				{
					nbt[key] = l.Value;
				}
				else
				{
					runtime[key] = val;
				}
			}

			if (!forceUseStorage && nbt.Count == dict.Count)
			{
				return new LiteralValue(nbt);
			}
			else if (setEmpty || nbt.Count != 0)
			{
				dest.Store(new LiteralValue(nbt), this);
			}

			foreach (var (key, value) in runtime)
			{
				dest.Property(key, value.Type).Store(value, this);
			}

			return dest;
		}

		public void StoreList(DataTargetValue dest, List<ValueRef> list, bool setEmpty = true)
		{
			var ret = StoreListOrReturnConstant(dest, list, setEmpty);
			if (ret is LiteralValue l)
			{
				dest.Store(l, this);
			}
		}

		public Value StoreListOrReturnConstant(DataTargetValue dest, List<ValueRef> list, bool setEmpty = true)
		{
			var nbt = new NBTList();
			var runtime = new List<Value>();
			var isStillConstant = true;

			foreach (var i in list.Select(i => i.Expect()))
			{
				if (isStillConstant && i is MacroValue m && m.Type.EffectiveType == NBTType.String)
				{
					nbt.Add(new NBTString(m.GetMacro()));
				}
				else if (isStillConstant && i is IConstantValue l)
				{
					nbt.Add(l.Value);
				}
				else
				{
					isStillConstant = false;
					runtime.Add(i);
				}
			}

			if (nbt.Count == list.Count)
			{
				return new LiteralValue(nbt);
			}
			else if (setEmpty || nbt.Count != 0)
			{
				dest.Store(new LiteralValue(nbt), this);
			}

			foreach (var item in runtime)
			{
				dest.ListAdd(item, this);
			}

			return dest;
		}

		public Command CallSubFunction(MCFunction func)
		{
			if (Func.IsMacroFunction)
			{
				return new FunctionCommand(func, [.. Func.Decl.FuncType.MacroParameters.Select(i =>
				{
					if (i.Type == PrimitiveType.String)
					{
						throw new MacroStringSubFunctionError();
					}

					return new KeyValuePair<string, NBTValue>(i.Name, new NBTRawString(i.GetMacro()));
				})]);
			}
			else
			{
				return new FunctionCommand(func);
			}
		}

		public Command CallSubFunction(MCFunction func, NBTCompound args)
		{
			if (args.Count > 0)
			{
				foreach (var i in Func.Decl.FuncType.MacroParameters)
				{
					if (i.Type == PrimitiveType.String)
					{
						throw new MacroStringSubFunctionError();
					}
				}

				return new FunctionCommand(func, args);
			}
			else
			{
				return new FunctionCommand(func);
			}
		}

		public void Call(NamespacedID id, params ValueRef[] args)
		{
			if (Func.GetGlobal(id) is not FunctionValue func)
			{
				throw new UndefinedSymbolError(id.ToString());
			}

			func.Call(this, args);
		}

		public List<Command> WithFaux(Action<FauxRenderContext> func)
		{
			var ctx = new FauxRenderContext(MCFunction, Block, Builder, Func);
			func(ctx);
			return ctx.Commands;
		}

		public void PossibleErrorChecker(Command cmd, string msg, params ValueRef[] extras) => PossibleErrorChecker(cmd, text => text.Text($": {msg} "), extras);

		public void PossibleErrorChecker(Command cmd, Action<FormattedText> msg, params ValueRef[] extras)
		{
			if (Builder.Options.Debug)
			{
				var success = SuccessScore;
				success.Store(new LiteralValue(1), this);
				Add(new Execute().Store(success.Target, success.Score, false).Run(cmd));

				var text = new FormattedText()
						.PushModifiers(new FormattedText.Modifiers { Color = "red" })
						.Text("Error at ")
						.Text(Func.LocationStack.Peek().ToString(), new FormattedText.Modifiers { Color = "#9A5CC6" });

				msg(text);

				if (extras.Length > 0)
				{
					text.PushModifiers(new FormattedText.Modifiers { Color = "dark_aqua" });

					foreach (var i in extras)
					{
						i.Expect().Render(text, this);
						text.Text(", ", new FormattedText.Modifiers { Color = "red" });
					}

					text.RemoveLast();
				}

				Add(new Execute().If.Score(success.Target, success.Score, 0).Run(new TellrawCommand(
					new TargetSelector(TargetType.a),
					text
				)));

				if (Block != Func.FirstBlock)
				{
					Add(new Execute().If.Score(success.Target, success.Score, 0).Run(WithFaux(ctx =>
					{
						Func.GetIsFunctionReturningValue().Store(success, ctx);
					}).Single()));
				}

				Add(new Execute().If.Score(success.Target, success.Score, 0).Run(new ReturnCommand(0))); // Purposeful not fail, maybe check it
			}
			else
			{
				Add(cmd);
			}
		}
	}

	public record class FauxRenderContext(MCFunction MCFunction, IR.Block Block, GeodeBuilder Builder, FunctionContext Func) : RenderContext(MCFunction, Block, Builder, Func)
	{
		public readonly List<Command> Commands = [];
		public override void Add(params IEnumerable<Command> cmds) => Commands.AddRange(cmds);
	}
}
