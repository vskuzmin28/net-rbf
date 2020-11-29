using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RBFNetwork;

namespace NetworkTests
{
    [TestClass]
    public class NetworkTests
    {
        private (Dataset trainData, Dataset testData) GetData(int testSamplesCount)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var datasetStream = assembly.GetManifestResourceStream("NetworkTests.Resources.Wine1.csv");
            var dataset = new Dataset(datasetStream);

            dataset.NormalizeInputs();

            //datasetStream = assembly.GetManifestResourceStream("NetworkTests.Resources.Wine-test.csv");
            var testDataset = new Dataset();

            var recs = dataset.Records.GroupBy(k => k.Label).ToDictionary(k => k.Key, v => v.ToList());

            foreach (var key in recs.Keys)
            {
                var portion = recs[key].Take(testSamplesCount);
                testDataset.Records.AddRange(portion);
                dataset.Records.RemoveAll(r => portion.Contains(r));
            }
            testDataset.ClassesNames = dataset.ClassesNames;
            return (dataset, testDataset);
        }

        public async Task<(Dataset trainData, Dataset testData, double inputsSum)> GetTemperatureDataAsync(int trainDataPercent = 100, int windowSize=6)
        {
            var dataset = new Dataset();
            Assembly assembly = Assembly.GetExecutingAssembly();
            var datasetStream = assembly.GetManifestResourceStream("NetworkTests.Resources.ALL.csv");
            await dataset.LoadTemperature(new StreamReader(datasetStream), windowSize);
            double inputsSum = dataset.Records.Sum(r => r.InputData.Sum(x => x * x) + r.Expected.Sum(x => x * x));
            inputsSum = Math.Sqrt(inputsSum);
            dataset.Normalize();
            var ds = new Dataset();
            if (trainDataPercent == 100)
            {
                var dsStream = assembly.GetManifestResourceStream("NetworkTests.Resources.RSMOSCOW-test.csv");
                await ds.LoadTemperature(new StreamReader(dsStream), windowSize);
            }
            else
            {
                var portion = dataset.Records.Skip(dataset.RowCount * trainDataPercent / 100);
                ds.Records.AddRange(portion);
                dataset.Records.RemoveAll(r => portion.Contains(r));
            }
            return (dataset, ds, inputsSum);
        }

        [TestMethod]
        public void TestClassificationLearningRate()
        {
            var data = GetData(10);          

            //0014
            //double alpha = 0.014;
            var reverseDict = data.testData.ClassesNames.ToDictionary(k => k.Value, v => v.Key);

            Func<RBFNeuralNetwork, Record, bool> callback = (network, record) =>
            {
                var outputs = network.Predict(record.InputData);
                var maxValue = outputs.Max();
                int maxIndex = network.OutputNeurons.FindIndex(n => n.Output == maxValue);

                return reverseDict[maxIndex] == record.Label;
            };

            PredictAndWriteLearningRate((data.trainData,data.testData, 0), callback);
            Console.ReadKey();
        }

        [TestMethod]
        public void TestPredictionLearningRate()
        {
            var data = GetTemperatureDataAsync().Result;

            Func<RBFNeuralNetwork, Record, bool> callback = (network, record) =>
            {
                var output = network.Predict(record.InputData)[0] * data.inputsSum;
                var expected = record.Expected[0] * data.inputsSum;

                return expected - output <= 2;
            };

            PredictAndWriteLearningRate(data, callback);
            Console.ReadKey();
        }

