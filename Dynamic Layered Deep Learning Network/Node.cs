using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicLayeredDeepLearningNetwork
{
    class Node
    {
        public int activationType; // 0 = Linear, 1 = Sigmoid, 2 = Tanh, 3 = Relu

        public double bias;
        public double output;
        public double error;
        public double[] input;
        public double[] weight;


        public Node()
        {

            try
            {
                Console.WriteLine("Node Created");
                input = new double[1];
                output = 0;
                Program.NodeCounter++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public Node(Layer previousLayer, Random rand, int _activationType)
        {
            try
            {
                Console.WriteLine("Node Created");
                input = new double[previousLayer.Neurons.Length];
                weight = new double[previousLayer.Neurons.Length];

                for (int i = 0; i < weight.Length; i++)
                {
                    weight[i] = rand.NextDouble();
                }

                bias = rand.NextDouble();
                output = 0;
                activationType = _activationType;
                Program.NodeCounter++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void calculateNodeOutput()
        {
            for (int i = 0; i < input.Length; i++)
            {
                output += input[i] * weight[i];
            }
            switch (activationType)
            {
                case 0:
                    {
                        output =(bias + output) / input.Length;
                        break;
                    }
                case 1:
                    {
                        output = 1 / (1 + (Math.Exp(bias + output)));
                        break;
                    }
                case 2:
                    {
                        output = Math.Tanh(bias + output);
                        break;
                    }
                case 3:
                    {
                        output = Math.Max(0.01f, bias + output);
                        break;
                    }


            }



        }

    }
}
