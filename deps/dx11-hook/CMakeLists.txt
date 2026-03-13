cmake_minimum_required(VERSION 3.15)
cmake_policy(SET CMP0091 NEW)
project(cimgui)

set(CMAKE_CXX_STANDARD 11)

if(NOT DEFINED IMGUI_SRC_DIR)
    set(IMGUI_SRC_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../ImGui.NET-nativebuild/cimgui/imgui)
endif()
if(NOT DEFINED MINHOOK_DIR)
    set(MINHOOK_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../minhook)
endif()
if(NOT DEFINED CIMGUI_DIR)
    set(CIMGUI_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../ImGui.NET-nativebuild/cimgui)
endif()
if(NOT DEFINED CIMGUIZMO_DIR)
    set(CIMGUIZMO_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../cimguizmo)
endif()
if(NOT DEFINED CIMPLOT_DIR)
    set(CIMPLOT_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../cimplot)
endif()
if(NOT DEFINED CIMNODES_DIR)
    set(CIMNODES_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../cimnodes)
endif()
if(NOT DEFINED HOOK_SRC_DIR)
    set(HOOK_SRC_DIR ${CMAKE_CURRENT_SOURCE_DIR})
endif()

# MinHook
add_library(minhook STATIC
    ${MINHOOK_DIR}/src/buffer.c
    ${MINHOOK_DIR}/src/hook.c
    ${MINHOOK_DIR}/src/trampoline.c
    ${MINHOOK_DIR}/src/hde/hde32.c
    ${MINHOOK_DIR}/src/hde/hde64.c
)
target_include_directories(minhook PRIVATE ${MINHOOK_DIR}/include)
set_property(TARGET minhook PROPERTY MSVC_RUNTIME_LIBRARY "MultiThreaded")

# Monolithic cimgui.dll: imgui + all addon libs + C wrappers + DX hook
add_library(cimgui SHARED
    # imgui core
    ${IMGUI_SRC_DIR}/imgui.cpp
    ${IMGUI_SRC_DIR}/imgui_demo.cpp
    ${IMGUI_SRC_DIR}/imgui_draw.cpp
    ${IMGUI_SRC_DIR}/imgui_tables.cpp
    ${IMGUI_SRC_DIR}/imgui_widgets.cpp
    # backends
    ${IMGUI_SRC_DIR}/backends/imgui_impl_dx11.cpp
    ${IMGUI_SRC_DIR}/backends/imgui_impl_dx12.cpp
    ${IMGUI_SRC_DIR}/backends/imgui_impl_win32.cpp
    # cimgui C wrappers
    ${CIMGUI_DIR}/cimgui.cpp
    # ImGuizmo
    ${CIMGUIZMO_DIR}/ImGuizmo/ImGuizmo.cpp
    ${CIMGUIZMO_DIR}/cimguizmo.cpp
    # implot
    ${CIMPLOT_DIR}/implot/implot.cpp
    ${CIMPLOT_DIR}/implot/implot_demo.cpp
    ${CIMPLOT_DIR}/implot/implot_items.cpp
    ${CIMPLOT_DIR}/cimplot.cpp
    # imnodes
    ${CIMNODES_DIR}/imnodes/imnodes.cpp
    ${CIMNODES_DIR}/cimnodes.cpp
    # DX hook
    ${HOOK_SRC_DIR}/dx11_hook.cpp
)

target_include_directories(cimgui PRIVATE
    ${IMGUI_SRC_DIR}
    ${IMGUI_SRC_DIR}/backends
    ${CIMGUI_DIR}
    ${CIMGUIZMO_DIR}
    ${CIMGUIZMO_DIR}/ImGuizmo
    ${CIMPLOT_DIR}
    ${CIMPLOT_DIR}/implot
    ${CIMNODES_DIR}
    ${CIMNODES_DIR}/imnodes
    ${MINHOOK_DIR}/include
)

target_compile_definitions(cimgui PRIVATE IMGUI_DEFINE_MATH_OPERATORS)

# Force-include compat header for addon sources that use renamed 1.87 API
foreach(src
    ${CIMGUIZMO_DIR}/ImGuizmo/ImGuizmo.cpp
    ${CIMGUIZMO_DIR}/cimguizmo.cpp
    ${CIMPLOT_DIR}/implot/implot.cpp
    ${CIMPLOT_DIR}/implot/implot_items.cpp
    ${CIMPLOT_DIR}/cimplot.cpp
    ${CIMNODES_DIR}/imnodes/imnodes.cpp
    ${CIMNODES_DIR}/cimnodes.cpp
)
    set_source_files_properties(${src} PROPERTIES
        COMPILE_FLAGS "/FI\"${HOOK_SRC_DIR}/imgui_compat.h\""
    )
endforeach()

set_target_properties(cimgui PROPERTIES PREFIX "")
target_link_libraries(cimgui PRIVATE minhook d3d11 d3d12 dxgi imm32)
set_property(TARGET cimgui PROPERTY MSVC_RUNTIME_LIBRARY "MultiThreaded")
