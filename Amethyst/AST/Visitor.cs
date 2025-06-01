using Amethyst.Antlr;
using Amethyst.AST.Expressions;
using Amethyst.AST.Statements;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
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
				else throw new NotImplementedException();
			}

			return root;
		}

		public override Node VisitFunction([NotNull] AmethystParser.FunctionContext context) => new FunctionNode(Loc(context), Visit(context.type()), new(currentNamespace, context.name.Text), Visit(context.block()));

		public override Node VisitStatement([NotNull] AmethystParser.StatementContext context) => Visit(context.children[0]);

		public override Node VisitBlock([NotNull] AmethystParser.BlockContext context)
		{
			var block = new BlockNode(Loc(context));

			foreach (var i in context.statement())
			{
				block.Statements.Add(Visit(i));
			}

			return block;
		}

		public override Node VisitInitAssignmentStatement([NotNull] AmethystParser.InitAssignmentStatementContext context) => new InitAssignmentNode(Loc(context), Visit(context.type()), context.Identifier().GetText(), Visit(context.expression()));
		public override Node VisitExpressionStatement([NotNull] AmethystParser.ExpressionStatementContext context) => new ExpressionStatement(Loc(context), Visit(context.expression()));
		public override Node VisitCommandStatement([NotNull] AmethystParser.CommandStatementContext context) => new CommandStatement(Loc(context), context.Command().GetText().Trim()[1..]);

		public override Node VisitType([NotNull] AmethystParser.TypeContext context)
		{
			if (context.Identifier() is ITerminalNode id) return new SimpleAbstractTypeSpecifier(Loc(context), id.GetText());
			else throw new NotImplementedException();
		}

		public override Node VisitExpression([NotNull] AmethystParser.ExpressionContext context) => Visit(context.children.First());

		public override Node VisitAssignmentExpression([NotNull] AmethystParser.AssignmentExpressionContext context)
		{
			if (context.additiveExpression() is not null) return Visit(context.additiveExpression());
			else return new AssignmentExpression(Loc(context), context.Identifier().GetText(), Visit(context.expression()));
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

		public override Node VisitPrimaryExpression([NotNull] AmethystParser.PrimaryExpressionContext context)
		{
			if (context.Identifier() is ITerminalNode id) return new VariableExpression(Loc(context), id.GetText());
			else if (context.String() is ITerminalNode str) return new LiteralExpression(Loc(context), new NBTRawString(str.GetText()));
			else if (context.Integer() is ITerminalNode i) return new LiteralExpression(Loc(context), new NBTInt(int.Parse(i.GetText())));
			else if (context.expression() is AmethystParser.ExpressionContext expr) return Visit(expr);
			else throw new NotImplementedException();
		}

		public AbstractTypeSpecifier Visit(AmethystParser.TypeContext context) => (AbstractTypeSpecifier)Visit((IParseTree)context);
		public BlockNode Visit(AmethystParser.BlockContext context) => (BlockNode)Visit((IParseTree)context);
		public Statement Visit(AmethystParser.StatementContext context) => (Statement)Visit((IParseTree)context);
		public Expression Visit(AmethystParser.ExpressionContext context) => (Expression)Visit((IParseTree)context);

		public LocationRange Loc(ParserRuleContext ctx) => LocationRange.From(Filename, ctx);
	}
}
