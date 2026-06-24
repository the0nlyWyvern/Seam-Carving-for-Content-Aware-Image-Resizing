namespace ContentAwareImageResizing.Helpers
{
	public static class Utils
	{
		/// <summary>
		/// Generate the path from the filename.
		/// </summary>
		/// <param name="destination">Folder name. It is either input or output. 
		/// If not specified, go to the root</param>
		/// <param name="fileName">Filename</param>
		/// <returns>Path leading to destination.</returns>
		public static string GeneratePath(string destination, string fileName)
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			if (destination == "input")
			{
				return Path.Combine(baseDirectory, @"..\..\..\..\InputImages\" + fileName);
			}
			else if (destination == "output")
			{
				return Path.Combine(baseDirectory, @"..\..\..\..\OutputImages\" + fileName);
			}

			return Path.Combine(baseDirectory, @"..\..\..\..\" + fileName);
		}


		/// <summary>
		/// The new width can be a specific value or a percentage of the original value.
		/// </summary>
		/// <param name="size">A value in percentage ("%") of the original value</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static int ReadSize(string size, int originalSize)
		{
			int strLength = size.Length;

			int newSize = int.MaxValue;

			if (size[strLength - 1] == '%')
			{
				int percentage = int.Parse(size.Substring(0, strLength - 1));
				newSize = (int)Math.Round((double)originalSize * percentage / 100);
			}

			//else if (size[n - 2] == 'p' && size[n - 1] == 'x')
			//{
			//	newSize = int.Parse(size.Substring(0, n - 2));
			//}

			return newSize;
		}


		/// <summary>
		/// Creating a copy of image.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static (byte, byte, byte)[,] DeepCopy((byte, byte, byte)[,] source)
		{
			int height = source.GetLength(0);
			int width = source.GetLength(1);

			(byte, byte, byte)[,] destination = new (byte, byte, byte)[height, width];

			for (int r = 0; r < height; r++)
			{
				for (int c = 0; c < width; c++)
				{
					destination[r, c] = source[r, c];
				}
			}

			return destination;
		}
	}
}
