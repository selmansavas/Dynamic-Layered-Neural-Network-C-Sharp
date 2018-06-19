using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

static class BinaryReaderExtension
{
    public static int ReadInt32Endian(this BinaryReader br)
    {
        var bytes = br.ReadBytes(sizeof(Int32));
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }


}

namespace WindowsFormsApp1
{




    static class Program
    {
        static int _NodeCounter;

        public static int NodeCounter
        {
            get
            {
                return _NodeCounter;
            }
            set
            {
                _NodeCounter = value;
            }
        }



        class Variables
        {
            public int InputSize;
            public int OutputSize;
            public int TrainingSize;
            public int TestSize;
            public int LearningRate;
            public int LayerAmount;

            public string TrainingFilePath;
            public string TestFilePath;
        }

        public static class RandomGenerator
        {
            private static Random rand = new Random();
            public static Random getRandomizer()
            {
                return rand;
            }
        }

        class Node
        {
            public int activationType; // 0 = Linear, 1 = Sigmoid, 2 = Tanh, 3 = Relu

            public double bias;
            public double output;
            public double[] input;
            public double[] weight;


            public Node()
            {

                try
                {
                    Console.WriteLine("Node Created");
                    input = new double[1];
                    output = 0;
                    NodeCounter++;
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
                    NodeCounter++;
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

                            output = output / input.Length;
                            break;
                        }

                    case 1:
                        {
                            output = 1 / (1 + (Math.Exp(output)));
                            break;
                        }
                    case 2:
                        {
                            output = Math.Tanh(output);
                            break;
                        }
                    case 3:
                        {
                            output = Math.Max(0, output);
                            break;
                        }


                }



            }


        }

