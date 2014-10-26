using System.IO;

namespace NHandlebars.Core
{
	internal sealed class IfNode : Node
	{
		private readonly string _expression;
		private readonly Node _true;
		private readonly Node _false;

		public IfNode(string expression, Node trueBlock, Node falseBlock)
		{
			_expression = expression;
			_true = trueBlock ?? Null;
			_false = falseBlock ?? Null;
		}

		public override void Write(TextWriter writer, Context context)
		{
			if (context.GetValue(_expression).IsTrue())
			{
				_true.Write(writer, context);
			}
			else
			{
				_false.Write(writer, context);
			}
		}
	}

	internal sealed class UnlessNode : Node
	{
		private readonly string _expression;
		private readonly Node _content;
		private readonly Node _else;

		public UnlessNode(string expression, Node unlessBlock, Node elseBlock)
		{
			_expression = expression;
			_content = unlessBlock ?? Null;
			_else = elseBlock ?? Null;
		}

		public override void Write(TextWriter writer, Context context)
		{
			if (context.GetValue(_expression).IsFalse())
			{
				_content.Write(writer, context);
			}
			else
			{
				_else.Write(writer, context);
			}
		}
	}
}
