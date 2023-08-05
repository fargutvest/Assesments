using System.Collections.Generic;

namespace AxonSoft.Assesment
{
    public struct InputData
    {
        public int CountOfTriangles { get; private set; }

        public List<Triangle> Triangles { get; private set; }

        public InputData(int countOfTriangles, List<Triangle> triangles)
        {
            CountOfTriangles = countOfTriangles;
            Triangles = triangles;
        }
    }
}
