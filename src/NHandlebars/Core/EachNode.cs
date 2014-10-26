using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			var items = context.GetValues(_expression).ToArray();

			int index = 0;
			foreach (var item in items)
			{
				try
				{
					var meta = new Dictionary<string, object>(StringComparer.Ordinal)
					{
						{"@index", index},
						{"@first", index == 0},
						{"@last", index == items.Length - 1}
					};

					context.Push(item, meta);
					_content.Write(writer, context);
					index++;
				}
				finally
				{
					context.Pop();
				}
			}
		}
	}
}
