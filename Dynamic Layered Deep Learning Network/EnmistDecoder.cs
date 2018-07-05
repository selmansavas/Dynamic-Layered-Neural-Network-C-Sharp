using System;
using System.IO;

namespace DynamicLayeredDeepLearningNetwork
{
    public class EnmistDecoder
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


        public void enmistDecoderInit(string labelFilePath, string imagesFilePath)
        {

            ifsLabels = new FileStream(labelFilePath, FileMode.Open); // test labels
            ifsImages = new FileStream(imagesFilePath, FileMode.Open); // test images

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

            GlobalVariables.OutputSize = numLabels;

            dimensions = numRows * numCols;

            GlobalVariables.InputSize = dimensions;

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

        public int getCurrentImageLabel(int index)
        {
            return labels[index];
        }

        public void enmistDecoderPrint(int currentIndex)
        {

            for (int j = 0; j < numRows; j++)
            {
                for (int k = 0; k < numCols; k++)
                {
                    if (images[currentIndex, j, k] == 0)
                        Console.Write("."); // white
                    else if (images[currentIndex, j, k] == 255)
                        Console.Write("O"); // black
                    else
                        Console.Write("X"); // gray

                }
                Console.WriteLine();
            }
            Console.WriteLine("Image Label is " + labels[currentIndex]);
            return;
        }

    }
}
