using System;
using System.Collections.Generic;
using System.Linq;


namespace RBFNetwork
{
    public class Neuron
    {
        public double Output { get; private set; }
        public double Expected { get; private set; }
        public List<double> Weights { get; set; }
        public double Bias { get; set; }
        public double LearningRate { get; private set; }

        private static Random rnd = new Random();

        public Neuron(int hiddenCount, double learningRate)
        {
            Weights = new List<double>(hiddenCount);
            InitWeights(hiddenCount);
            Bias = rnd.NextDouble();
            LearningRate = learningRate;
        }
        private void InitWeights(int hiddenCount)
        {
            for (int i = 0; i < hiddenCount; i++)
                Weights.Add(rnd.NextDouble());
        }

        public void CalculateOutput(List<RadialNeuron> hiddenNeurons)
        {
            //выходные нейроны просто суммируют произведения весов и выходов у радиальных нейронов + смещение функции (bias)
            Output = Weights.Select((w,i) => w * hiddenNeurons[i].Output).Sum() + Bias;
        }

        public void SetExpectedValue(double expected) => Expected = expected;

        public double Delta => Output - Expected;

        public void UpdateBias()
        {
            Bias -= LearningRate * Delta;
        }

        public void UpdateWeights(List<RadialNeuron> radialNeurons)
        {
            for (int i = 0; i < Weights.Count; i++)
                Weights[i] -= LearningRate * radialNeurons[i].Output * Delta;
        }

        public override string ToString()
        {
            return Output.ToString();
        }
    }
}
