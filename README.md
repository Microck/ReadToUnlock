# ReadToUnlock

A WPF application designed to help users practice reading through interactive passages and speech recognition.

## Overview

ReadToUnlock is a .NET 8 WPF application that provides reading practice using configurable passages in English and Spanish. The application tracks reading progress and provides feedback based on speech recognition accuracy.

## Features

- **Configurable Reading Passages**: Set minimum and maximum word counts for passages
- **Multi-language Support**: Practice with both English and Spanish passages
- **Customizable Requirements**: Configure how many passages are required in each language
- **Speech Recognition**: Track reading accuracy with configurable thresholds
- **Emergency Access**: Set emergency passwords and hotkeys for quick access
- **Black & White Theme**: Clean, high-contrast interface for better readability

## Requirements

- .NET 8.0 SDK
- Windows 10 or later (for WPF support)
- Microphone (for speech recognition features)

## Installation

1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd ReadToUnlock
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run --project ReadToUnlock.csproj
   ```

## Configuration

The application uses a `config.json` file for storing user preferences. Settings can be modified through the Settings window accessible from the File menu.

### Configuration Options

- **Passage Settings**:
  - Minimum Words: Minimum number of words in a passage (default: 60)
  - Maximum Words: Maximum number of words in a passage (default: 100)

- **Language Settings**:
  - English Passages Required: Number of English passages to complete (default: 1)
  - Spanish Passages Required: Number of Spanish passages to complete (default: 1)
  - Single Language Mode: Practice in only one language (default: false)

- **Timing Settings**:
  - Accuracy Threshold: Percentage accuracy required for successful reading (default: 90%)
  - Max Pause Time: Maximum allowed pause time in seconds (default: 3)

- **Emergency Settings**:
  - Emergency Password: Password for emergency access (default: "unlock")
  - Emergency Hotkey: Keyboard shortcut for emergency access (default: "Ctrl+Shift+U")

## Project Structure

```
ReadToUnlock/
├── Models/
│   └── Config.cs              # Configuration model
├── Services/
│   └── ConfigService.cs       # Configuration management
├── App.xaml                   # Application resources and styles
├── App.xaml.cs               # Application entry point
├── MainWindow.xaml           # Main application window
├── MainWindow.xaml.cs        # Main window logic
├── SettingsWindow.xaml       # Settings window UI
├── SettingsWindow.xaml.cs    # Settings window logic
├── english_quotes.json       # English reading passages
├── spanish_quotes.json       # Spanish reading passages
├── icon.ico                  # Application icon
└── config.json               # User configuration (generated)
```

## Usage

1. **First Run**: The application will create a default configuration file
2. **Settings**: Access settings via File → Settings to customize your experience
3. **Reading**: Click "Start Reading" to begin a reading session
4. **Progress**: The application tracks your reading progress across sessions

## Development

### Building from Source

1. Ensure you have .NET 8.0 SDK installed
2. Clone the repository
3. Run `dotnet build` to build the project
4. Run `dotnet run` to start the application

### Adding New Features

The application is structured with:
- **MVVM Pattern**: Clean separation of UI and logic
- **Service Layer**: Centralized configuration management
- **Resource Dictionaries**: Consistent styling across the application

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## Support

For issues or questions, please open an issue on the GitHub repository.