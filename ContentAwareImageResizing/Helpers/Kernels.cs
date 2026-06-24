namespace ContentAwareImageResizing.Helpers
{
	/// <summary>
	/// Kernel is a square matrix with odd sides.
	/// Kernel for finding edges should be a 3x3 matrix.
	/// </summary>
	public static class Kernels
	{
		/// <summary>
		/// Matrix sobel X is used to find vertical edges in convolution.
		/// </summary>;
		public static readonly double[,] _sobelX =
			{
			{ 1, 0, -1 },
			{ 2, 0, -2 },
			{ 1, 0, -1 }
		};

		/// <summary>
		/// Matrix sobel Y is used to find horizontal edges in convolution.
		/// </summary>
		public static readonly double[,] _sobelY =
			{
			{ 1, 2, 1 },
			{ 0, 0, 0 },
			{ -1, -2, -1 }
		};

		static Kernels()
		{
			Validate(_sobelX);
			Validate(_sobelY);
		}

		private static void Validate(double[,] kernel)
		{
			if (kernel.GetLength(0) != kernel.GetLength(1))
			{
				throw new Exception("Kernel must be a square matrix");
			}
			if (kernel.GetLength(0) % 2 != 1)
			{
				throw new Exception("Kernel must be an n x n matrix with n is an odd number.");
			}
		}
	}
}
