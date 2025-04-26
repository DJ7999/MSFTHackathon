using ChatbotBackend.Agent;
using System.ComponentModel;
using System.Reflection;

namespace ChatbotBackend.Services
{
    public static class AppDescriptionService
    {
        public static string RetrieveDescriptions()
        {
            string appDescription = "";
            // Get all assemblies in the current application domain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                // Get all types defined in the assembly
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    // Check if the type implements IAgent
                    if (typeof(IAgent).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        // Retrieve DescriptionAttributes from the type
                        var typeDescription = type.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
                        if (typeDescription != null)
                        {
                            appDescription += $"\nAgent: {type.FullName}, Description: {typeDescription.Description}";
                        }

                        //// Retrieve DescriptionAttributes from the methods
                        //var methods = type.GetMethods();
                        //foreach (var method in methods)
                        //{
                        //    var methodDescription = method.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
                        //    if (methodDescription != null)
                        //    {
                        //        var a= $"Method: {method.Name}, Description: {methodDescription.Description}";
                        //    }
                        //}

                        //// Retrieve DescriptionAttributes from the properties
                        //var properties = type.GetProperties();
                        //foreach (var property in properties)
                        //{
                        //    var propertyDescription = property.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
                        //    if (propertyDescription != null)
                        //    {
                        //        "Agent: {property.Name}, Description: {propertyDescription.Description}";
                        //    }
                        //}

                    }
                }
            }


            return appDescription;
        }
    }
}
