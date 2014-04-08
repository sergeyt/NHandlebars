#if NUNIT
using System;
using NUnit.Framework;

namespace NHandlebars.Tests
{
	[TestFixture]
	public class HandlebarsTests
	{
		[TestCase(null, ExpectedException = typeof(ArgumentNullException))]
		[TestCase("", Result = "")]
		[TestCase("abc", Result = "abc")]
		[TestCase("{{value}}", Result = "test")]
		[TestCase("before {{value}} after", Result = "before test after")]
		[TestCase("{", Result = "{")]
		[TestCase("}", Result = "}")]
		[TestCase("{{", Result = "{{")]
		[TestCase("}}", Result = "}}")]
		[TestCase("{{{", Result = "{{{")]
		[TestCase("}}}", Result = "}}}")]
		[TestCase("{{{{", ExpectedException = typeof(FormatException))]
		[TestCase("}}}}", Result = "}}}}")]
		[TestCase("{{a", ExpectedException = typeof(FormatException))]
		[TestCase("{{{a", ExpectedException = typeof(FormatException))]
		public string Simple(string input)
		{
			return Handlebars.Render(input, new {value = "test"});
		}

		[TestCase("{{#if val}}test{{/if}}", false, Result = "")]
		[TestCase("{{#if val}}test{{/if}}", null, Result = "")]
		[TestCase("{{#if val}}test{{/if}}", "", Result = "")]
		[TestCase("{{#if val}}test{{/if}}", 0, Result = "test")]
		[TestCase("{{#if val}}test{{/if}}", true, Result = "test")]
		[TestCase("{{#if val}}test{{/if}}", 1, Result = "test")]
		[TestCase("{{#if val}}test{{/if}}", "ok", Result = "test")]
		// surrounded
		[TestCase("[{{#if val}}test{{/if}}]", false, Result = "[]")]
		[TestCase("[{{#if val}}test{{/if}}]", null, Result = "[]")]
		[TestCase("[{{#if val}}test{{/if}}]", "", Result = "[]")]
		[TestCase("[{{#if val}}test{{/if}}]", 0, Result = "[test]")]
		[TestCase("[{{#if val}}test{{/if}}]", true, Result = "[test]")]
		[TestCase("[{{#if val}}test{{/if}}]", 1, Result = "[test]")]
		[TestCase("[{{#if val}}test{{/if}}]", "ok", Result = "[test]")]
		public string If(string input, object value)
		{
			return Handlebars.Render(input, new {val = value});
		}

		[TestCase("{{#unless val}}test{{/unless}}", false, Result = "test")]
		[TestCase("{{#unless val}}test{{/unless}}", null, Result = "test")]
		[TestCase("{{#unless val}}test{{/unless}}", "", Result = "test")]
		[TestCase("{{#unless val}}test{{/unless}}", 0, Result = "")]
		[TestCase("{{#unless val}}test{{/unless}}", true, Result = "")]
		[TestCase("{{#unless val}}test{{/unless}}", 1, Result = "")]
		[TestCase("{{#unless val}}test{{/unless}}", "ok", Result = "")]
		// surrounded
		[TestCase("[{{#unless val}}test{{/unless}}]", false, Result = "[test]")]
		[TestCase("[{{#unless val}}test{{/unless}}]", null, Result = "[test]")]
		[TestCase("[{{#unless val}}test{{/unless}}]", "", Result = "[test]")]
		[TestCase("[{{#unless val}}test{{/unless}}]", 0, Result = "[]")]
		[TestCase("[{{#unless val}}test{{/unless}}]", true, Result = "[]")]
		[TestCase("[{{#unless val}}test{{/unless}}]", 1, Result = "[]")]
		[TestCase("[{{#unless val}}test{{/unless}}]", "ok", Result = "[]")]
		public string Unless(string input, object value)
		{
			return Handlebars.Render(input, new { val = value });
		}

		// TODO with, each tests
	}
}
#endif
