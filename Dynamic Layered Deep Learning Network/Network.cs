using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicLayeredDeepLearningNetwork

{
    class Network
    {
        public List<Layer> Layers;
        public int networkSize;
        public int bestNeuronIndex;

        public double globalError;
        public double maxOutput;
        public double[] errorArray;

        public Network()
        {
            Layers = new List<Layer>();
            networkSize = 0;
            bestNeuronIndex = 0;
            maxOutput = 0;
            Console.WriteLine("Network Created");
        }

        public void addLayerToNetwork(int _size, int _layerType)
        {
            try
            {
                Layer tempLayerHolder = new Layer(_layerType, Layers.Count, _size, this, RandomGenerator.getRandomizer());
                Layers.Add(tempLayerHolder);
                networkSize++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void setInputLayerInputs(Input _inputArray)
        {
            Layers[0].setInputLayerInputs(_inputArray.inputArray);
        }


        public void feedForward()
        {
            for (int i = 1; i < networkSize; i++)
            {
                Layers[i].setLayerInputs(this);
                Layers[i].calculateLayerOutputs();
            }
        }

        public void calculateOutputError(EMNistDecoder eMNistDecoder, Output _outputArray)
        {
            for (int i = 0; i < Program.Variables.OutputSize; i++)
            {
                errorArray[i] = Layers[Layers.Count].Neurons[i].output - _outputArray.expectedOutputArray[i];
            }
        }

        public int getPrediction()
        {
            bestNeuronIndex = 0;
            maxOutput = 0;
            for (int i = 0; i < Layers[Layers.Count].Neurons.Length; i++)
            {
                // Console.WriteLine("Layer " + i + " output is = " + layer[i].output);
                if (Layers[Layers.Count].Neurons[i].output > maxOutput)
                {

                    maxOutput = Layers[Layers.Count].Neurons[i].output;
                    bestNeuronIndex = i;
                }
            }

            return bestNeuronIndex;
        }

    }
}
