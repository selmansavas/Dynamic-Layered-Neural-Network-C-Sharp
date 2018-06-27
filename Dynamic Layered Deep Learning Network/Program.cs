using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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

namespace DynamicLayeredDeepLearningNetwork
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



        public static class Variables
        {
            public static int InputSize;
            public static int OutputSize;
            public static int TrainingSize;
            public static int TestSize;
            public static int LayerAmount;

            public static double LearningRate;

            public static string TrainingFilePath;
            public static string TestFilePath;
        }

        

        

        

       

       


        static void Main()
        {

            int layerSize;
            int inputLayerSize;
            int outputLayerSize;
            int networkSizeCounter = 0; //Counts Network Size for display
            int connectionSizeCounter = 0; //Calculates Connection Size for Display
            int imageAndLabelCounter = 0;
            float imageCount;
            float succesfulPredictions = 0;
            float unsuccesfulPredictions = 0;
            int[] layerAndNodeAmountHolder;

            string labelFilePath;
            string imagesFilePath;

            labelFilePath = @"C:\Users\Selman\source\repos\Dynamic Layered Deep Learning Network\Dynamic-Layered-Neural-Network-C-Sharp\emnist-mnist-train-labels-idx1-ubyte";
            imagesFilePath = @"C:\Users\Selman\source\repos\Dynamic Layered Deep Learning Network\Dynamic-Layered-Neural-Network-C-Sharp\emnist-mnist-train-images-idx3-ubyte";

        /*
            Console.WriteLine("Please enter labelFilePath");
            while(true)
            {
                labelFilePath = Console.ReadLine();
                if(File.Exists(labelFilePath))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Entered wrong file location. Please enter labelFilePath");
                }
            }

            Console.WriteLine("Please enter imagesFilePath");

            while (true)
            {
                imagesFilePath = Console.ReadLine();
                if (File.Exists(imagesFilePath))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Entered wrong file location. Please enter imagesFilePath");
                }
            }
            */

            EMNistDecoder emnistDecoder = new EMNistDecoder();
            emnistDecoder.EMNistDecoderInit(labelFilePath,imagesFilePath);

            imageCount = emnistDecoder.numImages;

            inputLayerSize = emnistDecoder.dimensions;
            Console.WriteLine("Please enter classification output parameter");
            outputLayerSize = Convert.ToInt32(Console.ReadLine());

            Variables.OutputSize = outputLayerSize;
            Variables.LearningRate = 0.05f;
            Output outputArray = new Output(Variables.OutputSize);


            Stopwatch stopwatch = new Stopwatch();

            Network network = new Network();

            //Variables variables = new Variables();

            Random rand = new Random();

            Console.WriteLine("Please Enter Hidden Layer Amount");
            Variables.LayerAmount = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder = new int[Variables.LayerAmount];


            //Console.WriteLine("Please enter Input Layer Size");
            //layerSize = Convert.ToInt32(Console.ReadLine());

            //layerAndNodeAmountHolder[0] = layerSize;

            Variables.InputSize = inputLayerSize;
            Input inputArray = new Input(emnistDecoder);


            for (int i = 0; i < layerAndNodeAmountHolder.Length; i++)
            {
                Console.WriteLine("Please enter " + (i + 1) + "th Hidden Layer Size");
                layerSize = Convert.ToInt32(Console.ReadLine());
                layerAndNodeAmountHolder[i] = layerSize;

            }

            //Console.WriteLine("Please enter Output Layer Size");
            //layerSize = Convert.ToInt32(Console.ReadLine());
           // layerSize = emnistDecoder.labels.Length;
            //layerAndNodeAmountHolder[Variables.LayerAmount - 1] = layerSize;

            
            stopwatch.Start();

            network.addLayerToNetwork(inputLayerSize, 0);

            for (int i = 0; i < layerAndNodeAmountHolder.Length ; i++)
            {
                network.addLayerToNetwork(layerAndNodeAmountHolder[i], 1);

            }

            network.addLayerToNetwork(outputLayerSize, 2);




            networkSizeCounter = network.networkSize;

            for (int i = 0; i < network.networkSize - 1; i++)
            {
                connectionSizeCounter += network.Layers[i].layerSize * network.Layers[i + 1].layerSize;
            }

            stopwatch.Stop();

            Console.WriteLine("Network has " + networkSizeCounter + " Layers");
            Console.WriteLine("Network has " + NodeCounter + " Neurons");
            Console.WriteLine("Network has " + connectionSizeCounter + " Connections");
            Console.WriteLine("Creation of the network took " + stopwatch.Elapsed);

            //Console.WriteLine("Please enter Training Data Set Locaiton");
            //variables.TrainingFilePath = Console.ReadLine();

            Console.WriteLine("Magic number of training set is =" + emnistDecoder.magic1);
            Console.WriteLine("Image Count in database is = " + emnistDecoder.numImages);



            Console.Read();
            Console.Clear();
            imageAndLabelCounter = 17;

            while(/*imageAndLabelCounter < imageCount*/ true)
            {
                emnistDecoder.emnistDecoderPrint(imageAndLabelCounter);
                inputArray.setInput(emnistDecoder,imageAndLabelCounter);
                //inputArray.debugPrintInput();
                //Thread.Sleep(1000);
                outputArray.setExpectedOutputArray(emnistDecoder.getCurrentImageLabel(imageAndLabelCounter));
                network.setInputLayerInputs(inputArray);
                network.feedForward();
                Console.WriteLine("Network prediction was : " + network.getPrediction());
                //Console.ReadLine();
                
                if(network.getPrediction() == emnistDecoder.getCurrentImageLabel(imageAndLabelCounter))
                {
                    Console.WriteLine("Succesful Prediction!");
                    //Console.WriteLine("\n##Succesful Values##");
                    //network.printNetworkLayerOutputDelta(2);
                    //network.printNetworkLayerBias(2);
                    //network.printNetworkLayerOutput(2);
                    succesfulPredictions++;
                    unsuccesfulPredictions++;
                    network.calculateOutputError(outputArray);
                    network.startBackPropogation();
                    network.updateWeightsAndBiases();
                }
                else
                {
                    Console.WriteLine("Bad Prediction :(...");

                    //Console.WriteLine("##Before Values##");
                    //network.printNetworkLayerOutputDelta(2);
                    //network.printNetworkLayerBias(2);
                    //network.printNetworkLayerOutput(2);

                    unsuccesfulPredictions++;
                    network.calculateOutputError(outputArray);
                    network.startBackPropogation();
                    network.updateWeightsAndBiases();


                    //Console.WriteLine("\n##After Values##");
                    //network.printNetworkLayerOutputDelta(2);
                    //network.printNetworkLayerBias(2);
                    //network.printNetworkLayerOutput(2);
                }
                Console.WriteLine("Success Rate is : %" + ((succesfulPredictions / (imageAndLabelCounter + 1)) * 100));
                Console.WriteLine("Training Progress : " + ((imageAndLabelCounter + 1)/imageCount) * 100);
                //imageAndLabelCounter++;
                network.printNetworkLayerOutputDelta(2);
                network.printNetworkLayerBias(2);
                network.printNetworkLayerOutput(2);
                outputArray.resetExpectedOutputArray();
                network.resetAllDeltaValues();
                Console.SetCursorPosition(0, 0);
               
                //Console.ReadLine();
               // Console.Clear();
                // Thread.Sleep(1000);
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

