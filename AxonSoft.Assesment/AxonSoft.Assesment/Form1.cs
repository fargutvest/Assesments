using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace AxonSoft.Assesment
{
    using TreeNode = AxonSoft.Assesment.TreeNode<DrawableTriangle>;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string absPathToInputData = Path.Combine(Directory.GetCurrentDirectory(), Properties.Resources.RelativePathToInputData);
            Triangle[] triangles = ReadInputData(absPathToInputData);

            if (IsThereAnyIntersection(triangles))
            {
                label1.Text = "ERROR";
            }
            Color[] allShadesOfGreen = ColorsHelper.GetShadesOfGreen(triangles.Length);
            List<DrawableTriangle> trianglesToDraw = AssignColorsToTriangles(triangles, allShadesOfGreen);

            int usedShades = trianglesToDraw.Select(_ => _.ColorToFill).Distinct().Count();
            label1.Text = $"Used different shades: {usedShades + 1}";

            DrawTriangles(trianglesToDraw.ToArray(), allShadesOfGreen[0]);
        }

        //Input data:
        //The collection of triangles to be displayed is specified in an input file.
        //The first line of the file contains one non - negative number n(n <= 1000) specifying the number of triangles.
        //The next n lines contain the description of triangles in the format x1 y1 x2 y2 x3 y3,
        //where xi, yi(0 <= xi, yi <= 1000) are the coordinates of the vertices of the triangles.
        //Three points are guaranteed not to be collinear.
        private Triangle[] ReadInputData(string pathToInputData)
        {
            string[] lines = File.ReadAllLines(pathToInputData);
            int countOfTriangles;
            if (int.TryParse(lines[0], out countOfTriangles) == false)
            {
                throw new Exception($"Can`t read {nameof(countOfTriangles)}.");
            }

            if (countOfTriangles > 1000)
            {
                throw new Exception($"Too large value '{nameof(countOfTriangles)}' ({countOfTriangles}). It should be not more than 1000.");
            }

            if (countOfTriangles != lines.Length - 1)
            {
                throw new Exception($"ERROR: {nameof(countOfTriangles)} declared as {countOfTriangles}, but {lines.Count() - 1 } triangles listed");
            }

            Triangle[] triangles = new Triangle[countOfTriangles];
            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                string[] splitted = lines[lineIndex].Split(' ');
                int x1, y1, x2, y2, x3, y3;

                void Parse(out int value, string nameOfValue, int indexOfValue)
                {
                    if (int.TryParse(splitted[indexOfValue], out value) == false)
                    {
                        throw new Exception($"Can`t read '{nameOfValue}' in row {lineIndex}.");
                    }
                    if (value > 1000)
                    {
                        throw new Exception($"Too large value '{nameOfValue}' ({value}) in row {lineIndex}. It should be in range 0 - 1000.");
                    }
                }

                Parse(out x1, nameof(x1), 0);
                Parse(out y1, nameof(y1), 1);

                Parse(out x2, nameof(x2), 2);
                Parse(out y2, nameof(y2), 3);

                Parse(out x3, nameof(x3), 4);
                Parse(out y3, nameof(y3), 5);

                Triangle triangle = new Triangle(x1, y1, x2, y2, x3, y3);
                triangles[lineIndex - 1] = triangle;
            }

            return triangles;
        }

        private bool IsThereAnyIntersection(Triangle[] triangles)
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                for (int j = i + 1; j < triangles.Length; j++)
                {
                    if (triangles[i].IsIntersecting(triangles[j]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private TreeNode<DrawableTriangle> GetTreeOfTriangles(Triangle[] triangles)
        {
            TreeNode root;
            Triangle[] trianglesSquareDesc = triangles.OrderByDescending(_ => _.Square).ToArray();

            int rootIndex = 0;
            if (trianglesSquareDesc.Length > 1 && trianglesSquareDesc[0].Square == trianglesSquareDesc[1].Square)
            {
                root = new TreeNode(default(DrawableTriangle));
                while (trianglesSquareDesc[0].Square == trianglesSquareDesc[rootIndex].Square && rootIndex < trianglesSquareDesc.Length)
                {
                    root.ChildNodes.Add(new TreeNode(new DrawableTriangle(trianglesSquareDesc[rootIndex])));
                    rootIndex++;
                }
            }
            else
            {
                root = new TreeNode(new DrawableTriangle(trianglesSquareDesc[rootIndex]));
            }

            bool TryAddChild(TreeNode node, Triangle triangle)
            {
                if (node.Value.IsOverlaping(triangle))
                {
                    foreach (var childNode in node.ChildNodes)
                    {
                        if (TryAddChild(childNode, triangle))
                        {
                            return true;
                        }
                    }

                    node.ChildNodes.Add(new TreeNode(new DrawableTriangle(triangle)));
                    return true;
                }
                return false;
            }

            for (int i = rootIndex + 1; i < trianglesSquareDesc.Length; i++)
            {
                Triangle triangle = trianglesSquareDesc[i];
                TryAddChild(root, triangle);
            }

            return root;
        }

        List<DrawableTriangle> AssignColorsToTriangles(Triangle[] triangles, Color[] colors)
        {
            TreeNode tree = GetTreeOfTriangles(triangles);

            List<DrawableTriangle> trianglesToDraw = new List<DrawableTriangle>();

            void Recurse(TreeNode node, int colorIndex)
            {
                if (node.Value != default(DrawableTriangle))
                {
                    node.Value.ColorToFill = colors[colorIndex];
                    trianglesToDraw.Add(node.Value);
                }

                int nextColorIndex = colorIndex + 1;
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    Recurse(node.ChildNodes[i], nextColorIndex);
                }
            }

            Recurse(tree, 1);

            return trianglesToDraw;
        }

        private void DrawTriangles(DrawableTriangle[] triangles, Color backgroundColor)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(backgroundColor);

                for (int i = 0; i < triangles.Length; i++)
                {
                    triangles[i].Draw(graphics);
                }
            }

            pictureBox1.Image = bmp;
        }
    }
}
