[![Build Status](https://drone.io/github.com/sergeyt/NHandlebars/status.png)](https://drone.io/github.com/sergeyt/NHandlebars/latest)
[![NuGet Version](http://img.shields.io/nuget/v/NHandlebars.svg?style=flat)](https://www.nuget.org/packages/NHandlebars/)
[![NuGet Downloads](http://img.shields.io/nuget/dt/NHandlebars.svg?style=flat)](https://www.nuget.org/packages/NHandlebars/)

# NHandlebars

[Handlebars](http://handlebarsjs.com/) template system for .NET

## Examples

### Using Handlebars.Render

```c#
using System;
using NHandlebars;

static class Example
{
	public static void Main()
	{
		var output = Handlebars.Render("hi, {{user}}!", new { user = Environment.UserName });
		Console.WriteLine(output);
	}
}
```

### Using Handlebars.Compile

```c#
using System;
using NHandlebars;

static class Example
{
	public static void Main()
	{
		var template = Handlebars.Compile("hi, {{user}}!");
		template(Console.Out, new { user = Environment.UserName });
		template(Console.Out, new { "man" });
	}
}
```
