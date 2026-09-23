# OpenCode Configuration

Scripts and configuration fragments for installing and configuring OpenCode on
Windows.

## Quick start

Install OpenCode first. On Windows, run PowerShell:

```powershell
./install-opencode.ps1
```

On Linux, run the shell installer:

```sh
bash install-opencode.sh
```

The Windows installer uses WinGet. The Linux installer uses OpenCode's official
Linux installer. Both check that `opencode` and `bun` are available before you
continue.

From either platform, install the pinned dependencies and run the TypeScript
merge from this directory:

```powershell
bun install --frozen-lockfile
bun run merge
```

The script finds the central `opencode.jsonc` or `opencode.json`, creates a
timestamped backup, and shows a preflight summary. It then:

- Adds missing configuration values.
- Asks before replacing different values.
- Asks before adding or replacing MCPs.
- Creates missing MCP secret files without overwriting existing files.
- Merges watcher ignore patterns.
- Uses Microsoft's `jsonc-parser` for targeted, comment-preserving JSONC edits.
- Formats changed configuration with the project Prettier settings before validation.
- Validates the result with `opencode debug config`.
- Checks and optionally installs the configured global OpenCode skills.

If there are no configuration changes, the central file is left byte-for-byte unchanged.

To run the current version without cloning the repository, use this command in
PowerShell or a Linux shell. It downloads the small launcher to a temporary
file and starts it with the terminal attached, so interactive prompts work.

```sh
bun -e 'const fs=await import("node:fs/promises"),path=await import("node:path"),os=await import("node:os"),ref="main",dir=await fs.mkdtemp(path.join(os.tmpdir(),"opencode-"));try{const response=await fetch(`https://raw.githubusercontent.com/KeesCBakker/keestalkstech-code-gallery/${ref}/15.opencode/run-merge.ts`);if(!response.ok)throw Error(`Download failed: HTTP ${response.status}`);const entry=path.join(dir,"run-merge.ts");await Bun.write(entry,response);const child=Bun.spawn(["bun","run",entry,"--ref",ref],{stdin:"inherit",stdout:"inherit",stderr:"inherit"});process.exitCode=await child.exited}finally{await fs.rm(dir,{recursive:true,force:true})}'
```

The small launcher downloads the pinned package metadata and hands control to
`merge-config.ts`. The main program downloads all files in its `config`
directory, performs the merge, and removes the temporary directory when it
exits. Use a commit SHA instead of `main` when reproducibility is important.
Set the SHA in `ref`; the program and configuration files are then taken from
that revision.

The PowerShell scripts remain available as a fallback during the TypeScript
migration:

```powershell
./merge-config.ps1
```

## Configuration files

The configuration fragments are kept in the `config` directory:

- `config/opencode.jsonc`: central configuration fragment
- `config/opencode-mcps.jsonc`: MCP servers
- `config/opencode-ask.jsonc`: command permissions
- `config/opencode-config-files.jsonc`: access to configuration files
- `config/opencode-watcher.jsonc`: watcher ignore patterns
- `config/opencode-skills.yaml`: optional skills and their repositories
- `merge-config.ts`: Bun-based JSONC merge
- `run-merge.ts`: remote Bun launcher

OpenCode does not automatically include arbitrary JSONC files; the merge script
combines these fragments with the central configuration.

After a successful config merge, the merge program checks the optional skills.
It skips skills that are already installed and asks before installing each
missing skill. The skill list lives in `config/opencode-skills.yaml`.

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
- Existing comments and formatting are preserved by targeted `jsonc-parser` edits.
- `@clack/prompts`, `jsonc-parser`, `prettier`, `yaml`, and the `skills` CLI are pinned in `bun.lock`.
- The generated local config is ignored by Git.

## Tests

```powershell
bun run test
```
