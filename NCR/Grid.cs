using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCR
{
    public enum Color
    {
        Red,
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

        public Grid(uint size)
        {
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

        private int EvaluateSet(List<Rectangle> rectangles)
        {
            int count = 0;
            foreach (Rectangle r in rectangles)
            {
                count += r.IsNonChromatic() ? 0 : 1;
            }

            return count;
        }
    }
}
