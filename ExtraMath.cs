using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsC2XCore
{
    /// <summary>
    /// A static collection of math-related functions
    /// </summary>
    public static class ExtraMath
    {
        private static List<float> Weights { get; set; }
        private static List<float> BellWeights { get; set; }
        private static List<float> SinCurve { get; set; }
        private static float Precision { get; set; }
        private static bool WeightsSetup { get; set; }
        /// <summary>
        /// Returns a float of -value to +value
        /// </summary>
        /// <param name="rnd">The Random Object to do the picking</param>
        /// <param name="value">The value to base the return on (1 will return -1 to +1, 4 will return -4 to +4)</param>
        /// <returns></returns>
        public static float GetMinMaxFloat(Random rnd, float value)
        {
            return (float)(value - ((value * 2) * rnd.NextDouble()));
        }
        /// <summary>
        /// Returns a float from min to max
        /// </summary>
        /// <param name="rnd">The Random Object to do the picking</param>
        /// <param name="min">The minimum value that can be returned</param>
        /// <param name="max">The maximum value that can be returned</param>
        /// <returns></returns>
        public static float GetMinMaxFloat(Random rnd, float min, float max)
        {
            return (float)(min + ((max - min) * rnd.NextDouble()));
        }
        /// <summary>
        /// Returns a float from min to max
        /// </summary>
        /// <param name="rnd">The Random Object to do the picking</param>
        /// <param name="v1">The minimum value that can be returned</param>
        /// <param name="v2">The maximum value that can be returned</param>
        /// <returns></returns>
        public static float GetMinMaxFloatAutoChecked(Random rnd, float v1, float v2)
        {
            return (float)(Math.Min(v1, v2) + ((Math.Max(v1, v2) - Math.Min(v1, v2)) * rnd.NextDouble()));
        }
        public static float SmoothStep(float linearVal, int tightness = 1)
        {
            float returnVal = linearVal;
            for (int i = 0; i < tightness; i++)
            {
                returnVal = returnVal * returnVal * (3 - 2 * returnVal);
            }
            return returnVal;
        }
        public static float EaseInOut(float linearVal)
        {
            return (float)Math.Sin(Math.PI * linearVal);
        }
        /// <summary>
        /// Returns the distance between to Points
        /// </summary>
        /// <param name="p1">The first <see cref="Spot"/></param>
        /// <param name="p2">The second <see cref="Spot"/></param>
        /// <returns></returns>
        public static float GetDistance(Spot p1, Spot p2)
        {
            float xDiff = p1.X - p2.X;
            float yDiff = p1.Y - p2.Y;
            return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
        public static float GetAngle(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
        }
        /// <summary>
        /// Returns the height of a list of <see cref="Spot"/>(s)
        /// </summary>
        /// <param name="spotList"></param>
        /// <returns></returns>
        public static float GetHeight(List<Spot> spotList)
        {
            float minHeight = 999999;
            float maxHeight = -999999;

            float height = 0;

            foreach (Spot p in spotList)
            {
                minHeight = Math.Min(minHeight, p.Y);
                maxHeight = Math.Max(maxHeight, p.Y);
            }

            height = Math.Abs(minHeight - maxHeight);

            return height;
        }
        /// <summary>
        /// Returns the width of a list of <see cref="Spot"/>(s)
        /// </summary>
        /// <param name="pointList"></param>
        /// <returns></returns>
        public static float GetWidth(List<Spot> pointList)
        {
            float minWidth = 999999;
            float maxWidth = -999999;

            float width = 0;

            foreach (Spot p in pointList)
            {
                minWidth = Math.Min(minWidth, p.X);
                maxWidth = Math.Max(maxWidth, p.X);
            }

            width = Math.Abs(minWidth - maxWidth);

            return width;
        }
        /// <summary>
        /// Clamps a float (v) to (m)inimum and (M)aximum
        /// </summary>
        /// <param name="v">The value to clamp</param>
        /// <param name="m">The minimum the value can be</param>
        /// <param name="M">The maximum the value can be</param>
        /// <returns></returns>
        public static float Clamp(float v, float m, float M)
        {
            return Math.Max(Math.Min(v, M), m);
        }
        /// <summary>
        /// Returns a new value of baseValue by (factor / baseValue) for level amount of loops;
        /// </summary>
        /// <param name="baseValue">The base value to start from</param>
        /// <param name="factor">The modifier for the diminishing returns</param>
        /// <param name="level">How many times to run the loop</param>
        /// <returns></returns>
        public static float GetCurvedValue(float baseValue, float factor, int level)
        {
            float value = baseValue;
            if (factor != 0)
            {
                for (int i = 0; i < level; i++)
                {
                    value += factor / (value + 1);
                }
            }

            return value;
        }
        /// <summary>
        /// Returns a curved value limited by a maximum.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="addition">The addition.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public static float GetDiminishingReturn(float currentValue, float addition, float maxValue)
        {
            return Math.Min(currentValue + (addition * (1 - (currentValue / maxValue))), maxValue);
        }
        /// <summary>
        /// Returns a value that the addition is affected by the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="addition">The addition.</param>
        /// <returns></returns>
        public static float GetDiminishingReturn(float currentValue, float addition)
        {
            return currentValue + (addition / (1 + currentValue));
        }
        /// <summary>
        /// Returns a weighted random biased towards the bottom.
        /// </summary>
        /// <param name="rnd">The random.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="deviation">The deviation.</param>
        /// <returns></returns>
        public static float GetWeightedRandom(Random rnd, float min, float deviation)
        {
            if (!WeightsSetup)
            {
                SetupWeights();
                WeightsSetup = true;
            }
            return min + (deviation * Weights[rnd.Next(Weights.Count)]);
        }
        /// <summary>
        /// Returns a random value from 0 to 1 along a bell distributed list.
        /// </summary>
        /// <param name="rnd">The random.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="deviation">The deviation.</param>
        /// <returns></returns>
        public static float GetBellWeightedRandom(Random rnd, float min, float deviation)
        {
            if (!WeightsSetup)
            {
                SetupWeights();
                WeightsSetup = true;
            }
            return min + (deviation * BellWeights[rnd.Next(BellWeights.Count)]);
        }
        public static float GetGaussianRandom(Random rnd, float mean, float stdDev)
        {
            double u1 = 1.0 - rnd.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rnd.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return (float)(mean + stdDev * randStdNormal); //random normal(mean,stdDev^2)
        }
        public static float GetCurvedRandom(Random r, int tightness = 6)
        {
            /*
            float v = CurvedRandom(r);
            
            int cycle = 0;
            while (v < 0.01 && cycle < tightness)
            {
                v = CurvedRandom(r);
                cycle++;
            }
            */

            return 0;// (float)(r.NextDouble() * Bell * (r.Next(2) == 1 ? -1 : 1);
        }
        private static float CurvedRandom(Random r)
        {
            return (float)(1 - (Math.Sin((Math.PI) * r.NextDouble())));
        }
        public static float GetShapedCurvedRandom(Random r, float center, float spread, int factor = 3)
        {
            float v = 0;
            for (int i = 0; i < factor; i++)
            {
                v += (float)r.NextDouble();
            }
            v /= factor;
            return center + (spread - ((spread * 2) * v));
        }
        private static void SetupWeights()
        {
            Weights = new List<float>();
            BellWeights = new List<float>();
            Precision = 0.1f;
            //for weights list
            float current = Precision;
            float i = 0;
            while (current < 1f)
            {
                while (i < 1f - current)
                {
                    float n = current;
                    Weights.Add(n);
                    i += Precision;
                }
                i = 0;
                current += Precision;
            }

            //for bell weights list
            for (int f = 0; f < Weights.Count; f++)
            {
                BellWeights.Add(Weights[f]);
            }
            for (int f = Weights.Count - 1; f > -1; f--)
            {
                BellWeights.Add(Weights[f]);
            }

            SinCurve = new List<float>() { -1.00f, -0.99f, -0.98f, -0.97f, -0.96f, -0.95f, -0.94f, -0.93f, -0.92f, -0.91f, -0.90f, -0.89f, -0.88f, -0.87f, -0.86f, -0.85f, -0.84f, -0.83f, -0.82f, -0.81f, -0.80f, -0.79f, -0.78f, -0.77f, -0.76f, -0.75f, -0.74f, -0.73f, -0.72f, -0.71f, -0.70f, -0.69f, -0.69f, -0.68f, -0.67f, -0.66f, -0.65f, -0.64f, -0.63f, -0.62f, -0.61f, -0.60f, -0.59f, -0.58f, -0.57f, -0.57f, -0.56f, -0.55f, -0.54f, -0.53f, -0.52f, -0.51f, -0.50f, -0.49f, -0.49f, -0.48f, -0.47f, -0.46f, -0.45f, -0.44f, -0.44f, -0.43f, -0.42f, -0.41f, -0.40f, -0.39f, -0.39f, -0.38f, -0.37f, -0.36f, -0.36f, -0.35f, -0.34f, -0.33f, -0.33f, -0.32f, -0.31f, -0.30f, -0.30f, -0.29f, -0.28f, -0.28f, -0.27f, -0.26f, -0.26f, -0.25f, -0.24f, -0.24f, -0.23f, -0.22f, -0.22f, -0.21f, -0.20f, -0.20f, -0.19f, -0.19f, -0.18f, -0.18f, -0.17f, -0.16f, -0.16f, -0.15f, -0.15f, -0.14f, -0.14f, -0.13f, -0.13f, -0.12f, -0.12f, -0.11f, -0.11f, -0.10f, -0.10f, -0.10f, -0.09f, -0.09f, -0.08f, -0.08f, -0.08f, -0.07f, -0.07f, -0.06f, -0.06f, -0.06f, -0.05f, -0.05f, -0.05f, -0.04f, -0.04f, -0.04f, -0.04f, -0.03f, -0.03f, -0.03f, -0.03f, -0.02f, -0.02f, -0.02f, -0.02f, -0.02f, -0.01f, -0.01f, -0.01f, -0.01f, -0.01f, -0.01f, -0.01f, -0.01f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.02f, 0.02f, 0.02f, 0.02f, 0.02f, 0.03f, 0.03f, 0.03f, 0.03f, 0.04f, 0.04f, 0.04f, 0.04f, 0.05f, 0.05f, 0.05f, 0.06f, 0.06f, 0.06f, 0.07f, 0.07f, 0.07f, 0.08f, 0.08f, 0.09f, 0.09f, 0.09f, 0.10f, 0.10f, 0.11f, 0.11f, 0.12f, 0.12f, 0.13f, 0.13f, 0.14f, 0.14f, 0.15f, 0.15f, 0.16f, 0.16f, 0.17f, 0.17f, 0.18f, 0.19f, 0.19f, 0.20f, 0.20f, 0.21f, 0.22f, 0.22f, 0.23f, 0.23f, 0.24f, 0.25f, 0.25f, 0.26f, 0.27f, 0.27f, 0.28f, 0.29f, 0.30f, 0.30f, 0.31f, 0.32f, 0.32f, 0.33f, 0.34f, 0.35f, 0.35f, 0.36f, 0.37f, 0.38f, 0.39f, 0.39f, 0.40f, 0.41f, 0.42f, 0.43f, 0.43f, 0.44f, 0.45f, 0.46f, 0.47f, 0.48f, 0.48f, 0.49f, 0.50f, 0.51f, 0.52f, 0.53f, 0.54f, 0.55f, 0.55f, 0.56f, 0.57f, 0.58f, 0.59f, 0.60f, 0.61f, 0.62f, 0.63f, 0.64f, 0.65f, 0.66f, 0.67f, 0.67f, 0.68f, 0.69f, 0.70f, 0.71f, 0.72f, 0.73f, 0.74f, 0.75f, 0.76f, 0.77f, 0.78f, 0.79f, 0.80f, 0.81f, 0.82f, 0.83f, 0.84f, 0.85f, 0.86f, 0.87f, 0.88f, 0.89f, 0.90f, 0.91f, 0.92f, 0.93f, 0.94f, 0.95f, 0.96f, 0.97f, 0.98f, 0.99f, 1.00f };
        }
        /// <summary>
        /// Returns an index from a List of floats.
        /// </summary>
        /// <param name="pickList">The pick list.</param>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public static int PickFromWeightedList(List<float> pickList, Random r)
        {
            float runningCost = 0;
            float totalCost = 0;

            foreach (float cost in pickList)
            {
                totalCost += cost;
            }

            float drawRoll = totalCost * (float)r.NextDouble();
            int pickedIndex = -1;
            int runningIndex = 0;
            foreach (float cost in pickList)
            {
                if (runningCost <= drawRoll && drawRoll <= runningCost + cost)
                {
                    pickedIndex = runningIndex;
                }
                runningCost += cost;
                runningIndex++;
            }

            return pickedIndex;

        }
        /// <summary>
        /// Returns an index of a chosen element from a List of ints.
        /// </summary>
        /// <param name="pickList">The pick list.</param>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        public static int PickFromWeightedList(List<int> pickList, Random r)
        {
            float runningCost = 0;
            float totalCost = 0;

            foreach (float cost in pickList)
            {
                totalCost += cost;
            }

            float drawRoll = totalCost * (float)r.NextDouble();
            int pickedIndex = -1;
            int runningIndex = 0;
            foreach (float cost in pickList)
            {
                if (runningCost <= drawRoll && drawRoll <= runningCost + cost)
                {
                    pickedIndex = runningIndex;
                }
                runningCost += cost;
                runningIndex++;
            }

            return pickedIndex;

        }
        public static float GetRangedValue(float min, float max, float level)
        {
            return min + ((max - min) * level);
        }
        public static float GetDecreasingComponentValue(float v1, float v2, float factor)
        {
            return Math.Max(v1, v2) - (Math.Max(v1, v2) - Math.Min(v1, v2) * factor);
        }
        public static float GetIncreasingComponentValue(float v1, float v2, float factor)
        {
            return Math.Min(v1, v2) - (Math.Max(v1, v2) - Math.Min(v1, v2) * factor);
        }
        /// <summary>
        /// Tests the WeightedRanomd and BellWeightedRandom functions.
        /// </summary>
        /// <param name="r">The r.</param>
        public static void Test(Random r)
        {
            if (!WeightsSetup)
            {
                SetupWeights();
                WeightsSetup = true;
            }
            float weightsAverage = 0;
            float weightMin = 999999;
            float weightMax = 0;
            for (int i = 0; i < 10000; i++)
            {
                float newWeight = GetWeightedRandom(r, 0, 10);
                weightsAverage += newWeight;
                weightMin = Math.Min(weightMin, newWeight);
                weightMax = Math.Max(weightMax, newWeight);
            }
            Console.WriteLine("Weights Min:\t" + weightMin);
            Console.WriteLine("Weights Max:\t" + weightMax);
            Console.WriteLine("Weights Avg:\t" + weightsAverage / 10000f);
            float bellAverage = 0;
            float bellMin = 999999;
            float bellMax = 0;
            for (int i = 0; i < 10000; i++)
            {
                float newBell = GetBellWeightedRandom(r, 0, 10);
                bellAverage += newBell;
                bellMin = Math.Min(bellMin, newBell);
                bellMax = Math.Max(bellMax, newBell);
            }
            Console.WriteLine("Bell Min:\t" + bellMin);
            Console.WriteLine("Bell Max:\t" + bellMax);
            Console.WriteLine("Bell Avg:\t" + bellAverage / 10000f);
        }
        /// <summary>
        /// Returns a percentage.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public static float GetPercentage(float value, float minValue, float maxValue)
        {
            return value / (maxValue - minValue);
        }
        /// <summary>
        /// Returns an array of N elements that will total 1
        /// </summary>
        /// <param name="rnd">The random.</param>
        /// <param name="splits">The splits.</param>
        /// <returns></returns>
        public static float[] GetSplitPercentage(Random rnd, int splits)
        {
            float[] values = new float[splits];
            float total = 0;
            for (int i = 0; i < splits; i++)
            {
                values[i] = (float)rnd.NextDouble();
                total += values[i];
            }
            if (total != 0)
            {
                for (int i = 0; i < splits; i++)
                {
                    values[i] /= total;
                }
            }
            return values;
        }
        public static float ShortAngleDist(float angle, float targetAngle)
        {
            float max = (float)Math.PI * 2;
            float da = (targetAngle - angle) % max;
            return 2 * da % max - da;
        }
        public static float AngleLerp(float angle, float targetAngle, float t)
        {
            return angle + ShortAngleDist(angle, targetAngle) * t;
        }
        public static List<int[]> GetNeighborCells(int startX, int startY, int distance, int maxX, int maxY)
        {
            List<int[]> result = new List<int[]>();
            if (distance > 0)
            {
                for (int i = startX - distance; i <= startX + distance; i++)
                {
                    for (int j = startY - distance; j <= startY + distance; j++)
                    {
                        if (Math.Abs(startX - i) == distance || Math.Abs(startY - j) == distance)
                        {
                            if ((i != startX || j != startY))
                            {
                                if (i >= 0 && j >= 0)
                                {
                                    if (i < maxX && j < maxX)
                                    {
                                        result.Add(new int[] { i, j });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                result.Add(new int[] { MathHelper.Clamp(startX, 0, maxX - 1), MathHelper.Clamp(startY, 0, maxY - 1) });
            }
            return result;
        }
    }
}
