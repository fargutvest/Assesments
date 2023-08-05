using System.Drawing;

namespace AxonSoft.Assesment
{
    public struct Triangle
    {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public Point C { get; private set; }

        public Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            A = new Point(x1, y1);
            B = new Point(x2, y2);
            C = new Point(x3, y3);
        }

        public override string ToString()
        {
            return $"{nameof(A)}={A}; {nameof(B)}={B}; {nameof(C)}={C}";
        }
    }
}
