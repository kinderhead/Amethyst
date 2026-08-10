namespace Geode.IR
{
    public class FunctionContextRecorder
    {
        private readonly List<Func<FunctionContext, IValueLike?, IValueLike>> chain = [];

        public bool Recorded => chain.Count != 0;

        /// <summary>
        ///     Record instructions.
        /// </summary>
        /// <param name="action">Callback</param>
        public void Record(Func<FunctionContext, IValueLike> action)
        {
            chain.Add((ctx, _) => action(ctx));
        }

        /// <summary>
        ///     Record instructions. Callback accepts an argument for the result of the last recorded action.
        /// </summary>
        /// <param name="action">Callback</param>
        public void Record(Func<FunctionContext, IValueLike?, IValueLike> action) => chain.Add(action);

        public void Record(IValueLike val) => chain.Add((_, _) => val);
        public void Record(Instruction insn) => chain.Add((ctx, _) => ctx.Add(insn));

        public IValueLike? Execute(FunctionContext ctx) =>
            chain.Aggregate<Func<FunctionContext, IValueLike?, IValueLike>?, IValueLike?>(null, (current, i) => i?.Invoke(ctx, current));
    }
}