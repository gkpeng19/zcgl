using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using NM.Config;
using NM.OP;
using NM.Model;

namespace NM.Service
{
    public class ServiceManager
    {
        public static ServiceManager Default = new ServiceManager();
        Dictionary<string, Dictionary<string, MethodInfo>> _Services;
        private ServiceManager()
        {
            _Services = new Dictionary<string, Dictionary<string, MethodInfo>>();
            FreshService();
        }

        public void CallService(ServiceContext context)
        {
            string serviceCategory = context.E9_Request.Category;
            string serviceName = context.E9_Request.ServiceName;

            if (string.IsNullOrEmpty(serviceCategory) && !_Services.ContainsKey(serviceCategory))
                throw new ApplicationException(string.Format("Can't find effective service category."));

            if (string.IsNullOrEmpty(context.E9_Request.ServiceName) && !_Services[serviceCategory].ContainsKey(serviceName))
                throw new ApplicationException(string.Format("Can't find effective service in {0}.", serviceCategory));

            MethodInfo method = _Services[serviceCategory][serviceName];
            Object serviceInstance = Activator.CreateInstance(method.DeclaringType);
            object[] pars;
            if (method.GetParameters().Length == 1)
                pars = new object[] { context };
            else
                pars = new object[] { context.E9_Request, context.E9_Response, context.DataSource };
            Log.LogManager.DebugIt(string.Format("调用服务 {0} in {1}",method.Name, method.DeclaringType.FullName));
            method.Invoke(serviceInstance, pars);
        }

        private bool IsLoadService = false;
        public void FreshService()
        {
            if (!IsLoadService)
            {
                CollectionService(Assembly.GetExecutingAssembly());

                foreach (ServiceProvider item in ServiceProviderConfigSection.GetServiceProvider())
                {
                    if (item.Disabled) break;
                    if (!string.IsNullOrEmpty(item.AssemblyName))
                    {
                        string rootPath = string.Empty;
                        try
                        {
                            //if web application
                            rootPath = HttpRuntime.BinDirectory;
                        }
                        catch
                        { //if remoting application
                            rootPath = string.Empty;
                        }
                        try
                        {
                            string fileName = rootPath + item.AssemblyName + ".dll";
                            if (File.Exists(fileName))
                            {
                                Assembly assembly = Assembly.LoadFrom(fileName);
                                CollectionService(assembly);
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                }
                IsLoadService = true;
            }
        }

        public void CollectionService(Assembly assembly)
        {
            Assembly currentAssembly = assembly;
            Type serviveBaseType = typeof(ServiceFacadeBase);
            Type customEntityOPBase = typeof(EntityProviderOP<>);
            foreach (Type item in currentAssembly.GetTypes())
            {
                if (item.IsSubclassOf(serviveBaseType))
                    foreach (ServiceFacadeAttribute serviceClass in item.GetCustomAttributes(typeof(ServiceFacadeAttribute), false))
                    {
                        if (!_Services.ContainsKey(serviceClass.Category))
                            _Services.Add(serviceClass.Category, new Dictionary<string, MethodInfo>());

                        foreach (MethodInfo meth in item.GetMethods())
                        {
                            foreach (ServiceAttribute o in meth.GetCustomAttributes(typeof(ServiceAttribute), false))
                            {
                                // _Services[serviceClass.Category].Add(o.ServieName, meth);
                                _Services[serviceClass.Category][o.ServieName] = meth;
                                break; //todo   serviceName must identify
                            }
                        }
                    }
                else if (IsSubclassOf(item,customEntityOPBase)) //自定义实体操作
                    foreach (CustomEntityOPAttribute opClass in item.GetCustomAttributes(typeof(CustomEntityOPAttribute), false))
                    {                      
                        EntityMetaManager.Default[opClass.EntityType].CustomOPType = item;
                    }
            }
        }


        public static bool IsSubclassOf(Type childType, Type parentType)
        {
            bool isParentGeneric = parentType.IsGenericType;

            return IsSubclassOf(childType, parentType, isParentGeneric);
        }

        private static bool IsSubclassOf(Type childType, Type parentType, bool isParentGeneric)
        {
            if (childType == null)
            {
                return false;
            }

            childType = isParentGeneric && childType.IsGenericType ? childType.GetGenericTypeDefinition() : childType;

            if (childType == parentType)
            {
                return true;
            }

            return IsSubclassOf(childType.BaseType, parentType, isParentGeneric);
        } 


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in _Services)
            {
                sb.AppendLine(c.Key);
                foreach (var f in c.Value)
                    sb.AppendLine(string.Format("     {0,-30} in {2} [{1}]", f.Key, f.Value.Name, f.Value.DeclaringType.FullName));
            }
            return sb.ToString();
        }
    }
}
