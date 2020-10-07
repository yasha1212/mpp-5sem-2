using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Faker
{
    public class PluginsLoader<T> where T: class
    {
        private readonly string PLUGINS_PATH;

        public PluginsLoader(string path)
        {
            PLUGINS_PATH = path;
        }

        public List<T> Load()
        {
            var dirInfo = new DirectoryInfo(PLUGINS_PATH);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            var assemblies = GetAssemblies();
            var plugins = GetPluginsInstances(assemblies);

            return plugins;
        }

        private List<Assembly> GetAssemblies()
        {
            var files = Directory.GetFiles(PLUGINS_PATH, "*.dll");
            var assemblies = files.Select(file => Assembly.LoadFrom(file)).ToList();

            return assemblies;
        }

        private List<T> GetPluginsInstances(List<Assembly> assemblies)
        {
            var plugins = new List<T>();

            foreach (var asm in assemblies)
            {
                Type[] types;

                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    types = null;
                }

                if (types != null)
                {
                    foreach (var type in types)
                    {
                        if (type.GetInterface(typeof(T).Name) != null)
                        {
                            var plugin = asm.CreateInstance(type.FullName) as T;
                            plugins.Add(plugin);
                        }
                    }
                }
            }

            return plugins;
        }
    }
}
