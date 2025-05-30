using Amethyst.Antlr;
using Amethyst.AST;
using Antlr4.Runtime;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst
{
	public class Compiler
	{
		public readonly Options Options;
		public readonly Dictionary<string, RootNode> Files = [];

		public Compiler(Options opts)
		{
			Options = opts;
		}

		public Compiler(string[] args)
		{
			var res = CommandLine.Parser.Default.ParseArguments<Options>(args);
			if (res.Errors.Any()) Environment.Exit(1);
			Options = res.Value;
			Options.Output ??= Path.GetFileName(Options.Inputs.First()) + ".zip";
		}

		public bool Compile()
		{
			var errored = false;
			foreach (var i in Options.Inputs)
			{
				if (!ParseFile(i)) errored = true;
			}

			if (errored) return false;

			return true;
		}

		public bool ParseFile(string filename)
		{
			var file = File.ReadAllText(filename);
			var input = new AntlrInputStream(file);
			var lexer = new AmethystLexer(input);
			var tokens = new CommonTokenStream(lexer);
			var parser = new AmethystParser(tokens);
			var visitor = new Visitor(filename);

			var error = new ParserErrorHandler(filename, file);
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(error);
			parser.RemoveErrorListeners();
			parser.AddErrorListener(error);

			try
			{
				Files[filename] = (RootNode)visitor.Visit(parser.root());
			}
			catch
			{
				if (!error.Errored) throw;
			}

			if (error.Errored) return false;

			return true;
		}
	}
}
