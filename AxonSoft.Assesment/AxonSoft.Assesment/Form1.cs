using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AxonSoft.Assesment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string absPathToInputData = Path.Combine(Directory.GetCurrentDirectory(), Properties.Resources.RelativePathToInputData);
            InputData inputData = ReadInputData(absPathToInputData);
        }


        private InputData ReadInputData(string pathToInputData)
        {
            //Input data:
            //The collection of triangles to be displayed is specified in an input file.
            //The first line of the file contains one non - negative number n(n <= 1000) specifying the number of triangles.
            //The next n lines contain the description of triangles in the format x1 y1 x2 y2 x3 y3,
            //where xi, yi(0 <= xi, yi <= 1000) are the coordinates of the vertices of the triangles.
            //Three points are guaranteed not to be collinear.

            string[] lines = File.ReadAllLines(pathToInputData);
            int countOfTriangles;
            if (int.TryParse(lines[0], out countOfTriangles) == false)
            {
                throw new Exception($"Can`t read {nameof(countOfTriangles)}.");
            }

            List<Triangle> triangles = new List<Triangle>();
            for (int i = 1; i < lines.Length; i++)
            {
                string[] splitted = lines[i].Split(' ');
                int x1, y1, x2, y2, x3, y3;

                void Parse(out int value, string nameOfValue, int indexOfValue)
                {
                    if (int.TryParse(splitted[indexOfValue], out value) == false)
                    {
                        throw new Exception($"Can`t read '{nameOfValue}' in row {i}.");
                    }
                }

                Parse(out x1, nameof(x1), 0);
                Parse(out y1, nameof(y1), 1);

                Parse(out x2, nameof(x2), 2);
                Parse(out y2, nameof(y2), 3);

                Parse(out x3, nameof(x3), 4);
                Parse(out y3, nameof(y3), 5);

                Triangle triangle = new Triangle(x1, y1, x2, y2, x3, y3);
                triangles.Add(triangle);

            }

            InputData inputData = new InputData(countOfTriangles, triangles);
            return inputData;
        }

    }
}
