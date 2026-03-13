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
  dx11-hook/                DX11/DX12 Present hook source + prebuilt dehook.dll
  hexa-native/              Hexa.NET.ImGui 1.92.4 prebuilt DLLs + import libs
  imgui-src/                imgui 1.92.4 headers + backend sources (for building hook)
  cimguizmo/                ImGuizmo C bindings (submodule, headers only)
  minhook/                  MinHook library (submodule)
```

## Building

Requires Visual Studio 2022+ or MSBuild with .NET Framework 4.8 targeting pack.

```bash
git submodule update --init --recursive
nuget restore DragonEngine.sln
msbuild DragonEngine.sln /p:Configuration="YLAD Release" /p:Platform=x64
```

### Native DLLs

The prebuilt DLLs are committed to the repo — no build step required for normal use:

| File | Contents |
|------|----------|
| `deps/dx11-hook/dehook.dll` | Present hook + imgui 1.92.4 backends + MinHook |
| `deps/hexa-native/cimgui.dll` | ImGui 1.92.4 C bindings (Hexa.NET.ImGui) |
| `deps/hexa-native/cimguizmo.dll` | ImGuizmo C bindings |
| `deps/hexa-native/cimnodes.dll` | imnodes C bindings |
| `deps/hexa-native/cimplot.dll` | ImPlot C bindings |
| `deps/hexa-native/cimplot3d.dll` | ImPlot3D C bindings |

All six DLLs must be deployed alongside `DELibrary.NET.exe`.

### Rebuilding dehook.dll

Only needed if you modify `dx11_hook.cpp`. Requires CMake and MSVC.

```bash
cd deps/dx11-hook
mkdir build && cd build
cmake -G "Visual Studio 17 2022" -A x64 ^
  -DIMGUI_SRC_DIR=../../imgui-src ^
  -DHEXA_DIR=../../hexa-native ^
  -DMINHOOK_DIR=../../minhook ^
  -DCIMGUIZMO_DIR=../../cimguizmo ^
  -DHOOK_SRC_DIR=.. ^
  ../CMakeLists_prebuilt.txt
cmake --build . --config Release
```

Output: `build/Release/dehook.dll`

### dehook.dll exports

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

`ImGui.Init()` loads all six DLLs in the correct order and installs the Present hook. The hook syncs its ImGui context into `cimgui.dll` so C# P/Invoke calls land in the right place.

```csharp
using DragonEngineLibrary.Advanced;

ImGui.Init();
ImGui.RegisterUIUpdate(() =>
{
    // your ImGui drawing code here
});
```

`ImGuizmo::BeginFrame()` is called automatically each frame. Bindings are in their own namespace:

```csharp
using DragonEngineLibrary.ImGuizmoNET;

ImGuizmo.SetOrthographic(false);
ImGuizmo.SetDrawlist(foregroundDrawListPtr);
ImGuizmo.SetRect(0, 0, width, height);
ImGuizmo.Manipulate(ref view.M11, ref projection.M11, OPERATION.TRANSLATE, MODE.WORLD, ref model.M11);
```

## CI

Builds run on every push to `main`. All configurations are built in a single job with NuGet caching. Artifacts (per-game DLLs + hook DLLs) are zipped and attached to a GitHub release.
