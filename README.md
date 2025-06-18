# Convex in Unity - Experimental Demo

This is a **very experimental demo** showing [Convex](https://convex.dev) running inside of Unity. This project demonstrates real-time database functionality and live subscriptions working directly within a Unity game environment.

Massive thanks to Convex community member "v" for the initial prototype and work getting this working in c# then Unity: https://discord.com/channels/1019350475847499849/1019350478817079338/1369420230509465710

## 🎮 What's Included

This demo contains **two separate examples**:

### 1. 📱 Simple Chat Demo
- Real-time messaging system
- Live message updates across all connected clients
- Demonstrates basic Convex queries and mutations in Unity

### 2. 🍎 Fruit Popping Demo  
- Interactive fruit spawning and popping game
- Real-time synchronization of game state
- Particle effects and physics integration
- Shows how to handle game events with Convex mutations

## 🚀 Setup Instructions

### Prerequisites
- Unity 6000.1.7f1 (or compatible version)
- [Bun](https://bun.sh/) runtime
- A Convex account

### 1. Install Dependencies
```bash
bun install
```

### 2. Start the Convex Development Server
```bash
bun dev
```

This will:
- Create a new Convex deployment (if one doesn't exist)
- Start the development server
- Display your Convex URL in the terminal

### 3. Configure Unity
1. **Open the Unity project**
2. **Load either demo scene:**
   - `Assets/Chat/ChatScene.unity` - for the chat demo
   - `Assets/Fruit/FruitScene.unity` - for the fruit popping demo
3. **Find the "Convex" GameObject** in the scene hierarchy
4. **Click on it** to select it
5. **In the Inspector**, find the **Convex Client component**
6. **Set the "Convex Url"** field to your deployment URL from step 2
   - Should look like: `https://[deployment-name].convex.cloud`

### 4. Play and Test
- **Hit Play** in Unity
- The demo should connect to your Convex backend

## 📦 Using This in Your Own Project

**⚠️ Warning**: While possible, this is **not recommended** for production projects due to the experimental nature of this integration.

If you want to experiment with Convex in your own Unity project, you'll need these files:

### Required Files:
1. **Native Library**: `Assets/Plugins/x86_64/uniffi_convexmobile.dll`
   - Copy this DLL to your project's `Assets/Plugins/x86_64/` folder
   - This contains the native Rust/WebSocket implementation

2. **C# Scripts** from `Assets/Common/`:
   - `ConvexClient.cs` - Main client with strongly-typed API
   - `convexmobile.cs` - C# bindings for the native library

### Setup in Your Project:
1. Copy the above files to your Unity project
2. Add `"com.unity.nuget.newtonsoft-json": "3.2.1"` to your `/Packages/manifest.json`
3. Change Player Settings to allow unsafe code and change API compat to be .NET Framework
3. Create a GameObject with the `ConvexClient` component
4. Configure your Convex URL in the inspector
5. Use the typed `ConvexClient` API in your scripts

### Example Usage:
```csharp
// Get reference to ConvexClient
var convexClient = FindObjectOfType<ConvexClient>();

// Query with typed response
var messages = await convexClient.Query<ConvexMessage[]>("messages:list", new Dictionary<string, string>());

// Subscribe to real-time updates
var subscriber = new ConvexQuerySubscriber<ConvexMessage[]>(
    messages => Debug.Log($"Got {messages.Length} messages")
);
await convexClient.Subscribe("messages:list", new Dictionary<string, string>(), subscriber);
```

**Remember**: This is experimental code. Test thoroughly and expect potential issues, especially in builds.

## ⚠️ Important Notes

- This is an **experimental project** - expect rough edges
- The Convex Unity integration is not officially supported
- Some features may work differently in builds vs. the Unity editor
- Real-time features require an active internet connection

## 🛠️ Troubleshooting

### Connection Issues
- Ensure your Convex URL is correct in the Unity Inspector
- Check that `bun dev` is running and showing no errors
- Verify your internet connection

### Build Issues  
- Try different scripting backends (Mono vs IL2CPP)
- Use Development Build for better debugging
- Check the Unity console for connection logs

## 📚 Learn More

- [Convex Documentation](https://docs.convex.dev)
- [Unity Documentation](https://docs.unity3d.com)

## 🤝 Contributing

This is an experimental project. Feel free to:
- Report issues you encounter
- Suggest improvements
- Share your own Convex + Unity experiments!

---

**Note**: This demo showcases the potential of real-time databases in Unity games, but is not recommended for production use without thorough testing and optimization.
