# OpenCode Configuration

Scripts and configuration fragments for installing and configuring OpenCode on
Windows.

## Quick start

Node.js must be installed because the merge requires `npx`. The script stops
before reading or changing the central configuration when `npx` is unavailable.

Run the merge from this directory:

```powershell
./merge-config.ps1
```

The script finds the central `opencode.jsonc` or `opencode.json`, creates a
timestamped backup, and shows a preflight summary. It then:

- Adds missing configuration values.
- Asks before replacing different values.
- Asks before adding or replacing MCPs.
- Creates missing MCP secret files without overwriting existing files.
- Merges watcher ignore patterns.
- Uses Edikt for JSONC-preserving edits.
- Formats the result with Prettier when available.
- Validates the result with `opencode debug config`.
- Checks and optionally installs the configured global OpenCode skills.

If there are no configuration changes, Edikt and Prettier are skipped.

To run the current version without cloning the repository:

```powershell
& ([scriptblock]::Create((irm "https://raw.githubusercontent.com/KeesCBakker/keestalkstech-code-gallery/main/15.opencode/run-merge.ps1")))
```

The launcher downloads the scripts and configuration fragments into a unique
temporary directory, runs the merge, and removes the directory afterwards. Use
a pinned branch or tag instead of `main` when reproducibility is important.

## Configuration files

The configuration fragments are kept in the `config` directory:

- `config/opencode.jsonc`: central configuration fragment
- `config/opencode-mcps.jsonc`: MCP servers
- `config/opencode-ask.jsonc`: command permissions
- `config/opencode-config-files.jsonc`: access to configuration files
- `config/opencode-watcher.jsonc`: watcher ignore patterns
- `config/opencode-skills.yaml`: optional skills and their repositories

OpenCode does not automatically include arbitrary JSONC files; the merge script
combines these fragments with the central configuration.

After a successful config merge, the merge script also checks the optional
skills. It skips skills that are already installed and asks for approval before
installing each missing skill. To run only the skill check separately, use:

```powershell
./install-skills.ps1
```

Configured skills:

The list is maintained in `config/opencode-skills.yaml` and is read by
`install-skills.ps1`.

## Secrets

MCP credentials are referenced from files under the central OpenCode secrets
directory. Existing files are never overwritten. Missing values are requested
interactively and written only when a value is provided.

The project contains references only, never secret values:

```text
{file:./secrets/context7-api-key}
{file:./secrets/slack-client-id}
{file:./secrets/slack-client-secret}
```

## Notes

- The central configuration is changed only after validation succeeds.
- Existing comments and formatting are preserved during Edikt edits where possible.
- Prettier is best-effort; a formatting failure does not block a valid merge.
- Edikt is downloaded to a temporary directory for each run and removed afterwards.
- The generated local config is ignored by Git.
