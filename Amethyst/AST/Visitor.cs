using System.Text.RegularExpressions;
using Amethyst.Antlr;
using Amethyst.AST.Expressions;
using Amethyst.AST.Statements;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

namespace Amethyst.AST
{
	public partial class Visitor(string filename, Compiler compiler) : AmethystBaseVisitor<Node>
	{
		public readonly string Filename = filename;
		public readonly Compiler Compiler = compiler;

		private string currentNamespace = "minecraft";

		public override Node VisitRoot([NotNull] AmethystParser.RootContext context)
		{
			var root = new RootNode(Loc(context), Compiler);

			foreach (var i in context.children)
			{
				if (i.GetText() == "<EOF>" || i.GetText() == ";") continue;
				else if (i is AmethystParser.NamespaceContext ns) currentNamespace = Visit(ns.id());
				else if (i is AmethystParser.FunctionContext func) root.Functions.Add((FunctionNode)Visit(func));
				else if (i is AmethystParser.InitAssignmentStatementContext init) root.Children.Add(new GlobalVariableNode(Loc(init), Visit(init.type()), IdentifierToID(Visit(init.id())), init.expression() is null ? null : Visit(init.expression())));
				else if (i is AmethystParser.StructContext type) root.Children.Add((IRootChild)Visit(type));
			}

			return root;
		}

		public override Node VisitFunction([NotNull] AmethystParser.FunctionContext context)
		{
			var mod = FunctionModifiers.None;
			foreach (var i in context.functionModifier())
			{
				if (i.GetText() == "nostack") mod |= FunctionModifiers.NoStack;
				else if (i.GetText() == "inline") mod |= FunctionModifiers.Inline;
			}

			return new FunctionNode(Loc(context), [.. context.functionTag().Select(i =>
			{
				var text = Visit(i.id());
				if (text.Contains(':')) return new NamespacedID(text);
				else if (text == "load" || text == "tick") return new NamespacedID("minecraft", text);
				else return new NamespacedID(currentNamespace, text);
			})], mod, Visit(context.type()),
				IdentifierToID(Visit(context.name)),
				[.. context.paramList().paramPair().Select(i => {
					var mod = ParameterModifiers.None;

					foreach (var e in i.paramModifier())
					{
						if (e.GetText() == "macro") mod |= ParameterModifiers.Macro;
					}

					return new AbstractParameter(mod, Visit(i.type()), Visit(i.id()));
				})],
				Visit(context.block())
			);
		}

		public override Node VisitStatement([NotNull] AmethystParser.StatementContext context) => Visit(context.children[0]);

		public override Node VisitBlock([NotNull] AmethystParser.BlockContext context)
		{
			var block = new BlockNode(Loc(context));

			foreach (var i in context.statement())
			{
				block.Add(Visit(i));
			}

			return block;
		}

		public override Node VisitStruct([NotNull] AmethystParser.StructContext context)
		{
			return new AbstractStructTypeSpecifier(Loc(context), IdentifierToID(Visit(context.id())), new(context.declaration().Select(i => new KeyValuePair<string, AbstractTypeSpecifier>(i.RawIdentifier().GetText(), Visit(i.type())))));
		}

		public override Node VisitInitAssignmentStatement([NotNull] AmethystParser.InitAssignmentStatementContext context) => new InitAssignmentNode(Loc(context), Visit(context.type()), Visit(context.id()), context.expression() is null ? null : Visit(context.expression()));
		public override Node VisitExpressionStatement([NotNull] AmethystParser.ExpressionStatementContext context) => new ExpressionStatement(Loc(context), Visit(context.expression()));
		public override Node VisitIfStatement([NotNull] AmethystParser.IfStatementContext context) => context.statement().Length != 0 ? new IfStatement(Loc(context), Visit(context.expression()), Visit(context.statement().First()), context.statement().Length == 2 ? Visit(context.statement().Last()) : null) : new ExpressionStatement(Loc(context), new LiteralExpression(Loc(context), new NBTString("uh")));
		public override Node VisitForStatement([NotNull] AmethystParser.ForStatementContext context) => new ForStatement(Loc(context), context.initAssignmentStatement() is not null ? (Statement?)Visit(context.initAssignmentStatement()) : null, Visit(context.cond), Visit(context.it), Visit(context.statement()));
		public override Node VisitReturnStatement([NotNull] AmethystParser.ReturnStatementContext context) => new ReturnStatement(Loc(context), context.expression() is null ? null : Visit(context.expression()));

