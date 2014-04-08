using System;
using System.IO;

namespace NHandlebars.Core
{
	internal sealed class WithNode : Node
	{
		private readonly string _expression;
		private readonly Node _content;

		public WithNode(string expression, Node content)
		{
			_expression = expression;
			_content = content ?? Node.Null;
		}

		public override void Write(TextWriter writer, Context context)
		{
			var value = context.GetValue(_expression);
			if (value.IsNullOrNoValue()) return;

			try
			{
				context.Push(value);
				_content.Write(writer, context);
			}
			catch (Exception)
			{
				context.Pop();
			}
		}
	}
}