using System;
using System.IO;
using System.Threading;

namespace NewMatrixModule
{
    [Serializable]
    public class Matrix
    {
        public int[,] _data;
        public int Height { get; private set; }
        public int Width { get; private set; }
        private const int MaxRandomValue = 1000;

        public Matrix(int heigth, int width, bool randomFill = false)
        {
            Height = heigth;
            Width = width;
            _data = new int[Height, Width];
            if (randomFill)
            {
                RandomFill();
            }
        }

        public int this[int x, int y]
        {
            get
            {
                return _data[x, y];
            }

            set
            {
                _data[x, y] = value;
            }
        }

        public double Det(CancellationToken token = default(CancellationToken))
        {
            if (Width != Height)
            {
                Console.WriteLine("Wrong dimentions!");
            }

            return FindDet(Height, _data);
        }

        public double FindDet(int n, int [,] Mat)
        {
            double d = 0;
            int k, i, j, subi, subj;
            int[,] SUBMat = new int[n, n];
            if (n == 2)
            {
                return ((Mat[0, 0] * Mat[1, 1]) - (Mat[1, 0] * Mat[0, 1]));
            }
            else
            {
                for (k = 0; k < n; k++)
                {
                    subi = 0;
                    for (i = 1; i < n; i++)
                    {
                        subj = 0;
                        for (j = 0; j < n; j++)
                        {
                            if (j == k)
                            {
                                continue;
                            }
                            SUBMat[subi, subj] = Mat[i, j];
                            subj++;
                        }
                        subi++;
                    }
                    d = d + (Math.Pow(-1, k) * Mat[0, k] * FindDet(n - 1, SUBMat));
                }
            }
            return d;
        }

        public void Assign(Matrix matrix)
        {
            Height = matrix.Height;
            Width = matrix.Width;
            _data = new int[Height, Width];
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    this[i, j] = matrix[i, j];
                }
            }
        }

        public void FillSubMatrix(Matrix source, int top, int left)
        {
            if (top + source.Height <= Height && left + source.Width <= Width)
            {
                for (int i = 0; i < source.Height; i++)
                {
                    for (int j = 0; j < source.Width; j++)
                    {
                        _data[top + i, left + j] = source[i, j];
                    }
                }
            }
        }

        public void RandomFill()
        {
            var rand = new Random();
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    _data[i, j] = rand.Next(MaxRandomValue);
                }
            }
        }

        public void WriteToFile(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(Height);
                writer.Write("x");
                writer.Write(Width);
                writer.WriteLine();

                for (var i = 0; i < Height; i++)
                {
                    for (var j = 0; j < Width; j++)
                    {
                        writer.Write(this[i, j]);
                        writer.Write(" ");
                    }

                    writer.WriteLine();
                }
            }

        }
    }
}
