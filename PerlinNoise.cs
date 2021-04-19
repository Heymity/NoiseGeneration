using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PerlinNoise
{
    public class PerlinNoise : Noise
    {
        public PerlinNoise(int seed, int matrixWidth, int matrixHeight, int randomAmplitude = int.MaxValue - 1) : base(seed, matrixWidth, matrixHeight, randomAmplitude) { }

        float Interpolate(float a0, float a1, float w)
        {
            /* // You may want clamping by inserting:
             * if (0.0 > w) return a0;
             * if (1.0 < w) return a1;
             */
            //var a = (a1 - a0) * w + a0; 
            //return (a1 - a0) * w + a0;
            /* // Use this cubic interpolation [[Smoothstep]] instead, for a smooth appearance:
             * return (a1 - a0) * (3.0 - w * 2.0) * w * w + a0;
             *
             * // Use [[Smootherstep]] for an even smoother result with a second derivative equal to zero on boundaries:
             */
             return (a1 - a0) * ((w * (w * 6.0f - 15.0f) + 10.0f) * w * w * w) + a0;
        }

        float AtCoordinatesOnce(float x, float y)
        {
            var gridX = (int)x;
            var gridY = (int)y;

            float weigthX = x - (float)gridX;
            float weigthY = y - (float)gridY;

            var n0 = DotProduct(gridX, gridY, x, y);
            var n1 = DotProduct(gridX + 1, gridY, x, y);
            float interpolateX0 = Interpolate(n0, n1, weigthX);
            
            n0 = DotProduct(gridX, gridY + 1, x, y);
            n1 = DotProduct(gridX + 1, gridY + 1, x, y);
            float interpolateX1 = Interpolate(n0, n1, weigthX);

            var debug = Interpolate(interpolateX0, interpolateX1, weigthY);
            return (Interpolate(interpolateX0, interpolateX1, weigthY) + 1) / 2f;
        }

        float DotProduct(int gridX, int gridY, float x, float y)
        {
            Vector2 gradient = GetRandomVector(x, y);

            // Compute the distance vector
            float dx = x - (float)gridX;
            float dy = y - (float)gridY;

            // Compute the dot-product
            return (dx * gradient.X + dy * gradient.Y);
        }

        public override decimal[,] GetNoiseMatrix()
        {
            var decimalMatrix = new decimal[matrixWidth, matrixHeight];

            for (int y = 0; y < matrixHeight; y++)
            {
                for (int x = 0; x < matrixWidth; x++)
                {
                    var tmp = AtCoordinatesOnce(x / 512f, y / 512f);
                    decimalMatrix[x, y] = (decimal)AtCoordinatesOnce(x / 10f, y / 10f);
                }
            }

            return decimalMatrix;
        }

        Vector2 GetRandomVector(float ix, float iy)
        {
            //float random = 2920f * MathF.Sin(ix * 21942f + iy * 171324f + 8912f) * MathF.Cos(ix * 23157f * iy * 217832f + 9758f);
            //return new Vector2(MathF.Cos(random), MathF.Sin(random));

            return new Vector2(
                (float)rng.Next(0, randomAmplitude) / (float)randomAmplitude, 
                (float)rng.Next(0, randomAmplitude) / (float)randomAmplitude);
        }
    }
}
