using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicLayeredDeepLearningNetwork
{
    class Layer
    {
        public Neuron[] Neurons;
        public Network network;
        public int layerSize;
        public int layerType;
        public int layerActivationType;
        public int layerIndex;
        public int arrayI = 0;
        public int arrayX = 0;
        public int maxThreads = 0;
        public int availableThreads = 0;
        public int placeHolder = 0;

        public Layer(Network _network, int _layerIndex, int _layerSize, int _layerType, int _layerActivationType)
        {
            layerType = _layerType;
            layerSize = _layerSize;
            layerIndex = _layerIndex;
            layerActivationType = _layerActivationType;
            network = _network;
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

        async public void setLayerInputs(Network _network)
        {
            AutoResetEvent autoReset = new AutoResetEvent(false);
           // ThreadPool.SetMaxThreads(16, 0);
            //ThreadPool.GetMaxThreads(out maxThreads, out placeHolder);
            if (layerType != 0)
            {
                for (arrayI = 0; arrayI < layerSize -1 ; arrayI++)
                {
                    Parallel.For(arrayX, (network.Layers[layerIndex - 1].layerSize), setNeuronInput);
                    arrayX = 0;
                    //for (arrayX = 0; ; arrayX++)
                    //{
                    //    ThreadPool.QueueUserWorkItem(new WaitCallback(setNeuronInput), null);

                    //    //Neurons[i].input[x] = _network.Layers[layerIndex - 1].Neurons[x].output;
                    //}
                }
                
            }
            
            //ThreadPool.GetAvailableThreads(out availableThreads, out placeHolder);
            //while (availableThreads != maxThreads)
            //{
            //    ThreadPool.GetAvailableThreads(out availableThreads, out placeHolder);
            //}
            //return;

        }

        public void setNeuronInput(int i)
        {
            try
            {
                //Console.WriteLine("Thread is working");
                Neurons[arrayI].input[arrayX] = network.Layers[layerIndex - 1].Neurons[arrayX].output;
                arrayX++;
                //Console.WriteLine(arrayX);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("ArrayI is : " + arrayI);
                Console.WriteLine("ArrayX is : " + arrayX);
                Console.ReadLine();
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
