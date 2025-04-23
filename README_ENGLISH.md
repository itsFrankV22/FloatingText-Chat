# FloatingText-Chat

**TShock Plugin**: Displays floating chat bubbles above players when they send messages in chat.

✅ Now compatible with **[Floating-ItemDecoPlugin](https://github.com/itsFrankV22/ItemsDeco-Plugin)**  
🌐 Prefieres Leerlo en español, Visita esto en Español: [README_SPANISH.md](https://github.com/itsFrankV22/FloatingText-Chat/blob/main/README_SPANISH.md)

---

## 📥 Installation

1. **Download the Release**
   - Go to the [Releases](https://github.com/itsFrankV22/FloatingText-Chat/releases/) section.
   - Download the `.zip` or `.dll` corresponding to your TShock version.

2. **Install the Plugin**
   - If downloaded as `.zip`, extract the contents.
   - Copy the `.dll` file into your server’s `ServerPlugins` directory:  
     `TShock/ServerPlugins/`

3. **Restart Your Server**
   - Restart the TShock server to load the plugin.

4. **Confirm Installation**
   - Check the server console for plugin logs or run `/plugins` in-game to verify.

---

## ⚙️ Features

- Displays chat messages above players as floating text.
- Fully customizable via a `config.json` file.
- Optional sound bubble effect (configurable pitch and volume).
- Group-based visual coloring using group color.
- Filter options to restrict display for muted/dead players or by permission.
- Optional endpoint request to send server info to the developer (can be disabled).

---

## 🧠 Configuration Guide

Configuration is saved in:
```
TShock/Config/FloatingText/config.json
```

### Example Configuration (with comments)

```jsonc
{
  "General": {
    "EnableInitializationRequest": true, // Sends a non-sensitive ping to the developer's endpoint for debugging/stats.
    "ExcludedGroups": [ "Guest" ],       // Groups that will not show floating messages.
    "RequireRegistration": true          // Only registered players can show floating messages.
  },
  "Filters": {
    "PlayerNotDead": true,               // Only show messages if the player is alive.
    "RequirePermission": false,          // Require permission to show chat bubbles?
    "Permission": "floatingtext.show"    // Permission name used if above is true.
  },
  "Sound": {
    "Volume": 0.6                        // Volume of the chat bubble sound (0.0 - 1.0)
  }
}
```

---

## 🛠️ Permissions

- `floatingtext.quiet`: Allows players to use the `/quiet` command to send floating messages silently.
- `floatingtext.show`: (Optional) Required if you enable the `RequirePermission` filter.

---

## 📢 Commands

- `/quiet <message>`  
  Sends a floating message using group color, without chat text.

---

## 🧩 Dependencies

- TShockAPI
- TerrariaAPI Server
- Newtonsoft.Json

---

## 👨‍💻 Author

**FrankV22**  
[GitHub Profile](https://github.com/itsFrankV22)  
[Terraria Server: TLW - TerraLatamWorld](terrarlatamwordl.sytes.net:7777)

---

## 📜 License

MIT License
