using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLayeredDeepLearningNetwork
{
    class Network
    {
        public List<Layer> Layers;

        public Layer tempLayerHolder;

        public int networkSize;
        public int bestNeuronIndex;

        public double maxOutput;
        public double[] errorArray;

        public Network()
        {
            Layers = new List<Layer>();
            networkSize = 0;
            bestNeuronIndex = 0;
            maxOutput = 0;
            errorArray = new double[GlobalVariables.OutputSize];
            Console.WriteLine("Network Created");
        }

        public void addLayerToNetwork(int _layerSize, int _layerType, int _layerActivationType)
        {
            tempLayerHolder = new Layer(this, networkSize, _layerSize, _layerType, _layerActivationType);
            Layers.Add(tempLayerHolder);
            networkSize++;
        }

        public void setInputLayerOutputs(Input _inputArray)
        {
            Layers[0].setInputLayerOutputs(_inputArray);
        }

        public void feedForward()
        {
            for (int i = 1; i < networkSize; i++)
            {
                Layers[i].setLayerInputs(this);
                Layers[i].calculateLayerOutputs();
            }
        }


        public void calculateOutputErrorAndGradient(Output _outputArray)
        {
            // Console.Write("\n\n Output Error---\n");
            for (int i = 0; i < Layers[Layers.Count - 1].layerSize; i++)
            {
                errorArray[i] = _outputArray.expectedOutputArray[i] - Layers[Layers.Count - 1].Neurons[i].output;
                // Console.WriteLine(i + "th nodes error : " + errorArray[i]);
                //Output nodes gradient delta is calculated at calculateNodeOutput
                Layers[Layers.Count - 1].Neurons[i].gradient = Layers[Layers.Count - 1].Neurons[i].derivativeStorage * errorArray[i];
            }
        }

        public void calculateHiddenLayerGradients()
        {
            for (int layerCounter = Layers.Count - 2; layerCounter >= 0; layerCounter--)
            {
                for (int nodeCounterInner = 0; nodeCounterInner < Layers[layerCounter].layerSize; nodeCounterInner++)
                {
                    for (int nodeCounterOuter = 0; nodeCounterOuter < Layers[layerCounter + 1].layerSize; nodeCounterOuter++)
                    {
                        Layers[layerCounter].Neurons[nodeCounterInner].gradientCalculationHolder += Layers[layerCounter + 1].Neurons[nodeCounterOuter].gradient * Layers[layerCounter + 1].Neurons[nodeCounterOuter].inputWeights[nodeCounterInner];
                    }

                    Layers[layerCounter].Neurons[nodeCounterInner].gradient = Layers[layerCounter].Neurons[nodeCounterInner].gradientCalculationHolder * Layers[layerCounter].Neurons[nodeCounterInner].derivativeStorage;
                }

            }
        }

        public double returnErrorAverage()
        {
            return errorArray.Average();
        }

        public void updateWeightsAndBiases()
        {
            double prevDelta;

            for (int layerCounter = Layers.Count - 1; layerCounter > 1; layerCounter--)
            {
                for (int nodeCounterOuter = 0; nodeCounterOuter < Layers[layerCounter].layerSize; nodeCounterOuter++)
                {
                    prevDelta = Layers[layerCounter].Neurons[nodeCounterOuter].biasDelta;
                    Layers[layerCounter].Neurons[nodeCounterOuter].biasDelta = GlobalVariables.LearningRate * Layers[layerCounter].Neurons[nodeCounterOuter].gradient;
                    Layers[layerCounter].Neurons[nodeCounterOuter].bias += Layers[layerCounter].Neurons[nodeCounterOuter].biasDelta + GlobalVariables.Momentum * prevDelta;

                    for (int nodeCounterInner = 0; nodeCounterInner < Layers[layerCounter - 1].layerSize; nodeCounterInner++)
                    {
                        prevDelta = Layers[layerCounter].Neurons[nodeCounterOuter].weightDelta;
                        Layers[layerCounter].Neurons[nodeCounterOuter].weightDelta = GlobalVariables.LearningRate * Layers[layerCounter].Neurons[nodeCounterOuter].gradient * Layers[layerCounter - 1].Neurons[nodeCounterInner].output;
                        //Console.WriteLine("Shift Amount = " + (Layers[layerCounter].Neurons[nodeCounterOuter].weightDelta + GlobalVariables.Momentum * prevDelta));
                        Layers[layerCounter].Neurons[nodeCounterOuter].inputWeights[nodeCounterInner] += Layers[layerCounter].Neurons[nodeCounterOuter].weightDelta + GlobalVariables.Momentum * prevDelta;
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
            Layers[layer].outputGradientDebugPrint();
        }

        public void resetAllNeuronsGradientCalculationHolderValues()
        {
            foreach (Layer layer in Layers)
            {
                layer.resetNeuronsGradientCalculationHolder();
            }
        }


    }
}
