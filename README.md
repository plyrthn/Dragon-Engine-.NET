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
  cimgui/                   cimgui 1.92.4dock C wrappers + imgui 1.92.4-docking source
    imgui/                  imgui core + backends (DX11/DX12/Win32)
  ImGuizmo/                 ImGuizmo source + cimguizmo C wrappers
  implot/                   implot source + cimplot C wrappers
  imnodes/                  imnodes source + cimnodes C wrappers
  minhook/                  MinHook source
  dx11-hook/                DX11/DX12 Present hook source + CMakeLists.txt
  hexa-native/              Hexa.NET.ImGui 1.92.4 prebuilt DLLs (cimguizmo, cimnodes, cimplot, cimplot3d)
  prebuilt-libs/            Prebuilt cimgui.dll (monolithic)
```

All third-party source is vendored directly — no submodules.

## Building

Requires Visual Studio 2022+ or MSBuild with .NET Framework 4.8 targeting pack.

```bash
nuget restore DragonEngine.sln
msbuild DragonEngine.sln /p:Configuration="YLAD Release" /p:Platform=x64
```

### Native DLL

The prebuilt `cimgui.dll` is committed to `deps/prebuilt-libs/` — no build step required for normal use.

`cimgui.dll` is a monolithic DLL containing:
- imgui 1.92.4-docking core + DX11/DX12/Win32 backends
- cimgui C wrappers (with `CIMGUI_VARGS0` — required for Hexa.NET P/Invoke)
- ImGuizmo, implot, imnodes
- MinHook
- DX11/DX12 Present hook

This single DLL is the only native dependency. Deploy it alongside `DELibrary.NET.exe`.

### Rebuilding cimgui.dll

Only needed if you modify hook source or any vendored native dep. Requires CMake and MSVC.

```bash
cd deps/dx11-hook
mkdir build && cd build
cmake -G "Visual Studio 17 2022" -A x64 ..
cmake --build . --config Release
```

Output: `build/x64/Release/cimgui.dll`

### cimgui.dll exports

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

`ImGui.Init()` loads `cimgui.dll` and installs the Present hook. All ImGui draw calls P/Invoke into the same DLL.

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

Builds run on every push to `main`. All configurations are built in a single job with NuGet caching. Artifacts (per-game DLLs + hook DLL) are zipped and attached to a GitHub release.
