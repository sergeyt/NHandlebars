using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NHandlebars.Core;

namespace NHandlebars
{
	using RenderFn = Action<TextWriter, object>;

	/// <summary>
	/// Handlebars template system.
	/// </summary>
	public static class Handlebars
	{
		/// <summary>
		/// Renders given template with given data.
		/// </summary>
		/// <param name="template">The template to render.</param>
		/// <param name="data">The data to apply.</param>
		/// <returns>The template output.</returns>
		public static string Render(string template, object data)
		{
			if (template == null) throw new ArgumentNullException("template");
			if (data == null) throw new ArgumentNullException("data");

			return Render(Compile(template), data);
		}

		/// <summary>
		/// Renders given template with given data.
		/// </summary>
		/// <param name="template">The template source to parse.</param>
		/// <param name="data">The data to render.</param>
		/// <returns>The template output.</returns>
		public static string Render(TextReader template, object data)
		{
			if (template == null) throw new ArgumentNullException("template");
			if (data == null) throw new ArgumentNullException("data");

			return Render(Compile(template), data);
		}

		/// <summary>
		/// Renders given template with given data.
		/// </summary>
		/// <param name="template">The template source to parse.</param>
		/// <param name="output">The output for template result.</param>
		/// <param name="data">The data to render.</param>
		public static void Render(TextReader template, TextWriter output, object data)
		{
			if (template == null) throw new ArgumentNullException("template");
			if (output == null) throw new ArgumentNullException("output");
			if (data == null) throw new ArgumentNullException("data");

			var render = Compile(template);
			render(output, data);
		}

		private static string Render(RenderFn template, object data)
		{
			var sb = new StringBuilder();

			using (var writer = new StringWriter(sb))
				template(writer, data);

			return sb.ToString();
		}

		/// <summary>
		/// Compiles given text template into template rendering function.
		/// </summary>
		/// <param name="template">The input template to parse.</param>
		/// <returns>The function to render template.</returns>
		public static RenderFn Compile(string template)
		{
			if (template == null) throw new ArgumentNullException("template");

			using (var reader = new StringReader(template))
				return Compile(reader);
		}

		/// <summary>
		/// Compiles given text template into template rendering function.
		/// </summary>
		/// <param name="template">The input template to parse.</param>
		/// <returns>The function to render template.</returns>
		public static RenderFn Compile(TextReader template)
		{
			if (template == null) throw new ArgumentNullException("template");

			var node = CompileNode(template);
			return (writer, dto) => node.Write(writer, new Context(dto));
		}

		private static Node CompileNode(TextReader input)
		{
			var stack = new Stack<Block>();
			stack.Push(new Block());

			foreach (var token in Scan(input))
			{
				if (token.Kind == TokenKind.Text)
				{
					stack.Peek().Add(new TextNode(token.Value));
					continue;
				}

				var expr = token.Value;
				if (expr[0] == '#')
				{
					var type = "";
					for (var i = 1; i < expr.Length; i++)
					{
						if (char.IsWhiteSpace(expr[i]))
						{
							expr = expr.Substring(i + 1);
							break;
						}
						type += expr[i];
					}

					stack.Push(Block.Create(type, expr));
				}
				else if (expr[0] == '/') // end of block
				{
					if (stack.Count < 2)
						throw new FormatException("Unbalanced blocks");

					var type = expr.Substring(1).Trim();
					var block = stack.Pop();
					if (block.Type != type)
						throw new FormatException("Unmatched close tag");

					stack.Peek().Add(block.ToNode());
				}
				else if (expr == "else")
				{
					var block = stack.Peek();
					if (block.Kind == BlockKind.If || block.Kind == BlockKind.Unless)
					{
						block.EnterElse();
					}
					else
					{
						throw new FormatException("Invalid else usage");
					}
				}
				else
				{
					stack.Peek().Add(
						token.Kind == TokenKind.UnescapedExpression
							? (Node) new UnescapedExpressionNode(expr)
							: new ExpressionNode(expr)
						);
				}
			}

			if (stack.Count > 1)
			{
				throw new FormatException("There is unclosed block");
			}

			return stack.Pop().ToNode();
		}

		private sealed class Block
		{
			private string _expression;
			private readonly List<Node> _nodes = new List<Node>();
			private readonly List<Node> _elseNodes = new List<Node>();
			private bool _inElseBlock;

			public void EnterElse()
			{
				_inElseBlock = true;
			}

			public void Add(Node node)
			{
				if (_inElseBlock) _elseNodes.Add(node);
				else _nodes.Add(node);
			}

			public BlockKind Kind { get; private set; }
			public string Type { get { return Kind.ToString().ToLowerInvariant(); } }

			public static Block Create(string type, string expr)
			{
				switch (type)
				{
					case "with":
						return new Block
						{
							Kind = BlockKind.With,
							_expression = expr,
						};
					case "each":
						return new Block
						{
							Kind = BlockKind.Each,
							_expression = expr,
						};
					case "if":
						return new Block
						{
							Kind = BlockKind.If,
							_expression = expr,
						};
					case "unless":
						return new Block
						{
							Kind = BlockKind.Unless,
							_expression = expr,
						};
					default:
						throw new NotSupportedException(string.Format("Unsupported block '{0}'", type));
				}
			}

			public Node ToNode()
			{
				switch (Kind)
				{
					case BlockKind.Container:
						return new ContainerNode(_nodes);
					case BlockKind.With:
						return new WithNode(_expression, new ContainerNode(_nodes));
					case BlockKind.Each:
						return new EachNode(_expression, new ContainerNode(_nodes));
					case BlockKind.If:
						return new IfNode(_expression, new ContainerNode(_nodes), new ContainerNode(_elseNodes));
					case BlockKind.Unless:
						return new UnlessNode(_expression, new ContainerNode(_nodes), new ContainerNode(_elseNodes));
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private enum BlockKind { Container, With, Each, If, Unless }

		static IEnumerable<Token> Scan(TextReader input)
		{
			var c = input.Read();

			var token = new StringBuilder();
			while (c >= 0)
			{
				int n = 0;
				while (c == '{' && n < 3)
				{
					c = input.Read();
					n++;
				}

				if (n > 1)
				{
					if (token.Length > 0)
					{
						var val = token.ToString();
						token.Length = 0;
						yield return new Token(TokenKind.Text, val);
					}

					// read expression
					while (c >= 0)
					{
						var k = 0;
						while (c == '}' && k < n)
						{
							c = input.Read();
							k++;
						}

						if (k == n)
						{
							if (token.Length == 0)
								throw new FormatException("Empty expression");

							var kind = n == 3 ? TokenKind.UnescapedExpression : TokenKind.Expression;
							var expr = token.ToString();
							n = 0;
							token.Length = 0;
							yield return new Token(kind, expr);
							break;
						}

						token.Append((char) c);
						c = input.Read();
					}

					if (token.Length > 0)
						throw new FormatException("Expression is not closed");
				}
				
				for (var i = 0; i < n; i++)
					token.Append('{');

				if (c >= 0 && c != '{')
				{
					token.Append((char)c);
					c = input.Read();
				}
			}

			if (token.Length > 0)
			{
				yield return new Token(TokenKind.Text, token.ToString());
			}
		}

		private struct Token
		{
			public readonly TokenKind Kind;
			public readonly string Value;

			public Token(TokenKind kind, string value)
			{
				Kind = kind;
				Value = value;
			}
		}

		private enum TokenKind
		{
			Text,
			Expression,
			UnescapedExpression,
		}
	}
}