        [TestMethod]
        public void TestPredictionTrainDataSize()
        {
            bool useCalculated = false;
            using (StreamWriter sw = new StreamWriter(@"E:\repos\RBFNetwork\datasetSize.txt"))
            {
                for (int i = 70; i < 100; i+=5)
                {                    
                    var data = GetTemperatureDataAsync(i).Result;
                    int accuracy = 0;
                    var topology = new Topology(data.trainData.ClassesNames?.Count ?? 1, 0.01, data.trainData.ColumnCount);
                    var network = new RBFNeuralNetwork(topology);
                    double error = network.Train(data.trainData, 100, useCalculated);
                    useCalculated = true;
                    double predictError = 0;
                    foreach (Record record in data.testData.Records)
                    {
                        var expected = record.Expected[0];
                        var output = network.Predict(record.InputData)[0];
                        var diff = expected - output;
                        predictError += diff * diff;
                        
                        if ((expected * data.inputsSum - output * data.inputsSum) <= 2)
                            accuracy++;
                    }
                    predictError = Math.Sqrt(predictError / (data.testData.RowCount - 1));
                    Trace.WriteLine($"Current accuracy: {accuracy} of {data.testData.Records.Count} = {accuracy * 100 / data.testData.Records.Count}%");
                    var predicted = accuracy * 100 / data.testData.RowCount;
                    sw.WriteLine($"{data.testData.RowCount}   {predictError}   {predicted}");
                }
            }
        }

        [TestMethod]
        public void TestPredictionWindowSize()
        {
            using (StreamWriter sw = new StreamWriter(@"E:\repos\RBFNetwork\windowSize.txt"))
            {
                for (int i = 10; i <= 10; i ++)
                {
                    var data = GetTemperatureDataAsync(95,i).Result;
                    int accuracy = 0;
                    var topology = new Topology(data.trainData.ClassesNames?.Count ?? 1, 0.01, data.trainData.ColumnCount);
                    var network = new RBFNeuralNetwork(topology);
                    double error = network.Train(data.trainData, 100, false);
                    double predictError = 0;
                    foreach (Record record in data.testData.Records)
                    {
                        var expected = record.Expected[0];
                        var output = network.Predict(record.InputData)[0];
                        var diff = expected - output;
                        predictError += diff * diff;

                        if ((expected * data.inputsSum - output * data.inputsSum) <= 2)
                            accuracy++;
                    }
                    predictError = Math.Sqrt(predictError / (data.testData.RowCount - 1));
                    Trace.WriteLine($"Current accuracy: {accuracy} of {data.testData.Records.Count} = {accuracy * 100 / data.testData.Records.Count}%");
                    var predicted = accuracy * 100 / data.testData.RowCount;
                    sw.WriteLine($"{i}   {predictError}   {predicted}");
                }
            }
        }

        private static void PredictAndWriteLearningRate((Dataset trainData, Dataset testData, double inputsSum) data, Func<RBFNeuralNetwork, Record, bool> checkResult)
        {
            bool useCalculated = false;
            int bestAccuracy = 0;
            double bestAlpha = 0.001;
            using (StreamWriter sw = new StreamWriter(@"E:\repos\RBFNetwork\learningRate.txt"))
            {
                int epoch = 10;
                var topology = new Topology(data.trainData.ClassesNames?.Count ?? 1, 0.01, data.trainData.ColumnCount);
                var network = new RBFNeuralNetwork(topology);
                double error = network.Train(data.trainData, epoch, false);
                useCalculated = true;
                while (epoch <= 1000)
                {
                    for (double alpha = 0.001; alpha < 0.05; alpha += 0.001)
                    {
                        int accuracy = 0;
                        topology = new Topology(data.trainData.ClassesNames?.Count ?? 1, alpha, data.trainData.ColumnCount);
                        network = new RBFNeuralNetwork(topology);
                        error = network.Train(data.trainData, epoch, useCalculated);                        
                        foreach (Record record in data.testData.Records)
                        {
                            if (checkResult(network, record))
                                accuracy++;

                            if (accuracy > bestAccuracy)
                            {
                                bestAccuracy = accuracy;
                                bestAlpha = alpha;
                            }
                        }
                        Trace.WriteLine($"Current accuracy: {accuracy} of {data.testData.Records.Count} = {accuracy * 100 / data.testData.Records.Count}% with alpha = {alpha}  epoch = {epoch}");
                        sw.WriteLine($"{epoch}   {alpha}   {error}   {accuracy * 100 / data.testData.Records.Count}");
                    }
                    epoch += epoch >= 100 ? 100 : 10;
                }
            }
            Trace.WriteLine($"Best accuracy: {bestAccuracy} of {data.testData.Records.Count} = {bestAccuracy * 100 / data.testData.Records.Count}% with alpha = {bestAlpha}");
        }

