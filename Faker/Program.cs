using System;

namespace Faker
{
    class Program
    {
        static void Main(string[] args)
        {
            var faker = new Faker();
            Foo foo = faker.Create<Foo>();
            Console.WriteLine($"i: {foo.i}, s: {foo.s}, dt: {foo.dt}, bar.i: {foo.bar.i}, bar.foo: {foo.bar.foo}");
            Console.WriteLine("bar.list:");

            foreach (var item in foo.bar.list)
            {
                Console.Write($"{item}, ");
            }

            Console.ReadKey();
        }
    }
}
