using System;
using System.Runtime.InteropServices;
using DragonEngineLibrary.Service;

namespace DragonEngineLibrary
{
    /// <summary>
    /// Provides access to cencount_manager for controlling the encounter system.
    /// Obtained at runtime via SceneEntity.encount_manager.
    ///
    /// Memory layout (verified in YLAD release binary):
    ///   +0x628  is_enable_encount_              main toggle
    ///   +0x629  is_enable_spawn_new_encounter_  spawn gate
    ///   +0x62A  is_enable_spawn                 spawn flag
    ///   +0x62C  is_force_enable_encount_        force override (overrides main toggle)
    /// </summary>
    public static class EncounterManager
    {
        private const int OFF_ENABLE_ENCOUNT = 0x628;
        private const int OFF_ENABLE_SPAWN_NEW = 0x629;
        private const int OFF_ENABLE_SPAWN = 0x62A;
        private const int OFF_FORCE_ENABLE = 0x62C;

        private static IntPtr _instance = IntPtr.Zero;

        /// <summary>
        /// Native pointer to cencount_manager. Zero if not yet captured.
        /// </summary>
        public static IntPtr Pointer => _instance;

        /// <summary>
        /// Try to capture the cencount_manager pointer from the current scene.
        /// Call this after scene is loaded. Safe to call repeatedly - no-ops if already captured.
        /// </summary>
        public static bool TryCapture()
        {
            if (_instance != IntPtr.Zero) return true;

            try
            {
                var scene = SceneService.CurrentScene;
                if (!scene.IsValid()) return false;

                var handle = scene.Get().GetSceneEntity<EntityBase>(SceneEntity.encount_manager);
                if (!handle.IsValid()) return false;

                _instance = handle.Get().Pointer;
                return _instance != IntPtr.Zero;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reset the cached pointer. Call on stage change.
        /// </summary>
        public static void Reset()
        {
            _instance = IntPtr.Zero;
        }

        /// <summary>
        /// Whether the encounter manager has been captured.
        /// </summary>
        public static bool IsValid => _instance != IntPtr.Zero;

        /// <summary>
        /// Main encounter enable toggle (+0x628).
        /// </summary>
        public static bool EnableEncount
        {
            get => IsValid && Marshal.ReadByte(_instance + OFF_ENABLE_ENCOUNT) != 0;
            set { if (IsValid) Marshal.WriteByte(_instance + OFF_ENABLE_ENCOUNT, value ? (byte)1 : (byte)0); }
        }

        /// <summary>
        /// Spawn gate for new encounters (+0x629).
        /// </summary>
        public static bool EnableSpawnNew
        {
            get => IsValid && Marshal.ReadByte(_instance + OFF_ENABLE_SPAWN_NEW) != 0;
            set { if (IsValid) Marshal.WriteByte(_instance + OFF_ENABLE_SPAWN_NEW, value ? (byte)1 : (byte)0); }
        }

        /// <summary>
        /// Spawn flag (+0x62A).
        /// </summary>
        public static bool EnableSpawn
        {
            get => IsValid && Marshal.ReadByte(_instance + OFF_ENABLE_SPAWN) != 0;
            set { if (IsValid) Marshal.WriteByte(_instance + OFF_ENABLE_SPAWN, value ? (byte)1 : (byte)0); }
        }

        /// <summary>
        /// Force override - when true, overrides main toggle (+0x62C).
        /// </summary>
        public static bool ForceEnable
        {
            get => IsValid && Marshal.ReadByte(_instance + OFF_FORCE_ENABLE) != 0;
            set { if (IsValid) Marshal.WriteByte(_instance + OFF_FORCE_ENABLE, value ? (byte)1 : (byte)0); }
        }

        /// <summary>
        /// Enable all encounter flags (main + spawn gate + spawn + force).
        /// </summary>
        public static void EnableAll()
        {
            if (!IsValid) return;
            Marshal.WriteByte(_instance + OFF_ENABLE_ENCOUNT, 1);
            Marshal.WriteByte(_instance + OFF_ENABLE_SPAWN_NEW, 1);
            Marshal.WriteByte(_instance + OFF_ENABLE_SPAWN, 1);
            Marshal.WriteByte(_instance + OFF_FORCE_ENABLE, 1);
        }

        /// <summary>
        /// Disable all encounter flags.
        /// </summary>
        public static void DisableAll()
        {
            if (!IsValid) return;
            Marshal.WriteByte(_instance + OFF_ENABLE_ENCOUNT, 0);
            Marshal.WriteByte(_instance + OFF_ENABLE_SPAWN_NEW, 0);
            Marshal.WriteByte(_instance + OFF_ENABLE_SPAWN, 0);
            Marshal.WriteByte(_instance + OFF_FORCE_ENABLE, 0);
        }
    }
}
