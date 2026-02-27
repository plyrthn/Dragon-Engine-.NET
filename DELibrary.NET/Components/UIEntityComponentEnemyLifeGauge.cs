using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DragonEngineLibrary
{
    public class UIEntityComponentEnemyLifeGauge : UIEntityComponentLifeGauge
    {
        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CUI_ENTITY_COMPONENT_ENEMY_LIFE_GAUGE_SET_CATEGORY_NAME", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint DELib_UIEntityComponentEnemyLifeGauge_SetCategoryName(IntPtr gauge, IntPtr nameUtf8);

        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CUI_ENTITY_COMPONENT_ENEMY_LIFE_GAUGE_ATTACH", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint DELib_UIEntityComponentEnemyLifeGauge_Attach(IntPtr character);


        ///<summary>Set name on the UI gauge. Encodes as UTF-8 for full language support.</summary>
        public void SetCategoryName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                byte[] empty = new byte[] { 0 };
                IntPtr emptyPtr = Marshal.AllocHGlobal(1);
                Marshal.WriteByte(emptyPtr, 0);
                try { DELib_UIEntityComponentEnemyLifeGauge_SetCategoryName(Pointer, emptyPtr); }
                finally { Marshal.FreeHGlobal(emptyPtr); }
                return;
            }

            byte[] utf8 = Encoding.UTF8.GetBytes(name + "\0");
            IntPtr ptr = Marshal.AllocHGlobal(utf8.Length);
            try
            {
                Marshal.Copy(utf8, 0, ptr, utf8.Length);
                DELib_UIEntityComponentEnemyLifeGauge_SetCategoryName(Pointer, ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static EntityComponentHandle<UIEntityComponentEnemyLifeGauge> Attach(Character chara)
        {
            return DELib_UIEntityComponentEnemyLifeGauge_Attach(chara.Pointer);
        }
    }
}
