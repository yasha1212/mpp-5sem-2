using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Faker
{
    [DTO]
    public class Foo
    {
        public int i;
        public string s;
        public Bar bar;
        public DateTime dt;

        public Foo(int x)
        {
            i = x;
        }

        private Foo()
        {
            i = 1;
            s = "a";
        }
    }
}
