using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLayeredDeepLearningNetwork
{
    class Layer
    {
        public Neuron[] Neurons;

        public int layerSize;
        public int layerType;
        public int layerActivationType;
        public int layerIndex;

        public Layer(Network network, int _layerIndex, int _layerSize, int _layerType, int _layerActivationType)
        {
            layerType = _layerType;
            layerSize = _layerSize;
            layerIndex = _layerIndex;
            layerActivationType = _layerActivationType;

            Neurons = new Neuron[layerSize];

            if (layerType == 0)
            {
                for (int i = 0; i < layerSize; i++)
                {
                    Neurons[i] = new Neuron();
                }
            }
            else
            {
                for (int i = 0; i < layerSize; i++)
                {
                    Neurons[i] = new Neuron(network.Layers[layerIndex - 1].layerSize, layerActivationType);
                }

            }

            Console.WriteLine("Layer Created");
        }

        public void setInputLayerOutputs(Input _inputArray)
        {
            if (layerType == 0)
            {
                for (int i = 0; i < layerSize; i++)
                {
                    Neurons[i].output = _inputArray.input[i];
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

                }
            }

        }

        public void calculateLayerOutputs()
        {
            foreach (Neuron n in Neurons)
            {
                n.calculateNodeOutput();
            }
        }

        public void resetNeuronsGradientCalculationHolder()
        {
            foreach (Neuron n in Neurons)
            {
                n.resetGradientCalculationHolder();
            }
        }

        public void outputDebugPrint()
        {
            Console.WriteLine(this.layerIndex + " Layer Output");
            foreach (Neuron a in Neurons)
            {
                Console.WriteLine(a.output + " -- ");
            }
        }

        public void outputBiasDebugPrint()
        {
            Console.WriteLine(this.layerIndex + " Layer Bias");
            foreach (Neuron a in Neurons)
            {
                Console.WriteLine(a.bias + " -- ");
            }
        }

        public void outputGradientDebugPrint()
        {
            Console.WriteLine(this.layerIndex + " Layer Delta Output");
            foreach (Neuron a in Neurons)
            {
                Console.WriteLine(a.gradient + " -- ");
            }
        }

    }
}
