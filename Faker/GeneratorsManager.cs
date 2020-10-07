using PluginInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Faker
{
    public class GeneratorsManager : IGenerator
    {
        private readonly string PLUGINS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        
        private List<IGenerator> generators;

        public GeneratorsManager()
        {
            generators = LoadGenerators();
        }

        private List<IGenerator> LoadGenerators()
        {
            var dirInfo = new DirectoryInfo(PLUGINS_PATH);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            var plugins = new List<IGenerator>();
            var files = Directory.GetFiles(PLUGINS_PATH, "*.dll");

            foreach (var file in files)
            {
                Type[] types;
                var asm = Assembly.LoadFrom(file);

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
                        if (type.GetInterface(typeof(IGenerator).Name) != null)
                        {
                            var plugin = asm.CreateInstance(type.FullName) as IGenerator;
                            plugins.Add(plugin);
                        }
                    }
                }
            }

            return plugins;
        }

        public bool CanGenerate(Type type)
        {
            foreach (var generator in generators)
            {
                if (generator.CanGenerate(type)) {
                    return true;
                }
            }

            return false;
        }

        public object Next(Type type)
        {
            foreach (var generator in generators)
            {
                if (generator.CanGenerate(type))
                {
                    return generator.Next(type);
                }
            }

            return null;
        }
    }
}
