using System;
using System.Diagnostics;
using System.Threading;

namespace DynamicLayeredDeepLearningNetwork
{
    class Program
    {

        static void Main(string[] args)
        {
            int layerSize;
            int inputLayerSize;
            int outputLayerSize;
            int networkSizeCounter = 0; //Counts Network Size for display
            int connectionSizeCounter = 0; //Calculates Connection Size for Display
            int imageAndLabelCounter = 0;
            int totalIterationsCounter = 0;
            int globalSuccessCounter = 0;
            int globalUnsuccessCounter = 0;
            int layerActivationType = 1;
            int networkOutputLayerIndex;

            double globalError = 0;

            float imageCount;
            float succesfulPredictions = 0;
            float unsuccesfulPredictions = 0;
            float successCurrentHolder = 0;


            int[] layerAndNodeAmountHolder;

            string labelFilePath;
            string imagesFilePath;

            labelFilePath = @"C:\EMNist Dataset\gzip\gzip\emnist-mnist-train-labels-idx1-ubyte\emnist-mnist-train-labels-idx1-ubyte";
            imagesFilePath = @"C:\EMNist Dataset\gzip\gzip\emnist-mnist-train-images-idx3-ubyte\emnist-mnist-train-images-idx3-ubyte";

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

            EnmistDecoder enmistDecoder = new EnmistDecoder();
            enmistDecoder.enmistDecoderInit(labelFilePath, imagesFilePath);

            imageCount = enmistDecoder.numImages;

            //inputLayerSize = enmistDecoder.dimensions;
            Console.WriteLine("Please enter classification output parameter");
            outputLayerSize = Convert.ToInt32(Console.ReadLine());

            GlobalVariables.OutputSize = outputLayerSize;
            GlobalVariables.InputSize = enmistDecoder.dimensions;

            GlobalVariables.LearningRate = 0.001f;
            GlobalVariables.Momentum = 0f;

            Output outputArray = new Output(GlobalVariables.OutputSize);


            Stopwatch stopwatch = new Stopwatch();

            Network network = new Network();

            //GlobalVariables GlobalVariables = new GlobalVariables();

            Random rand = new Random();

            Console.WriteLine("Please Enter Hidden Layer Amount");
            GlobalVariables.LayerAmount = Convert.ToInt32(Console.ReadLine());
            layerAndNodeAmountHolder = new int[GlobalVariables.LayerAmount];


            //Console.WriteLine("Please enter Input Layer Size");
            //layerSize = Convert.ToInt32(Console.ReadLine());

            //layerAndNodeAmountHolder[0] = layerSize;

            //GlobalVariables.InputSize = inputLayerSize;
            Input inputArray = new Input(GlobalVariables.InputSize);


            for (int i = 0; i < layerAndNodeAmountHolder.Length; i++)
            {
                Console.WriteLine("Please enter " + (i + 1) + "th Hidden Layer Size");
                layerSize = Convert.ToInt32(Console.ReadLine());
                layerAndNodeAmountHolder[i] = layerSize;

            }

            //Console.WriteLine("Please enter Output Layer Size");
            //layerSize = Convert.ToInt32(Console.ReadLine());
            // layerSize = EnmistDecoder.labels.Length;
            //layerAndNodeAmountHolder[GlobalVariables.LayerAmount - 1] = layerSize;


            stopwatch.Start();

            network.addLayerToNetwork(GlobalVariables.InputSize, 0, 0);

            for (int i = 0; i < layerAndNodeAmountHolder.Length; i++)
            {
                network.addLayerToNetwork(layerAndNodeAmountHolder[i], 1, layerActivationType);
            }

            network.addLayerToNetwork(GlobalVariables.OutputSize, 2, 0);

            networkOutputLayerIndex = network.Layers.Count - 1;


            networkSizeCounter = network.networkSize;

            for (int i = 0; i < network.networkSize - 1; i++)
            {
                connectionSizeCounter += network.Layers[i].layerSize * network.Layers[i + 1].layerSize;
            }

            stopwatch.Stop();

            Console.WriteLine("Network has " + networkSizeCounter + " Layers");
            Console.WriteLine("Network has " + GlobalVariables.NeuronCount + " Neurons");
            Console.WriteLine("Network has " + connectionSizeCounter + " Connections");
            Console.WriteLine("Creation of the network took " + stopwatch.Elapsed);

            //Console.WriteLine("Please enter Training Data Set Locaiton");
            //GlobalVariables.TrainingFilePath = Console.ReadLine();

            Console.WriteLine("Magic number of training set is =" + enmistDecoder.magic1);
            Console.WriteLine("Image Count in database is = " + enmistDecoder.numImages);



            Console.Read();
            Console.Clear();

            stopwatch.Start();
            while (true)
            {
                imageAndLabelCounter = 0;
                succesfulPredictions = 0;
                unsuccesfulPredictions = 0;
                while (imageAndLabelCounter < imageCount)
                {
                    //EnmistDecoder.EnmistDecoderPrint(imageAndLabelCounter);
                    inputArray.setInput(enmistDecoder, imageAndLabelCounter);
                    //inputArray.debugPrintInput();
                    //Thread.Sleep(1000);
                    outputArray.setExpectedOutputArray(enmistDecoder.getCurrentImageLabel(imageAndLabelCounter));
                    network.setInputLayerOutputs(inputArray);
                    network.feedForward();
                    //network.printNetworkLayerOutputDelta(2);
                    //network.printNetworkLayerBias(2);
                    //network.printNetworkLayerOutput(2);
                    Console.WriteLine("Actual Output was : " + enmistDecoder.labels[imageAndLabelCounter]);
                    Console.WriteLine("Network prediction was : " + network.getPrediction());


                    //Console.ReadLine();
                    //if (network.getPrediction() > 62)
                    //{
                    //    Console.WriteLine("OUTPUT ERROR");
                    //    Console.ReadLine();
                    //}

                    if (network.getPrediction() == enmistDecoder.getCurrentImageLabel(imageAndLabelCounter))
                    {
                        Console.WriteLine("Succesful Prediction!");
                        //Console.WriteLine("\n##Succesful Values##");
                        //network.printNetworkLayerOutputDelta(2);
                        //network.printNetworkLayerBias(2);
                        //network.printNetworkLayerOutput(2);
                        succesfulPredictions++;
                        globalSuccessCounter++;
                    }
                    else
                    {
                        Console.WriteLine("Bad Prediction :(...");



                        unsuccesfulPredictions++;
                        globalUnsuccessCounter++;
                        network.calculateOutputErrorAndGradient(outputArray);
                        globalError = network.returnErrorAverage();
                        network.calculateHiddenLayerGradients();
                        network.updateWeightsAndBiases();


                    }
                    successCurrentHolder = (succesfulPredictions / (imageAndLabelCounter + 1) * 100);

                    Console.WriteLine("Success Rate is : %" + successCurrentHolder);
                    Console.WriteLine("Training Progress : " + ((imageAndLabelCounter + 1) / imageCount) * 100);
                    Console.WriteLine("Error is : " + globalError);
                    Console.WriteLine("Time Passed : " + stopwatch.Elapsed);
                    imageAndLabelCounter++;
                    totalIterationsCounter++;
                    //network.printNetworkLayerOutputDelta(1);
                    //network.printNetworkLayerBias(1);
                    //network.printNetworkLayerOutput(1);
                    Console.WriteLine("Global Success Counter = " + globalSuccessCounter);
                    Console.WriteLine("Global Fail Counter = " + globalUnsuccessCounter);
                    outputArray.resetExpectedOutputArray();
                    //network.resetAllNeuronsGradientCalculationHolderValues();


                    //Console.ReadLine();

                    Console.SetCursorPosition(0, 0);
                    // Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Entering Test Phase");
            labelFilePath = @"C:\EMNist Dataset\gzip\gzip\emnist-mnist-test-labels-idx1-ubyte\emnist-mnist-test-labels-idx1-ubyte";
            imagesFilePath = @"C:\EMNist Dataset\gzip\gzip\emnist-mnist-test-images-idx3-ubyte\emnist-mnist-test-images-idx3-ubyte";
            enmistDecoder.enmistDecoderInit(labelFilePath, imagesFilePath);
            imageCount = enmistDecoder.numImages;


            Console.ReadLine();
            Console.Clear();

            succesfulPredictions = 0;
            unsuccesfulPredictions = 0;
            imageAndLabelCounter = 0;

            while (imageAndLabelCounter < imageCount)
            {
                //EnmistDecoder.EnmistDecoderPrint(imageAndLabelCounter);
                inputArray.setInput(enmistDecoder, imageAndLabelCounter);
                //inputArray.debugPrintInput();
                //Thread.Sleep(1000);
                outputArray.setExpectedOutputArray(enmistDecoder.getCurrentImageLabel(imageAndLabelCounter));
                network.setInputLayerOutputs(inputArray);
                network.feedForward();
                //network.printNetworkLayerOutputDelta(2);
                //network.printNetworkLayerBias(2);
                //network.printNetworkLayerOutput(2);
                Console.WriteLine("Actual Output was : " + enmistDecoder.labels[imageAndLabelCounter]);
                Console.WriteLine("Network prediction was : " + network.getPrediction());


                //Console.ReadLine();

                if (network.getPrediction() == enmistDecoder.getCurrentImageLabel(imageAndLabelCounter))
                {
                    Console.WriteLine("Succesful Prediction!");
                    //Console.WriteLine("\n##Succesful Values##");
                    //network.printNetworkLayerOutputDelta(2);
                    //network.printNetworkLayerBias(2);
                    //network.printNetworkLayerOutput(2);
                    succesfulPredictions++;
                    globalSuccessCounter++;
                }
                else
                {
                    Console.WriteLine("Bad Prediction :(...");



                    unsuccesfulPredictions++;
                    globalUnsuccessCounter++;
                    network.calculateOutputErrorAndGradient(outputArray);
                    globalError = network.returnErrorAverage();
                    network.calculateHiddenLayerGradients();
                    network.updateWeightsAndBiases();


                }
                Console.WriteLine("Success Rate is : %" + ((succesfulPredictions / (imageAndLabelCounter + 1)) * 100));
                Console.WriteLine("Training Progress : " + ((imageAndLabelCounter + 1) / imageCount) * 100);
                Console.WriteLine("Error is : " + globalError);
                imageAndLabelCounter++;
                totalIterationsCounter++;
                //network.printNetworkLayerOutputDelta(networkOutputLayerIndex);
                //network.printNetworkLayerBias(networkOutputLayerIndex);
                //network.printNetworkLayerOutput(networkOutputLayerIndex);
                Console.WriteLine("Global Success Counter = " + globalSuccessCounter);
                Console.WriteLine("Global Fail Counter = " + globalUnsuccessCounter);
                outputArray.resetExpectedOutputArray();
                //network.resetAllNeuronsGradientCalculationHolderValues();


                //Console.ReadLine();

                //while (true)
                //{
                //    if (Console.Read() == 1)
                //    {
                //        Thread.Sleep(1);
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
                //Console.Clear();
                Console.SetCursorPosition(0, 0);
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
    }
}
