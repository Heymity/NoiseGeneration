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
        NoiseGrid grid;

        public PerlinNoise(int seed, int matrixWidth, int matrixHeight, int gridWidth, int gridHeight, int randomAmplitude = int.MaxValue - 1) : base(seed, matrixWidth, matrixHeight, randomAmplitude) 
        {
            grid = new NoiseGrid(gridWidth, gridHeight, matrixWidth, matrixHeight, rng, randomAmplitude);
            grid.PopulateGrid();
        }

        float Interpolate(float a0, float a1, float w)
        {
            if (0.0 > w) return a0;
            if (1.0 < w) return a1;
           
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
            Vector2 gradient = GetRandomVector(gridX, gridY);

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
                    decimalMatrix[x, y] = (decimal)AtCoordinates(x, y);
                }
            }

            return decimalMatrix;
        }

        public float AtCoordinates(int x, int y)
        {
            var corners = grid.GetCellNodesInPos(x, y);

            float weightX = (x - corners[0, 0].pos.X) / grid.gridSpacing.X;
            float weightY = (y - corners[0, 0].pos.Y) / grid.gridSpacing.Y;

            float dotProductL, dotProductR;

            dotProductL = corners[0, 0].DistanceDotProduct(x, y, grid.gridSpacing);
            dotProductR = corners[1, 0].DistanceDotProduct(x, y, grid.gridSpacing);
            var topInterpolated = Interpolate(dotProductL, dotProductR, weightX);

            dotProductL = corners[0, 1].DistanceDotProduct(x, y, grid.gridSpacing);
            dotProductR = corners[1, 1].DistanceDotProduct(x, y, grid.gridSpacing);
            var bottomInterpolated = Interpolate(dotProductL, dotProductR, weightX);

            return (Interpolate(topInterpolated, bottomInterpolated, weightY) + 1f) / 2f;
        }

        Vector2 GetRandomVector(float ix, float iy)
        {
            //return Vector2.UnitX;
            float random = 2920f * MathF.Sin(ix * 21942f + iy * 171324f + 8912f) * MathF.Cos(ix * 23157f * iy * 217832f + 9758f);
            return new Vector2(MathF.Cos(random), MathF.Sin(random));

            //return new Vector2(
            //    (float)rng.Next(0, randomAmplitude) / (float)randomAmplitude, 
            //    (float)rng.Next(0, randomAmplitude) / (float)randomAmplitude);
        }

        internal class NoiseGrid
        {
            public Vector2 gridSize;
            public GridNode[,] nodes;
            public int randomAmplitude;
            public Random rng;

            public Vector2 gridSpacing;
            
            public NoiseGrid(int gridWidth, int gridHeight, int imageWidth, int imageHeight, Random rng, int randomAmplitude = int.MaxValue - 1)
            {
                gridSize = new Vector2(gridWidth, gridHeight);
                nodes = new GridNode[gridWidth, gridHeight];
                gridSpacing = new Vector2(imageWidth / gridWidth, imageHeight / gridHeight);
                this.rng = rng;
                this.randomAmplitude = randomAmplitude;
            }

            public void PopulateGrid()
            {
                for(int y = 0; y < gridSize.Y; y++)
                {
                    for(int x = 0; x < gridSize.X; x++)
                    {
                        // (int)(x * gridSpacing.X)
                        nodes[x, y] = new GridNode((int)(x * gridSpacing.X), (int)(y * gridSpacing.Y), GetRandomUnitVector());
                    }
                }
            }

            public GridNode[,] GetCellNodesInPos(int x, int y)
            {
                // 0 ---- gridWidth
                Vector2 relativePos = new Vector2
                {
                    X = x / gridSpacing.X,
                    Y = y / gridSpacing.Y
                };

                int xIndex = (int)MathF.Floor(relativePos.X);
                int yIndex = (int)MathF.Floor(relativePos.Y);

                GridNode[,] corners = new GridNode[2,2];

                if (xIndex < nodes.GetLength(0) && yIndex < nodes.GetLength(1))
                    corners[0, 0] = nodes[xIndex, yIndex];
                else
                    corners[0, 0] = new GridNode((int)(x * gridSpacing.X), (int)(y * gridSpacing.Y), Vector2.One);

                if (xIndex + 1 < nodes.GetLength(0) && yIndex < nodes.GetLength(1))
                    corners[1, 0] = nodes[xIndex + 1, yIndex];
                else
                    corners[1, 0] = new GridNode((int)(x * gridSpacing.X), (int)(y * gridSpacing.Y), Vector2.One);

                if (xIndex < nodes.GetLength(0) && yIndex + 1 < nodes.GetLength(1))
                    corners[0, 1] = nodes[xIndex, yIndex + 1];
                else
                    corners[0, 1] = new GridNode((int)(x * gridSpacing.X), (int)(y * gridSpacing.Y), Vector2.One);

                if (xIndex + 1 < nodes.GetLength(0) && yIndex + 1 < nodes.GetLength(1))
                    corners[1, 1] = nodes[xIndex + 1, yIndex + 1];
                else
                    corners[1, 1] = new GridNode((int)(x * gridSpacing.X), (int)(y * gridSpacing.Y), Vector2.One);

                return corners;
            }

            Vector2 GetRandomUnitVector()
            {
                var rand = rng.Next(0, randomAmplitude);
                return new Vector2(
                    MathF.Cos(rand), 
                    MathF.Sin(rand));
            }
        }

        internal struct GridNode
        {
            public Vector2 pos;
            public Vector2 gradient;

            public GridNode(int x, int y, Vector2 gradient)
            {
                pos = new Vector2(x, y);
                this.gradient = gradient;
            }

            public float DistanceDotProduct(int x, int y, Vector2 scale)
            {
                float distanceX = (x - pos.X) / scale.X;
                float distanceY = (y - pos.Y) / scale.Y;

                return (distanceX * gradient.X + distanceY * gradient.Y);
            }
        }
    }
}
