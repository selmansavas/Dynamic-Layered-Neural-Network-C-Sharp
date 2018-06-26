﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicLayeredDeepLearningNetwork
{
    class Output
    {
        public double[] networkOutputArray;
        public double[] expectedOutputArray;


        public Output(int outputSize)
        {
            networkOutputArray = new double[outputSize];
            expectedOutputArray = new double[outputSize];

            resetExpectedOutputArray();
            //resetNetworkOutputArray();
        }
        /*
        public void setNetworkOutputArray(Network _network)
        {
            networkOutputArray[_network.bestNeuronIndex] = 1;
        }

        public void resetNetworkOutputArray()
        {
            for (int i = 0; i < networkOutputArray.Length; i++)
            {
                networkOutputArray[i] = 0;
                expectedOutputArray[i] = 0;
            }
        }
        */
        public void setExpectedOutputArray(EMNistDecoder _emnistDecoder, int _labelNum)
        {
            expectedOutputArray[_emnistDecoder.labels[_labelNum]] = 1;
        }

        public void resetExpectedOutputArray()
        {
            for (int i = 0; i < networkOutputArray.Length; i++)
            {
                expectedOutputArray[i] = 0;
            }
        }
    }
}
