using System;
using System.IO;

namespace NHandlebars.Core
{
	internal sealed class EachNode : Node
	{
		private readonly string _expression;
		private readonly Node _content;

		public EachNode(string expression, Node content)
		{
			_expression = expression;
			_content = content ?? Null;
		}

		public override void Write(TextWriter writer, Context context)
		{
			var items = context.GetValues(_expression);

			foreach (var item in items)
			{
				try
				{
					context.Push(item);
					_content.Write(writer, context);
				}
				catch (Exception)
				{
					context.Pop();
				}
				
			}
		}
	}
}
