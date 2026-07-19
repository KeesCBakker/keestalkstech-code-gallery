# Named arguments in Bash

Examples for parsing readable `--name value` arguments in a standalone Bash script or through a sourced library.

The parser updates an argument only when its variable is declared and its name appears in the readonly `allowed_arguments` whitelist:

```bash
declare repo="" branch=""
declare -ar allowed_arguments=(repo branch)
```

## Files

- `standalone.sh`: Complete standalone parser with required-argument validation.
- `lib/arguments.sh`: Reusable parser and validation functions.
- `library-example.sh`: Calling script with help and version support.
- `tests.sh`: Dependency-free regression tests for both approaches.

## Run

```bash
bash ./standalone.sh \
  --service_name catalog \
  --tag v1 \
  --namespace app \
  --env dev

bash ./library-example.sh \
  --repo KeesCBakker/keestalkstech-code-gallery \
  --branch main
```

Run the tests:

```bash
bash ./tests.sh
```

## Articles

- [Named Arguments in a Bash Script](https://keestalkstech.com/named-arguments-in-a-bash-script/)
- [Bash Script with a Lib for Named Parameters](https://keestalkstech.com/bash-script-with-a-lib-for-named-parameters/)
