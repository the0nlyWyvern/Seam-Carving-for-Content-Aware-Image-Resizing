namespace ContentAwareImageResizing.Helpers
{
	public static class MatrixMath
	{
		/// <summary>
		/// Apply convolution to a single cell: matrix[r, c].
		/// Edge handling: Extend.
		/// Corner pixels are extended in 90° wedges. Other edge pixels are extended in lines.
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="kernel"></param>
		/// <param name="r">Row index</param>
		/// <param name="c">Column index</param>
		/// <returns>A vector in which direction obeys kernel's characteristic.</returns>
		public static double CellConvolve(double[,] matrix, double[,] kernel, int r, int c)
		{
			double vector = 0;

			for (int i = 0; i < kernel.GetLength(0); i++)
			{
				for (int j = 0; j < kernel.GetLength(1); j++)
				{
					int rowIndex = r + i - 1;
					int colIndex = c + j - 1;

					if (rowIndex < 0)
					{
						rowIndex = 0;
					}
					else if (rowIndex >= matrix.GetLength(0))
					{
						rowIndex = matrix.GetLength(0) - 1;
					}

					if (colIndex < 0)
					{
						colIndex = 0;
					}
					else if (colIndex >= matrix.GetLength(1))
					{
						colIndex = matrix.GetLength(1) - 1;
					}

					vector += matrix[rowIndex, colIndex] * kernel[i, j];
				}
			}

			return vector;
		}


		/// <summary>
		/// Convolution on a portion of the matrix. Edge handling: Extend.  
		/// </summary>
		/// <param name="matrix">Matrix (grayscale image)</param>
		/// <param name="kernel">3x3 square matrix</param>
		/// <returns>A vector field in which direction obeys kernel's characteristic.</returns>
		public static double[,] Convolve(double[,] matrix, double[,] kernel)
		{
			double[,] vectorField = new double[matrix.GetLength(0), matrix.GetLength(1)];

			for (int r = 0; r < matrix.GetLength(0); r++)
			{
				for (int c = 0; c < matrix.GetLength(1); c++)
				{
					vectorField[r, c] = CellConvolve(matrix, kernel, r, c);
				}
			}

			return vectorField;
		}


		/// <summary>
		/// Find the magnitude of two vector fields with formula: G = Sqrt((Gx)^2 + (Gy)^2).
		/// </summary>
		/// <param name="x">Vector field in x direction</param>
		/// <param name="y">Vector field in y direction</param>
		/// <returns>Magnitude matrix.</returns>
		public static double[,] CalculateMagnitude(double[,] x, double[,] y)
		{
			int height = x.GetLength(0);
			int width = x.GetLength(1);

			double[,] result = new double[height, width];

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					// Formula
					result[i, j] = Math.Sqrt(x[i, j] * x[i, j] + y[i, j] * y[i, j]);
				}
			}

			return result;
		}
	}
}
