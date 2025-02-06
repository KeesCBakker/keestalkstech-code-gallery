# .NET Console Application with injectable commands
How to use `System.CommandLine` to build a CLI with commands and dependency injection.

The code is a companion of the blog <a href="https://keestalkstech.com/2023/03/net-console-application-with-injectable-commands/">.NET Console Application with injectable commands</a>.

## Command to execute

`System.CommandLine` is pretty awesome.

```sh

cd MyCli

# show help:
dotnet run -- --help
dotnet run current -- --help
dotnet run forecast -- --help

# execute current command:
dotnet run current
dotnet run current --city Zwolle

# execute forecast command:
dotnet run forecast
dotnet run forecast --city Zwolle

```