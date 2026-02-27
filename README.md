# Dragon-Engine.NET

.NET library for the Dragon Engine (RGG Studio).

## Supported Games
- Yakuza: Like a Dragon (YLAD)
- Like a Dragon: Infinite Wealth
- Like a Dragon Gaiden
- Lost Judgment
- Yakuza Kiwami 2

## Repo Structure
```
DELibrary.NET/          Main library (net48, x64)
DELibrary.NET.Loader/   Standalone loader utility
deps/
  ImGui.NET-nativebuild/  cimgui native build (submodule)
packages/               NuGet packages (auto-restored, not tracked)
```

## Building

Requires Visual Studio 2022+ or MSBuild with .NET Framework 4.8 targeting pack.

```
nuget restore Y7Internal.NET.sln
msbuild Y7Internal.NET.sln /p:Configuration="YLAD Release" /p:Platform=x64
```

Build configurations: `YLAD Release`, `Gaiden Release`, `Infinite Wealth Release`, `Lost Judgment Release`, `Release` (Kiwami 2).

### Building cimgui.dll

Requires CMake and MSVC (C++ workload). Builds stock cimgui + DX11/Win32 backends + DX11 Present hook + MinHook, all statically linked into one DLL.

```
cd deps/dx11-hook
mkdir build\x64 && cd build\x64
cmake -DCMAKE_GENERATOR_PLATFORM=x64 -DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreaded ..\..
cmake --build . --config Release
```

Output: `deps/dx11-hook/build/x64/Release/cimgui.dll`

Custom exports (on top of stock cimgui + backend exports):
- `InitDX11Hook` - hooks DX11 Present, creates ImGui context + backends
- `Register_Present_Function` - register per-frame render callback
- `Register_PreFirstFrame_Function` - register callback after context creation but before first NewFrame (for font loading)

## Setup (after cloning)

```
git submodule update --init --recursive
nuget restore Y7Internal.NET.sln
```
