using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicLayeredDeepLearningNetwork
{
    class Input
    {
        public double[] inputArray;

        public Input(EMNistDecoder _emnistDecoder)
        {
            inputArray = new double[_emnistDecoder.dimensions];
        }

        public void setInput(EMNistDecoder _emnistDecoder, int imageNumber)
        {
            int k = 0;
            for (int i = 0; i < _emnistDecoder.numRows; i++)
            {
                for (int j = 0; j < _emnistDecoder.numCols; j++)
                {
                    inputArray[k] = _emnistDecoder.images[imageNumber, i, j];
                }
            }
        }
    }
}