        class Layer
        {
            public Node[] Neurons;
            public int layerSize;
            public int layerType; // 0 = Input Layer, 1 = Hidden Layer, 2 = Output Layer
            public int layerActivationType; // 0 = Linear, 1 = Sigmoid, 2 = TanH
            public int layerIndex; // To track where we are in Network Structure

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
                            Neurons[i] = new Node(network.Layers[layerIndex - 1], RandomGenerator.getRandomizer(), 0);
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
                        Neurons[i].input[0] = _inputArray[i];
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
                foreach (Node n in Neurons)
                {
                    n.calculateNodeOutput();
                }
            }
        }

        class Network
        {
            public List<Layer> Layers;
            public int networkSize;

            public Network()
            {
                Layers = new List<Layer>();
                networkSize = 0;
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

            public void calculateNetworkError()
            {

            }



        }

        class Input
        {
            public double[] inputArray;

            public Input(EMNistDecoder _emnistDecoder)
            {
                inputArray = new double[_emnistDecoder.dimensions];
            }

            public void setInput(EMNistDecoder _emnistDecoder, int imageNumber)
            {
                int k = 0;
                for (int i = 0; i < _emnistDecoder.numRows; i++)
                {
                    for (int j = 0; j < _emnistDecoder.numCols; j++)
                    {
                        inputArray[k] = _emnistDecoder.images[imageNumber, i, j];
                    }
                }
            }
        }

        class Output
        {
            double[] networkOutputArray;


            public Output(EMNistDecoder _emnistDecoder)
            {
                networkOutputArray = new double[_emnistDecoder.numLabels];
            }

            public void setOutput(Network _network)
            {
                int i = 0;
                foreach(Node n in _network.Layers[_network.Layers.Count].Neurons)
                {
                    networkOutputArray[i] = n.output;
                    i++;
                }
            }
        }


        static void Main()
        {

            int layerSize;
            int[] layerAndNodeAmountHolder;
            int counter1 = 0; //Counts Network Size for display
            int counter3 = 0; //Calculates Connection Size for Display



            EMNistDecoder emnistDecoder = new EMNistDecoder();
            emnistDecoder.EMNistDecoderInit();

            Output outputArray = new Output(emnistDecoder);


            Stopwatch stopwatch = new Stopwatch();

            Network network = new Network();

            Variables variables = new Variables();

            Random rand = new Random();

            Console.WriteLine("Please Enter Layer Amount");
            variables.LayerAmount = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder = new int[variables.LayerAmount];

            if (variables.LayerAmount == 0)
            {
                return;
            }

            Console.WriteLine("Please enter Input Layer Size");
            layerSize = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder[0] = layerSize;
            variables.InputSize = layerSize;
            Input inputArray;


            for (int i = 1; i < layerAndNodeAmountHolder.Length - 1; i++)
            {
                Console.WriteLine("Please enter " + i + "th Hidden Layer Size");
                layerSize = Convert.ToInt32(Console.ReadLine());
                layerAndNodeAmountHolder[i] = layerSize;

            }

            Console.WriteLine("Please enter Output Layer Size");
            layerSize = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder[variables.LayerAmount - 1] = layerSize;

            stopwatch.Start();

            network.addLayerToNetwork(layerAndNodeAmountHolder[0], 0);

            for (int i = 1; i < layerAndNodeAmountHolder.Length - 1; i++)
            {
                network.addLayerToNetwork(layerAndNodeAmountHolder[i], 1);

            }

            network.addLayerToNetwork(layerAndNodeAmountHolder[variables.LayerAmount - 1], 2);




            counter1 = network.networkSize;

            for (int i = 0; i < network.networkSize - 1; i++)
            {
                counter3 += network.Layers[i].layerSize * network.Layers[i + 1].layerSize;
            }

            stopwatch.Stop();

            Console.WriteLine("Network has " + counter1 + " Layers");
            Console.WriteLine("Network has " + NodeCounter + " Neurons");
            Console.WriteLine("Network has " + counter3 + " Connections");
            Console.WriteLine("Creation of the network took " + stopwatch.Elapsed);

            //Console.WriteLine("Please enter Training Data Set Locaiton");
            //variables.TrainingFilePath = Console.ReadLine();

            Console.WriteLine("Magic number of training set is =" + emnistDecoder.magic1);
            Console.WriteLine("Image Count in database is = " + emnistDecoder.numImages);



            Console.Read();



            for (int i = 0; i < emnistDecoder.numImages; i++)
            {
                for (int j = 0; j < emnistDecoder.numRows; j++)
                {
                    for (int k = 0; k < emnistDecoder.numCols; k++)
                    {
                        if (emnistDecoder.images[i, j, k] == 0)
                            Console.Write("."); // white
                        else if (emnistDecoder.images[i, j, k] == 255)
                            Console.Write("O"); // black
                        else
                            Console.Write("X"); // gray

                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Image Label is " + emnistDecoder.labels[i]);
            }

            /* for (int i = 0; i < 1000; i++)
             {
                 mnistDecoder.mNistSetImage();
                 mnistDecoder.mNistSetLabel();
                 
                 Console.WriteLine();
             }
             */
            Console.ReadLine();


        }

        class EMNistDecoder
        {
            public int magic1;
            public int numImages;
            public int numRows;
            public int numCols;
            public int magic2;
            public int numLabels;
            public int dimensions;

            public byte[] labels;

            public byte[,,] images;

            BinaryReader brLabels;
            BinaryReader brImages;

            FileStream ifsLabels;
            FileStream ifsImages;


            public void EMNistDecoderInit()
            {

                ifsLabels = new FileStream(@"C:\EMNist Dataset\gzip\gzip\emnist-byclass-train-labels-idx1-ubyte\emnist-byclass-train-labels-idx1-ubyte", FileMode.Open); // test labels
                ifsImages = new FileStream(@"C:\EMNist Dataset\gzip\gzip\emnist-byclass-train-images-idx3-ubyte\emnist-byclass-train-images-idx3-ubyte", FileMode.Open); // test images

                brLabels = new BinaryReader(ifsLabels);

                brImages = new BinaryReader(ifsImages);

                magic1 = brImages.ReadInt32Endian();
                if (magic1 != 2051)
                    throw new Exception($"Invalid magic number {magic1}!");
                numImages = brImages.ReadInt32Endian();
                numRows = brImages.ReadInt32Endian();
                numCols = brImages.ReadInt32Endian();

                Console.WriteLine($"Loading {numImages} images with {numRows} rows and {numCols} columns...");

                magic2 = brLabels.ReadInt32Endian();
                if (magic2 != 2049)
                    throw new Exception($"Invalid magic number {magic2}!");
                numLabels = brLabels.ReadInt32Endian();
                if (numLabels != numImages)
                    throw new Exception($"Number of labels ({numLabels}) does not equal number of images ({numImages})");

                images = new byte[numImages, numRows, numCols];
                labels = new byte[numLabels];
                dimensions = numRows * numCols;
                for (int i = 0; i < numImages; i++)
                {
                    for (int j = 0; j < numRows; j++)
                    {
                        for (int k = 0; k < numCols; k++)
                        {
                            images[i, k, j] = brImages.ReadByte();
                        }

                    }

                    labels[i] = brLabels.ReadByte();
                }

            }

        }

        public class DigitImage
        {
            public byte[] pixels;
            public byte label;

            public DigitImage(byte[] pixels,
              byte label)
            {
                this.pixels = new byte[784];


                for (int i = 0; i < 784; ++i)
                    this.pixels[i] = pixels[i];

                this.label = label;
            }

            public override string ToString()
            {
                string s = "";
                for (int i = 0; i < 784;)
                {
                    for (int j = 0; j < 28; ++j)
                    {
                        if (this.pixels[i] == 0)
                            s += "."; // white
                        else if (this.pixels[i] == 255)
                            s += "O"; // black
                        else
                            s += "X"; // gray
                        ++i;
                    }
                    s += "\n";
                }

                Console.WriteLine();
                s += this.label.ToString();
                return s;
            } // ToString

        }



        class MnistDecoder

        {
            public int magic1;
            public int numImages;
            public int numRows;
            public int numCols;
            public int magic2;
            public int numLabels;

            public byte label;

            public byte[] pixels;

            BinaryReader brLabels;
            BinaryReader brImages;

            FileStream ifsLabels;
            FileStream ifsImages;

            public void mNistDecoderInit()
            {
                ifsLabels = new FileStream(@"D:\Selman\Temp\gzip\gzip\emnist-digits-train-labels-idx1-ubyte\emnist-digits-train-labels-idx1-ubyte", FileMode.Open); // test labels
                ifsImages = new FileStream(@"D:\Selman\Temp\gzip\gzip\emnist-digits-train-images-idx3-ubyte\emnist-digits-train-images-idx3-ubyte", FileMode.Open); // test images

                brLabels = new BinaryReader(ifsLabels);

                brImages = new BinaryReader(ifsImages);


                magic1 = BinaryReaderExtension.ReadInt32Endian(brImages); // brImages.ReadInt32(); // discard
                numImages = BinaryReaderExtension.ReadInt32Endian(brImages); //brImages.ReadInt32();
                numRows = BinaryReaderExtension.ReadInt32Endian(brImages); // brImages.ReadInt32();
                numCols = BinaryReaderExtension.ReadInt32Endian(brImages); //brImages.ReadInt32();

                magic2 = BinaryReaderExtension.ReadInt32Endian(brLabels); //brLabels.ReadInt32();
                numLabels = BinaryReaderExtension.ReadInt32Endian(brLabels); // brLabels.ReadInt32();

                pixels = new byte[784];

            }

            public void mNistSetImage()
            {
                int k = 0;
                /*for (int i = 0; i < 784; i++)
                {
                    byte b = brImages.ReadByte();
                    pixels[i] = b;
                }*/
                //BinaryReaderExtension.ReadInt32Endian(brImages);
                pixels = brImages.ReadBytes(784);


                try
                {
                    for (; k < 784;)
                    {
                        for (int j = 0; j < 28; j++)
                        {


                            if (pixels[k] == 0)
                                Console.Write("."); // white
                            else if (pixels[k] == 255)
                                Console.Write("O");// black
                            else
                                Console.Write("X"); // gray
                            k++;



                        }
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + "   " + k);
                }


            }

            public void mNistSetLabel()
            {
                label = brLabels.ReadByte();
            }



        }


    }
}

