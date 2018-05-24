using GizCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GizZad1
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new GraphCore();

            for (int i = 0; i < 6; i++)
            {
                graph.AddVertex();
            }

            graph.ConnectVertex(1, 4);
            graph.ConnectVertex(2, 4);
            graph.ConnectVertex(3, 4);
            graph.ConnectVertex(4, 5);
            graph.ConnectVertex(5, 6);

            Console.WriteLine(string.Join(";", graph.GeneratePruferCode()));

            Console.ReadKey();
        }
    }
}
