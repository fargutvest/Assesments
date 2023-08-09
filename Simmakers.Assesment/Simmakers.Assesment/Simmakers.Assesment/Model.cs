namespace Simmakers.Assesment
{
    public class Model
    {
        public string TempLabel => "Температура [°C]";
        public string AbsLabel => "Абсолютная отметка [мм]";

        public double[] PointsX = { -1, -1, -1, -1, -1, -1.1, -1.2, -1.3, -1.5, -1.9, -2.3, -3, -3.8, -6.5 };
        public double[] PointsY = { -14000, -13000, -12000, -11000, -10000, -9000, -8000, -7000, - 6000, -5000, -4000, -3000, -2000, 0 };

    }
}
