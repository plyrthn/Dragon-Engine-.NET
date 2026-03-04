using System;
using System.Runtime.InteropServices;

namespace DragonEngineLibrary
{
    /// <summary>
    /// Provides access to cdispose_manager fields for controlling entity streaming layout.
    /// Instance pointer must be set externally (e.g. via MinHook on job_uid_load).
    ///
    /// The layout mode controls which entities are streamed in/out. During battles,
    /// the engine switches to Battle mode which unloads world geometry outside the battle area.
    ///
    /// Memory layout (from Binary Ninja analysis with full LJ symbols):
    ///   +0x9CC98  m_args_uid_tree_load+24         (byte, used by job_uid_load pattern)
    ///   +0x9CD60  m_p_file_callback_xbox_layout    (t_instance_list, present in LJ/debug, eliminated in YLAD release)
    ///   +0x9CD70  m_e_layout                       (byte, LJ / debug builds)
    ///   +0x9CD71  m_e_layout_request               (byte, LJ / debug builds)
    ///   +0x9CD74  m_tick_layout_request             (uint, LJ / debug builds)
    ///
    /// In YLAD release, the 16-byte callback list at +0x9CD60 was eliminated,
    /// shifting layout fields down: +0x9CD70 -> +0x9CD60, +0x9CD71 -> +0x9CD61.
    /// </summary>
    public static class CDisposeManager
    {
        /// <summary>
        /// Entity streaming layout mode (e_xbox_layout enum from engine).
        /// </summary>
        public enum Layout : byte
        {
            Invalid = 0,
            Default = 1,
            Loading = 2,
            Battle = 3,
            Adv = 4,
            AdvCamera = 5,
            Event = 6,
            Pause = 7,
            Map = 8
        }

        // LJ and later: callback list present, original debug offsets
        // YLAD release: callback list eliminated, shifted -16 bytes
#if LJ_AND_UP
        public const int OFF_LAYOUT = 0x9CD70;
        public const int OFF_LAYOUT_REQUEST = 0x9CD71;
        public const int OFF_TICK_LAYOUT_REQUEST = 0x9CD74;
#else
        public const int OFF_LAYOUT = 0x9CD60;
        public const int OFF_LAYOUT_REQUEST = 0x9CD61;
        public const int OFF_TICK_LAYOUT_REQUEST = 0x9CD64;
#endif

        /// <summary>
        /// Pattern for job_uid_load: cmp byte [rcx+0x9CC98], 0
        /// rcx = cdispose_manager this pointer. Unique in release binary.
        /// </summary>
        public const string PAT_JOB_UID_LOAD = "80 B9 98 CC 09 00 00";

        private static IntPtr _instance = IntPtr.Zero;

        /// <summary>
        /// Native pointer to cdispose_manager. Set by hook consumers.
        /// </summary>
        public static IntPtr Pointer
        {
            get => _instance;
            set => _instance = value;
        }

        /// <summary>
        /// Whether the instance pointer has been captured.
        /// </summary>
        public static bool IsValid => _instance != IntPtr.Zero;

        /// <summary>
        /// Current layout mode (+OFF_LAYOUT).
        /// </summary>
        public static Layout CurrentLayout
        {
            get => IsValid ? (Layout)Marshal.ReadByte(_instance + OFF_LAYOUT) : Layout.Invalid;
            set { if (IsValid) Marshal.WriteByte(_instance + OFF_LAYOUT, (byte)value); }
        }

        /// <summary>
        /// Requested layout mode (+OFF_LAYOUT_REQUEST). Engine transitions to this over subsequent frames.
        /// </summary>
        public static Layout RequestedLayout
        {
            get => IsValid ? (Layout)Marshal.ReadByte(_instance + OFF_LAYOUT_REQUEST) : Layout.Invalid;
            set { if (IsValid) Marshal.WriteByte(_instance + OFF_LAYOUT_REQUEST, (byte)value); }
        }

        /// <summary>
        /// Override battle layout back to default. No-ops if not in battle layout.
        /// Returns true if any override was applied.
        /// </summary>
        public static bool OverrideLayoutToDefault()
        {
            if (!IsValid) return false;
            bool overridden = false;

            if (CurrentLayout == Layout.Battle)
            {
                CurrentLayout = Layout.Default;
                overridden = true;
            }
            if (RequestedLayout == Layout.Battle)
            {
                RequestedLayout = Layout.Default;
                overridden = true;
            }

            return overridden;
        }

        /// <summary>
        /// Force both layout and request to default regardless of current state.
        /// </summary>
        public static void ForceDefaultLayout()
        {
            if (!IsValid) return;
            CurrentLayout = Layout.Default;
            RequestedLayout = Layout.Default;
        }

        /// <summary>
        /// Reset the cached pointer. Call on stage change or cleanup.
        /// </summary>
        public static void Reset()
        {
            _instance = IntPtr.Zero;
        }
    }
}
