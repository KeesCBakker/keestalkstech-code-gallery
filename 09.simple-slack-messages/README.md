# Simple Python code to send messages to a Slack channel (without packages)

For more information check my blog: <a href="https://keestalkstech.com/2019/10/simple-python-code-to-send-message-to-slack-channel-without-packages/#manifest">Simple Python code to send messages to a Slack channel (without packages)</a>.

To get started:

1. Goto https://api.slack.com/apps?new_app=1
1. Choose _From An App Manifest_ and paste the contents of the `slack-app-manifest.yml`.
1. Install the app and copy the token.
1. Copy the `example.env` to `.env`.
1. Paste the token in the file.
1. Now you can change the `slack_channel` variable. Make sure your bot is added to that Slack channel.

More on app manifests here: https://keestalkstech.com/2022/09/dont-use-slack-incoming-webhooks-app-creation-is-dead-simple/
