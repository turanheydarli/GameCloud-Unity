# GameCloud-Unity

![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)
![Development Status](https://img.shields.io/badge/Status-In%20Development-yellow.svg)

<p align="center">
  <img src="Documentation~/images/logo.png" alt="GameCloud Unity Logo" width="300"/>
</p>

## 📋 Overview

**GameCloud-Unity** is a Unity package that provides seamless integration with the GameCloud backend-as-a-service (BaaS) platform. Designed for game developers, it simplifies implementing cloud-based multiplayer features, player authentication, game state synchronization, and more in Unity projects.

> ⚠️ **Note:** This package is currently in development. APIs may change before the stable release.

## 🚀 Key Features

- **Simplified API Integration**
  - Connect to the GameCloud backend with just a few lines of code
  - Automatic serialization and deserialization of game data
  - Comprehensive error handling and connection management

- **Optional UniTask Support**
  - Use standard C# Tasks or enable UniTask for better performance
  - All async methods follow the `Async` naming convention
  - Memory-efficient asynchronous operations

- **Rich Multiplayer Functionality**
  - Matchmaking system integration
  - Lobby management
  - Turn-based game synchronization
  - Real-time player statistics

- **Cross-Platform Compatibility**
  - Works on all platforms supported by Unity (including WebGL)
  - Consistent API experience across platforms
  - Platform-specific optimizations where needed

## ⚙️ Installation

### Using Unity Package Manager (Recommended)

1. Open the Package Manager window in Unity (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter: `https://github.com/turanheydarli/GameCloud-Unity.git`
5. Click "Add"

### Manual Installation

1. Clone this repository:
```bash
git clone https://github.com/turanheydarli/GameCloud-Unity.git
```
2. Copy the contents into your Unity project's `Packages` folder
3. Restart Unity if it's already running

## 🔧 Getting Started

### 1. Configuration

Before using GameCloud-Unity, you need to configure your API credentials:

1. Navigate to **Edit > Project Settings > GameCloud**
2. Enter your API credentials from the [GameCloud Developer Portal](https://cloud.playables.studio/login)
3. Configure additional settings like environment (Production/Staging) and timeout values
4. Save the settings

### 2. Using UniTask (Optional)

To enable UniTask support:

1. Add the [UniTask](https://github.com/Cysharp/UniTask) package to your project
2. Add the `UNITASK_SUPPORT` scripting define symbol in **Project Settings > Player > Scripting Define Symbols**

### 3. Basic Usage

```csharp
using GameCloud;
using UnityEngine;
using System.Threading.Tasks;

public class GameCloudExample : MonoBehaviour
{
    private IGameCloudClient _client;
    
    async void Start()
    {
        // Load settings from the project configuration
        var settings = GameCloudSettings.Load();
        
        // Initialize the GameCloud client with settings
        _client = GameCloudClient.FromSettings(settings);
        
        try {
            // Connect to the GameCloud service
            await _client.InitializeAsync();
            Debug.Log("Successfully connected to GameCloud!");
            
            // Create or join a game session
            var session = await _client.Sessions.CreateOrJoinAsync("my-game-type");
            Debug.Log($"Joined session: {session.Id}");
        }
        catch (GameCloudException ex) {
            Debug.LogError($"GameCloud error: {ex.Message}");
        }
    }
}
```

### 4. Using with VContainer (Optional)

If you're using VContainer for dependency injection:

```csharp
using GameCloud;
using UnityEngine;
using VContainer;
using VContainer.Unity;

// Register GameCloud services in your LifetimeScope
public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Load settings from the project configuration
        var settings = GameCloudSettings.Load();
        
        // Register GameCloud client as a service
        builder.Register<IGameCloudClient>(r => GameCloudClient.FromSettings(settings), Lifetime.Singleton);
        
        // Register other services that depend on GameCloud
        builder.Register<IMatchmakingService, MatchmakingService>(Lifetime.Singleton);
        builder.Register<IPlayerService, PlayerService>(Lifetime.Singleton);
        
        // Register game managers
        builder.RegisterEntryPoint<GameManager>();
    }
}
```

### 5. Platform-Specific Configuration

GameCloud-Unity automatically adapts to different platforms:

```csharp
// Example of platform-specific registration
public class ServiceLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        var settings = GameCloudSettings.Load();
        builder.Register<IGameCloudClient>(r => GameCloudClient.FromSettings(settings), Lifetime.Singleton);

#if UNITY_WEBGL
        // Use WebGL-specific network implementation
        builder.Register<INetworkManager, NetworkManager>(Lifetime.Singleton);
#else
        // Use default or mock implementation for other platforms
        builder.Register<INetworkManager, MockNetworkManager>(Lifetime.Singleton);
#endif
    }
}
```

## 📚 Documentation

### Naming Conventions

GameCloud-Unity follows these naming conventions:

- All asynchronous methods have the `Async` suffix (e.g., `InitializeAsync()`, `GetPlayerDataAsync()`)
- Service interfaces begin with `I` (e.g., `IGameCloudClient`, `IMatchmakingService`)

### Core Interfaces

| Interface | Description |
|----------|-------------|
| `IGameCloudClient` | Main client interface for interacting with GameCloud services |
| `ISessionService` | Manage multiplayer game sessions |
| `IMatchmakingService` | Find and create matches between players |
| `IPlayerService` | Handle player authentication and profiles |
| `ILobbyService` | Create and manage game lobbies |

### Examples and Tutorials

| Tutorial | Description |
|----------|-------------|
| [Getting Started](Documentation~/tutorials/getting-started.md) | Set up the package and make your first API call |
| [UniTask Integration](Documentation~/tutorials/unitask-integration.md) | Working with UniTask in GameCloud |
| [Turn-Based Multiplayer](Documentation~/tutorials/turn-based-games.md) | Build a simple turn-based game with GameCloud |
| [WebGL Support](Documentation~/tutorials/webgl-support.md) | Special considerations for WebGL builds |

## 📦 Package Structure

```
GameCloud-Unity/
├── Documentation~/ - Documentation files
├── Editor/ - Unity editor extensions
│   ├── GameCloudSettings.cs - Settings provider
│   └── GameCloudWindow.cs - Debug window
├── Runtime/ - Core runtime scripts
│   ├── Api/ - API client implementations
│   ├── Models/ - Data models
│   ├── Interfaces/ - Core service interfaces
│   └── GameCloudClient.cs - Main client class
├── Samples~/ - Example projects
│   ├── BasicIntegration/
│   └── MultiplayerGame/
└── Tests/ - Unit and integration tests
```

## 🔍 Requirements

- **Unity 2020.3** or higher
- **.NET Standard 2.0** compatible project
- Internet connection for API communication
- GameCloud developer account ([Sign up here](https://cloud.playables.studio/login))

## 🤝 Contributing

Contributions are welcome! Here's how you can help improve the package:

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add some amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## 🐛 Reporting Issues

If you encounter any bugs or have feature requests, please [open an issue](https://github.com/turanheydarli/GameCloud-Unity/issues/new) on GitHub. Include as much detail as possible:

- Unity version
- Package version
- Steps to reproduce
- Expected vs. actual behavior

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Turan Heydarli** - [GitHub](https://github.com/turanheydarli)

## 🔗 Links

- [GameCloud Backend Repository](https://github.com/turanheydarli/GameCloud)
- [GameCloud Documentation](https://docs.cloud.playables.studio)
- [Developer Portal](https://cloud.playables.studio/login)
