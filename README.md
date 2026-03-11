# Dragon Engine .NET

.NET library for the Dragon Engine (RGG Studio). Provides managed access to game internals, entity systems, battle mechanics, UI, and more.

## Supported Games

| Game | Config | Define |
|------|--------|--------|
| Yakuza Kiwami 2 | `Release` | `YK2` |
| Yakuza: Like a Dragon | `YLAD Release` | `YLAD`, `YLAD_AND_UP` |
| Lost Judgment | `Lost Judgment Release` | `LJ`, `LJ_AND_UP` |
| Like a Dragon Gaiden | `Gaiden Release` | `GAIDEN`, `GAIDEN_AND_UP` |
| Like a Dragon: Infinite Wealth | `Infinite Wealth Release` | `IW`, `IW_AND_UP` |

Each config also has a `Debug` variant with the `DEBUG` constant.

## Repo Structure

```
DELibrary.NET/              Main library (net48, x64)
  Advanced/                 DX hook, ImGui setup, ImGuizmo bindings
  Components/               Entity components (ECMotion, ECAI, etc.)
  Entity/                   Game entities, managers, services
  Enums/                    Game enums (per-game variants)
  Structs/                  Data structures, math types
DELibrary.NET.Loader/       Standalone loader utility
deps/
  dx11-hook/                DX11/DX12 Present hook + ImGui init
  ImGui.NET-nativebuild/    cimgui source (submodule)
  cimplot/                  ImPlot C bindings (submodule)
  cimguizmo/                ImGuizmo C bindings (submodule)
  cimnodes/                 imnodes C bindings (submodule)
  minhook/                  MinHook library (submodule)
```

## Building

Requires Visual Studio 2022+ or MSBuild with .NET Framework 4.8 targeting pack.

```bash
git submodule update --init --recursive
nuget restore DragonEngine.sln
msbuild DragonEngine.sln /p:Configuration="YLAD Release" /p:Platform=x64
```

### cimgui.dll

Requires CMake and MSVC (C++ workload). Builds cimgui + cimplot + cimguizmo + cimnodes + DX11/DX12 backends + Present hook + MinHook, all statically linked into one DLL.

```bash
cd deps/dx11-hook
mkdir build\x64 && cd build\x64
cmake -DCMAKE_GENERATOR_PLATFORM=x64 -DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreaded ..\..
cmake --build . --config Release
```

Output: `deps/dx11-hook/build/x64/Release/cimgui.dll`

### Custom exports

On top of stock cimgui, cimplot, cimguizmo, and cimnodes exports:

| Export | Description |
|--------|-------------|
| `InitDX11Hook` | Hooks DX11/DX12 Present, creates ImGui context + backends |
| `Register_Present_Function` | Register a per-frame render callback |
| `Register_PreFirstFrame_Function` | Register a callback after context creation but before first NewFrame (for font loading) |
| `Register_WndProc_Function` | Register a WndProc callback through the hook's window subclass |
| `GetGameHwnd` | Returns the game window handle |
| `GetFontAtlas` | Returns the ImGui font atlas pointer |
| `AddFontFromMemoryTTF` | Add a font from memory to the atlas |

## ImGui / ImGuizmo Usage

The DX hook automatically calls `ImGui::NewFrame()` and `ImGuizmo::BeginFrame()` each frame. Register your render callback via C#:

```csharp
using DragonEngineLibrary.Advanced;

ImGui.Init();  // loads cimgui.dll and installs the DX hook
ImGui.RegisterUIUpdate(() =>
{
    // your ImGui drawing code here
});
```

ImGuizmo bindings are in their own namespace to avoid conflicts:

```csharp
using DragonEngineLibrary.ImGuizmoNET;

ImGuizmo.SetOrthographic(false);
ImGuizmo.SetDrawlist(foregroundDrawListPtr);
ImGuizmo.SetRect(0, 0, width, height);
ImGuizmo.Manipulate(ref view.M11, ref projection.M11, OPERATION.TRANSLATE, MODE.WORLD, ref model.M11);
```

## CI

Builds run on every push to `main`. All configurations are built in a single job with cimgui and NuGet caching. Artifacts are zipped and attached to a GitHub release.
