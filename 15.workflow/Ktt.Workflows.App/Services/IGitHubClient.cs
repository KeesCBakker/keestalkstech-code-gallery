using Ktt.Workflows.App.Models;

namespace Ktt.Workflows.App.Services;

public interface IGitHubClient
{
    /// <summary>
    /// Creates a new branch in the specified repository if it doesn't exist.
    /// </summary>
    /// <param name="repository">The repository name</param>
    /// <param name="branchName">The branch name</param>
    /// <returns>True if the branch was created, false if it already existed</returns>
    Task<bool> CreateBranchIfNotExistsAsync(string repository, string branchName);

    /// <summary>
    /// Edits a file in the specified repository and branch.
    /// </summary>
    /// <param name="repository">The repository name</param>
    /// <param name="branchName">The branch name</param>
    /// <param name="filePath">The path to the file</param>
    /// <param name="content">The new content of the file</param>
    /// <returns>True if the file was edited successfully, false otherwise</returns>
    Task<bool> EditFileAsync(string repository, string branchName, string filePath, string content);

    /// <summary>
    /// Gets the content of a file from the specified repository and branch.
    /// </summary>
    /// <param name="repository">The repository name</param>
    /// <param name="branchName">The branch name</param>
    /// <param name="filePath">The path to the file</param>
    /// <returns>The content of the file as a string</returns>
    Task<string> GetFileAsync(string repository, string branchName, string filePath);

    /// <summary>
    /// Checks if a file exists in the specified repository and branch.
    /// </summary>
    /// <param name="repository">The repository name</param>
    /// <param name="branchName">The branch name</param>
    /// <param name="filePath">The path to the file</param>
    /// <returns>True if the file exists, false otherwise</returns>
    Task<bool> FileExistsAsync(string repository, string branchName, string filePath);
}
