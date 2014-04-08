using System.IO;

namespace NHandlebars.Core
{
	internal sealed class IfNode : Node
	{
		private readonly string _expression;
		private readonly Node _content;

		public IfNode(string expression, Node content)
		{
			_expression = expression;
			_content = content ?? Null;
		}

		public override void Write(TextWriter writer, Context context)
		{
			if (context.GetValue(_expression).IsFalse())
				return;

			_content.Write(writer, context);
		}
	}

	internal sealed class UnlessNode : Node
	{
		private readonly string _expression;
		private readonly Node _content;

		public UnlessNode(string expression, Node content)
		{
			_expression = expression;
			_content = content ?? Null;
		}

		public override void Write(TextWriter writer, Context context)
		{
			if (!context.GetValue(_expression).IsFalse())
				return;

			_content.Write(writer, context);
		}
	}
}
