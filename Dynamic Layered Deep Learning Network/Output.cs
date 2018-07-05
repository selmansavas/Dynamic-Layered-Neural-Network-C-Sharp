using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLayeredDeepLearningNetwork
{
    class Output
    {
        public double[] expectedOutputArray;


        public Output(int outputSize)
        {
            //networkOutputArray = new double[outputSize];
            expectedOutputArray = new double[outputSize];

            resetExpectedOutputArray();
            //resetNetworkOutputArray();
        }

        public void setExpectedOutputArray(int _labelNum)
        {
            expectedOutputArray[_labelNum] = 1;
        }

        public void resetExpectedOutputArray()
        {
            for (int i = 0; i < expectedOutputArray.Length; i++)
            {
                expectedOutputArray[i] = 0;
            }
        }

    }
}
