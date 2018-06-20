using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicLayeredDeepLearningNetwork
{
    public static class RandomGenerator
    {
        private static Random rand = new Random();
        public static Random getRandomizer()
        {
            return rand;
        }
    }
}
