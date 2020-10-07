using System;
using System.Collections.Generic;
using System.Text;

namespace Faker
{
    [DTO]
    public class Bar
    {
        public int i;
        public List<string> list;
        public Foo foo;
    }
}
