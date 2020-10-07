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
            var loader = new PluginsLoader<IGenerator>(PLUGINS_PATH);

            generators = loader.Load();
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
