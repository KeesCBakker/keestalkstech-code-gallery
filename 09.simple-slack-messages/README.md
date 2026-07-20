# Simple Python code to send messages to a Slack channel (without packages)

For more information check my blog: [Simple Python code to send messages to a Slack channel (without packages)](https://keestalkstech.com/2019/10/simple-python-code-to-send-message-to-slack-channel-without-packages/#manifest).

## Prerequisites

- Python 3.8+
- Jupyter Notebook (optional — the code can also run as a regular Python script)
- A Slack workspace where you can install apps

## Files

| File | Description |
|------|-------------|
| `slack.ipynb` | Jupyter notebook with functions to post messages, blocks, and files to Slack |
| `slack-app-manifest.yml` | Slack app manifest — use this to create the bot app |
| `example.env` | Template for the `.env` file — contains `SLACK_BOT_TOKEN` |
| `wall-e-and-eve.jpg` | Example image used in the file upload demo |

## Setup

1. Go to [Create a Slack App](https://api.slack.com/apps?new_app=1)
2. Choose _From An App Manifest_ and paste the contents of `slack-app-manifest.yml`
3. Install the app and copy the **Bot User OAuth Token**
4. Copy `example.env` to `.env` and paste the token:

```
SLACK_BOT_TOKEN=xoxb-...
```

5. Invite the bot to your target channel (e.g. `/invite @Eve`)
6. Open `slack.ipynb` and set `slack_channel` to your channel name

> The app manifest requests the following OAuth scopes: `channels:read`, `chat:write`, `chat:write.customize`, `files:read`, `files:write`.

## Dependencies

Install the required packages:

```sh
pip install python-dotenv requests
```

## Usage

Open the notebook:

```sh
jupyter notebook slack.ipynb
```

Or extract the functions into a standalone `.py` script. The key functions are:

- `post_message_to_slack(text, blocks, unfurl_links)` — send a plain or richly formatted message
- `post_file_to_slack(text, file_name, file_bytes, snippet_type, title)` — upload a file or image

More on app manifests: [Don't use Slack Incoming Webhooks — app creation is dead simple](https://keestalkstech.com/2022/09/dont-use-slack-incoming-webhooks-app-creation-is-dead-simple/)

## Checkout only this project

```sh
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-code-gallery.git
cd keestalkstech-code-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 09.simple-slack-messages
git checkout main
cd 09.simple-slack-messages
ls
```
