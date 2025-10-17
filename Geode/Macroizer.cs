using Datapack.Net.Function;
using Geode.Types;
using Geode.Values;

namespace Geode
{
	public class Macroizer(GeodeBuilder builder)
    {
		public readonly GeodeBuilder Builder = builder;
		public readonly Dictionary<string, MCFunction> CachedFunctions = [];

		public void Run(RenderContext ctx, IEnumerable<ValueRef> dependencies, Action<IConstantValue[], RenderContext> func) => Run(ctx, dependencies.Select(i => i.Expect()), func);
		public void Run(RenderContext ctx, IEnumerable<Value> dependencies, Action<IConstantValue[], RenderContext> func)
		{
			var args = new List<IConstantValue>();
			var toMacro = new Dictionary<string, Value>();

			IConstantValue apply(Value val)
			{
				if (val is IConstantValue c)
                {
					return c;
                }

				var macro = new MacroValue($"arg{toMacro.Count}", val.Type);
				toMacro.Add(macro.Name, val);
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

			if (toMacro.Count == 0)
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

				FunctionValue.Call(ctx, mcFunc.ID, new FunctionType(FunctionModifiers.None, new VoidType(), toMacro.Select(i => new Parameter(ParameterModifiers.Macro, i.Value.Type, i.Key))), [.. toMacro.Values]);
			}
		}
    }
}
