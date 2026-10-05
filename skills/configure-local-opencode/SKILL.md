---
name: configure-local-opencode
description: Use when creating or configuring a directory-specific opencode.json, including limiting providers, selecting a default model, or choosing MCP servers enabled by default.
---

# Configure OpenCode for this directory

Create or update `opencode.json` in the current working directory to control
OpenCode behavior for this directory. Run the OpenCode CLI from the current
directory and use its actual output as the source of selectable providers,
models, and MCP servers.

## Workflow

1. Check whether `./opencode.json` exists. Read and preserve its valid existing
   settings. If it is malformed or cannot be parsed, explain the problem and
   ask the user how they want to proceed; do not silently discard its contents.
2. Ask the user which of these they want to configure. Make each choice
   independent and optional:
   - Restrict which providers are available in this directory.
   - Set a default model.
   - Choose which configured MCP servers are enabled by default.
3. For provider restrictions, run `opencode providers list` and show the
   providers it reports as a multi-select list. Include `opencode` as an
   additional selectable provider even though that command does not list it.
   If the user enables provider restrictions, write the selected provider IDs
   to `enabled_providers`. Do not write that field if they skip this option.
4. For a default model, run `opencode models`. If provider restrictions were
   selected, show only models whose provider is among the selected providers;
   include `opencode` models only if `opencode` was selected. If there are no
   models for the selected providers, explain that and do not set a model.
   Otherwise, let the user select one model and write its full
   `provider/model-id` identifier to `model`. If provider restrictions were
   skipped, offer the full model list.
5. For MCP defaults, run `opencode mcp list` and let the user multi-select
   servers to enable by default. Preserve each server's existing definition
   and set `enabled` to `true` for selected servers. Do not explicitly set
   `enabled` to `false` for unselected servers; leave their existing setting
   unchanged. Do not invent MCP definitions or write an `mcp` section if the
   user skips this option. If a listed server has no definition in the
   current project's config, do not copy unknown connection details into the
   project file; explain that it is inherited and ask before adding an
   override.
6. Preserve every unrelated key and value. Ensure the config contains
   `"$schema": "https://opencode.ai/config.json"`. Validate the exact schema
   and supported field shapes against <https://opencode.ai/config.json> if
   uncertain; OpenCode rejects invalid configuration.
7. If `opencode.json` already existed, show a clear before-and-after diff and
   ask the user to confirm before writing anything. If they decline, leave the
   file unchanged. For a new file, show the proposed JSON and ask for
   confirmation before creating it.
8. Do not run `opencode debug config` to validate the proposed configuration.
   If the exact schema or supported field shapes are uncertain, consult
   <https://opencode.ai/config.json>. After a successful write, tell the user
   to restart OpenCode for the directory configuration to take effect.

## Editing rules

- Use the provider IDs and model identifiers exactly as emitted by the CLI.
- Treat `opencode` as a provider option even if it is missing from
  `opencode providers list`; do not fabricate models for it. Only offer its
  models if `opencode models` actually lists them.
- A skipped option means preserve the existing configuration for that feature,
  not disable or clear it.
- When updating JSON, retain unrelated user settings and produce valid,
  formatted JSON. Never overwrite without the required confirmation.
