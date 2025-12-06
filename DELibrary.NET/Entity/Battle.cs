using System;
using System.Runtime.InteropServices;

namespace DragonEngineLibrary
{
    public static class Battle
    {
        [DllImport("Y7Internal.dll", EntryPoint = "LIB_BATTLE_PLAYEFFECT", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DELib_Battle_PlayEffect(ParticleID id, IntPtr matrix);

        [DllImport("Y7Internal.dll", EntryPoint = "LIB_BATTLE_GETCOLLISIONEC", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr DELib_Battle_GetCollision(uint fighterID);


        ///<summary>Play battle effect. (Crash prone?)</summary>
        public static void PlayEffect(ParticleID id, Matrix4x4 matrix)
        {
            IntPtr matrixPtr = matrix.ToIntPtr();
            DELib_Battle_PlayEffect(id, matrixPtr);

            Marshal.AllocHGlobal(matrixPtr);
        }

        public static ECBattleCollision GetCollision(uint fighterID)
        {
            return new ECBattleCollision();
        }
    }
}
