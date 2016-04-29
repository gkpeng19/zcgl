
/// <Author>  shuagnfu </Author>   
/// <CreateDate> 2007-6-15 17:24:19  </CreateDate>
 /// <summary>  
///  Lib_System.cs
 /// <summary>  
/// <Update>2007-5-8 16:12:03</Update> 
/// <remarks> </remarks>

using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Net;
using System.Diagnostics;
using NM.OP;

namespace NM.Util
{
    /// <summary>
    /// AdvancedFunction ���ڼ�����ĸ߼�������
    /// 2005.12.16
    /// </summary>
    public class AdvancedFunction
    { 

        #region GetType
        // assemblyName ���ô���չ�� �����Ŀ�������ڵ�ǰ�����У�assemblyName����null	
        public static Type GetType(string typeFullName, string assemblyName)
        {
            if (assemblyName == null)
            {
                return Type.GetType(typeFullName);
            }

            //������ǰ�����Ѽ��صĳ���
            Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ass in asses)
            {
                string[] names = ass.FullName.Split(',');
                if (names[0].Trim() == assemblyName.Trim())
                {
                    return ass.GetType(typeFullName);
                }
            }

            //����Ŀ�����
            Assembly tarAssem = Assembly.Load(assemblyName);
            if (tarAssem != null)
            {
                return tarAssem.GetType(typeFullName);
            }

            return null;
        }
        #endregion

        #region StartApplication ����һ��Ӧ�ó���/����
        public static void StartApplication(string appFilePath)
        {
            Process downprocess = new Process();
            downprocess.StartInfo.FileName = appFilePath;
            downprocess.Start();
        }
        #endregion

        #region IsAppInstanceExist
        //Ŀ��Ӧ�ó����Ƿ��Ѿ�����
        public static bool IsAppInstanceExist(string instanceName)
        {
            bool createdNew = false;
            AdvancedFunction.MutexForSingletonExe = new System.Threading.Mutex(false, instanceName, out createdNew);
            return (!createdNew);
        }

        private static System.Threading.Mutex MutexForSingletonExe = null;
        #endregion



        #region SetProperty
        //�磬Ϊ������Ҫ�����װ����־������
        public static void SetProperty(IList objs, string propertyName, object proValue)
        {
            object[] args = { proValue };
            foreach (object target in objs)
            {
                Type t = target.GetType();
               ;
              //  if (t.GetProperty(propertyName) == null)
               if (SerializableData.GetObjPro(propertyName, t) == null)
                {
                    continue;
                }

                t.InvokeMember(propertyName, BindingFlags.Default | BindingFlags.SetProperty, null, target, args);
            }
        }
        #endregion
    }

}

