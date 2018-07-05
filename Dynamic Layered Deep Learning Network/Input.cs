using System;


namespace DynamicLayeredDeepLearningNetwork
{
    class Input
    {
        public double[] input;

        public Input(int inputSize)
        {
            input = new double[inputSize];
        }

        public void setInput(EnmistDecoder _emnistDecoder, int imageNumber)
        {
            int k = 0;
            for (int i = 0; i < _emnistDecoder.numRows; i++)
            {
                for (int j = 0; j < _emnistDecoder.numCols; j++)
                {
                    input[k] = _emnistDecoder.images[imageNumber, i, j];
                    k++;
                }
            }
        }

        public void debugPrintInput()
        {
            for (int i = 0; i < 784;)
            {
                for (int j = 0; j < 28; j++)
                {
                    Console.Write(input[i]);
                    i++;
                }
                Console.WriteLine();
            }
        }
    }
}
