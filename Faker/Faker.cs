using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Faker
{
    public class Faker : IFaker
    {
        private GeneratorsManager generatorsManager;
        private Stack<Type> processedDTO;

        public Faker()
        {
            generatorsManager = new GeneratorsManager();
            processedDTO = new Stack<Type>();
        }

        public bool IsDTO(Type type)
        {
            return type.GetCustomAttributes(typeof(DTOAttribute), false).Length == 1;
        }

        private object Generate(Type type)
        {
            if (generatorsManager.CanGenerate(type))
            {
                return generatorsManager.Next(type);
            }

            if (IsDTO(type) && !processedDTO.Contains(type))
            {
                MethodInfo createMethod = typeof(Faker).GetMethod("Create").MakeGenericMethod(type);
                return createMethod.Invoke(this, null);
            }

            if (IsDTO(type) && processedDTO.Contains(type))
            {
                return null;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listType = type.GetGenericTypeDefinition();
                    var genericType = type.GetGenericArguments()[0];
                    var constructedListType = listType.MakeGenericType(genericType);

                    var random = new Random();
                    int length = random.Next(2, 15);

                    var list = Activator.CreateInstance(constructedListType);

                    for (int i = 0; i < length; i++)
                    {
                        list.GetType().GetMethod("Add").Invoke(list, new[] { Generate(genericType) });
                    }

                    return list;
                }
            }

            return null;
        }

        public T Create<T>()
        {
            Type type = typeof(T);

            if (!IsDTO(type))
            {
                return default;
            }

            processedDTO.Push(type);

            var constructor = GetConstructor(type);

            object[] parameters = GenerateParameters(constructor);
            object obj = constructor.Invoke(parameters);

            SetFieldsAndProperties(obj);

            processedDTO.Pop();

            return (T)obj;
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            FieldInfo[] fields = type.GetFields();
            Type[] types = fields.Select(field => field.GetType()).ToArray();

            var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, types, null);

            if (constructor == null)
            {
                constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            }

            return constructor;
        }

        private object[] GenerateParameters(ConstructorInfo constructor)
        {
            var parameters = new List<object>();

            constructor.GetParameters()
                .ToList()
                .ForEach(t => parameters.Add(Generate(t.ParameterType)));

            return parameters.ToArray();
        }

        private void SetFieldsAndProperties(object obj)
        {
            obj.GetType().GetFields().ToList()
                .ForEach(f => f.SetValue(obj, Generate(f.FieldType)));
            obj.GetType().GetProperties().ToList()
                .ForEach(p => p.SetValue(obj, Generate(p.PropertyType)));
        }
    }
}
