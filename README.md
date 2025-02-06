# GameCloud-Unity

A Unity Package for GameCloud API integration.

## Description

GameCloud-Unity is an in-progress Unity package that provides seamless integration with the GameCloud API. This package helps Unity developers easily implement cloud-based features in their games.

## Features

- Easy-to-use API integration
- Unity Editor support
- Runtime API access
- Cloud service connectivity

## Installation

### Using Unity Package Manager

1. Open the Package Manager window in Unity (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter: `https://github.com/turanheydarli/GameCloud-Unity.git`
5. Click "Add"

### Manual Installation

1. Clone this repository
2. Copy the contents into your Unity project's `Packages` folder
3. Restart Unity if it's already running

## Usage

```csharp
// Basic usage example
using GameCloud.Unity;

public class GameCloudExample : MonoBehaviour
{
    async void Start()
    {
        // Initialize the GameCloud client
        var gameCloud = new GameCloudClient();
        
        // Use the API
        await gameCloud.Initialize();
    }
}
```

## Configuration

1. Navigate to Edit > Project Settings > GameCloud
2. Enter your API credentials
3. Save the settings

## Requirements

- Unity 2020.3 or higher
- .NET Standard 2.0 compatible project

## Documentation

Detailed documentation will be available soon. For now, please refer to the inline code documentation and example scripts in the `Editor` and `Runtime` folders.

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, please open an issue in the GitHub repository or contact the maintainers.

## Authors

- Turan Heydarli (@turanheydarli)
