namespace RBFNetwork
{
    public class Topology
    {
        public int OutputCount { get; }
        public double LearningRate { get; set; }
        public int HiddenNeuronsCount { get; }

        // Создает сеть (удобный класс) с главными параметрами сети: кол-во нейронов в выходном слое. всегда один, коэф обучения, кол-во нейронов в скрытом слое
        public Topology(int outputCount, double learningRate, int hiddenCount)
        {
            OutputCount = outputCount;
            LearningRate = learningRate;
            HiddenNeuronsCount = hiddenCount;
        }
    }
}
