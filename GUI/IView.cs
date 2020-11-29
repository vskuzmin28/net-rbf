namespace GUI
{
    public interface IView
    {
        System.Collections.Generic.List<DataPoint> DataPoints { get; set; }
    }

    public struct DataPoint
    {
        public double Expected { get; }
        public double Output { get; }
        public double Error { get; }
        public int X { get; }

        public DataPoint(double expected, double output, double error, int x)
        {
            Expected = expected;
            Output = output;
            Error = error;
            X = x;
        }
    }
}
