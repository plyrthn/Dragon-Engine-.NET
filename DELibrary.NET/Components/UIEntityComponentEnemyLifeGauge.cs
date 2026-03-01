using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DragonEngineLibrary
{
    public class UIEntityComponentEnemyLifeGauge : UIEntityComponentLifeGauge
    {
        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CUI_ENTITY_COMPONENT_ENEMY_LIFE_GAUGE_SET_CATEGORY_NAME", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint DELib_UIEntityComponentEnemyLifeGauge_SetCategoryName(IntPtr gauge, IntPtr name);

        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CUI_ENTITY_COMPONENT_ENEMY_LIFE_GAUGE_ATTACH", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint DELib_UIEntityComponentEnemyLifeGauge_Attach(IntPtr character);


        ///<summary>Set name on the UI gauge. Encodes as UTF-8 for full language support.</summary>
        public void SetCategoryName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                IntPtr emptyPtr = Marshal.AllocHGlobal(1);
                Marshal.WriteByte(emptyPtr, 0);
                try { DELib_UIEntityComponentEnemyLifeGauge_SetCategoryName(Pointer, emptyPtr); }
                finally { Marshal.FreeHGlobal(emptyPtr); }
                return;
            }

            byte[] encoded = Encoding.UTF8.GetBytes(name + "\0");

            IntPtr ptr = Marshal.AllocHGlobal(encoded.Length);
            try
            {
                Marshal.Copy(encoded, 0, ptr, encoded.Length);
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
