using System;
using System.Runtime.InteropServices;

namespace DragonEngineLibrary
{
    public class ECBattleCollision : ECCharaComponent
    {
        [DllImport("Y7Internal.dll", EntryPoint = "LIB_ECBATTLECOLLISION_IS_WALL_HIT", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool DELibrary_ECBattleCollision_IsWallHit(IntPtr battleColl, in int destAttr, in Vector4 wallPos);


        public bool IsWallHit(in int destAttr, in Vector4 wallPos)
        {
            return DELibrary_ECBattleCollision_IsWallHit(Pointer, in destAttr, in wallPos);
        }
    }
}
