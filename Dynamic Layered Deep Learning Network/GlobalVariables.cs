using System;


namespace DynamicLayeredDeepLearningNetwork
{
    public static class GlobalVariables
    {
        private static readonly Random Random = new Random();

        public static int NeuronCount;

        public static double LearningRate;
        public static double Momentum;

        public static int InputSize;
        public static int OutputSize;
        public static int LayerAmount;

        public static double GetRandom()
        {
            return 2 * Random.NextDouble() - 1;
        }


    }
}
