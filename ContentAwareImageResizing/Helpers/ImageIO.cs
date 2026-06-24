// Six Labors version should be lower then v4 because of license.
// Meanwhile v3 is license free.
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace ContentAwareImageResizing.Helpers
{
	public static class ImageIO
	{
		/// <summary>
		/// Read the image.
		/// </summary>
		/// <param name="path">Path to the file.</param>
		/// <exception cref="FileNotFoundException"></exception>
		public static (byte, byte, byte)[,] ReadImage(string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Could not find this file: " + path);
			}

			(byte R, byte G, byte B)[,] newImage;

			using (Image<Rgba32> img = Image.Load<Rgba32>(path))
			{
				int height = img.Height;
				int width = img.Width;

				// initialize the matrix of RGB image
				newImage = new (byte R, byte G, byte B)[height, width];


				// Read the data thru the external library
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						Rgba32 pixel = img[j, i];
						newImage[i, j].R = pixel.R;
						newImage[i, j].G = pixel.G;
						newImage[i, j].B = pixel.B;
					}
				}
			}
			return newImage;
		}


		/// <summary>
		/// Create a matrix of grayscale image with the formula: 
		/// grayValue = R*coeffR + G*coeffG + B*coeffB
		/// </summary>
		/// <param name="coeffR">Coefficient for red</param>
		/// <param name="coeffG">Coefficient for green</param>
		/// <param name="coeffB">Coefficient for blue</param>
		/// <exception cref="ArgumentException"></exception>
		public static double[,] CreateGrayImage((byte R, byte G, byte B)[,] img, double coeffR = 0.299, double coeffG = 0.587, double coeffB = 0.114)
		{
			double sumOfCoefficients = coeffR + coeffG + coeffB;
			if (sumOfCoefficients < 0.9999 || sumOfCoefficients > 1)
			{
				throw new ArgumentException("Total of coefficients must equal or close to 1.");
			}

			int height = img.GetLength(0);
			int width = img.GetLength(1);

			// initialize the matrix of grayscale image
			double[,] newImage = new double[height, width];

			for (int r = 0; r < height; r++)
			{
				for (int c = 0; c < width; c++)
				{
					newImage[r, c] = coeffR * img[r, c].R + coeffG * img[r, c].G + coeffB * img[r, c].B;
				}
			}
			return newImage;
		}


		/// <summary>
		/// Save image as jpeg file.
		/// </summary>
		/// <param name="matrix">Either energy map or grayscale image.</param>
		/// <param name="height">Height of image</param>
		/// <param name="width">Width of image</param>
		/// <param name="path">Destination of saved file</param>
		public static void SaveAs(double[,] matrix, int height, int width, string path)
		{
			using (Image<Rgba32> img = new(width, height))
			{
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						byte pixel = (byte)Math.Round(matrix[i, j]);
						img[j, i] = new Rgba32(pixel, pixel, pixel, 255);
					}
				}
				img.Save(path, new JpegEncoder());
			}
		}


		/// <summary>
		/// Save image.
		/// </summary>
		/// <param name="matrix">Colored image</param>
		/// <param name="height">Height of image</param>
		/// <param name="width">Width of image</param>
		/// <param name="path">Destination of saved file</param>
		public static void SaveAs((byte R, byte G, byte B)[,] matrix, int height, int width, string path)
		{
			using (Image<Rgba32> img = new(width, height))
			{
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						(byte R, byte G, byte B) = matrix[i, j];
						img[j, i] = new Rgba32(R, G, B, 255);
					}
				}
				img.Save(path, new JpegEncoder());
			}
		}
	}
}