        [TestMethod]
        public void TestDatasetSize()
        {
            var bestAccuracy = 0;
            (Dataset trainData, Dataset testData) data = GetData(4);
            bool useCalculated = false;
            using (StreamWriter sw = new StreamWriter(@"E:\repos\RBFNetwork\datasetSize.txt"))
            {
                for (int i = 20; i >= 5; i--)
                {
                    data = GetData(i);
                    int accuracy = 0;
                    var topology = new Topology(data.trainData.ClassesNames?.Count ?? 1, 0.014, data.trainData.ColumnCount);
                    var network = new RBFNeuralNetwork(topology);
                    double error = network.Train(data.trainData, 30, useCalculated);
                    useCalculated = true;
                    var reverseDict = data.testData.ClassesNames.ToDictionary(k => k.Value, v => v.Key);
                    foreach (Record record in data.testData.Records)
                    {
                        double[] outputs = network.Predict(record.InputData);
                        var maxValue = outputs.Max();
                        int maxIndex = network.OutputNeurons.FindIndex(n => n.Output == maxValue);

                        string resultLabel = reverseDict[maxIndex];

                        if (resultLabel == record.Label)
                            accuracy++;

                        if (accuracy > bestAccuracy)
                            bestAccuracy = accuracy;
                    }
                    Trace.WriteLine($"Current accuracy: {accuracy} of {data.testData.Records.Count} = {accuracy * 100 / data.testData.Records.Count}%");
                    sw.WriteLine($"{data.trainData.RowCount}   {error}   {accuracy * 100 / data.testData.Records.Count}");
                }
            }
            Console.ReadKey();
        }


        [TestMethod]
        public void TestEpochCount()
        {
            var data = GetData(10);
            var bestAccuracy = 0;

            bool useCalculated = false;
            var reverseDict = data.testData.ClassesNames.ToDictionary(k => k.Value, v => v.Key);

            using (StreamWriter sw = new StreamWriter(@"E:\repos\RBFNetwork\epochCount.txt"))
            {
                int e = 10;
                while (e <= 1000)
                {
                    int accuracy = 0;
                    var topology = new Topology(data.trainData.ClassesNames?.Count ?? 1, 0.014, data.trainData.ColumnCount);
                    var network = new RBFNeuralNetwork(topology);
                    double error = network.Train(data.trainData, e, useCalculated);
                    useCalculated = true;
                    foreach (Record record in data.testData.Records)
                    {
                        double[] outputs = network.Predict(record.InputData);
                        var maxValue = outputs.Max();
                        int maxIndex = network.OutputNeurons.FindIndex(n => n.Output == maxValue);

                        string resultLabel = reverseDict[maxIndex];

                        if (resultLabel == record.Label)
                            accuracy++;

                        if (accuracy > bestAccuracy)
                        {
                            bestAccuracy = accuracy;
                        }
                    }
                    Trace.WriteLine($"Current accuracy: {accuracy} of {data.testData.Records.Count} = {accuracy * 100 / data.testData.Records.Count}%");
                    sw.WriteLine($"{e}   {error}   {accuracy * 100 / data.testData.Records.Count}");
                    if (e >= 100)
                        e += 100;
                    else
                        e += 10;
                }
            }
            Trace.WriteLine($"Best accuracy: {bestAccuracy} of {data.testData.Records.Count} = {bestAccuracy * 100 / data.testData.Records.Count}%");
            Console.ReadKey();
        }
    }
}
