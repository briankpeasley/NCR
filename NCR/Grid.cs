using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace NCR
{
    public enum Color
    {
        Red = 0,
        Green,
        Blue
    }

    public class Node
    {
        public Node()
        {
            AssociatedRectangles = new List<Rectangle>();
            Color = Color.Red;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }

        public List<Rectangle> AssociatedRectangles { get; set; }
    }

    public class Rectangle
    {
        public Rectangle(Node n1, Node n2, Node n3, Node n4)
        {
            Nodes = new Node[4];
            Nodes[0] = n1;
            Nodes[1] = n2;
            Nodes[2] = n3;
            Nodes[3] = n4;

            n1.AssociatedRectangles.Add(this);
            n2.AssociatedRectangles.Add(this);
            n3.AssociatedRectangles.Add(this);
            n4.AssociatedRectangles.Add(this);
        }

        public bool IsNonChromatic()
        {
            Color color = Nodes[0].Color;
            for (int idx = 1; idx < Nodes.Count(); idx++)
            {
                if (color != Nodes[idx].Color)
                {
                    return true;
                }
            }

            return false;
        }

        public Node[] Nodes;
    }

    public class Grid
    {
        private uint _size;
        private Node[] _nodes;
        private float _acceptHigherEnergy;

        public Grid(uint size)
        {
            _acceptHigherEnergy = 0;
            _size = size;
            _nodes = new Node[size * size];
            for (int idx = 0; idx < _size * _size; idx++)
            {
                _nodes[idx] = new Node();
            }

            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < _size; col++)
                {
                    Node n = this.At(row, col);
                    n.X = col;
                    n.Y = row;
                }
            }

            Rectangles = new List<Rectangle>();
            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < _size; col++)
                {
                    for (int width = 1; width < _size - col; width++)
                    {
                        for (int height = 1; height < _size - row; height++)
                        {
                            Node n1 = At(row, col);
                            Node n2 = At(row, col + width);
                            Node n3 = At(row + height, col + width);
                            Node n4 = At(row + height, col);

                            Rectangle r = new Rectangle(n1, n2, n3, n4);
                            Rectangles.Add(r);
                        }
                    }
                }
            }
        }

        public List<Rectangle> Rectangles { get; private set; }

        public int Evaluate()
        {
            return EvaluateSet(Rectangles);
        }

        public Node At(int row, int col)
        {
            return _nodes[row * _size + col];
        }

        public void Solve(float startingEntropy = 0, int coolingRate = 1)
        {
            List<int> walk = RandomWalk();
            Random rng = new Random();
            int energy = Evaluate();
            int cooling = 0;
            Console.WriteLine($"Energy = {energy}");

            _acceptHigherEnergy = startingEntropy;
            int prevEnergy = energy;
            do
            {
                float lowestEnergyProb = 100 - (66 * _acceptHigherEnergy / 100);
                float otherEnergyProb = (33 * _acceptHigherEnergy / 100);

                foreach (int idx in walk)
                {
                    float acceptRed = 0;
                    float acceptBlue = 0;
                    float acceptGreen = 0;

                    Node n = _nodes[idx];

                    n.Color = Color.Red;
                    int red = Evaluate();

                    n.Color = Color.Green;
                    int green = Evaluate();

                    n.Color = Color.Blue;
                    int blue = Evaluate();

                    if (red < green && red < blue)
                    {
                        acceptRed = lowestEnergyProb; // 0 -33
                        acceptGreen = acceptRed + otherEnergyProb; // 33 - 66
                        acceptBlue = acceptGreen + otherEnergyProb; // 66 - 100
                    }
                    else if (green < blue)
                    {
                        acceptRed = otherEnergyProb; // 0 -33
                        acceptGreen = acceptRed + lowestEnergyProb; // 33 - 66
                        acceptBlue = acceptGreen + otherEnergyProb; // 66 - 100
                    }
                    else
                    {
                        acceptRed = otherEnergyProb; // 0 -33
                        acceptGreen = acceptRed + otherEnergyProb; // 33 - 66
                        acceptBlue = acceptGreen + lowestEnergyProb; // 66 - 100
                    }

                    int val = rng.Next(100);
                    if(val <= acceptRed)
                    {
                        n.Color = Color.Red;
                    }
                    else if(val <= acceptGreen)
                    {
                        n.Color = Color.Green;
                    }
                    else
                    {
                        n.Color = Color.Blue;
                    }
                }

                energy = Evaluate();

                if(energy == prevEnergy)
                {
                    cooling = 0;
                    _acceptHigherEnergy = Math.Min(100, _acceptHigherEnergy + 1);
                }
                else
                {
                    cooling++;
                    if (cooling == coolingRate)
                    {
                        _acceptHigherEnergy = Math.Max(0, _acceptHigherEnergy - 1);
                        cooling = 0;
                    }
                }

                prevEnergy = energy;

                Console.WriteLine($"Energy = {energy} ({_acceptHigherEnergy}) ({lowestEnergyProb})");
            } while (energy > 0);
        }

        public void Save()
        {
            using (FileStream fs = new FileStream("output.txt", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for(int row = 0; row < _size; row++)
                    {
                        for(int col = 0; col < _size; col++)
                        {
                            Node n = At(row, col);
                            switch (n.Color)
                            {
                                case Color.Red:
                                    sw.Write("r");
                                    break;
                                case Color.Green:
                                    sw.Write("g");
                                    break;
                                case Color.Blue:
                                    sw.Write("b");
                                    break;
                            }
                        }

                        sw.WriteLine("");
                    }
                }
            }
        }

        private int EvaluateSet(List<Rectangle> rectangles)
        {
            int count = 0;
            foreach (Rectangle r in rectangles)
            {
                count += r.IsNonChromatic() ? 0 : 1;
            }

            return count;
        }

        private List<int> RandomWalk()
        {
            Random rng = new Random();
            List<int> randomIndex = new List<int>();
            for (int idx = 0; idx < _nodes.Count(); idx++)
            {
                randomIndex.Add(idx);
            }

            List<int> shuffled = new List<int>();
            while (randomIndex.Count > 0)
            {
                int idx = rng.Next(randomIndex.Count - 1);
                shuffled.Add(randomIndex[idx]);
                randomIndex.RemoveAt(idx);
            }

            return shuffled;
        }
    }
}
