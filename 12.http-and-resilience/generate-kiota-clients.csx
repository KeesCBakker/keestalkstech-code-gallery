#! "dotnet-script"
#nullable enable

string Namespace => "Ktt.Resilience.Clients.Kiota";
string HttpClientsDir => Namespace;
string DefaultWorkingDir => Path.GetFullPath(HttpClientsDir);

// 1. Install Kiota
Console.WriteLine("🔄 Installing (or updating) Kiota tool...");
Run("dotnet tool install --global Microsoft.OpenApi.Kiota", Environment.CurrentDirectory, quiet: false);

// 2. Make sure HTTP project is present
Console.WriteLine("🔄 Ensure HTTP projects is configured correctly...");
if (!Directory.Exists(DefaultWorkingDir))
  throw new DirectoryNotFoundException($"❌ HTTP client project folder not found at '{DefaultWorkingDir}'");

// 3. Install packages
Run("dotnet add package Microsoft.Extensions.DependencyInjection");
Run("dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions");
Run("dotnet add package Microsoft.Extensions.Options");
Run("dotnet add package Microsoft.Extensions.Http.Resilience");
Run("dotnet add package Microsoft.Extensions.Resilience");
Run("dotnet add package Microsoft.Kiota.Bundle");

// 4. Generate Kiota clients
Console.WriteLine("🔄 Generating Kiota clients...");

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
void Run(string fullCommand, string? workingDir = null, bool quiet = true)
{
  var parts = fullCommand.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
  var psi = new ProcessStartInfo
  {
    FileName = parts[0],
    Arguments = parts.Length > 1 ? parts[1] : "",
    WorkingDirectory = workingDir ?? DefaultWorkingDir,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false
  };

  using var proc = Process.Start(psi)
      ?? throw new Exception($"Failed to start: {parts[0]}");

  string stdout = proc.StandardOutput.ReadToEnd();
  string stderr = proc.StandardError.ReadToEnd();
  proc.WaitForExit();

  if (!quiet && !string.IsNullOrWhiteSpace(stdout))
    Console.WriteLine(stdout);

  if (!string.IsNullOrWhiteSpace(stderr))
    Console.Error.WriteLine(stderr);

  if (proc.ExitCode != 0)
    throw new Exception($"{parts[0]} exited with code {proc.ExitCode}");
}

void GenerateKiotaClient(string openApiUrl, string clientName, string includePath = "/**")
{
  if (!openApiUrl.Contains("://"))
    openApiUrl = Path.Combine("..", openApiUrl);

  Console.WriteLine($"▶ Generating client '{clientName}' in {DefaultWorkingDir}...");
  Run($"""
    kiota generate 
      --openapi {openApiUrl} 
      --language CSharp 
      --output HttpClients/{clientName} 
      --namespace-name {Namespace}.HttpClients.{clientName} 
      --class-name {clientName}Client 
      --exclude-backward-compatible 
      --clean-output 
      --clear-cache 
      --include-path {includePath}
  """.ReplaceLineEndings(" ").Trim(), quiet: false);

  Console.WriteLine($"✔ Generated client: '{clientName}'");
}
