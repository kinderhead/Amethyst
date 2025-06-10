using Amethyst.Antlr;
using Amethyst.AST.Expressions;
using Amethyst.AST.Statements;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public class Visitor(string filename, Compiler compiler) : AmethystBaseVisitor<Node>
	{
		public readonly string Filename = filename;
		public readonly Compiler Compiler = compiler;

		private string currentNamespace = "minecraft";

		public override Node VisitRoot([NotNull] AmethystParser.RootContext context)
		{
			var root = new RootNode(Loc(context), Compiler);

			foreach (var i in context.children)
			{
				if (i.GetText() == "<EOF>") continue;
				else if (i is AmethystParser.NamespaceContext ns) currentNamespace = ns.Identifier().GetText();
				else if (i is AmethystParser.FunctionContext func) root.Functions.Add((FunctionNode)Visit(func));
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
				var text = i.Identifier().GetText();
				if (text.Contains(':')) return new NamespacedID(text);
				else if (text == "load" || text == "tick") return new NamespacedID("minecraft", text);
				else return new NamespacedID(currentNamespace, text);
			})], mod, Visit(context.type()),
				IdentifierToID(context.name.Text),
				[.. context.paramList().paramPair().Select(i => {
					var mod = ParameterModifiers.None;

					foreach (var e in i.paramModifier())
					{
						if (e.GetText() == "macro") mod |= ParameterModifiers.Macro;
					}

					return new AbstractParameter(mod, Visit(i.type()), i.Identifier().GetText());
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

		public override Node VisitInitAssignmentStatement([NotNull] AmethystParser.InitAssignmentStatementContext context) => new InitAssignmentNode(Loc(context), Visit(context.type()), context.Identifier().GetText(), Visit(context.expression()));
		public override Node VisitExpressionStatement([NotNull] AmethystParser.ExpressionStatementContext context) => new ExpressionStatement(Loc(context), Visit(context.expression()));
		public override Node VisitCommandStatement([NotNull] AmethystParser.CommandStatementContext context) => new CommandStatement(Loc(context), context.Command().GetText().Trim()[2..]);
		public override Node VisitIfStatement([NotNull] AmethystParser.IfStatementContext context) => context.statement().Length != 0 ? new IfStatement(Loc(context), Visit(context.expression()), Visit(context.statement().First()), context.statement().Length == 2 ? Visit(context.statement().Last()) : null) : new ExpressionStatement(Loc(context), new LiteralExpression(Loc(context), new NBTString("uh")));
		public override Node VisitReturnStatement([NotNull] AmethystParser.ReturnStatementContext context) => new ReturnStatement(Loc(context), context.expression() is null ? null : Visit(context.expression()));

		public override Node VisitType([NotNull] AmethystParser.TypeContext context)
		{
			if (context.Identifier() is ITerminalNode id) return new SimpleAbstractTypeSpecifier(Loc(context), id.GetText());
			else if (context.type() is AmethystParser.TypeContext t) return new AbstractListTypeSpecifier(Loc(context), Visit(t));
			else throw new NotImplementedException();
		}

		public override Node VisitExpression([NotNull] AmethystParser.ExpressionContext context) => Visit(context.children.First());

		public override Node VisitAssignmentExpression([NotNull] AmethystParser.AssignmentExpressionContext context)
		{
			if (context.logicalExpression() is not null) return Visit(context.logicalExpression());
			else return new AssignmentExpression(Loc(context), context.Identifier().GetText(), Visit(context.expression()));
		}

		public override Node VisitLogicalExpression([NotNull] AmethystParser.LogicalExpressionContext context)
		{
			Expression node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "&&" ? LogicalOperation.And : LogicalOperation.Or;
				node = new LogicalExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitEqualityExpression([NotNull] AmethystParser.EqualityExpressionContext context)
		{
			Expression node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "==" ? ComparisonOperator.Eq : ComparisonOperator.Neq;
				node = new ComparisonExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitRelationalExpression([NotNull] AmethystParser.RelationalExpressionContext context)
		{
			Expression node = (Expression)Visit(context.children.First());
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
			Expression node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "+" ? ScoreOperation.Add : ScoreOperation.Sub;
				node = new ArithmeticExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitMultiplicativeExpression([NotNull] AmethystParser.MultiplicativeExpressionContext context)
		{
			Expression node = (Expression)Visit(context.children.First());
			for (int i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "*" ? ScoreOperation.Mul : ScoreOperation.Div;
				node = new ArithmeticExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}
			return node;
		}

		public override Node VisitPostfixExpression([NotNull] AmethystParser.PostfixExpressionContext context)
		{
			Expression node = (Expression)Visit(context.children.First());

			for (int i = 1; i < context.children.Count; i++)
			{
				if (context.children[i] is AmethystParser.ExpressionListContext call) node = new CallExpression(Loc(context), node, Visit(call));
				else if (context.children[i] is AmethystParser.IndexExpressionContext ind) node = new IndexExpression(Loc(context), node, Visit(ind.expression()));
				else throw new NotImplementedException();
			}

			return node;
		}

		public override Node VisitPrimaryExpression([NotNull] AmethystParser.PrimaryExpressionContext context)
		{
			if (context.Identifier() is ITerminalNode id) return new VariableExpression(Loc(context), id.GetText());
			else if (context.String() is ITerminalNode str) return new LiteralExpression(Loc(context), new NBTString(NBTString.Unescape(str.GetText()[1..^1])));
			else if (context.Integer() is ITerminalNode i) return new LiteralExpression(Loc(context), ParseNumber(i.GetText()));
			else if (context.expression() is AmethystParser.ExpressionContext expr) return Visit(expr);
			else return Visit(context.children[0]);
		}

		public override Node VisitListLiteral([NotNull] AmethystParser.ListLiteralContext context) => new ListExpression(Loc(context), [.. context.expression().Select(Visit)]);

		public AbstractTypeSpecifier Visit(AmethystParser.TypeContext context) => (AbstractTypeSpecifier)Visit((IParseTree)context);
		public BlockNode Visit(AmethystParser.BlockContext context) => (BlockNode)Visit((IParseTree)context);
		public Statement Visit(AmethystParser.StatementContext context) => (Statement)Visit((IParseTree)context);
		public Expression Visit(AmethystParser.ExpressionContext context) => (Expression)Visit((IParseTree)context);
		public List<Expression> Visit(AmethystParser.ExpressionListContext context) => [.. context.expression().Select(Visit)];

		public LocationRange Loc(ParserRuleContext ctx) => LocationRange.From(Filename, ctx);
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
	}
}
