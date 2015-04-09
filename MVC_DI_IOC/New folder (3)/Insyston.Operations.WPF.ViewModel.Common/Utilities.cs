using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace Insyston.Operations.WPF.ViewModels.Common
{
    public delegate List<T> DelegateSearch<T>(string filterField, string filterOperator, object filterValue1, out int count, object filterValue2 = null);

    public delegate List<T> DelegateSearchFilter<T>(string filterField, string filterOperator, object filterValue1, out int count, object filterValue2 = null, params object[] filter);    

    public static class Utilities
    {
        public static List<string> CommonCommands;
        public static List<string> CommonImages;

        internal static readonly string _ViewCommonAssemblyName = "Insyston.Operations.WPF.Views.Common";

        static Utilities()
        {
            Assembly assembly;
            Stream stream;
            ResourceReader reader;

            CommonImages = new List<string>();
            CommonCommands = new List<string>();
            assembly = Assembly.Load(_ViewCommonAssemblyName);
            stream = assembly.GetManifestResourceStream(string.Format("{0}.g.resources", _ViewCommonAssemblyName));
            
            reader = new ResourceReader(stream);
            
            foreach (DictionaryEntry resource in reader)
            {
                if (resource.Key.ToString().StartsWith("images"))
                {
                    CommonImages.Add(resource.Key.ToString().Substring(resource.Key.ToString().IndexOf("/") + 1));
                }
                else if (resource.Key.ToString().StartsWith("commands"))
                {
                    CommonCommands.Add(resource.Key.ToString().Substring(resource.Key.ToString().IndexOf("/") + 1));
                }
            }
        }

        public static List<string> GetProperties<T>()
        {
            List<string> properties;

            properties = new List<string>();

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                properties.Add(Regex.Replace(property.Name, "([a-z])([A-Z])", "$1 $2"));
            }

            return properties;
        }
    }
}