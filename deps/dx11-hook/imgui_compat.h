#pragma once
// compat shims for addon libs that target older imgui versions
#include "imgui.h"
#include "imgui_internal.h"

#ifndef ImGuiKeyModFlags
typedef ImGuiKeyChord ImGuiKeyModFlags;
enum ImGuiKeyModFlags_ {
    ImGuiKeyModFlags_None  = 0,
    ImGuiKeyModFlags_Ctrl  = ImGuiMod_Ctrl,
    ImGuiKeyModFlags_Shift = ImGuiMod_Shift,
    ImGuiKeyModFlags_Alt   = ImGuiMod_Alt,
    ImGuiKeyModFlags_Super = ImGuiMod_Super
};
#endif

// _c typedefs removed from cimgui in 1.92.x - addon wrappers still use them
#ifndef ImVec2_c
typedef ImVec2 ImVec2_c;
#endif
#ifndef ImVec4_c
typedef ImVec4 ImVec4_c;
#endif
#ifndef ImRect_c
typedef ImRect ImRect_c;
#endif
#ifndef ImTextureRef_c
typedef ImTextureRef ImTextureRef_c;
#endif

// CaptureMouseFromApp was renamed to SetNextFrameWantCaptureMouse in 1.89.x and removed in 1.92.x
#if IMGUI_VERSION_NUM >= 19200
namespace ImGui {
    inline void CaptureMouseFromApp(bool want_capture_mouse = true) {
        SetNextFrameWantCaptureMouse(want_capture_mouse);
    }
}
#endif
