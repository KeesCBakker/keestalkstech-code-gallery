# Option Injection

Options themselves can be collections as well. This
project shows how to use a `Dictionary<string, string>` and a
`List<string>` as base classes for options. Both are supported
by default.

## Features

- Support for .NET 10
- Support for Dictionary-style options
- Support for List-style options
- Support for IOption data validation on startup

## Configuration

The application uses `SourceOptions` and `SupportedLanguagesOptions` from `appsettings.json`:

- `SourceOptions` — dictionary of source names to connection strings
- `SupportedLanguagesOptions` — list of supported language codes

