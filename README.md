# CnCNet Client #

The MonoGame / XNA CnCNet client, a platform for playing classic Command & Conquer games and their mods both online and offline. Supports setting up and launching both singleplayer and multiplayer games with [a CnCNet game spawner](https://github.com/CnCNet/ts-patches). Includes an IRC-based chat client with advanced features like private messaging, a friend list, a configurable game lobby, flexible and moddable UI graphics, and extras like game setting configuration and keeping track of match statistics. And much more!

Targets
-------

The primary targets of the client project are


* [Dawn of the Tiberium Age](http://www.moddb.com/mods/the-dawn-of-the-tiberium-age)
* [Twisted Insurrection](http://www.moddb.com/mods/twisted-insurrection)
* [Mental Omega](http://www.moddb.com/mods/mental-omega)
* [CnCNet Yuri's Revenge](https://cncnet.org/yuris-revenge)


However, there is no limitation in the client that would prevent incorporating it into other projects. Any game or mod project that utilizes the CnCNet spawner for Tiberian Sun and Red Alert 2 can be supported. Several other projects also use the client or an unofficial fork of it, including [Tiberian Sun Client](https://www.moddb.com/mods/tiberian-sun-client), [Project Phantom](https://www.moddb.com/mods/project-phantom), [YR Red-Resurrection](https://www.moddb.com/mods/yr-red-resurrection), [The Second Tiberium War](https://www.moddb.com/mods/the-second-tiberium-war) and [CnC: Final War](https://www.moddb.com/mods/cncfinalwar).

Requirements
------------

The client has 3 builds: Windows (DirectX11), OpenGL and XNA.
* The Windows and OpenGL builds rely on .NET Framework 4.8 and MonoGame.
* The XNA build relies on .NET Framework 4.8 and Microsoft's XNA Framework 4.0 Refresh.
  * [Installing XNA for Visual Studio 2019](http://flatredball.com/visual-studio-2019-xna-setup/)
  * [Installing XNA for Visual Studio 2017](http://flatredball.com/visual-studio-2017-xna-setup/)

Building the solution requires a C# 12 compiler (Visual Studio 2022 17.8 or newer, or equivalent MSBuild tools) and the .NET Framework 4.8 targeting pack. Restore NuGet packages before building. The V3 tunnel code uses System.Memory and its dependencies; `BuildScripts/CopyCompiled.bat` includes these in the common binaries directory.

Compiling, debugging and usage
------------------------------

* Compiling itself is simple: assuming you have the prerequisites installed, you can just open the solution with Visual Studio and compile it right away; the repository includes MonoGame and other necessary DLLs for quick compiling.
* When built as a debug build, the client executable expects to reside in the same directory with the target project's main game executable. Resources should exist in a "Resources" sub-directory in the same directory. The repository contains sample resources and post-build commands for copying them so that you can immediately run the client in debug mode by just hitting the Debug button in Visual Studio.
* When built in release mode, the client executable expects to reside in the "Resources" sub-directory itself. In target projects, the client executables are named `clientdx.exe`, `clientogl.exe` and `clientxna.exe` respectively for each platform.
* The `BuildScripts` directory has automated build scripts that build the client for all 3 platforms and copy the output files to a folder named `Compiled` in the project root. You can then copy the contents of this `Compiled` directory into the `Resources` sub-directory of any target project. Note that by default the build scripts also build the XNA version of the client, which requires XNA Framework 4.0 Refresh to be installed. If you don't want to install XNA, then you need to modify `BuildGame.bat` to leave the XNAFramework build out.

End-user usage
--------------

For end-users running Vista or newer Windows, the MonoGame (DirectX11) build is preferred. The MonoGame build cannot be run on Windows XP, so the XNA build is intended for XP users and for those users whose GPUs do not properly support DX11.

The MonoGame WindowsGL / DesktopGL build is primarily meant for experimental Linux and Mac support.

V3 tunnels
----------

The normal and saved-game CnCNet lobbies support automatic V3 relay selection for each player pair, manual V3 server selection, and legacy V2 servers. Automatic V3 is the default in game creation; hosts can change it through the existing tunnel selector. `/TUNNELINFO` shows the regular lobby's selected paths, and `/RENEGOTIATE` retries dynamic connections. The game still speaks its existing protocol to a client-owned UDP bridge bound to loopback, so this port does not require a new game spawner.

For automatic V3 connections, the regular lobby's **Connections** button opens the upstream negotiation status panel. `/NEGSTATUS` (or `/NS`) opens it in either the regular or saved-game lobby. Its list and matrix views update with negotiation results and live pair pings, including reports about connections between other players. The host can use **Renegotiate All** to retry connections. In the regular lobby, player-name tooltips show negotiation progress or the selected relay, pair ping, and negotiated packet loss. The panel shows status and ping for all pairs; relay names are available for the local player's connections. Themes can supply their own `btnNegotiationStatus` button; existing themes get a button beside the tunnel/load-game controls automatically.

The networking implementation comes from CnCNet/xna-cncnet-client at commit `238ce02243cc388fb805ca861dae5a2e1a2c7fd8`. Its negotiation, matchmaking, keepalive, and bridge classes retain their upstream names and structure. `V3LobbySession` adapts them to DTA's two lobby implementations; `V3FrameworkCompatibility` supplies missing Framework APIs, and `V3TunnelConfiguration` retains the upstream timing defaults. Localization calls were removed because DTA does not have that localization API. Direct P2P remains disabled; its supporting code is retained to keep the imported networking code close to upstream. Upstream's unrelated lobby features and UI were not imported.

The DTA CnCNet protocol revision is now `DB`. All players in a room need the updated client, including when selecting V2. This avoids old clients misreading the new tunnel mode and launch messages. The LAN protocol is unchanged.

The limited security review covers tunnel-list parsing, incoming packet routing, negotiation reports, launch payloads, and socket lifetime. The port fetches the master list over HTTPS, validates full launch rosters before applying them, restricts control packets to the selected player/relay pair, bounds incoming tunnel-list chunks, ignores stale negotiation callbacks, and keeps the game bridge alive across IRC loss. The underlying UDP protocol still has no cryptographic authentication or encryption: relay operators and on-path attackers remain within its trust boundary. This is not a formal security audit.

Run the local regression harness from a Visual Studio developer shell:

```text
msbuild Tests\V3Tunnels\V3Tunnels.csproj /restore /p:Configuration=Release
Tests\V3Tunnels\bin\Release\net48\V3Tunnels.exe
```

The harness links the production networking sources and substitutes only UI, IRC, and application globals. It uses local UDP relay emulators to exercise three-player pair selection, launch checks, malformed input, game forwarding, repeated games, and disconnect cleanup. It does not exercise the actual game executable or Internet NAT behavior. Before release, manually test a three-player Internet match, return and relaunch, both directions of the normal/saved-game lobby handoff, loading a saved game, and a V2 match.

After building and packaging all platforms, test dependency loading from the release folder layout in a Visual Studio developer command prompt:

```bat
msbuild Tests\ClientAssemblyLoading\ClientAssemblyLoading.csproj /restore /p:Configuration=Release
copy Tests\ClientAssemblyLoading\bin\Release\net48\ClientAssemblyLoading.exe Compiled\
Compiled\ClientAssemblyLoading.exe clientdx.exe
Compiled\ClientAssemblyLoading.exe clientogl.exe
Compiled\ClientAssemblyLoading.exe clientxna.exe
del Compiled\ClientAssemblyLoading.exe
```

This test runs the packaged client's assembly resolver, constructs the V3 communicator, creates a packet, and loads all four new dependency assemblies from `Binaries`. Each platform runs in a separate process, without starting the UI or contacting tunnel servers.

To exercise and render the status panel with the shipped XNAUI controls, first build the SharpDX release, then run (replace the example installation path):

```bat
msbuild Tests\V3TunnelUI\V3TunnelUI.csproj /restore /p:Configuration=Release
Tests\V3TunnelUI\bin\Release\net48\V3TunnelUI.exe "C:\Games\DTA\Resources"
```

This uses the installation's assets and configuration without saving changes. It checks both views, three- and eight-player layouts, column alignment, scrolling across live updates, host controls, departed players, and closing. PNG previews are saved beside the test executable.

Client launcher
---------------

This repository does not contain the client launcher (for example, `DTA.exe` in Dawn of the Tiberium Age) that selects which platform's client executable is most suitable for each user's system. For that, see [dta-mg-client-launcher](https://github.com/CnCNet/dta-mg-client-launcher).

Branches
--------

Currently there are only two major active branches. `master` is where development happens, and while things should be fairly stable, occasionally there can also be bugs. If you want stability and reliability, the `stable` branch is recommended.

Screenshots
-----------

![Screenshot](cncnetchatlobby.png?raw=true "CnCNet IRC Chat Lobby")
![Screenshot](cncnetgamelobby.png?raw=true "CnCNet Game Lobby")
