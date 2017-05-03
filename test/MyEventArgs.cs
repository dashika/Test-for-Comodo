using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class MyEventArgs
    {
        public MyEventArgs(double arg) { Ready = arg; }

        public double Ready { get; private set; }
    }
}
