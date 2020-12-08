using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace RBFNetwork
{
    public class DataStructure
    {
        [Index(0)]
        public double SepalLength { get; set; }
        [Index(1)]
        public double SepalWidth { get; set; }
        [Index(2)]
        public double PetalLength { get; set; }
        [Index(3)]
        public double PetalWidth { get; set; }
        [Index(4)]
        public string Class { get; set; }

        public override string ToString()
        {
            return Class;
        }
    }
    public class Record
    {
        public double[] InputData { get; set; }
        public double[] Expected { get; set; }
        public double A { get; set; }
        public string Label { get; set; }

        public Record(double[] InputData, double[] Expected, string label = null)
        {
            this.InputData = InputData;
            this.Expected = Expected;
            this.Label = label;
        }

        public Record() { }

        public override string ToString()
        {
            return $"Input: [{InputData.EnumerableToString()}], Expected: [{Expected.EnumerableToString()}], Label: {Label}";
        }
    }

    public class Dataset : IEnumerable
    {
        public List<Record> Records { get; set; }
        public Dictionary<string,int> ClassesNames { get; set; }
        public int RowCount => Records.Count;
        public int ColumnCount => Records[0].InputData.Length;

        private static int numberOfDataColumns = 4; 

        public Dataset(Stream stream) : this()
        {           
            Load(new StreamReader(stream));
        }

        public Dataset()
        {
            this.Records = new List<Record>();
        }

        public void SetInputsFromList(List<double[]> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
                Records[i].InputData = inputs[i];
        }

        public void SetExpectedFromList(List<double[]> expectedData)
        {
            for (int i = 0; i < expectedData.Count; i++)
                Records[i].Expected = expectedData[i];
        }

        public void LoadFromSinus()
        {
            int windowLength = 7;
            int count = 0;
            double[] inputData = new double[windowLength];
            for (double a = 0; a < 360; a += 0.1)
            {
                var sinA = Math.Sin(Math.PI * a / 180.0);
                SetInputValue(windowLength, ref count, ref inputData, sinA);
            }
        }

        private void SetInputValue(int windowLength, ref int count, ref double[] inputData, double sinA)
        {
            if (count == 0)
            {
                inputData = new double[windowLength];
                inputData[count] = sinA;
                count++;
            }
            else if (count == windowLength)
            {
                var rec = new Record(inputData, new double[1] { sinA });
                Records.Add(rec);
                count = 0;
            }
            else
            {
                inputData[count] = sinA;
                count++;
            }
        }

        public async Task LoadTemperature(StreamReader sr, int windowLength = 6)
        {
            using (CsvReader reader = new CsvReader(sr))
            {
                reader.Configuration.HasHeaderRecord = false;
                reader.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                double value;
                int count = 0;
                double[] inputData = new double[windowLength];
                while (await reader.ReadAsync())
                {
                    List<double> row = new List<double>();
                    for (int i = 0; reader.TryGetField(i, out value); i++)
                    {
                        if (value == -99)
                            value = random.Next(-5, 70) + random.NextDouble();
                        row.Add(value.ToCelsius());
                    }

                    SetInputValue(windowLength, ref count, ref inputData, row[0]);
                }
            }
        }

        public async Task LoadTestTemparature(StreamReader sr)
        {
            using (CsvReader reader = new CsvReader(sr))
            {
                reader.Configuration.HasHeaderRecord = false;
                reader.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                double value;
                while (await reader.ReadAsync())
                {
                    List<double> row = new List<double>();
                    for (int i = 0; reader.TryGetField(i, out value); i++)
                    {
                        if (value == -99)
                            value = random.Next(-5, 70) + random.NextDouble();
                        Record rec = new Record();
                        rec.InputData = new double[] { value.ToCelsius() };
                        Records.Add(rec);
                    }
                }
            }

            var expected = Records.Skip(31).ToList();
            Records.RemoveAll(r => expected.Contains(r));
            for (int i = 0; i < Records.Count; i++)
            {
                Records[i].Expected = expected[i].InputData;
            }
        }

        public void Load(StreamReader sr)
        {
            using (CsvReader reader = new CsvReader(sr))
            {
                reader.Configuration.HasHeaderRecord = false;
                reader.Configuration.Delimiter = ",";
                reader.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                double value;
                while (reader.Read())
                {
                    List<double> row = new List<double>();
                    for (int i = 0; reader.TryGetField(i, out value); i++)                    
                        row.Add(value);
                    
                    Record record = new Record();
                    record.InputData = row.Skip(1).ToArray();
                    record.Label = row.First().ToString();
                    Records.Add(record);
                }
            }

            //получаем уникальные классы и записываем их в словарь. Ключ - класс, значение - индекс. 
            this.ClassesNames = Records.Select(rec => rec.Label).Distinct().ToDictionary(k => k, v => int.Parse(v) - 1);
            var labelsCount = ClassesNames.Count;
            foreach (var rec in Records)
            {
                var expected = new double[labelsCount];
                var labelNum = double.Parse(rec.Label);
                for (int i = 0; i < labelsCount; i++)
                    if (i + 1 == labelNum)
                        expected[i] = 1.0;
                    else
                        expected[i] = 0.0;

                rec.Expected = expected;
            }

        }

        private static Random random = new Random();


        public void NormalizeInputs()
        {
            SetInputsFromList(GetNormalizedData(Records.ToListOfArrays(r => r.InputData)));
            SetExpectedFromList(GetNormalizedData(Records.ToListOfArrays(r => r.Expected)));
        }

        // Нормализация
        public void Normalize()
        {

            double inputsSum = Records.Sum(r => r.InputData.Sum(x => x * x) + r.Expected.Sum(x => x * x));
            inputsSum = Math.Sqrt(inputsSum);
            foreach (var record in Records)
            {
                for (int i = 0; i < record.InputData.Length; i++)
                    record.InputData[i] /= inputsSum;
                for (int i = 0; i < record.Expected.Length; i++)
                    record.Expected[i] /= inputsSum;
                // inputsSum преобразовать
            }
        }

        public void Denormalize()
        {
            double inputsSum = Records.Select(r => r.InputData.Sum(x => x * x)).Sum();
            double expectedSum = Records.Select(r => r.Expected.Sum(x => x * x)).Sum();
            inputsSum = Math.Sqrt(inputsSum);
            expectedSum = Math.Sqrt(inputsSum);
            foreach (var record in Records)
            {
                for (int i = 0; i < record.InputData.Length; i++)
                    record.InputData[i] *= inputsSum;
                for (int i = 0; i < record.Expected.Length; i++)
                    record.Expected[i] *= expectedSum;
            }
        }

        public List<double[]> GetNormalizedData(List<double[]> Data)
        {
            var result = new List<double[]>(Data.Count);

            for (int i = 0; i < Data.Count; i++)
                result.Add(new double[Data[0].Length]);

            for (int column = 0; column < Data[0].Length; column++)
            {
                //вычисляем среднее значение сигнала нейрона
                var sum = 0.0;

                for (int row = 0; row < Data.Count; row++)
                    sum += Data[row][column];

                var average = sum / Data.Count;

                // СКО нейрона
                var error = 0.0;
                for (int row = 0; row < Data.Count; row++)
                    error += Math.Pow(Data[row][column] - average, 2);

                var standartError = Math.Sqrt(error / Data.Count);

                for (int row = 0; row < Data.Count; row++)
                    result[row][column] = (Data[row][column] - average) / standartError;
            }
            return result;
        }

        public IEnumerator GetEnumerator()
        {
            return Records.GetEnumerator();
        }
    } 
}
