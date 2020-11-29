namespace RBFNetwork
{
    public class Topology
    {
        public int OutputCount { get; }
        public double LearningRate { get; set; }
        public int HiddenNeuronsCount { get; }

        public Topology(int outputCount, double learningRate, int hiddenCount)
        {
            OutputCount = outputCount;
            LearningRate = learningRate;
            HiddenNeuronsCount = hiddenCount;
        }
    }
}
