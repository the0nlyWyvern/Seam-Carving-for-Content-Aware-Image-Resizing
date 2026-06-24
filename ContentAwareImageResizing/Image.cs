using ContentAwareImageResizing.Helpers;

namespace ContentAwareImageResizing
{
	public class ImageBase
	{
		protected readonly string _imgName;

		protected (byte R, byte G, byte B)[,] _img; // matrix for handling colored img

		protected readonly int _originalHeight;
		protected readonly int _originalWidth;

		protected int _currentHeight;
		protected int _currentWidth;


		public ImageBase((byte R, byte G, byte B)[,] img, string imgName)
		{
			_img = img;
			_imgName = imgName;

			// Read the dimensions
			_originalHeight = _img.GetLength(0);
			_originalWidth = _img.GetLength(1);

			_currentHeight = _originalHeight;
			_currentWidth = _originalWidth;
		}


		/// <summary>
		/// Save image.
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="fileName"></param>
		/// <param name="height"></param>
		/// <param name="width"></param>
		protected void SaveAs((byte, byte, byte)[,] matrix, string fileName = "", int height = -1, int width = -1)
		{
			if (fileName == "")
			{
				fileName = _imgName;
			}

			if (height == -1)
			{
				height = _currentHeight;
			}

			if (width == -1)
			{
				width = _currentWidth;
			}

			string path = Utils.GeneratePath("output", fileName + ".jpg");
			ImageIO.SaveAs(matrix, height, width, path);
		}


		/// <summary>
		/// Save image.
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="fileName"></param>
		/// <param name="height"></param>
		/// <param name="width"></param>
		protected void SaveAs(double[,] matrix, string fileName = "", int height = -1, int width = -1)
		{
			if (fileName == "")
			{
				fileName = _imgName;
			}

			if (height == -1)
			{
				height = _currentHeight;
			}

			if (width == -1)
			{
				width = _currentWidth;
			}

			string path = Utils.GeneratePath("output", fileName + ".jpg");
			ImageIO.SaveAs(matrix, height, width, path);
		}
	}
}
