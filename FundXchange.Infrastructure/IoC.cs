using System;
using Microsoft.Practices.Unity;

namespace FundXchange.Infrastructure
{
    public class IoC
    {
        private static IUnityContainer container = new UnityContainer();

        public static void RegisterInstance<T>(T instance)
        {
            container.RegisterInstance<T>(instance);
        }

        public static void RegisterInstance<T>(T instance, string name)
        {
            container.RegisterInstance<T>(name, instance);
        }

        public static T Resolve<T>()
        {
            try
            {
                return container.Resolve<T>();
            }
            catch(ResolutionFailedException)
            {
            }
            return default(T);
        }

        public static T Resolve<T>(string name)
        {
            return container.Resolve<T>(name);
        } 

        public static bool DoesExist<T>()
        {
            try
            {
                
                 T instance = container.Resolve<T>();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
