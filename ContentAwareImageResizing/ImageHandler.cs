using ContentAwareImageResizing.Helpers;

namespace ContentAwareImageResizing
{
	public class ImageHandler
	{
		private (byte R, byte G, byte B)[,] _img;
		private readonly string _imgName;

		public ImageHandler(string fileName)
		{
			// read name of the input image
			string[] arr = fileName.Split(".");
			_imgName = arr[0]; // remove the extension


			// Check whether the file exists & Read the colored image
			string path = Utils.GeneratePath("input", _imgName + ".jpg");
			_img = ImageIO.ReadImage(path);
		}


		/// <summary>
		/// Print image's width and height in pixel.
		/// </summary>
		public void PrintDimension()
		{
			Console.WriteLine("Image name: " + _imgName);
			Console.WriteLine("width x height: " + _img.GetLength(1) + " x " + _img.GetLength(0));
		}


		/// <summary>
		/// ShrinkTo image in both height and width. If height is reduced, only the final
		/// result is saved. Energy map, grayscale image, and marked seams are not saved as images.
		/// If height is preserved (100% for desiredHeight), then the user can see
		/// how the image is processed through grayscale image, energy map and marked seams.
		/// </summary>
		/// <param name="desiredHeight">New height in percentage.</param>
		/// <param name="desiredWidth">New width in percentage.</param>
		/// <exception cref="Exception"></exception>
		public void ShrinkTo(string desiredHeight, string desiredWidth)
		{
			int targetHeight = Utils.ReadSize(desiredHeight, _img.GetLength(0));
			int targetWidth = Utils.ReadSize(desiredWidth, _img.GetLength(1));

			if (targetHeight > _img.GetLength(0) || targetHeight <= 0)
			{
				throw new Exception("0 < new height <= original height.");
			}
			if (targetWidth > _img.GetLength(1) || targetWidth <= 0)
			{
				throw new Exception("0 < new width <= original width.");
			}

			SeamCarving imgProcessor = new(_img, _imgName);

			if (targetHeight == _img.GetLength(0))
			{
				imgProcessor.CarveVertically(targetWidth);
			}
			else
			{
				imgProcessor.Carve(targetHeight, targetWidth);
			}
		}



		public void ExpandWidthTo(string desiredWidth)
		{
			if (desiredWidth == "150%")
			{
				int targetWidth = Utils.ReadSize(desiredWidth, _img.GetLength(1));

				Insertion imgProcessor = new(_img, _imgName);
				imgProcessor.Insert(targetWidth);
			}
			else if (desiredWidth == "200%")
			{
				// first expansion
				int width150percent = Utils.ReadSize("150%", _img.GetLength(1));

				Insertion firstExpansion = new(_img, _imgName);
				(byte, byte, byte)[,] expansion150img = firstExpansion.Insert(width150percent, "Not showing middle steps", false);

				// second expansion
				int targetWidth = Utils.ReadSize(desiredWidth, _img.GetLength(1));

				Insertion secondExpansion = new(expansion150img, _imgName + "_2ndExtension_");
				//secondExpansion.Insert(targetWidth);
				secondExpansion.Insert(targetWidth, "Not showing middle steps");
			}
			else
			{
				throw new Exception("Image width can only be enlarged by 150% or 200%.");
			}
		}
	}
}