		public override Node VisitCommandStatement([NotNull] AmethystParser.CommandStatementContext context)
		{
			var cmd = context.Command().GetText().Trim()[2..];
			var loc = Loc(context);
			var frags = new List<CommandFragment>();

			foreach (Match i in CommandExprRegex().Matches(cmd))
			{
				if (i.Groups["other"].Success)
				{
					frags.Add(new CommandTextFragment(i.Groups["other"].Value));
					continue;
				}

				var match = i.Groups["content"];

				var input = CharStreams.fromString(match.Value);
				var lexer = new AmethystLexer(input);
				var tokens = new CommonTokenStream(lexer);
				var parser = new AmethystParser(tokens);
				var visitor = new SubVisitor(this, new(loc.Start.File, loc.Start.Line, loc.Start.Column + 2 + match.Index));

				var error = new ParserErrorHandler(Filename, Compiler.Files[Filename], visitor);
				lexer.RemoveErrorListeners();
				lexer.AddErrorListener(error);
				parser.RemoveErrorListeners();
				parser.AddErrorListener(error);

				var expr = parser.expression();
				var node = visitor.Visit(expr);

				frags.Add(new CommandExprFragment(node));

				if (error.Errored) throw new Exception(); // Do this later
			}

			return new CommandStatement(loc, frags);
		}

		public override Node VisitType([NotNull] AmethystParser.TypeContext context)
		{
			if (context.id() is AmethystParser.IdContext id) return new SimpleAbstractTypeSpecifier(Loc(context), Visit(id));
			else if (context.LSquareBrak() is not null) return new AbstractListTypeSpecifier(Loc(context), Visit(context.type()));
			else if (context.And() is not null) return new AbstractReferenceTypeSpecifier(Loc(context), Visit(context.type()));
			else if (context.WeakRef() is not null) return new AbstractWeakReferenceTypeSpecifier(Loc(context), Visit(context.type()));
			else throw new NotImplementedException();
		}

		public override Node VisitExpression([NotNull] AmethystParser.ExpressionContext context) => Visit(context.children.First());

		public override Node VisitAssignmentExpression([NotNull] AmethystParser.AssignmentExpressionContext context)
		{
			if (context.expression() is null) return Visit(context.logicalExpression());
			else
			{
				var type = AssignmentType.Normal;

				if (context.PlusEq() is not null) type = AssignmentType.Addition;
				else if (context.MinusEq() is not null) type = AssignmentType.Subtraction;
				else if (context.StarEq() is not null) type = AssignmentType.Multiplication;
				else if (context.SlashEq() is not null) type = AssignmentType.Division;

                return new AssignmentExpression(Loc(context), (Expression)Visit(context.logicalExpression()), type, Visit(context.expression()));
			}
		}

