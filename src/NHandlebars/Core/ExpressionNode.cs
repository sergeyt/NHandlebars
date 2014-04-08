using System;
using System.IO;

namespace NHandlebars.Core
{
	internal sealed class ExpressionNode : Node
	{
		private readonly string _expression;

		public ExpressionNode(string expression)
		{
			_expression = expression.Trim();
		}

		public override void Write(TextWriter writer, Context context)
		{
			var value = context.GetValue(_expression);
			if (value.IsNullOrNoValue()) return;

			// TODO escape
			writer.Write(Convert.ToString(value));
		}
	}
}