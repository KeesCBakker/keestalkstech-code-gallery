namespace Ktt.Workflows.App.Models;

public record PostgresSettings(
    string Name,
    string Environment,
    string Team,
    string InstanceType,
    string PostgresVersion,
    int StorageGB
);
