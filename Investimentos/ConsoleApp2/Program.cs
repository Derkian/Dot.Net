using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            int erros = 0;
            ConsoleApp1.Fundos.Yubb.Sync(ref erros);
        }
    }
}
