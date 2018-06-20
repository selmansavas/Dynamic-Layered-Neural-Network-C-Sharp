using System;
using System.IO;

namespace DynamicLayeredDeepLearningNetwork
{
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

        ifsLabels = new FileStream(@"D:\Selman\Temp\gzip\gzip\emnist-byclass-train-labels-idx1-ubyte\emnist-byclass-train-labels-idx1-ubyte", FileMode.Open); // test labels
        ifsImages = new FileStream(@"D:\Selman\Temp\gzip\gzip\emnist-byclass-train-images-idx3-ubyte\emnist-byclass-train-images-idx3-ubyte", FileMode.Open); // test images

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

        Program.Variables.OutputSize = numLabels;

        dimensions = numRows * numCols;

        Program.Variables.InputSize = dimensions;

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
}
