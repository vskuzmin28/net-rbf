using System;
using System.Collections.Generic;
using System.Linq;

namespace RBFNetwork
{
    public class RBFNeuralNetwork
    {
        public Topology Topology { get; }
        public List<RadialNeuron> RadialNeurons { get; private set; }
        public List<Neuron> OutputNeurons { get; private set; }

        public RBFNeuralNetwork(Topology topology)
        {
            Topology = topology;

            // В скрытом слое создал список скрытых нейронов
            RadialNeurons = CreateHiddenLayer();

            // Список нейронов в выходном слое
            OutputNeurons = CreateOutputLayer();
        }

        public static List<List<double>> calculatedCentroids = new List<List<double>>();
        public static List<List<double>> calculatedWidths = new List<List<double>>();
        public static List<List<double>> calculatedWeights = new List<List<double>>();
        public static List<double> calculatedBiases = new List<double>();

        public double Train(Dataset dataset, int epoch, bool useCalculatedValues = false)
        {
            // Количество на обучение
            var signals = dataset.Records.ToListOfArrays(r => r.InputData);
            // Ожидаемые значения
            var expected = dataset.Records.ToListOfArrays(r => r.Expected);

            var error = 0.0;
            var othersLearningRate = 0.005;
            var minError = double.MaxValue;
            var minErrorEpoch = 0;
            var totalError = 0.0;
            if (!useCalculatedValues)
            {
                // Инициализируются центроиды
                InitCentroids(signals);

                // Считаются центроиды
                CalculateCentroids(signals, othersLearningRate);

                // Параметр который отвечает за ширину полосы в функции Гаусса
                CalculateWidths();

                // В скрытом слое у нейронов функция активации Гаусов
                // У радиальных нейронов функция активации Гауса
                // У выходного, у нас один нейнрон - линейная
                // У Гауса два параметра - центроиды и ню
                for (int i = 0; i < RadialNeurons.Count; i++)
                {
                    calculatedCentroids.Add(RadialNeurons[i].Centroids);
                    calculatedWidths.Add(RadialNeurons[i].Widths);
                }

                for (int i = 0; i < OutputNeurons.Count; i++)
                {
                    // Линейные функции
                    calculatedWeights.Add(OutputNeurons[i].Weights);
                    calculatedBiases.Add(OutputNeurons[i].Bias);
                }
            }
            else
            {
                for (int i = 0; i < calculatedCentroids.Count; i++)
                {
                    RadialNeurons[i].Centroids = calculatedCentroids[i];
                    RadialNeurons[i].Widths = calculatedWidths[i];
                }

                for (int i = 0; i < calculatedWeights.Count; i++)
                {
                    OutputNeurons[i].Weights = calculatedWeights[i];
                    OutputNeurons[i].Bias = calculatedBiases[i];
                }
            }


            for (int e = 0; e < epoch; e++)
            {
                error = 0;
                foreach (Record record in dataset)
                {
                    var input = record.InputData;
                    // Распространение
                    RadialNeurons.ForEach(n => n.CalculateOutput(input));
                    // Передовое распространение
                    OutputNeurons.ForEach(n => n.CalculateOutput(RadialNeurons));

                    for (int i = 0; i < OutputNeurons.Count; i++)
                        OutputNeurons[i].SetExpectedValue(record.Expected[i]);

                    for (int i = 0; i < RadialNeurons.Count; i++)
                    {
                        var outputNeuronsSumExpression = OutputNeurons.Sum(n => n.Delta * n.Weights[i]);
                        RadialNeurons[i].UpdateCentroidsAndWidths(outputNeuronsSumExpression, input);
                    }

                    OutputNeurons.ForEach(n =>
                    {
                        n.UpdateBias();
                        n.UpdateWeights(RadialNeurons);
                    });
                    
                    // СКО (ожидаемое - практическое ^2) / кол-во нейронов
                    error += OutputNeurons.Select(n => Math.Pow(n.Delta, 2)).Sum() / OutputNeurons.Count;
                }
                // Считаем ошибку как СКО
                error = Math.Sqrt(error / (dataset.RowCount - 1));

                if (error < minError)
                {
                    minError = error;
                    minErrorEpoch = e;
                }

                totalError += error;
            }
            return totalError / epoch;
        }

        public double[] Predict(double[] inputData)
        {
            RadialNeurons.ForEach(n => n.CalculateOutput(inputData));
            OutputNeurons.ForEach(n => n.CalculateOutput(RadialNeurons));
            return OutputNeurons.Select(n => n.Output).ToArray();
        }

        private void CalculateWidths()
        {
            double sumOfDists = 0.0;
            int pairsCount = 0;

            for (int i = 0; i < RadialNeurons.Count; i++)
            {
                sumOfDists = 0;
                pairsCount = 0;
                for (int k = 0; k < RadialNeurons.Count; k++)
                    if (k != i)
                    {                        
                        var dist = RadialNeurons[i].Centroids.GetEuclideanDistance(RadialNeurons[k].Centroids);
                        sumOfDists += dist * dist;
                        pairsCount++;
                    }

                double widthI = Math.Sqrt(sumOfDists / pairsCount);
                RadialNeurons[i].Widths = new List<double>(Enumerable.Repeat(widthI, RadialNeurons[i].Centroids.Count));
            }
        }

        private void CalculateCentroids(List<double[]> signals, double othersLearningRate)
        {
            //Алгоритм К-усреднений
            //здесь считается Евклидова мера для каждого радиального нейрона,
            //победителем является нейрон с наименьшим значением, остальные проигравшие
            //потом для победителя и проигравших по своим формулам корректируются центры
            foreach (var (signal, winner, losers) in from signal in signals
                                                     let orderedByDistances = RadialNeurons.OrderBy(n => signal.GetEuclideanDistance(n.Centroids))
                                                     let winner = orderedByDistances.First()
                                                     let losers = orderedByDistances.Skip(1).ToList()
                                                     select (signal, winner, losers))
            {
                for (int j = 0; j < signal.Length; j++)
                {
                    var ct = winner.Centroids[j];
                    winner.Centroids[j] += Topology.LearningRate * (signal[j] - ct);
                }
            }
        }

        static Random rnd = new Random();

        // У каждого нейрона есть массив центроидов, каждый из элементов это расстояние до другого нейрона
        // Получаем значение для инициализации
        private void InitCentroids(List<double[]> data)
        {
            RadialNeurons.ForEach(rn =>
            {
                for (int i = 0; i < data[0].Length; i++)
                {
                    // Считаем число и ширину
                    rn.Centroids.Add(rnd.NextDouble());
                    rn.Widths.Add(rnd.NextDouble());
                }
            }
            );
        }

        // Создается список скрытых нейронов в слое
        private List<RadialNeuron> CreateHiddenLayer()
        {
            List<RadialNeuron> hiddenNeurons = new List<RadialNeuron>(Topology.HiddenNeuronsCount);
            for (int i = 0; i < Topology.HiddenNeuronsCount; i++)
                hiddenNeurons.Add(new RadialNeuron(Topology.LearningRate,i));
            return hiddenNeurons;
        }


        // Создается список которые содержит нейроны выходного слоя. Для нашей сети 1 нейрон
        private List<Neuron> CreateOutputLayer()
        {
            List<Neuron> neurons = new List<Neuron>(Topology.OutputCount);
            for (int i = 0; i < Topology.OutputCount; i++)
                neurons.Add(new Neuron(Topology.HiddenNeuronsCount, Topology.LearningRate));
            return neurons;
        }
    }
}
