using System.Text.RegularExpressions;
using Amethyst.Antlr;
using Amethyst.AST.Expressions;
using Amethyst.AST.Statements;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using Geode;
using Geode.Chains;
using Geode.Types;

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
				if (i.GetText() is "<EOF>" or ";")
				{
					continue;
				}
				else if (i is AmethystParser.NamespaceContext ns)
				{
					currentNamespace = Visit(ns.id());
				}
				else if (i is AmethystParser.FunctionContext func)
				{
					root.Children.Add((FunctionNode)Visit(func));
				}
				else if (i is AmethystParser.InitAssignmentStatementContext init)
				{
					root.Children.Add(new GlobalVariableNode(Loc(init), Visit(init.type()), IdentifierToID(Visit(init.id())), init.expression() is null ? null : Visit(init.expression())));
				}
				else if (i is AmethystParser.StructContext type)
				{
					root.Children.Add((IRootChild)Visit(type));
				}
			}

			return root;
		}

		public override Node VisitFunction([NotNull] AmethystParser.FunctionContext context)
		{
			var mod = FunctionModifiers.None;
			foreach (var i in context.functionModifier())
			{
				if (i.GetText() == "virtual")
				{
					mod |= FunctionModifiers.Virtual;
				}
				else if (i.GetText() == "inline")
				{
					mod |= FunctionModifiers.Inline;
				}
			}

			return new FunctionNode(Loc(context), [.. context.functionTag().Select(i =>
			{
				var text = Visit(i.id());
				if (text.Contains(':'))
				{
					return new NamespacedID(text);
				}
				else if (text is "load" or "tick")
				{
					return new NamespacedID("minecraft", text);
				}
				else
				{
					return new NamespacedID(currentNamespace, text);
				}
			})],
				mod, Visit(context.type()),
				IdentifierToID(Visit(context.name)),
				Visit(context.paramList()),
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
			var props = new Dictionary<string, AbstractTypeSpecifier>();

			foreach (var i in context.declaration())
			{
				props[i.RawIdentifier().GetText()] = Visit(i.type());
			}

			var id = IdentifierToID(Visit(context.id()));

			var oldNs = currentNamespace;
			currentNamespace = id.ToString();
			List<MethodNode> methods = [.. context.method().Select(i => (MethodNode)Visit(i))];
			currentNamespace = oldNs;

			return new StructNode(Loc(context), id, context.type() is null ? null : Visit(context.type()), props, methods);
		}

		public override Node VisitMethod([NotNull] AmethystParser.MethodContext context)
		{
			var mod = FunctionModifiers.None;
			foreach (var i in context.functionModifier().Length == 0 ? context.methodModifier().Select(i => i.GetText()) : context.functionModifier().Select(i => i.GetText()))
			{
				if (i == "virtual")
				{
					mod |= FunctionModifiers.Virtual;
				}
				else if (i == "inline")
				{
					mod |= FunctionModifiers.Inline;
				}
			}

			var id = IdentifierToID(context.RawIdentifier().GetText());
			var args = Visit(context.paramList());
			var block = Visit(context.block());

			if (context.type() is not null)
			{
				return new MethodNode(Loc(context), [], mod, Visit(context.type()), id, args, block);
			}
			else
			{
				return new ConstructorNode(Loc(context), [], mod, id, args, context.expression() is null ? null : Visit(context.expression()), block);
			}
		}

		public override Node VisitInitAssignmentStatement([NotNull] AmethystParser.InitAssignmentStatementContext context) => new InitAssignmentNode(Loc(context), Visit(context.type()), Visit(context.id()), context.expression() is null ? null : Visit(context.expression()));
		public override Node VisitExpressionStatement([NotNull] AmethystParser.ExpressionStatementContext context) => new ExpressionStatement(Loc(context), Visit(context.expression()));
		public override Node VisitExecuteStatement([NotNull] AmethystParser.ExecuteStatementContext context) => new ExecuteStatement(Loc(context), context.executeSubcommand().Select(i => (ExecuteStatementSubcommand)Visit(i)), Visit(context.statement().First()), context.statement().Length == 2 ? Visit(context.statement().Last()) : null);

		public override Node VisitExecuteSubcommand([NotNull] AmethystParser.ExecuteSubcommandContext context)
        {
			if (context.If() is not null)
			{
				return new IfSubcommand(Loc(context), Visit(context.expression()));
			}
			else if (context.As() is not null)
			{
				return new AsSubcommand(Loc(context), Visit(context.expression()));
			}
			else if (context.At() is not null)
			{
				return new AtSubcommand(Loc(context), Visit(context.expression()));
			}
			else
			{
				throw new NotImplementedException();
			}
        }

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

				if (error.Errored)
				{
					throw new Exception(); // Do this later
				}
			}

			return new CommandStatement(loc, frags);
		}

		public override Node VisitType([NotNull] AmethystParser.TypeContext context)
		{
			if (context.id() is AmethystParser.IdContext id)
			{
				return new SimpleAbstractTypeSpecifier(Loc(context), Visit(id));
			}
			else if (context.LSquareBrak() is not null)
			{
				return new AbstractListTypeSpecifier(Loc(context), Visit(context.type()));
			}
			else if (context.And() is not null)
			{
				return new AbstractReferenceTypeSpecifier(Loc(context), Visit(context.type()));
			}
			else if (context.WeakRef() is not null)
			{
				return new AbstractWeakReferenceTypeSpecifier(Loc(context), Visit(context.type()));
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public override Node VisitExpression([NotNull] AmethystParser.ExpressionContext context) => Visit(context.children.First());

		public override Node VisitAssignmentExpression([NotNull] AmethystParser.AssignmentExpressionContext context)
		{
			if (context.expression() is null)
			{
				return Visit(context.logicalExpression());
			}
			else
			{
				var type = AssignmentType.Normal;

				if (context.PlusEq() is not null)
				{
					type = AssignmentType.Addition;
				}
				else if (context.MinusEq() is not null)
				{
					type = AssignmentType.Subtraction;
				}
				else if (context.StarEq() is not null)
				{
					type = AssignmentType.Multiplication;
				}
				else if (context.SlashEq() is not null)
				{
					type = AssignmentType.Division;
				}

				return new AssignmentExpression(Loc(context), (Expression)Visit(context.logicalExpression()), type, Visit(context.expression()));
			}
		}

		public override Node VisitLogicalExpression([NotNull] AmethystParser.LogicalExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (var i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "&&" ? LogicalOperation.And : LogicalOperation.Or;
				node = new LogicalExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}

			return node;
		}

		public override Node VisitEqualityExpression([NotNull] AmethystParser.EqualityExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (var i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "==" ? ComparisonOperator.Eq : ComparisonOperator.Neq;
				node = new ComparisonExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}

			return node;
		}

		public override Node VisitRelationalExpression([NotNull] AmethystParser.RelationalExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (var i = 1; i < context.children.Count; i++)
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
			for (var i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "+" ? ScoreOperation.Add : ScoreOperation.Sub;
				node = new ArithmeticExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}

			return node;
		}

		public override Node VisitMultiplicativeExpression([NotNull] AmethystParser.MultiplicativeExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());
			for (var i = 1; i < context.children.Count; i++)
			{
				var op = context.children[i].GetText() == "*" ? ScoreOperation.Mul : ScoreOperation.Div;
				node = new ArithmeticExpression(Loc(context), node, op, (Expression)Visit(context.children[++i]));
			}

			return node;
		}

		public override Node VisitCastExpression([NotNull] AmethystParser.CastExpressionContext context)
		{
			if (context.type() is not null)
			{
				return new CastExpression(Loc(context), Visit(context.type()), (Expression)Visit(context.castExpression()));
			}

			return Visit(context.unaryExpression());
		}

		public override Node VisitUnaryExpression([NotNull] AmethystParser.UnaryExpressionContext context)
		{
			var node = (Expression)Visit(context.children.Last());
			for (var i = context.children.Count - 2; i >= 0; i--)
			{
				if (context.children[i].GetText() == "++")
				{
					node = new UnaryExpression(Loc(context), UnaryOperation.Increment, node);
				}
				else if (context.children[i].GetText() == "--")
				{
					node = new UnaryExpression(Loc(context), UnaryOperation.Decrement, node);
				}
				else if (context.children[i].GetText() == "!")
				{
					node = new NotExpression(Loc(context), node);
				}
				else if (context.children[i].GetText() == "-")
				{
					node = new UnaryExpression(Loc(context), UnaryOperation.Negate, node);
				}
				// No check for other cases because parser errors might hit it and we don't want to stop the error checker
			}

			return node;
		}

		public override Node VisitPostfixExpression([NotNull] AmethystParser.PostfixExpressionContext context)
		{
			var node = (Expression)Visit(context.children.First());

			for (var i = 1; i < context.children.Count; i++)
			{
				if (context.children[i] is AmethystParser.ExpressionListContext call)
				{
					node = new CallExpression(Loc(call), node, Visit(call));
				}
				else if (context.children[i] is AmethystParser.IndexExpressionContext ind)
				{
					node = new IndexExpression(Loc(ind), node, Visit(ind.expression()));
				}
				else if (context.children[i] is AmethystParser.PropertyExpressionContext prop)
				{
					node = new PropertyExpression(Loc(prop), node, prop.RawIdentifier().GetText());
				}
				else
				{
					throw new NotImplementedException();
				}
			}

			return node;
		}

		public override Node VisitPrimaryExpression([NotNull] AmethystParser.PrimaryExpressionContext context)
		{
			if (context.id() is AmethystParser.IdContext id)
			{
				return new VariableExpression(Loc(context), Visit(id));
			}
			else if (context.String() is ITerminalNode str)
			{
				return new LiteralExpression(Loc(context), new NBTString(NBTString.Unescape(str.GetText()[1..^1])));
			}
			else if (context.Number() is ITerminalNode i)
			{
				return new LiteralExpression(Loc(context), ParseNumber(i.GetText()));
			}
			else if (context.expression() is AmethystParser.ExpressionContext expr)
			{
				return Visit(expr);
			}
			else
			{
				return Visit(context.children[0]);
			}
		}

		public override Node VisitListLiteral([NotNull] AmethystParser.ListLiteralContext context) => new ListExpression(Loc(context), [.. context.expression().Select(Visit)]);

		public override Node VisitCompoundLiteral([NotNull] AmethystParser.CompoundLiteralContext context) => new CompoundExpression(Loc(context), context.compoundKeyPair().Select(i =>
		{
			if (i.RawIdentifier() is not null)
			{
				return new KeyValuePair<string, Expression>(i.RawIdentifier().GetText(), Visit(i.expression()));
			}
			else
			{
				return new KeyValuePair<string, Expression>(i.RawIdentifier().GetText(), Visit(i.expression()));
			}
		}));

		public override Node VisitTargetSelector([NotNull] AmethystParser.TargetSelectorContext context)
		{
			var type = context.TargetSelectorVariable().GetText().Last() switch
			{
				'p' => TargetType.p,
				'e' => TargetType.e,
				'a' => TargetType.a,
				'n' => TargetType.n,
				's' => TargetType.s,
				_ => TargetType.r
			};

			var args = new Dictionary<string, Expression>();

			foreach (var i in context.targetSelectorArgument())
			{
				args[i.RawIdentifier().GetText()] = Visit(i.expression());
			}

			return new TargetSelectorExpression(Loc(context), type, args);
		}

		public AbstractTypeSpecifier Visit(AmethystParser.TypeContext context) => (AbstractTypeSpecifier)Visit((IParseTree)context);
		public BlockNode Visit(AmethystParser.BlockContext context) => (BlockNode)Visit((IParseTree)context);
		public Statement Visit(AmethystParser.StatementContext context) => (Statement)Visit((IParseTree)context);
		public Expression Visit(AmethystParser.ExpressionContext context) => (Expression)Visit((IParseTree)context);
		public List<Expression> Visit(AmethystParser.ExpressionListContext context) => [.. context.expression().Select(Visit)];

		public List<AbstractParameter> Visit(AmethystParser.ParamListContext context) => [..context.paramPair().Select(i => {
			var mod = ParameterModifiers.None;

			foreach (var e in i.paramModifier())
			{
				if (e.GetText() == "macro")
				{
					mod |= ParameterModifiers.Macro;
				}
			}

			return new AbstractParameter(mod, Visit(i.type()), Visit(i.id()));
		})];

		public static string Visit(AmethystParser.IdContext context) => context.GetText();

		public LocationRange Loc(ParserRuleContext ctx) => LocOffset(LocationUtils.From(Filename, ctx));

		public virtual Location LocOffset(Location loc) => loc;
		public virtual LocationRange LocOffset(LocationRange loc) => new(LocOffset(loc.Start), LocOffset(loc.End));

		public NamespacedID IdentifierToID(string name)
		{
			if (name.Contains(':'))
			{
				return new(name);
			}
			else
			{
				return new(currentNamespace, name);
			}
		}

		public static NBTValue ParseNumber(string raw)
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
					if (raw.Contains('.'))
					{
						return new NBTDouble(double.Parse(raw));
					}

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
