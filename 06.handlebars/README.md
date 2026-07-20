# Handlebars.Net & JSON templates

I ❤️ Handlebars! So I was very very very happy to see that Handlebars was ported to .NET! It is a mega flexible templating engine as it can easily be extended. I'm working on a project where I need to parse objects via JSON templates to JSON strings. This project shows how to instruct Handlebars to parse into JSON and add some nice error messages if your template fails.

More information can be found in these blogs:
- <a href="https://keestalkstech.com/2022/09/handlebars-net-json-templates/">Handlebars.Net & JSON templates</a>
- <a href="https://keestalkstech.com/2022/09/handlebars-net-fun-with-flags/">Handlebars.Net: Fun with [Flags]</a>

## Features

- Support for .NET 10
- Support for JSON templates via <a href="https://github.com/Handlebars-Net/Handlebars.Net">Handlebars.Net</a>
- Support for JSON validation with enriched error messages
- Support for <a href="https://learn.microsoft.com/en-us/dotnet/api/system.flagsattribute">[Flags]</a> enums in templates
- Custom <code>JsonTextEncoder</code> for proper JSON encoding
- Loading templates from embedded resources

## Configuration

The solution has no external configuration dependencies. Templates and data are provided at runtime through the <code>IJsonTemplateGenerator</code> interface.
