using RBFNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RBFNetwork
{
    public static class Extensions
    {
        public static List<double[]> ToListOfArrays(this List<Record> records, Func<Record, double[]> selector) => records.Select(selector).ToList();

        public static string EnumerableToString<T>(this IEnumerable<T> input) => String.Join(",", input);

        public static double GetEuclideanDistance(this double[] v1, List<double> v2)
        {
            return GetEuclideanDistance(v1, v2.ToArray());
        }
        public static double GetEuclideanDistance(this List<double> v1, List<double> v2)
        {
            return GetEuclideanDistance(v1.ToArray(), v2.ToArray());
        }

        public static double GetEuclideanDistance(this double[] v1, double[] v2)
        {
            var sum = v1.Select((v, i) => Math.Pow(v - v2[i], 2)).Sum();
            return Math.Sqrt(sum);
        }

        public static double[] GetRow(this List<double[]> matrix, int row)
        {
            var columns = matrix[0].Length;
            var array = new double[columns];
            for (int i = 0; i < columns; i++)
                array[i] = matrix[row][i];

            return array;
        }

        public static double ToCelsius(this double fahrenheit) => (fahrenheit - 32) * 5 / 9;
    }

    public class Shared
    {
        public static List<double[]> InitializeMatrix(int rows, int columns)
        {
            List<double[]> result = new List<double[]>(rows);
            for (int i = 0; i < rows; i++)
            {
                result.Add(new double[columns]);
            }
            return result;
        }
    }
}