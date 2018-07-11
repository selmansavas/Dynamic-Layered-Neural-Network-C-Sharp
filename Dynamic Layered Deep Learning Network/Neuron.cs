using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLayeredDeepLearningNetwork
{
    class Neuron
    {
        public int activationType; // 0 = Linear, 1 = Sigmoid, 2 = TanH, 3 = ReLu

        public double tempOutput;
        public double output;
        public double derivativeStorage; //Stores derivative of output, calculation depends on activationType
        public double bias;
        public double biasDelta;
        public double gradient;
        public double gradientCalculationHolder;
        public double weightDelta;

        public double[] input;
        public double[] inputWeights;



        public Neuron()
        {

            output = 0;
            GlobalVariables.NeuronCount++;
            Console.WriteLine("Neuron Created");
        }

        public Neuron(int inputArraySize, int _activationType)
        {
            input = new double[inputArraySize];
            inputWeights = new double[inputArraySize];

            for (int i = 0; i < input.Length; i++)
            {
                input[i] = 0;
                inputWeights[i] = GlobalVariables.GetRandom();
            }

            bias = GlobalVariables.GetRandom();
            //gradient = 0;
            activationType = _activationType;
            GlobalVariables.NeuronCount++;
            Console.WriteLine("Neuron Created");
        }

        public void calculateNodeOutput()
        {
            //tempOutput = 0;
            for (int i = 0; i < input.Length; i++)
            {
                output += input[i] * inputWeights[i];
            }

            output += bias;

            switch (activationType)
            {
                case 0:
                    {
                        //Console.WriteLine("Node activation type is LINEAR");
                        output = (output) / input.Length;
                        derivativeStorage = 1;
                        break;
                    }
                case 1:
                    {
                        //Console.WriteLine("Node activation type is SIGMOID");
                        output = 1 / (1 + (Math.Exp(-output)));
                        derivativeStorage = output * (1 - output);
                        break;
                    }
                case 2:
                    {
                        //Console.WriteLine("Node activation type is TANH");
                        if (bias + output < -20)
                        {
                            output = -1;
                        }
                        else if (bias + output > 20)
                        {
                            output = 1;
                        }
                        else
                        {
                            output = Math.Tanh(output);
                        }
                        //output = Math.Tanh(bias + output);
                        derivativeStorage = 1 - (Math.Pow(output, output));
                        break;
                    }
                case 3:
                    {
                        //Console.WriteLine("Node activation type is RELU");
                        output = Math.Max(0, output);
                        derivativeStorage = output > 0 ? 1 : 0;
                        break;
                    }
            }
        }

        public void resetGradientCalculationHolder()
        {
            gradientCalculationHolder = 0;
        }

    }
}
