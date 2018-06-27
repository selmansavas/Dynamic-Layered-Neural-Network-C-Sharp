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

        public void calculateOutputError( Output _outputArray)
        {
            for (int i = 0; i < Layers[Layers.Count - 1].layerSize; i++)
            {
                Layers[Layers.Count - 1].Neurons[i].error = _outputArray.expectedOutputArray[i] - Layers[Layers.Count - 1].Neurons[i].output;
                //Output nodes gradient delta is calculated at calculateNodeOutput
                Layers[Layers.Count - 1].Neurons[i].delta = Layers[Layers.Count - 1].Neurons[i].derivativeStorage * Layers[Layers.Count - 1].Neurons[i].error;
            }
        }

        public void startBackPropogation()
        {
            for (int layerCounter = Layers.Count - 2; layerCounter > 0; layerCounter--)
            {
                for (int nodeCounterOuter = 0; nodeCounterOuter < Layers[layerCounter + 1].layerSize; nodeCounterOuter++)
                {
                    for (int nodeCounterInner = 0; nodeCounterInner < Layers[layerCounter].layerSize; nodeCounterInner++)
                    {
                        Layers[layerCounter].Neurons[nodeCounterInner].delta += Layers[layerCounter].Neurons[nodeCounterInner].derivativeStorage * Layers[layerCounter + 1].Neurons[nodeCounterOuter].weight[nodeCounterInner] * Layers[layerCounter + 1].Neurons[nodeCounterOuter].delta;
                    }
                }
            }
        }

        public void updateWeightsAndBiases()
        {
            double _learningRate = Program.Variables.LearningRate;

            for (int layerCounter = (Layers.Count - 1); layerCounter > 0; layerCounter--)
            {
                for (int nodeCounterOuter = 0; nodeCounterOuter < Layers[layerCounter].layerSize; nodeCounterOuter++)
                {
                    Layers[layerCounter].Neurons[nodeCounterOuter].bias = _learningRate * Layers[layerCounter].Neurons[nodeCounterOuter].delta;
                    
                    for (int nodeCounterInner = 0; nodeCounterInner < Layers[layerCounter - 1].layerSize; nodeCounterInner++)
                    {
                        Layers[layerCounter].Neurons[nodeCounterOuter].weight[nodeCounterInner] = _learningRate * Layers[layerCounter - 1].Neurons[nodeCounterInner].output * Layers[layerCounter].Neurons[nodeCounterOuter].delta;
                    }
                }
            }

            
        }

        public int getPrediction()
        {
            bestNeuronIndex = 0;
            maxOutput = 0;
            for (int i = 0; i < Layers[Layers.Count - 1].Neurons.Length; i++)
            {
                //Console.WriteLine("Layer " + i + " output is = " + Layers[Layers.Count - 1].Neurons[i].output);
                if (Layers[Layers.Count - 1].Neurons[i].output > maxOutput)
                {

                    maxOutput = Layers[Layers.Count - 1].Neurons[i].output;
                    bestNeuronIndex = i;
                }
            }

            return bestNeuronIndex;
        }

        public void printNetworkLayerOutput(int layer)
        {
            Console.WriteLine();
            Layers[layer].outputDebugPrint();
        }

        public void printNetworkLayerBias(int layer)
        {
            Console.WriteLine();
            Layers[layer].outputBiasDebugPrint();
        }

        public void printNetworkLayerOutputDelta(int layer)
        {

            Console.WriteLine();
            Layers[layer].outputDeltaDebugPrint();
        }

        public void resetAllDeltaValues()
        {
            foreach (Layer layer in Layers)
            {
                layer.resetNeuronsDelta();
            }
        }

    }
}
