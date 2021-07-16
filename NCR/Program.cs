using System;
using System.Collections.Generic;

namespace NCR
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid = new Grid(10);
            Console.WriteLine($"Number of rectangles is {grid.Rectangles.Count}.  Energy = {grid.Evaluate()}");

            DateTimeOffset start = DateTimeOffset.Now;
            grid.Solve(100, 5);
            //grid.Solve();
            grid.Save();
            DateTimeOffset end = DateTimeOffset.Now;

            Console.WriteLine($"{(end - start).TotalMilliseconds}");
        }
    }
}
