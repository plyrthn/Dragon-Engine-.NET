using System;
using System.Runtime.InteropServices;
using DragonEngineLibrary.Unsafe;

namespace DragonEngineLibrary
{
    public class ECPosture : ECCharaBaseComponent
    {
        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CEC_POSTURE_GET_ROOT_MATRIX", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Matrix4x4 DELib_ECPosture_GetRootMatrix(IntPtr posture);

        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CEC_POSTURE_GETTER_SCALE", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float DELib_ECPosture_Getter_Scale(IntPtr posture);

        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CEC_POSTURE_GETTER_BONEMESH", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr DELib_ECPosture_Getter_BoneMesh(IntPtr posture);

        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CEC_POSTURE_SETTER_SCALE", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DELib_ECPosture_Setter_Scale(IntPtr posture, float val);


        public IntPtr BoneMesh
        {
            get
            {
                return DELib_ECPosture_Getter_BoneMesh(Pointer);
            }
        }

        public float Scale
        {
            get
            {
                return DELib_ECPosture_Getter_Scale(Pointer);
            }
            set
            {
                DELib_ECPosture_Setter_Scale(Pointer, value);
            }
        }

        ///<summary>Get the root matrix of the character.</summary>
        public Matrix4x4 GetRootMatrix()
        {
            return DELib_ECPosture_GetRootMatrix(Pointer);

            /*
            if(matrixPtr != IntPtr.Zero)
            {
                Matrix4x4 matrixObj = Marshal.PtrToStructure<Matrix4x4>(matrixPtr);            
                CPP.FreeUnmanagedMemory(matrixPtr);

                return matrixObj;
            }

            return new Matrix4x4();
            */
        }
    }
}
