---
name: configure-local-opencode
description: Use when creating or configuring a directory-specific opencode.json, including requests to configure OpenCode with a named provider and default model, limit providers, or choose MCP servers enabled by default.
---

# Configure OpenCode for this directory

Create or update `opencode.json` in the current working directory to control
OpenCode behavior for this directory. Run the OpenCode CLI from the current
directory and use its actual output as the source of selectable providers,
models, and MCP servers.

## Understand the request

Extract the requested provider(s), default model, and MCP changes from the
user's message before asking questions. For example:

- "config opencode with github provider and luna as default"
- "config opencode with openai provider and luna as default"
- "configure opencode with [provider] and [default model] as default"

These are generic configuration requests, not fixed presets. Resolve the
requested names to the actual available provider and model identifiers using
the CLI output and existing provider/model settings. A provider name such as
"GitHub" may be a human-readable name rather than its exact provider ID; a
model name such as "Luna" may be a display name rather than its exact model ID.
Never hard-code a provider/model combination or invent an identifier.

When the request specifies both a provider and a default model, configure
`enabled_providers` with the requested provider(s) and `model` with the resolved
full identifier. Match the model within those providers. If there is one clear
match, use it without asking the user to select it again. If there are multiple
plausible matches or none, explain what was found and ask a targeted question;
do not silently choose a different provider or model.

Only ask for information that is missing or ambiguous and necessary for the
requested change. For a specific provider/model request, preserve existing MCP
settings and other unrelated settings without presenting an extra setup menu.

## Workflow

1. Check whether `./opencode.json` exists. Read and preserve its valid existing
   settings. If it is malformed or cannot be parsed, explain the problem and
   ask the user how they want to proceed; do not silently discard its contents.
2. Use the settings already specified in the user's request. For an open-ended
   configuration request, ask which of these they want to configure. Make each
   choice independent and optional:
   - Restrict which providers are available in this directory.
   - Set a default model.
   - Choose which configured MCP servers are enabled by default.
3. For provider restrictions, run `opencode providers list`. Resolve any
   requested provider names from its output; otherwise show the providers it
   reports as a multi-select list. Include `opencode` as an
   additional selectable provider even though that command does not list it.
   If the user enables provider restrictions, write the selected provider IDs
   to `enabled_providers`. Do not write that field if they skip this option.
4. For a default model, run `opencode models`. If provider restrictions were
   selected, show only models whose provider is among the selected providers;
   include `opencode` models only if `opencode` was selected. If there are no
   models for the selected providers, explain that and do not set a model.
   Otherwise, resolve the requested model name using the available identifiers
   and existing provider/model settings. If no model was requested, let the
   user select one. Write its full `provider/model-id` identifier to `model`.
   If provider restrictions were skipped, use the full model list.
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
   Reconcile settings that conflict with the requested change: remove the
   selected provider(s) from an existing `disabled_providers` list so they can
   actually be used, while retaining all unrelated entries. Include any such
   adjustment in the proposed diff. Preserve provider connection settings.
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
