using System;
using System.Drawing;

namespace PerlinNoise
{
    class Program
    {
        private const int Width = 512;
        private const int Height = 512;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            PerlinNoise noise = new("Random Seed".GetHashCode(), Width, Height);
            
            Bitmap image = noise.OutputNoiseToBitmap(); 
            
            image.Save(@"D:\Gabriel\LangFiles\C#\PerlinNoise\Test\Noise.png");
        }
    }
}
