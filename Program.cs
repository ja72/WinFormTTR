using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JA.UI;
using static System.Math;

namespace JA
{

    static class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {
            Application.Run(new GeometryForm());
        }

    }

}
