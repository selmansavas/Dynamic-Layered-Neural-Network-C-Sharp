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
        public double delta;
        public double derivativeStorage;
       // public double[] weightDelta;
        public double[] input;
        public double[] weight;


        public Node()
        {

            try
            {
                Console.WriteLine("Node Created");
                //input = new double[1];
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

                for (int i = 0; i < previousLayer.Neurons.Length; i++)
                {
                    weight[i] = rand.NextDouble();
                    input[i] = 0;
                }

                bias = rand.NextDouble();
                //bias = 0;
                output = 0;
                activationType = _activationType;
                delta = rand.NextDouble();
                //delta = 0;
                Program.NodeCounter++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           // calculateNodeOutput();

        }

        public void calculateNodeOutput()
        {
            output = 0;
            for (int i = 0; i < input.Length; i++)
            {
                output += input[i] * weight[i];
            }
            switch (activationType)
            {
                case 0:
                    {
                        //Console.WriteLine("Node activation type is LINEAR");
                        output = (bias + output) / input.Length;
                        break;
                    }
                case 1:
                    {
                        //Console.WriteLine("Node activation type is SIGMOID");
                        output = 1 / (1 + (Math.Exp((bias + output)-1)));
                        derivativeStorage = output * (1 - output);
                        break;
                    }
                case 2:
                    {
                        //Console.WriteLine("Node activation type is TANH");
                        output = Math.Tanh(bias + output);
                        derivativeStorage = 1 - (output * output);
                        break;
                    }
                case 3:
                    {
                        //Console.WriteLine("Node activation type is RELU");
                        output = Math.Max(0.01f, bias + output);
                        derivativeStorage = output > 0.01f ? 1 : 0;
                        break;
                    }
            }
        }

        public void resetDelta()
        {
            delta = 0;
        }

       

    }
}
