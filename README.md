# RB Port Killer 🔌🗡️

> **View and terminate processes by network port with a sleek, interactive CLI.**

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)

**RBPortKiller** is a modern, cross-platform (Windows-focused currently) command-line tool designed to help developers and system administrators easily identify and terminate processes occupying network ports. Built with .NET 8 and utilizing `Spectre.Console`, it offers a rich, interactive terminal user interface.

## ✨ Features

- **🔍 Port Discovery**: Instantly list all active TCP and UDP ports.
- **📊 Detailed Info**: View Process ID (PID), Process Name, Protocol, and Port Number.
- **🎮 Interactive UI**: Navigate and select processes to kill using a simple, keyboard-driven interface.
- **🛡️ Safe & Secure**: clear confirmation prompts before terminating any process.
- **🚀 High Performance**: Lightweight and fast, built on the robust .NET 8 runtime.

## 🚀 Installation

### Prerequisites
- Windows 10/11 (or later)
- [PowerShell 5.1+](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows) (for installation scripts)

### Quick Install (Windows)

You can easily install the tool using the provided PowerShell script. This will build the project and add it to your system PATH.

1. Clone the repository:
   ```powershell
   git clone https://github.com/rahul-a-bangera/RBPortKiller.git
   cd RBPortKiller
   ```

2. Run the install script:
   ```powershell
   .\install.ps1
   ```

3. Restart your terminal to refresh the PATH environment variables.

### Manual Build

If you prefer to build it yourself:

1. Ensure you have the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

2. Build the project:
   ```powershell
   dotnet build -c Release
   ```

3. Publish as a single file executable:
   ```powershell
   dotnet publish RBPortKiller.CLI/RBPortKiller.CLI.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
   ```

## 📖 Usage

Once installed, simply run the following command in your terminal:

```powershell
rbportkiller
```

### Interactive Mode
1. The tool will scan and display a list of valid open ports and their associated processes.
2. Use the **Up/Down arrow keys** to highlight a process.
3. Press **Enter** to select the process you wish to terminate.
4. Confirm the action when prompted.

> **Note**: You may need administrative privileges (Run as Administrator) to view or terminate certain system processes.

## 🏗️ Architecture

The project follows a clean, modular architecture:

- **`RBPortKiller.Core`**: Contains domain entities, interfaces, and core business logic.
- **`RBPortKiller.Infrastructure`**: Implements system-specific logic (e.g., Windows API calls via `netstat`, `taskkill`).
- **`RBPortKiller.CLI`**: The presentation layer using `Spectre.Console` for the TUI.

## 🤝 Contributing

Contributions are welcome! If you have suggestions for improvements or bug fixes, please see our [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.

---

<p align="center">
  Built with ❤️ by Rahul
</p>
