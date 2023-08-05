using System.Drawing;

namespace AxonSoft.Assesment
{
    public class DrawableTriangle : Triangle
    {
        public Color ColorToFill { get; set; }
        public Color ColorOfBorder { get; set; }

        public DrawableTriangle(Triangle triangle)
           : base(triangle.A.X, triangle.A.Y, triangle.B.X, triangle.B.Y, triangle.C.X, triangle.C.Y)
        {
            ColorOfBorder = Color.Black;
        }

        public void Draw(Graphics graphics)
        {
            Brush brush = new SolidBrush(ColorToFill);
            graphics.DrawPolygon(new Pen(ColorOfBorder, 2), Points);
            graphics.FillPolygon(brush, Points);
        }
    }
}
