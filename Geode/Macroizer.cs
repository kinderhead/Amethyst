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

		public void Run(RenderContext ctx, IValue[] dependencies, Action<IConstantValue[], RenderContext> func)
		{
			var args = new List<IConstantValue>();
			var toMacro = new Dictionary<string, IValue>();
			var propagatedMacroMap = new Dictionary<string, MacroValue>();

			IConstantValue apply(IValue val)
			{
				// Include macros in the dependencies
				if (val is IConstantValue c and not MacroValue)
				{
					return c;
				}

				var macro = new MacroValue($"arg{toMacro.Count}", val.Type);

				if (val is MacroValue m)
				{
					propagatedMacroMap[macro.GetMacro()] = m;
					// TODO: refactor how string macros are handled.
					toMacro.Add(macro.Name, new LiteralValue(m.Type.WrapInQuotesForMacro ? new NBTString(m.GetMacro()) : new NBTRawString(m.GetMacro()), m.Type));
				}
				else
				{
					toMacro.Add(macro.Name, val);
				}

				return macro;
			}

			foreach (var i in dependencies)
			{
				if (i is IAdvancedMacroValue amv)
				{
					args.Add(amv.Macroize(apply));
				}
				else
				{
					args.Add(apply(i));
				}
			}

			if (toMacro.Count == propagatedMacroMap.Count)
			{
				func([.. args.Select(i => propagatedMacroMap.TryGetValue(i.Value.Build(), out var orig) && i.Type == orig.Type ? orig : i)], ctx);
			}
			else
			{
				var faux = ctx.WithFaux(i => func([.. args], i)).Select(i =>
				{
					i.Macro = true;
					return i;
				});

				var mcFunc = new MCFunction($"{Builder.Namespace}:{GeodeBuilder.InternalPath}/{GeodeBuilder.RandomString}");
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

		public void RunAndPropagateMacros(RenderContext ctx, IValue[] dependencies, Action<IConstantValue[], NBTCompound, RenderContext> func) => Run(ctx, [.. dependencies, .. ctx.Func.Decl.FuncType.MacroParameters], (args, ctx) =>
		{
			IConstantValue[] realArgs = [.. args.Take(dependencies.Length)];
			IConstantValue[] propagated;

			if (args.Length == 0)
			{
				propagated = ctx.Func.Decl.FuncType.MacroParameters;
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
