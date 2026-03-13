using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;


//Advanced because the mere use of this can introduce decent amount of bugs
namespace DragonEngineLibrary.Advanced
{
    public static class ImGui
    {
        internal delegate void DX11Present();
        private static List<DX11Present> _dx11Delegates = new List<DX11Present>();

        public static bool toInit = false;

        public static void RegisterUIUpdate(Action func)
        {
            DragonEngine.Log($"[ImGui] RegisterUIUpdate: {func.Method.Name}");
            DX11Present del = new DX11Present(func);
            _dx11Delegates.Add(del);

            DXHook.DELibrary_DXHook_RegisterPresentFunc(Marshal.GetFunctionPointerForDelegate(del));
            DragonEngine.Log("[ImGui] RegisterUIUpdate done");
        }

        /// <summary>
        /// Register a callback that fires after ImGui context creation but before the first NewFrame.
        /// Font atlas is unlocked at this point - add custom fonts here.
        /// </summary>
        public static void RegisterPreFirstFrame(Action func)
        {
            DragonEngine.Log($"[ImGui] RegisterPreFirstFrame: {func.Method.Name}");
            DX11Present del = new DX11Present(func);
            _dx11Delegates.Add(del);

            DXHook.DELibrary_DXHook_RegisterPreFirstFrameFunc(Marshal.GetFunctionPointerForDelegate(del));
            DragonEngine.Log("[ImGui] RegisterPreFirstFrame done");
        }

        internal delegate void WndProcDelegate(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        private static List<WndProcDelegate> _wndProcDelegates = new List<WndProcDelegate>();

        /// <summary>
        /// Register a WndProc callback through cimgui's window subclass.
        /// The DX11 hook subclasses the game window and forwards messages here.
        /// ImGui_ImplWin32_WndProcHandler is called automatically before these callbacks.
        /// </summary>
        public static void RegisterWndProc(Action<IntPtr, int, IntPtr, IntPtr> func)
        {
            WndProcDelegate del = new WndProcDelegate(func);
            _wndProcDelegates.Add(del);

            DXHook.DELibrary_DXHook_RegisterWndProcFunc(Marshal.GetFunctionPointerForDelegate(del));
        }

        public static void Init()
        {
            DragonEngine.Log("[ImGui] Init() called");
            string libPath = Path.Combine(Library.Root, "Y7Internal.dll");
            string cimguiPath = Path.Combine(new FileInfo(libPath).Directory.FullName, "cimgui.dll");

            DragonEngine.Log($"[ImGui] cimgui path: {cimguiPath} exists={File.Exists(cimguiPath)}");

            if (File.Exists(cimguiPath))
            {
                DragonEngine.Log("[ImGui] Loading cimgui.dll");
                IntPtr h = DragonEngine.LoadLibrary(cimguiPath);
                DragonEngine.Log($"[ImGui] cimgui.dll handle: {h}");
                VerifyModules(h);
            }

#if !IW_AND_UP
            DragonEngine.Log("[ImGui] Calling DXHook.Init()");
            DXHook.Init();
            DragonEngine.Log("[ImGui] DXHook.Init() returned");
#else
            DragonEngine.Log("[ImGui] Skipping DXHook.Init() (IW_AND_UP)");
#endif
        }

        // Check that each compiled-in module exported at least one known symbol
        private static void VerifyModules(IntPtr hCimgui)
        {
            Check(hCimgui, "core",     "igBegin");
            Check(hCimgui, "hook",     "InitDX11Hook");
            Check(hCimgui, "ImGuizmo", "ImGuizmo_BeginFrame");
            Check(hCimgui, "implot",   "ImPlot_BeginPlot");
            Check(hCimgui, "imnodes",  "imnodes_BeginNodeEditor");
            // legacy compat alias
            Check(hCimgui, "igGetIO compat", "igGetIO");
        }

        private static void Check(IntPtr hModule, string label, string symbol)
        {
            bool ok = DXHook.GetProcAddress(hModule, symbol) != IntPtr.Zero;
            DragonEngine.Log($"[ImGui] {(ok ? "✓" : "✗")} {label} ({symbol})");
        }
    }
}
