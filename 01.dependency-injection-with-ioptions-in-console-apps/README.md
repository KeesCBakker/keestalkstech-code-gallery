# Dependency injection (with IOptions) in Console Apps in .NET

When you are used to building web applications, you kind of get hooked to the 
ease of Dependency Injection (DI) and the way settings can be specified in a 
JSON file and accessed through DI (``IOptions<T>``). It's only logical to 
want the same features in your Console app.

<a href="https://keestalkstech.com/2018/04/dependency-injection-with-ioptions-in-console-apps-in-dotnet/">Read the blog on KeesTalksTech: Dependency injection (with IOptions) in Console Apps in .NET</a>

## Features

- Support for .NET 10
- Support for <a href="https://learn.microsoft.com/en-us/dotnet/api/system.commandline.rootcommand">System.CommandLine.RootCommand</a>
- Support for appsettings.json configuration with environment-specific overrides (``appsettings.{ENVIRONMENT}.json``)
- Support for environment variables configuration
- Support for IOption injection
- Support for IOption data validation on startup

## Configuration

The application uses the `ENVIRONMENT` environment variable to load environment-specific settings:

- `appsettings.json` — base settings for all environments
- `appsettings.{ENVIRONMENT}.json` — environment-specific overrides (optional)

See <a href="https://keestalkstech.com/2019/01/setup-multiple-setting-files-with-a-net-console-application/">Setup Multiple Settings Files with a .NET Console Application</a> for more details.

