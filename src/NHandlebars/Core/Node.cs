using System.IO;

namespace NHandlebars.Core
{
	/// <summary>
	/// Template node.
	/// </summary>
	internal abstract class Node
	{
		public abstract void Write(TextWriter writer, Context context);

		public static readonly Node Null = new NullNode();

		private class NullNode : Node
		{
			public override void Write(TextWriter writer, Context context)
			{
			}
		}
	}

	internal sealed class TextNode : Node
	{
		private readonly string _text;

		public TextNode(string text)
		{
			_text = text ?? "";
		}

		public override void Write(TextWriter writer, Context context)
		{
			writer.Write(_text);
		}
	}
}
