using ContentAwareImageResizing.Helpers;

namespace ContentAwareImageResizing
{
	public class Insertion : ImageBase
	{
		private int _targetWidth;
		public Insertion((byte R, byte G, byte B)[,] img, string imgName) : base(img, imgName) { }


		/// <summary>
		/// Insert low energy seams to extend the width of image. By using original image 
		/// and marked seams (both have the same dimension), any pixel that is marked as red 
		/// in seamImg will tell algorithm to copy twice pixel in _coloredImg.
		/// </summary>
		/// <param name="markedSeam"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private (byte, byte, byte)[,] AddLowEnergySeams(bool[,] markedSeam)
		{
			if (markedSeam.GetLength(1) != _img.GetLength(1))
			{
				throw new Exception("Marked seams and original image do not match their widths.");
			}

			(byte R, byte G, byte B)[,] newImg =
				new (byte R, byte G, byte B)[_originalHeight, _targetWidth];


			for (int r = 0; r < newImg.GetLength(0); r++)
			{
				int i = 0; // pointer for newImg
				int j = 0; // pointer for marked seams and original image

				while (i < _targetWidth)
				{
					newImg[r, i] = _img[r, j];

					// if the pixel is marked as red
					if (markedSeam[r, j])
					{
						i++;
						newImg[r, i] = _img[r, j]; // duplicate pixels
					}
					i++;
					j++;
				}
				if (i != _targetWidth && j != _img.GetLength(1))
				{
					throw new Exception("Two pointers failed to go to the end of their arrays.");
				}
			}
			return newImg;
		}


		/// <summary>
		/// Increase width of image by using seam insertion.
		/// </summary>
		/// <param name="targetWidth">Target width is wider than original width.</param>
		public (byte, byte, byte)[,] Insert(int targetWidth, string showSteps = "Showing middle steps", bool saveExpandedImg = true)
		{
			_targetWidth = targetWidth;

			SeamCarving sc = new(Utils.DeepCopy(_img), _imgName);

			if (showSteps == "Showing middle steps")
			{
				sc.CarveVertically(2 * _originalWidth - targetWidth);
			}
			else if (showSteps == "Not showing middle steps")
			{
				sc.CarveVertically(2 * _originalWidth - targetWidth, "Not showing middle steps", false);
			}

			bool[,] markedSeam = sc._markedSeam;

			(byte, byte, byte)[,] extendedImg = AddLowEnergySeams(markedSeam);

			if (saveExpandedImg)
			{
				SaveAs(extendedImg, "Expanded_" + _imgName, extendedImg.GetLength(0), extendedImg.GetLength(1));
			}


			return extendedImg;
		}
	}
}
