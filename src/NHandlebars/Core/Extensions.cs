using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Patronix
{
	/// <summary>
	/// Provides utility extension methods.
	/// </summary>
	internal static class Extensions
	{
		/// <summary>
		/// Gets value of the attribute with specified name.
		/// </summary>
		/// <param name="element">The xml element to get attribute of.</param>
		/// <param name="name">The name of attribute to find.</param>
		/// <returns>Value of the specified attribute or null if the attribute is not found.</returns>
		public static string GetAttribute(this XElement element, string name)
		{
			var attr = element.Attribute(name);
			return attr != null ? attr.Value : null;
		}

		/// <summary>
		/// Escapes reserved HTML chars like '&lt;'.
		/// </summary>
		/// <param name="text">The text where replace chars.</param>
		/// <returns>The same or escaped string.</returns>
		public static string HtmlEncode(this string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			var sb = new StringBuilder(text.Length);

			foreach (char c in text)
			{
				switch (c)
				{
					case '<':
						sb.Append("&lt;");
						break;
					case '>':
						sb.Append("&gt;");
						break;
					case '"':
						sb.Append("&quot;");
						break;
					case '\'':
						sb.Append("&apos;");
						break;
					case '&':
						sb.Append("&amp;");
						break;
					default:
						if (c > 159)
						{
							sb.Append("&#");
							sb.Append(((int)c).ToString(CultureInfo.InvariantCulture));
							sb.Append(";");
						}
						else
						{
							sb.Append(c);
						}
						break;
				}
			}

			return sb.ToString();
		}
	}
}