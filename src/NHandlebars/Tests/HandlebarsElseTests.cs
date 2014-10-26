#if NUNIT
using NUnit.Framework;

namespace NHandlebars.Tests
{
	/// <summary>
	/// From https://github.com/sergeyt/NHandlebars/issues/4
	/// </summary>
	[TestFixture]
	public class HandlebarsElseTests
	{
		private const string Template = "{{#each County}}" +
											"{{#if @first}}" +
												"{{this}}" +
											"{{else}}" +
												"{{#if @last}}" +
													" and {{this}}" +
												"{{else}}" +
													", {{this}}" +
												"{{/if}}" +
											"{{/if}}" +
										"{{/each}}";

		[Test]
		public void CountyHasOneValue()
		{
			var options = new
			{
				County = new[] { "Kane" }
			};

			var actual = Handlebars.Render(Template, options);

			Assert.That(actual, Is.EqualTo("Kane"));
		}

		[Test]
		public void CountyHasTwoValue()
		{
			var options = new
			{
				County = new[] { "Kane", "Salt Lake" }
			};

			var actual = Handlebars.Render(Template, options);

			Assert.That(actual, Is.EqualTo("Kane and Salt Lake"));
		}

		[Test]
		public void CountyHasMoreThanTwoValue()
		{
			var options = new
			{
				County = new[] { "Kane", "Salt Lake", "Weber" }
			};

			var actual = Handlebars.Render(Template, options);

			Assert.That(actual, Is.EqualTo("Kane, Salt Lake and Weber"));
		}
	}
}
#endif
