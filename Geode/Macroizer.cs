using Datapack.Net.Data;
using Datapack.Net.Function;
using Geode.Types;
using Geode.Values;

namespace Geode
{
	public class Macroizer(GeodeBuilder builder)
	{
		public readonly GeodeBuilder Builder = builder;
		public readonly Dictionary<string, MCFunction> CachedFunctions = [];

		public void Run(RenderContext ctx, IValueLike[] dependencies, Action<IConstantValue[], RenderContext> func)
		{
			var args = new List<IConstantValue>();
			var toMacro = new Dictionary<string, IValue>();
			var propagatedMacroList = new List<MacroValue>();
			var applied = new Dictionary<IValue, IConstantValue>();

			int macroIdx = 0;

			IConstantValue apply(IValue val)
			{
				if (applied.TryGetValue(val, out var existing))
				{
					return existing;
				}

				IConstantValue ret;

				if (val is IAdvancedMacroValue amv)
				{
					ret = amv.Macroize(apply);
					goto end;
				}

				// Include macros in the dependencies
				if (val is IConstantValue c and not MacroValue)
				{
					if (val.Type.WrapInQuotesForMacro && c.Value is NBTString str)
					{
						ret = new LiteralValue(new NBTRawString(str.Value), val.Type);
						goto end;
					}

					ret = c;
					goto end;
				}

				if (val is MacroValue m)
				{
					propagatedMacroList.Add(m);
					// TODO: refactor how string macros are handled.
					toMacro.Add(m.Name, new LiteralValue(m.Type.WrapInQuotesForMacro ? new NBTString(m.GetMacro()) : new NBTRawString(m.GetMacro()), m.Type));
					ret = m;
					goto end;
				}

				// Allow existing macros to keep their name
				string macroName = $"arg{macroIdx++}";

				while (toMacro.ContainsKey(macroName))
				{
					macroName = $"arg{macroIdx++}";
				}

				var macro = new MacroValue(macroName, val.Type);
				toMacro.Add(macro.Name, val);
				ret = macro;
			end:
				applied[val] = ret;
				return ret;
			}

			foreach (var i in dependencies)
			{
				args.Add(apply(i.Expect()));
			}

			if (toMacro.Count == propagatedMacroList.Count)
			{
				func([.. args], ctx);
			}
			else
			{
				var faux = ctx.WithFaux(i => func([.. args], i)).Select(i =>
				{
					i.Macro = true;
					return i;
				});

				var mcFunc = new MCFunction($"{Builder.Namespace}:{GeodeBuilder.InternalPath}/{GeodeBuilder.UniqueString}");
				mcFunc.Add(faux);
				var compiled = mcFunc.Build();

				if (CachedFunctions.TryGetValue(compiled, out var newFunc))
				{
					mcFunc = newFunc;
				}
				else
				{
					CachedFunctions[compiled] = mcFunc;
					Builder.Datapack.Functions.Add(mcFunc);
				}

				ctx.Func.AddDependency(mcFunc);

				FunctionValue.Call(ctx, mcFunc.ID, new FunctionType(FunctionModifiers.None, new VoidType(), toMacro.Select(i => new Parameter(ParameterModifiers.Macro, i.Value.Type, i.Key))), [.. toMacro.Values.Select(i => new ValueRef(i))]);
			}
		}

		public void RunAndPropagateMacros(RenderContext ctx, IValueLike[] dependencies, Action<IConstantValue[], NBTCompound, RenderContext> func) => Run(ctx, [.. dependencies, .. ctx.Func.Decl.FuncType.MacroParameters], (args, ctx) =>
		{
			IConstantValue[] realArgs = [.. args.Take(dependencies.Length)];
			IConstantValue[] propagated;

			if (args.Length == 0)
			{
				propagated = [.. ctx.Func.Decl.FuncType.MacroParameters];
			}
			else
			{
				propagated = [.. args.Skip(dependencies.Length)];
			}

			func(realArgs, [.. ctx.Func.Decl.FuncType.MacroParameters.Zip(propagated).Select(i => {
				if (i.First.Type.WrapInQuotesForMacro && i.Second.Value.GetType() != typeof(NBTString)) return new KeyValuePair<string, NBTValue>(i.First.Name, new NBTString(i.Second.Value.ToString()));
				else return new KeyValuePair<string, NBTValue>(i.First.Name, i.Second.Value);
			})], ctx);
		});
	}
}