		public override Node VisitLogicalExpression([NotNull] AmethystParser.LogicalExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "&&" ? LogicalOperation.And : LogicalOperation.Or;
				node = new LogicalExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitEqualityExpression([NotNull] AmethystParser.EqualityExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "==" ? ComparisonOperator.Eq : ComparisonOperator.Neq;
				node = new ComparisonExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitRelationalExpression([NotNull] AmethystParser.RelationalExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() switch
				{
					">" => ComparisonOperator.Gt,
					"<" => ComparisonOperator.Lt,
					">=" => ComparisonOperator.Gte,
					_ => ComparisonOperator.Lte,
				};
				node = new ComparisonExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitAdditiveExpression([NotNull] AmethystParser.AdditiveExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "+" ? ScoreOperation.Add : ScoreOperation.Sub;
				node = new ArithmeticExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitMultiplicativeExpression([NotNull] AmethystParser.MultiplicativeExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "*" ? ScoreOperation.Mul : ScoreOperation.Div;
				node = new ArithmeticExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitCastExpression([NotNull] AmethystParser.CastExpressionContext context)
		{
			return Visit(context.unaryExpression());
		}

		public override Node VisitUnaryExpression([NotNull] AmethystParser.UnaryExpressionContext context)
		{
			var node = (Expression)Visit(context.children.Last());
			for (int i = context.children.Count - 2; i >= 0; i--)
			{
				if (context.children[i].GetText() == "++") node = new UnaryExpression(Loc(context), UnaryOperation.Increment, node);
				else if (context.children[i].GetText() == "--") node = new UnaryExpression(Loc(context), UnaryOperation.Decrement, node);
				else if (context.children[i].GetText() == "!") node = new UnaryExpression(Loc(context), UnaryOperation.Not, node);
				else if (context.children[i].GetText() == "-") node = new UnaryExpression(Loc(context), UnaryOperation.Negate, node);
                // No check for other cases because parser errors might hit it and we don't want to stop the error checker
            }
			return node;
		}

		public override Node VisitPostfixExpression([NotNull] AmethystParser.PostfixExpressionContext context)
		{
			Expression node = (Expression)Visit(context.children.First());

			for (int i = 1; i < context.children.Count; i++)
			{
				if (context.children[i] is AmethystParser.ExpressionListContext call) node = new CallExpression(Loc(call), node, Visit(call));
				else if (context.children[i] is AmethystParser.IndexExpressionContext ind) node = new IndexExpression(Loc(ind), node, Visit(ind.expression()));
				else if (context.children[i] is AmethystParser.PropertyExpressionContext prop) node = new PropertyExpression(Loc(prop), node, prop.RawIdentifier().GetText());
				else throw new NotImplementedException();
			}

			return node;
		}

		public override Node VisitPrimaryExpression([NotNull] AmethystParser.PrimaryExpressionContext context)
		{
			if (context.id() is AmethystParser.IdContext id) return new VariableExpression(Loc(context), Visit(id));
			else if (context.String() is ITerminalNode str) return new LiteralExpression(Loc(context), new NBTString(NBTString.Unescape(str.GetText()[1..^1])));
			else if (context.Integer() is ITerminalNode i) return new LiteralExpression(Loc(context), ParseNumber(i.GetText()));
			else if (context.expression() is AmethystParser.ExpressionContext expr) return Visit(expr);
			else return Visit(context.children[0]);
		}

		public override Node VisitListLiteral([NotNull] AmethystParser.ListLiteralContext context) => new ListExpression(Loc(context), [.. context.expression().Select(Visit)]);
		public override Node VisitCompoundLiteral([NotNull] AmethystParser.CompoundLiteralContext context) => new CompoundExpression(Loc(context), context.compoundKeyPair().Select(i =>
		{
			if (i.RawIdentifier() is not null) return new KeyValuePair<string, Expression>(i.RawIdentifier().GetText(), Visit(i.expression()));
			else return new KeyValuePair<string, Expression>(i.RawIdentifier().GetText(), Visit(i.expression()));
		}));

		public AbstractTypeSpecifier Visit(AmethystParser.TypeContext context) => (AbstractTypeSpecifier)Visit((IParseTree)context);
		public BlockNode Visit(AmethystParser.BlockContext context) => (BlockNode)Visit((IParseTree)context);
		public Statement Visit(AmethystParser.StatementContext context) => (Statement)Visit((IParseTree)context);
		public Expression Visit(AmethystParser.ExpressionContext context) => (Expression)Visit((IParseTree)context);
		public List<Expression> Visit(AmethystParser.ExpressionListContext context) => [.. context.expression().Select(Visit)];
		public string Visit(AmethystParser.IdContext context) => context.GetText();

		public LocationRange Loc(ParserRuleContext ctx) => LocOffset(LocationRange.From(Filename, ctx));

		public virtual Location LocOffset(Location loc) => loc;
		public virtual LocationRange LocOffset(LocationRange loc) => new(LocOffset(loc.Start), LocOffset(loc.End));

		public NamespacedID IdentifierToID(string name)
		{
			if (name.Contains(':')) return new(name);
			else return new(currentNamespace, name);
		}

		public NBTValue ParseNumber(string raw)
		{
			switch (char.ToLower(raw.Last()))
			{
				case 'b':
					return new NBTByte(sbyte.Parse(raw[..^1]));
				case 's':
					return new NBTShort(short.Parse(raw[..^1]));
				case 'i':
					return new NBTInt(int.Parse(raw[..^1]));
				default:
					if (raw.Contains('.')) return new NBTDouble(double.Parse(raw));
					return new NBTInt(int.Parse(raw));
				case 'l':
					return new NBTLong(long.Parse(raw[..^1]));
				case 'f':
					return new NBTFloat(float.Parse(raw[..^1]));
				case 'd':
					return new NBTDouble(double.Parse(raw[..^1]));
			}
		}

		[GeneratedRegex(@"(?<other>[^\$]+)|\$\((?<content>(?>[^()]+|(?<open>\()|(?<-open>\)))+(?(open)(?!)))\)")]
		private static partial Regex CommandExprRegex();
	}
}
