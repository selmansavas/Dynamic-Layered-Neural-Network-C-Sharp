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


        public void feedForward(Network network)
        {
            
            for (int i = 1; i < networkSize; i++)
            {
                Layers[i].setLayerInputs(network);
                Layers[i].calculateLayerOutputs();
            }
        }

        public void calculateOutputError( Output _outputArray)
        {
            Console.Write("\n\n Output Error---\n");
            for (int i = 0; i < Layers[Layers.Count - 1].layerSize; i++)
            {
                Layers[Layers.Count - 1].Neurons[i].error = _outputArray.expectedOutputArray[i] - Layers[Layers.Count - 1].Neurons[i].output;
                //Layers[Layers.Count - 1].Neurons[i].error = (0.5f) * (( _outputArray.expectedOutputArray[i] -Layers[Layers.Count - 1].Neurons[i].output ) * (_outputArray.expectedOutputArray[i] - Layers[Layers.Count - 1].Neurons[i].output));
                Console.WriteLine(i + "th nodes error : " + Layers[Layers.Count - 1].Neurons[i].error);
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
                        Layers[layerCounter].Neurons[nodeCounterInner].delta +=  Layers[layerCounter + 1].Neurons[nodeCounterOuter].weight[nodeCounterInner] * Layers[layerCounter + 1].Neurons[nodeCounterOuter].delta;
                        //Console.WriteLine(nodeCounterOuter + "th Node's delta is  : " + Layers[layerCounter].Neurons[nodeCounterInner].delta);
                    }
                   // Console.WriteLine();
                }
                foreach(Node n in Layers[layerCounter].Neurons)
                {
                    n.delta *= n.derivativeStorage;
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
                    double prevBias = Layers[layerCounter].Neurons[nodeCounterOuter].biasDelta;
                    Layers[layerCounter].Neurons[nodeCounterOuter].biasDelta = _learningRate * Layers[layerCounter].Neurons[nodeCounterOuter].delta;
                    Layers[layerCounter].Neurons[nodeCounterOuter].bias += Layers[layerCounter].Neurons[nodeCounterOuter].biasDelta * prevBias;
                    //Layers[layerCounter].Neurons[nodeCounterOuter].bias *= _learningRate * Layers[layerCounter].Neurons[nodeCounterOuter].delta;
                    
                    for (int nodeCounterInner = 0; nodeCounterInner < Layers[layerCounter - 1].layerSize; nodeCounterInner++)
                    {
                        double prevDelta = Layers[layerCounter].Neurons[nodeCounterOuter].delta;
                        Layers[layerCounter].Neurons[nodeCounterOuter].delta = _learningRate * Layers[layerCounter].Neurons[nodeCounterOuter].delta * Layers[layerCounter - 1].Neurons[nodeCounterInner].output;
                        Layers[layerCounter].Neurons[nodeCounterOuter].weight[nodeCounterInner] += Layers[layerCounter].Neurons[nodeCounterOuter].delta * prevDelta;
                        //Layers[layerCounter].Neurons[nodeCounterOuter].weight[nodeCounterInner] += _learningRate * Layers[layerCounter - 1].Neurons[nodeCounterInner].output * Layers[layerCounter].Neurons[nodeCounterOuter].delta;
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
           // Layers[layer].outputBiasDebugPrint();
        }

        public void printNetworkLayerOutputDelta(int layer)
        {

            Console.WriteLine();
            Layers[layer].outputDeltaDebugPrint();
        }

        public void printInputDebug()
        {
            foreach(Layer l in Layers)
            {
                Console.WriteLine(l.layerIndex + "th layer inputs\n");
                foreach(Node n in Layers[l.layerIndex].Neurons)
                {
                    int i = 0;
                    Console.WriteLine(i + "th Node input is : "+  n.output);
                    i++;
                }
            }
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
