# OpenCode

Installing and configuring OpenCode on Windows.

This project contains the code and configuration examples for the OpenCode
installation and configuration guide.

## Contents

- OpenCode installation
- Provider configuration
- MCP configuration
- Permission configuration
- Skill installation

## Configuration files

The configuration fragments are kept in the `config` directory:

- `config/opencode.jsonc`: central configuration fragment
- `config/opencode-mcps.jsonc`: MCP servers
- `config/opencode-ask.jsonc`: command permissions
- `config/opencode-config-files.jsonc`: access to configuration files
- `config/opencode-watcher.jsonc`: watcher ignore patterns
- `config/opencode-skills.yaml`: optional skills and their repositories

OpenCode does not automatically include arbitrary JSONC files. Run
`./merge-config.ps1` to merge these fragments into the central OpenCode
configuration. The script creates a timestamped backup, asks before replacing
conflicting values, asks before adding MCPs, and prompts for missing secret
values.

Node.js must be installed because the merge requires `npx` for Prettier and
skill management. The script stops before reading or changing the central
configuration when `npx` is unavailable.

The merge uses Edikt for the edits and then reformats the resulting JSONC with
Prettier. It checks for `prettier` first and then uses `npx --yes prettier`,
which downloads Prettier when it is not installed locally. If neither command
is available, the merge continues without formatting. Edikt is downloaded to
a temporary directory for each run and removed afterwards.

```powershell
./merge-config.ps1
```

To run the current merge script without cloning the repository, download the
temporary launcher from GitHub:

```powershell
& ([scriptblock]::Create((irm "https://raw.githubusercontent.com/KeesCBakker/keestalkstech-code-gallery/main/15.opencode/run-merge.ps1")))
```

The launcher downloads the merge script and JSONC fragments to a unique
temporary directory, runs the merge there, and removes the directory when it
finishes. Use a pinned branch or tag instead of `main` when reproducibility is
important.

After a successful config merge, the merge script also checks the optional
skills. It skips skills that are already installed and asks for approval before
installing each missing skill. To run only the skill check separately, use:

```powershell
./install-skills.ps1
```

Configured skills:

The list is maintained in `config/opencode-skills.yaml` and is read by
`install-skills.ps1`.
