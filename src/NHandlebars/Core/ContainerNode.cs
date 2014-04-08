using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NHandlebars.Core
{
	internal sealed class ContainerNode : Node
	{
		private readonly IEnumerable<Node> _nodes;

		public ContainerNode(IEnumerable<Node> nodes)
		{
			_nodes = nodes ?? Enumerable.Empty<Node>();
		}

		public override void Write(TextWriter writer, Context context)
		{
			foreach (var node in _nodes)
			{
				node.Write(writer, context);
			}
		}
	}
}