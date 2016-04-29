using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using System;

namespace NM.Util
{
    //http://www.cnblogs.com/goldenocean/archive/2006/09/20/509558.html
    public class DLD
    {
        public enum ModePass
        {
            ByValue = 0x0001,
            ByRef = 0x0002
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);
        private IntPtr hModule = IntPtr.Zero;

        private IntPtr farProc = IntPtr.Zero;

        public void LoadDll(string lpFileName)
        {
            hModule = LoadLibrary(lpFileName);
            if (hModule == IntPtr.Zero)
                throw (new Exception(" 没有找到 :" + lpFileName + "."));
        }
        //    若已有已装载Dll的句柄，可以使用LoadDll方法的第二个版本：
        public void LoadDll(IntPtr HMODULE)
        {
            if (HMODULE == IntPtr.Zero)
                throw (new Exception(" 所传入的函数库模块的句柄 HMODULE 为空 ."));
            hModule = HMODULE;
        }

        public void LoadFun(string lpProcName)
        {
            if (hModule == IntPtr.Zero)
                throw (new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !"));
            farProc = GetProcAddress(hModule, lpProcName);
            if (farProc == IntPtr.Zero)
                throw (new Exception(" 没有找到 :" + lpProcName + " 这个函数的入口点 ")); 
        }

        public void LoadFun(string lpFileName, string lpProcName)
        {
            hModule = LoadLibrary(lpFileName);
            if (hModule == IntPtr.Zero)
                throw (new Exception(" 没有找到 :" + lpFileName + "."));
            farProc = GetProcAddress(hModule, lpProcName);
            if (farProc == IntPtr.Zero)
                throw (new Exception(" 没有找到 :" + lpProcName + " 这个函数的入口点 "));
        }

        public void UnLoadDll()
        {
            FreeLibrary(hModule);
            hModule = IntPtr.Zero;
            farProc = IntPtr.Zero;
        }

        public object Invoke(object[] ObjArray_Parameter, Type[] TypeArray_ParameterType, ModePass[] ModePassArray_Parameter, Type Type_Return)
        {
            if (hModule == IntPtr.Zero)
                throw (new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !"));
            if (farProc == IntPtr.Zero)
                throw (new Exception(" 函数指针为空 , 请确保已进行 LoadFun 操作 !"));
            if (ObjArray_Parameter.Length != ModePassArray_Parameter.Length)
                throw (new Exception(" 参数个数及其传递方式的个数不匹配 ."));
            AssemblyName MyAssemblyName = new AssemblyName();
            MyAssemblyName.Name = "InvokeFun";
            AssemblyBuilder MyAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(MyAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder MyModuleBuilder = MyAssemblyBuilder.DefineDynamicModule("InvokeDll");
            MethodBuilder MyMethodBuilder = MyModuleBuilder.DefineGlobalMethod("MyFun", MethodAttributes.Public | MethodAttributes.Static, Type_Return, TypeArray_ParameterType);
            ILGenerator IL = MyMethodBuilder.GetILGenerator();
            int i;
            for (i = 0; i < ObjArray_Parameter.Length; i++)
            {
                switch (ModePassArray_Parameter[i])
                {
                    case ModePass.ByValue:
                        IL.Emit(OpCodes.Ldarg, i);
                        break;
                    case ModePass.ByRef:
                        IL.Emit(OpCodes.Ldarga, i);
                        break;
                    default:
                        throw (new Exception(" 第 " + (i + 1).ToString() + " 个参数没有给定正确的传递方式 ."));
                }
            }

            if (IntPtr.Size == 4)
            {
                IL.Emit(OpCodes.Ldc_I4, farProc.ToInt32());
            }

            else if (IntPtr.Size == 8)
            {
                IL.Emit(OpCodes.Ldc_I8, farProc.ToInt64());
            }

            else
            {
                throw new PlatformNotSupportedException();
            }

            IL.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, Type_Return, TypeArray_ParameterType);

            IL.Emit(OpCodes.Ret); // 返回值 

            MyModuleBuilder.CreateGlobalFunctions();

            MethodInfo MyMethodInfo = MyModuleBuilder.GetMethod("MyFun");
            return MyMethodInfo.Invoke(null, ObjArray_Parameter);// 调用方法，并返回其值 

        }
        public object Invoke(IntPtr IntPtr_Function, object[] ObjArray_Parameter, Type[] TypeArray_ParameterType, ModePass[] ModePassArray_Parameter, Type Type_Return)
        {
            if (hModule == IntPtr.Zero)
                throw (new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !"));
            if (IntPtr_Function == IntPtr.Zero)
                throw (new Exception(" 函数指针 IntPtr_Function 为空 !"));
            farProc = IntPtr_Function;
            return Invoke(ObjArray_Parameter, TypeArray_ParameterType, ModePassArray_Parameter, Type_Return);
        }
    }
}
