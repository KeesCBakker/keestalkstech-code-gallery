#r "System.Diagnostics.Process"
#r "System.IO.FileSystem"

using System;
using System.IO;
using System.Diagnostics;

// --- Globals ---
string Namespace => "Ktt.Resilience.Clients.Kiota";
string HttpClientsDir => Namespace;
string FullHttpClientsDir => Path.GetFullPath(HttpClientsDir);

// --- Entry Point ---
EnsureKiotaInstalled();
EnsureHttpClientsProjectReady();

GenerateKiotaClient(
    openApiUrl: "https://petstore.swagger.io/v2/swagger.json",
    clientName: "PetStore",
    includePath: "/pet/**"
);

GenerateKiotaClient(
    openApiUrl: "httpstatus-open-api.yml",
    clientName: "HttpStatus"
);


// --- Helpers ---

string Quote(string s) => $"\"{s.Replace("\"", "\\\"")}\"";

void Run(string command, string[] args, string workingDir = null)
{
  var psi = new ProcessStartInfo
  {
    FileName = command,
    Arguments = string.Join(" ", args),
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    WorkingDirectory = workingDir ?? Environment.CurrentDirectory
  };

  using var proc = Process.Start(psi);
  if (proc == null) throw new Exception($"Failed to start: {command}");

  proc.OutputDataReceived += (_, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
  proc.ErrorDataReceived += (_, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };
  proc.BeginOutputReadLine();
  proc.BeginErrorReadLine();
  proc.WaitForExit();

  if (proc.ExitCode != 0)
    throw new Exception($"{command} exited with code {proc.ExitCode}");
}

void EnsureKiotaInstalled()
{
  Console.WriteLine("▶ Ensuring Kiota is installed...");

  try
  {
    Run("kiota", new[] { "--version" });
  }
  catch
  {
    Console.WriteLine("Kiota not found. Installing...");
    Run("dotnet", new[] { "tool", "install", "--global", "Microsoft.OpenApi.Kiota" });

    try
    {
      Run("kiota", new[] { "--version" });
    }
    catch
    {
      throw new Exception("❌ Kiota installation failed or is still not available in PATH.");
    }
  }
}

void EnsureHttpClientsProjectReady()
{
  Console.WriteLine("▶ Ensuring HttpClients project is ready...");

  if (!Directory.Exists(HttpClientsDir))
    throw new DirectoryNotFoundException($"❌ HTTP client project folder not found at '{HttpClientsDir}'");

  string[] packages =
  {
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Abstractions",
        "Microsoft.Extensions.Options",
        "Microsoft.Extensions.Http.Resilience",
        "Microsoft.Extensions.Resilience",
        "Microsoft.Kiota.Bundle"
    };

  foreach (var pkg in packages)
  {
    Run("dotnet", new[] { "add", "package", pkg }, FullHttpClientsDir);
  }
}

void GenerateKiotaClient(string openApiUrl, string clientName, string includePath = "/**")
{
  Console.WriteLine($"▶ Generating client '{clientName}'...");
  if (!openApiUrl.Contains("://"))
  {
    openApiUrl = Path.Combine("..", openApiUrl);
  }

  var @namespace = $"{Namespace}.HttpClients.{clientName}";
  var className = $"{clientName}Client";
  string[] args = new[]
  {
        "generate",
        "--openapi", Quote(openApiUrl),
        "--language", "CSharp",
        "--output", Quote($"HttpClients/{clientName}"),
        "--namespace-name", Quote(@namespace),
        "--class-name", Quote(className),
        "--exclude-backward-compatible",
        "--clean-output",
        "--clear-cache",
        "--include-path", Quote(includePath)
  };

  // Run Kiota in the correct working directory
  Run("kiota", args, FullHttpClientsDir);

  Console.WriteLine($"✔ Generated client: {clientName}");
}
