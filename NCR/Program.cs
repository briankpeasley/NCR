using System;
using System.Collections.Generic;

namespace NCR
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid = new Grid(2);
            Console.WriteLine($"Number of rectangles is {grid.Rectangles.Count}.  Energy = {grid.Evaluate()}");
        }
    }
}
