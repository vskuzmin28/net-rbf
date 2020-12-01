using System;
using System.Collections.Generic;
using System.Linq;

namespace RBFNetwork
{
    public class RadialNeuron
    {
        public int ID { get; }
        public List<double> Centroids { get; set; }
        public List<double> Widths { get; set; }
        public double Output { get; private set; }
        public double LearningRate { get; private set; }

        public RadialNeuron(double learningRate, int ID)
        {
            Centroids = new List<double>();
            Widths = new List<double>();
            LearningRate = learningRate;
            this.ID = ID;
        }        

        public void CalculateOutput(double[] input)
        {
            Output = Gaussian(input);
        }

        // В качестве радиальной функции чаще всего применяется функция Гаусса. При размещении ее центра в точке  она может быть определена в сокращенной форме как:
        public double Gaussian(double[] input)
        {
            Func<int, double> func = j =>
             {
                 var numerator = input[j] - Centroids[j];
                 numerator *= numerator;
                 var denominator = Widths[j] * Widths[j];
                 return numerator / denominator;
             };
            //для каждого центра считаем функцию гаусса, потом суммируем результаты
            var sum = Centroids.Select((c, j) => func(j)).Sum();
            return Math.Pow(Math.E, -0.5 * sum);
        }

        public void UpdateCentroidsAndWidths(double outputNeuronsSumExpression, double[] input)
        {
            var prodLeft = LearningRate * outputNeuronsSumExpression * Output;
            for (int j = 0; j < Centroids.Count; j++)
            {                
                var prodRight = (input[j] - Centroids[j]) / Math.Pow(Widths[j], 2);
                Centroids[j] -= prodLeft * prodRight;
                Widths[j] -= prodLeft * prodRight / Widths[j];
            }
        }

        public override string ToString()
        {
            return ID.ToString() + " " + Output.ToString();
        }
    }
}
