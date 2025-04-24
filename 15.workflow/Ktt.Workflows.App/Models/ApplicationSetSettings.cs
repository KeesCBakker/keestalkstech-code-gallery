namespace Ktt.Workflows.App.Models;

public record ApplicationSetSettings(
    string Environment,
    string EcrRepository,
    string ImageTag,
    int CpuCores,
    int RamMB
);
