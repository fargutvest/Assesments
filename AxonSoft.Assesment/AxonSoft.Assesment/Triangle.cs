using System;
using System.Drawing;

namespace AxonSoft.Assesment
{
    public class Triangle
    {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public Point C { get; private set; }

        public Point[] Points => new Point[] { A, B, C };

        public Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            A = new Point(x1, y1);
            B = new Point(x2, y2);
            C = new Point(x3, y3);
        }

        /// <summary>
        /// Useful string representaton of Triangle object for debug.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(A)}={A}; {nameof(B)}={B}; {nameof(C)}={C}; Square={Square}";
        }

        public bool IsPointBelongsToTriangle(Point point, Triangle triangle)
        {
            int[] x = new int[4] { point.X, triangle.A.X, triangle.B.X, triangle.C.X };
            int[] y = new int[4] { point.Y, triangle.A.Y, triangle.B.Y, triangle.C.Y };

            int a = (x[1] - x[0]) * (y[2] - y[1]) - (x[2] - x[1]) * (y[1] - y[0]);
            int b = (x[2] - x[0]) * (y[3] - y[2]) - (x[3] - x[2]) * (y[2] - y[0]);
            int c = (x[3] - x[0]) * (y[1] - y[3]) - (x[1] - x[3]) * (y[3] - y[0]);

            if ((a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsOverlaping(Triangle triangle) => 
            IsPointBelongsToTriangle(triangle.A, this) && 
            IsPointBelongsToTriangle(triangle.B, this) && 
            IsPointBelongsToTriangle(triangle.C, this);

        public bool IsIntersecting(Triangle triangle) => 
            IsOverlaping(triangle) == false && 
            (IsPointBelongsToTriangle(triangle.A, this) ||
            IsPointBelongsToTriangle(triangle.B, this) ||
            IsPointBelongsToTriangle(triangle.C, this));
        
        private double square = 0;
        public double Square
        {
            get 
            {
                if (this.square == 0)
                {
                    double AB, BC, CA;

                    double GetLineLength(Point a, Point b)
                    {
                        return Math.Sqrt(Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.X - a.X), 2));
                    }

                    AB = GetLineLength(A, B);
                    BC = GetLineLength(B, C);
                    CA = GetLineLength(C, A);


                    double perimeter = (AB + BC + CA) / 2;
                    this.square = Math.Sqrt(perimeter * (perimeter - AB) * (perimeter - BC) * (perimeter - CA));
                }

                return square;
            }
            
        }
    }
}
