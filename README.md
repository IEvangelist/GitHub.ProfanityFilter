# GitHub - Profanity Filter

This project is an Azure function to handle a GitHub webhook, which will take action on issues/pull requests that contain profanity. Currently a webhook on this repository calls the compiled version of this project - the Azure function wakes up and takes appropriate action by filtering out profanity on issue and pull request titles and body text.

![Profanity Filter](profanity-filter.png)