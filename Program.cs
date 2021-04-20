using System;
using System.Drawing;

namespace PerlinNoise
{
    class Program
    {
        private const int Width = 512;
        private const int Height = 512;
        private const string SavePath = @"C:\Users\GABRIEL\Desktop\LangFiles\C#\NoiseGeneration\Test";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            PerlinNoise noise = new("Random Seed".GetHashCode(), Width, Height, 50, 50);
            
            Bitmap image = noise.OutputNoiseToBitmap(); 
            
            image.Save(@$"{SavePath}\Noise.png");
        }
    }
}
