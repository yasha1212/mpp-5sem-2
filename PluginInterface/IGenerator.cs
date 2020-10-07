using System;
using System.Runtime.InteropServices.ComTypes;

namespace PluginInterface
{
    public interface IGenerator
    {
        object Next(Type type);

        bool CanGenerate(Type type);
    }
}
