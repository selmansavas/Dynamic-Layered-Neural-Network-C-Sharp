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
            public static int LearningRate;
            public static int LayerAmount;

            public static string TrainingFilePath;
            public static string TestFilePath;
        }

        

        

        

       

       


        static void Main()
        {

            int layerSize;
            int networkSizeCounter = 0; //Counts Network Size for display
            int connectionSizeCounter = 0; //Calculates Connection Size for Display
            int[] layerAndNodeAmountHolder;
           



            EMNistDecoder emnistDecoder = new EMNistDecoder();
            emnistDecoder.EMNistDecoderInit();

            Output outputArray = new Output(emnistDecoder);


            Stopwatch stopwatch = new Stopwatch();

            Network network = new Network();

            //Variables variables = new Variables();

            Random rand = new Random();

            Console.WriteLine("Please Enter Layer Amount");
            Variables.LayerAmount = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder = new int[Variables.LayerAmount];

            if (Variables.LayerAmount == 0)
            {
                return;
            }

            Console.WriteLine("Please enter Input Layer Size");
            layerSize = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder[0] = layerSize;
            Variables.InputSize = layerSize;
            Input inputArray;


            for (int i = 1; i < layerAndNodeAmountHolder.Length - 1; i++)
            {
                Console.WriteLine("Please enter " + i + "th Hidden Layer Size");
                layerSize = Convert.ToInt32(Console.ReadLine());
                layerAndNodeAmountHolder[i] = layerSize;

            }

            Console.WriteLine("Please enter Output Layer Size");
            layerSize = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder[Variables.LayerAmount - 1] = layerSize;

            stopwatch.Start();

            network.addLayerToNetwork(layerAndNodeAmountHolder[0], 0);

            for (int i = 1; i < layerAndNodeAmountHolder.Length - 1; i++)
            {
                network.addLayerToNetwork(layerAndNodeAmountHolder[i], 1);

            }

            network.addLayerToNetwork(layerAndNodeAmountHolder[Variables.LayerAmount - 1], 2);




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

