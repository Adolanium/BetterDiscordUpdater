
# BetterDiscordUpdater

BetterDiscordUpdater is a simple command-line application that automates the process of updating BetterDiscord for both DiscordPTB and regular Discord versions. It simplifies the update process by killing the Discord process, downloading the latest BetterDiscord files, updating the necessary files, and restarting Discord.

## Features

- Automatically updates BetterDiscord to the latest version
- Supports both DiscordPTB and regular Discord versions
- Easy configuration through the `config.json` file
- Handles killing and restarting the Discord process during the update

## Prerequisites

- .NET Core SDK (version 8.0 or higher)
- DiscordPTB or regular Discord installed on your system

## Installation

1. Clone the repository or download the source code:
   ```
   git clone https://github.com/Adolanium/BetterDiscordUpdater.git
   ```
2. Navigate to the project directory:
   ```
   cd BetterDiscordUpdater
   ```
3. Build the application:
   ```
   dotnet build
   ```

## Configuration

By default, BetterDiscordUpdater is configured to update DiscordPTB. If you want to update regular Discord instead, you can modify the `config.json` file located next to the executable.

The `config.json` file has the following structure:
```json
{
  "discordVersion": "DiscordPTB"
}
```

To switch to regular Discord, change the `discordVersion` value to "Discord":
```json
{
  "discordVersion": "Discord"
}
```

If the `config.json` file doesn't exist, BetterDiscordUpdater will automatically create it with the default configuration for DiscordPTB.

## Usage

To run BetterDiscordUpdater, execute the main (exe) file.

The updater will perform the following steps:

- Kill the currently running Discord process (DiscordPTB or regular Discord, based on the configuration).
- Download the latest BetterDiscord files.
- Update the necessary files in the Discord directory.
- Restart the Discord process.

After the update is complete, Discord will be restarted with the latest version of BetterDiscord.

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.
