using PluginInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeGenerator
{
    public class DateTimeGenerator : IGenerator
    {
        private readonly Random random;

        public DateTimeGenerator()
        {
            random = new Random();
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(DateTime);
        }

        public object Next(Type type)
        {
            return new DateTime(random.Next(0, DateTime.Now.Year), random.Next(1, 12), random.Next(1, 30),
                random.Next(0, 24), random.Next(0,60), random.Next(0, 60));
        }
    }
}
