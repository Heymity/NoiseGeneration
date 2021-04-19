using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerlinNoise
{
    public class Noise
    {
        public Random rng;
        public int randomAmplitude;
        public int matrixHeight;
        public int matrixWidth;

        public Noise(int seed, int matrixWidth, int matrixHeight, int randomAmplitude = int.MaxValue - 1)
        {
            rng = new Random(seed);
            this.randomAmplitude = randomAmplitude;
            this.matrixHeight = matrixHeight;
            this.matrixWidth = matrixWidth;
        }

        public Noise() : this("Random Seed".GetHashCode(), 512, 512) { }

        public virtual decimal[,] GetNoiseMatrix()
        {
            var decimalMatrix = new decimal[matrixWidth, matrixHeight];

            for (int y = 0; y < matrixHeight; y++)
            {
                for(int x = 0; x < matrixWidth; x++)
                {
                    var rand = rng.Next(0, randomAmplitude);
                    var normalized = (decimal)rand / randomAmplitude;
                     
                    decimalMatrix[x, y] = normalized;
                }
            }

            return decimalMatrix;
        }

        public virtual unsafe Bitmap OutputNoiseToBitmap(decimal[,] noise = null)
        {
            noise ??= GetNoiseMatrix();

            int width = noise.GetLength(0);
            int height = noise.GetLength(1);
            Bitmap bitmap = new(width, height);

            var imageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            uint* ptr = (uint*)imageData.Scan0.ToPointer();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte color = (byte)(noise[x, y] * 255);

                    *(ptr + y * width + x) = 0xFF000000 | (uint)(color << 16) | (uint)(color << 8) | color;
                }
            }

            bitmap.UnlockBits(imageData);
            return bitmap;
        }
    }
}
