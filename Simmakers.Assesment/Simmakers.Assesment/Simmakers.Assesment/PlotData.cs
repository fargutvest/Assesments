namespace Simmakers.Assesment
{
    public class PlotData
    {
        public double[] DataX { get; private set; }
        public double[] DataY { get; private set; }

        public PlotData(double[] dataX, double[] dataY)
        {
            DataX = dataX;
            DataY = dataY;
        }
    }
}
