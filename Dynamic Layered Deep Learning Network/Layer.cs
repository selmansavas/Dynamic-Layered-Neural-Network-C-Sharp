using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicLayeredDeepLearningNetwork
{
    class Layer
    {
        public Node[] Neurons;
        public int layerSize;
        public int layerType; // 0 = Input Layer, 1 = Hidden Layer, 2 = Output Layer
        public int layerActivationType; // 0 = Linear, 1 = Sigmoid, 2 = TanH
        public int layerIndex; // To track where we are in Network Structure

       
        public double errorTemp;

        public double[] errorDelta;

        public Layer(int _layerType, int _layerIndex, int _layerSize, Network network, Random rand)
        {
            try
            {
                Console.WriteLine("Layer Created");
                layerType = _layerType;
                layerIndex = _layerIndex;
                layerSize = _layerSize;

                if (layerType == 0)
                {
                    Neurons = new Node[layerSize];

                    for (int i = 0; i < layerSize; i++)
                    {
                        Neurons[i] = new Node();
                    }
                }
                else if (layerType == 1 || layerType == 2)
                {
                    Neurons = new Node[layerSize];

                    for (int i = 0; i < layerSize; i++)
                    {
                        Neurons[i] = new Node(network.Layers[layerIndex - 1], RandomGenerator.getRandomizer(), 1); //Last Argument "0" needs to be changed as ActivationFunction type.
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }

        public void setInputLayerInputs(double[] _inputArray)
        {
            if (layerType == 0)
            {
                for (int i = 0; i < layerSize; i++)
                {
                    Neurons[i].output = _inputArray[i];
                }
            }
        }

        public void setLayerInputs(Network _network)
        {
            if (layerType != 0)
            {
               
                for (int i = 0; i < layerSize; i++)
                {
                    for (int x = 0; x < _network.Layers[layerIndex - 1].layerSize; x++)
                    {
                        Neurons[i].input[x] = _network.Layers[layerIndex - 1].Neurons[x].output;       
                    }
                   // Neurons[i].calculateNodeOutput();
                }
            }
            
        }

        public void calculateLayerOutputs()
        {
            foreach (Node n in Neurons)
            {
                n.calculateNodeOutput();
            }
        }

        public void resetNeuronsDelta()
        {
            foreach(Node neuron in Neurons)
            {
                neuron.resetDelta();
            }
        }

        public void outputDebugPrint()
        {
            Console.WriteLine(this.layerIndex + " Layer Output");
            foreach (Node a in Neurons)
            {
                Console.WriteLine(a.output + " -- ");
            }
        }

        public void outputDeltaDebugPrint()
        {
            Console.WriteLine(this.layerIndex + " Layer Delta Output");
            foreach (Node a in Neurons)
            {
                Console.WriteLine(a.delta + " -- ");
            }
        }

        //public void outputBiasDebugPrint()
        //{
        //    Console.WriteLine(this.layerIndex + " Layer Bias Output");
        //    foreach (Node a in Neurons)
        //    {
        //        Console.WriteLine(a.bias + " -- ");
        //    }
        //}
    }
}
