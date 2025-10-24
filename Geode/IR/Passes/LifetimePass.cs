namespace Geode.IR.Passes
{
	public class InOutPass : Pass
	{
		public readonly Dictionary<Block, HashSet<ValueRef>> Ins = [];
		public readonly Dictionary<Block, HashSet<ValueRef>> Outs = [];

		//protected override bool Reversed => true;
		protected override bool SkipInsns => true;

		protected override void OnBlock(FunctionContext ctx, Block block)
		{
			Outs[block] = [.. block.Next.SelectMany(i => {
				if (Ins.TryGetValue(i, out var ins)) { return ins; } return [];
			})];

			HashSet<ValueRef> ins = [.. Outs[block]];

			foreach (var i in block.Instructions)
			{
				foreach (var arg in i.Arguments)
				{
					if (arg is ValueRef v && v.NeedsScoreReg)
					{
						ins.Add(v);
					}

					foreach (var dep in arg.Dependencies)
					{
						if (dep is ValueRef d && d.NeedsScoreReg)
						{
							ins.Add(d);
						}
					}
				}
			}

			// Maybe somehow put this in the other loop
			foreach (var i in block.Instructions)
			{
				ins.Remove(i.ReturnValue);
			}

			if (!Ins.TryGetValue(block, out var existingIns))
			{
				existingIns = Ins[block] = [];
			}

			if (!ins.SetEquals(existingIns))
			{
				Ins[block] = ins;

				foreach (var pre in block.Previous)
				{
					RevisitBlock(pre);
				}
			}
		}
	}

	public class LifetimePass(InOutPass inout) : Pass
	{
		public readonly Dictionary<FunctionContext, LifetimeGraph> Graphs = [];
		public readonly InOutPass InOut = inout;

		protected override bool SkipInsns => true;

		protected override void OnFunction(FunctionContext ctx) => Graphs[ctx] = new();

		protected override void OnBlock(FunctionContext ctx, Block block)
		{
			HashSet<ValueRef> alive = [.. InOut.Outs[block]];

			void markAlive(IInstructionArg arg)
			{
				if (arg is ValueRef v && v.NeedsScoreReg)
				{
					alive.Add(v);
					Graphs[ctx].Connect(v, alive);
				}
			}

			foreach (var insn in block.Instructions.AsEnumerable().Reverse())
			{
				alive.Remove(insn.ReturnValue);

				foreach (var arg in insn.Arguments)
				{
					markAlive(arg);
					foreach (var dep in arg.Dependencies)
					{
						markAlive(dep);
					}
				}

				insn.ConfigureLifetime((val1, val2) =>
				{
					if (Graphs[ctx].DoConnect(val1, val2))
					{
						return false;
					}

					Graphs[ctx].Link(val1, val2);
					return true;
				},
				(val1, val2) =>
				{
					Graphs[ctx].Connect(val1, val2);
				});
			}
		}
	}

	public class LifetimeGraph
	{
		public readonly Dictionary<ValueRef, LifetimeGraphNode> Graph = [];

		public LifetimeGraphNode Connect(ValueRef val)
		{
			if (!Graph.TryGetValue(val, out var valNode))
			{
				valNode = Graph[val] = new(val);
			}

			return valNode;
		}

		public void Connect(ValueRef val, params IEnumerable<ValueRef> vals) => Connect(val).Connect(vals.Select(Connect));
		public bool DoConnect(ValueRef val1, ValueRef val2) => Graph.TryGetValue(val1, out var val1Node) && Graph.TryGetValue(val2, out var val2Node) && val1Node.Edges.Contains(val2Node);
		public void Link(ValueRef val1, ValueRef val2)
		{
			if (Graph.TryGetValue(val1, out var val1Node) && Graph.TryGetValue(val2, out var val2Node))
			{
				val1Node.Link(val2Node);
			}
		}

		// Probably could find a better algorithm
		public Dictionary<ValueRef, int> CalculateDSatur()
		{
			List<LifetimeGraphNode> nodes = new(Graph.Values.Count);

			foreach (var i in Graph.Values)
			{
				i.ResetDegree();
				nodes.Add(i);
			}

			nodes.Sort();

			while (nodes.Count > 0)
			{
				var colors = new bool[Graph.Count]; // Apparently this is cheaper than Array.Clear for reasonable sizes

				var node = nodes[0];
				nodes.RemoveAt(0);

				foreach (var i in node.Edges)
				{
					if (i.Color >= 0)
					{
						colors[i.Color] = true;
					}
				}

				node.SetColor(Array.IndexOf(colors, false));

				foreach (var i in node.Links)
				{
					if (i.Color < 0)
					{
						nodes.Remove(i);
						i.SetColor(node.Color);
					}
				}

				nodes.Sort();
			}

			return new(Graph.Select(i => new KeyValuePair<ValueRef, int>(i.Key, i.Value.Color)));
		}
	}

	public class LifetimeGraphNode(ValueRef val) : IComparable<LifetimeGraphNode>
	{
		public readonly ValueRef Value = val;

		private readonly HashSet<LifetimeGraphNode> edges = [];
		public IReadOnlyCollection<LifetimeGraphNode> Edges => edges;

		private readonly HashSet<LifetimeGraphNode> links = [];
		public IReadOnlyCollection<LifetimeGraphNode> Links => links;

		public int Degree { get; private set; }
		public int Saturation { get; private set; }
		public int Color { get; private set; } = -1;

		public int CompareTo(LifetimeGraphNode? other)
		{
			if (other is null)
			{
				return -1;
			}
			else if (Saturation != other.Saturation)
			{
				return other.Saturation - Saturation;
			}
			else if (Degree != other.Degree)
			{
				return other.Degree - Degree;
			}
			else
			{
				return GetHashCode() - other.GetHashCode(); // idk
			}
		}

		public void Link(LifetimeGraphNode other)
		{
			if (other == this)
			{
				return;
			}

			links.Add(other);
			other.links.Add(this);
		}

		public void Connect(LifetimeGraphNode other)
		{
			if (other == this)
			{
				return;
			}

			edges.Add(other);
			other.edges.Add(this);
		}

		public void Connect(IEnumerable<LifetimeGraphNode> others)
		{
			foreach (var i in others)
			{
				Connect(i);
			}
		}

		public void ResetDegree() => Degree = Edges.Count;

		public void SetColor(int value)
		{
			if (Color < 0 && value >= 0)
			{
				foreach (var i in edges)
				{
					i.Saturation++;
					i.Degree--;
				}
			}
			else if (Color >= 0 && value < 0)
			{
				foreach (var i in edges)
				{
					i.Saturation--;
					i.Degree++;
				}
			}

			Color = value;
		}
	}
}
