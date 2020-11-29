using RBFNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI
{
    public class NeuralNetworkModel
    {
        private RBFNeuralNetwork neuralNetwork;
        private Dataset trainDataset = new Dataset();
        private Dataset testDataset = new Dataset();

        public NeuralNetworkModel(Dataset dataset, Dataset testSet)
        {
            //for (int i = 0; i < dataset.RowCount; i++)
            //    if (i < (dataset.RowCount*0.95))
            //    trainDataset.Records.Add(dataset.Records[i]);
            //else
            //        testDataset.Records.Add(dataset.Records[i]);

            for (int i = 0; i < dataset.RowCount; i++)
                trainDataset.Records.Add(dataset.Records[i]);

            for (int i = 0; i < testSet.RowCount; i++)
                testDataset.Records.Add(testSet.Records[i]);

            var topology = new Topology(trainDataset.ClassesNames?.Count ?? 1, 0.01, trainDataset.ColumnCount);
            neuralNetwork = new RBFNeuralNetwork(topology);
        }

        public double Train()
        {
            trainDataset.Normalize();
            double error = neuralNetwork.Train(trainDataset, 30);
            return error;
        }

        public List<DataPoint> Predict()
        {
            List<DataPoint> result = new List<DataPoint>();
            int k = 1;

            double inputsSum = testDataset.Records.Sum(r => r.InputData.Sum(x => x * x) + r.Expected.Sum(x => x * x));
            inputsSum = Math.Sqrt(inputsSum);

            testDataset.Normalize();

            foreach (Record record in testDataset.Records)
            {
                var expected = record.Expected[0];
                var output = neuralNetwork.Predict(record.InputData)[0];

                var diff = expected - output;
                var dp = new DataPoint(expected * inputsSum, output * inputsSum, diff * diff, k);
                result.Add(dp);
                k++;
                //k += 7;
                //if (k > 30)
                //    k = 0;
            }
            return result;
        }
    }
}
