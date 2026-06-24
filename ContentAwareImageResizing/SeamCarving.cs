using ContentAwareImageResizing.Helpers;

namespace ContentAwareImageResizing
{
	public class SeamCarving : ImageBase
	{
		private double[,] _grayImg; // matrix for handling grayscale image
		private double[,] _edgeMap; // matrix for handling edges (can be called energy map)

		private int[][] _seamTracker;

		// mark the pixel in the original image. If true, the pixel is removed from the final image.
		public bool[,] _markedSeam { get; private set; }


		public SeamCarving((byte R, byte G, byte B)[,] img, string imgName) : base(img, imgName)
		{
			_grayImg = null;
			_edgeMap = null;
			_markedSeam = null;
		}


		/// <summary>
		/// Recalculate the energy of cell of matrix[r, c].
		/// </summary>
		/// <param name="matrix">Matrix (_edgeMap)</param>
		/// <param name="r">Row index</param>
		/// <param name="c">Column index</param>
		private void RecalculateCellEnergy(double[,] matrix, int r, int c)
		{
			if (r < 0 || r >= matrix.GetLength(0) || c < 0 || c >= matrix.GetLength(1))
			{
				return; // ignore when the index is invalid
			}

			double x = MatrixMath.CellConvolve(_grayImg, Kernels._sobelX, r, c);
			double y = MatrixMath.CellConvolve(_grayImg, Kernels._sobelY, r, c);

			matrix[r, c] = Math.Sqrt(x * x + y * y);
		}


		/// <summary>
		/// Create an edge map (energy map) focusing on details on the grayscale image.
		/// </summary>
		private static double[,] DetectEdges(double[,] grayImg)
		{
			double[,] vectorFieldXDirection = MatrixMath.Convolve(grayImg, Kernels._sobelX);
			double[,] vectorFieldYDirection = MatrixMath.Convolve(grayImg, Kernels._sobelY);

			return MatrixMath.CalculateMagnitude(vectorFieldXDirection, vectorFieldYDirection);
		}


		// ---------------------------- Handling vertical seam ----------------------------

		/// <summary>
		/// Using dynamic programming to calculate energy of edge map.
		/// </summary>
		/// <returns></returns>
		private (double[], int[,]) CalEnergyRelativeToBottom()
		{
			double[] dpTopRow = new double[_currentWidth];
			double[] dpBottomRow = new double[_currentWidth];

			// used for tracing the seam from top to bottom
			int[,] path = new int[_currentHeight, _currentWidth]; // last row of path will be ignored

			// copy the last row
			for (int col = 0; col < _currentWidth; col++)
			{
				//(_currentHeight - 1) is the last row index
				dpBottomRow[col] = _edgeMap[_currentHeight - 1, col];
			}

			// calculate using dynamic programming:
			// start from the 2nd row to the last and go upward
			for (int i = _currentHeight - 2; i >= 0; i--)
			{
				for (int j = 0; j < _currentWidth; j++)
				{
					int minIdx = j;

					if (j - 1 >= 0 && dpBottomRow[minIdx] > dpBottomRow[j - 1])
					{
						minIdx = j - 1;
					}
					if (j + 1 < _currentWidth && dpBottomRow[minIdx] > dpBottomRow[j + 1])
					{
						minIdx = j + 1;
					}
					dpTopRow[j] = _edgeMap[i, j] + dpBottomRow[minIdx];
					path[i, j] = minIdx;
				}
				dpBottomRow = (double[])dpTopRow.Clone();
			}
			return (dpTopRow, path);
		}


		/// <summary>
		/// Find coordinates (column index) of pixels in a seam. 
		/// </summary>
		/// <param name="firstRow">The first row computed by dynamic programming</param>
		/// <param name="path">Path the lead to the first row</param>
		/// <returns>
		/// An array with column indices of low-energy seam. 
		/// ith element of the array corresponds to [i, array[i]]
		/// </returns>
		private int[] FindVerticalLowestEnergySeam(double[] firstRow, int[,] path)
		{
			// find seam with smallest energy
			// step 1: find smallest energy relative to the last row in the 1st row
			int[] seamIdx = new int[_currentHeight]; // column index of the seam pixel
			seamIdx[0] = 0;

			for (int i = 1; i < _currentWidth; i++)
			{
				if (firstRow[seamIdx[0]] > firstRow[i])
				{
					seamIdx[0] = i;
				}
			}

			// step 2: find the path that lead to the last row
			// the current row will point to the index of the next row
			for (int i = 1; i < _currentHeight; i++)
			{
				seamIdx[i] = path[i - 1, seamIdx[i - 1]];
			}

			return seamIdx;
		}


		/// <summary>
		/// Remove one seam at a time by shifting left the pixels to the right of the seam. 
		/// This operation is performed on grayscale image and color image simultaneously. 
		/// </summary>
		/// <returns></returns>
		private void RemoveVerticalSeam(int[] seamIdx)
		{
			for (int r = 0; r < _currentHeight; r++)
			{
				for (int c = seamIdx[r]; c < _currentWidth - 1; c++)
				{
					_edgeMap[r, c] = _edgeMap[r, c + 1];
					_grayImg[r, c] = _grayImg[r, c + 1];
					_img[r, c] = _img[r, c + 1];
				}
			}
			_currentWidth--;
		}


		/// <summary>
		/// Removing a seam may change the energy of it's neighbors. 
		/// Hence, we need to recompute these neighbors (to the left and right).
		/// </summary>
		/// <param name="seamIdx"></param>
		private void RecomputeEnergyVertically(int[] seamIdx)
		{
			for (int r = 0; r < _currentHeight; r++)
			{
				RecalculateCellEnergy(_edgeMap, r, seamIdx[r] - 1);
				RecalculateCellEnergy(_edgeMap, r, seamIdx[r]);
			}
		}


		// ---------------------------- End of Handling vertical seam ----------------------------

		// ---------------------------- Handling horizontal seam ----------------------------
		/// <summary>
		/// Using dynamic programming to calculate energy of _edgeMap
		/// </summary>
		/// <returns></returns>
		private (double[], int[,]) CalEnergyRelativeToRight()
		{
			double[] dpLeftCol = new double[_currentHeight];
			double[] dpRightCol = new double[_currentHeight];

			// used for tracing the seam from top to bottom
			int[,] path = new int[_currentHeight, _currentWidth]; // last row of path will be ignored

			// copy the last column
			for (int row = 0; row < _currentHeight; row++)
			{
				//(_currentWidth - 1) is the rightmost column index
				dpRightCol[row] = _edgeMap[row, _currentWidth - 1];
			}

			// calculate using dynamic programming:
			// start from the 2nd col to the last and go left
			for (int i = _currentWidth - 2; i >= 0; i--)
			{
				for (int j = 0; j < _currentHeight; j++)
				{
					int minIdx = j;

					if (j - 1 >= 0 && dpRightCol[minIdx] > dpRightCol[j - 1])
					{
						minIdx = j - 1;
					}
					if (j + 1 < _currentHeight && dpRightCol[minIdx] > dpRightCol[j + 1])
					{
						minIdx = j + 1;
					}
					dpLeftCol[j] = _edgeMap[j, i] + dpRightCol[minIdx];
					path[j, i] = minIdx;
				}
				dpRightCol = (double[])dpLeftCol.Clone();
			}
			return (dpLeftCol, path);
		}


		/// <summary>
		/// Find coordinates (row index) of pixels in a seam. 
		/// </summary>
		/// <param name="firstRow">The first column computed by dynamic programming</param>
		/// <param name="path">Path the lead to the first column</param>
		/// <returns>
		/// An array with row indices of low-energy seam. 
		/// ith element of the array corresponds to [array[i], i]
		/// </returns>
		private int[] FindHorizontalLowestEnergySeam(double[] firstCol, int[,] path)
		{
			// find seam with smallest energy
			// step 1: find smallest energy relative to the rightmost col in the 1st col
			int[] seamIdx = new int[_currentWidth]; // row index of the seam pixel
			seamIdx[0] = 0;

			for (int i = 1; i < _currentHeight; i++)
			{
				if (firstCol[seamIdx[0]] > firstCol[i])
				{
					seamIdx[0] = i;
				}
			}

			// step 2: find the path that lead to the last row
			// the current row will point to the index of the next row
			for (int i = 1; i < _currentWidth; i++)
			{
				seamIdx[i] = path[seamIdx[i - 1], i - 1];
			}

			return seamIdx;
		}


		/// <summary>
		/// Remove one seam at a time. This operation is performed on edge map and
		/// color image simultaneously. Shift upward the pixels below the seam.
		/// </summary>
		/// <returns></returns>
		private void RemoveHorizontalSeam(int[] seamIdx)
		{
			for (int col = 0; col < _currentWidth; col++)
			{
				for (int row = seamIdx[col]; row < _currentHeight - 1; row++)
				{
					_edgeMap[row, col] = _edgeMap[row + 1, col];
					_grayImg[row, col] = _grayImg[row + 1, col];
					_img[row, col] = _img[row + 1, col];
				}
			}
			_currentHeight--;
		}


		/// <summary>
		/// Removing a seam may change the energy of it's neighbors. 
		/// Hence, we need to recompute these neighbors (to the top & bottom).
		/// </summary>
		/// <param name="seamIdx"></param>
		private void RecomputeEnergyHorizontally(int[] seamIdx)
		{
			for (int c = 0; c < _currentHeight; c++)
			{
				RecalculateCellEnergy(_edgeMap, seamIdx[c] - 1, c);
				RecalculateCellEnergy(_edgeMap, seamIdx[c], c);
			}
		}

		// ---------------------------- End of Handling horizontal seam ----------------------------


		// ---------------------------- Carve one seam ----------------------------
		/// <summary>
		/// Carve one seam in vertical direction.
		/// </summary>
		private void CarveVerticalSeam()
		{
			// calculate the energy of the top row relative to the bottom row
			// and memorize the path 
			(double[] firstRowRelativeToBottom, int[,] path) = CalEnergyRelativeToBottom();

			// find the lowest energy seam
			int[] seamIdx = FindVerticalLowestEnergySeam(firstRowRelativeToBottom, path);

			// remove the seam and shrink current height by 1
			RemoveVerticalSeam(seamIdx);

			// recompute energy of the left and right pixels of the seam
			RecomputeEnergyVertically(seamIdx);
		}


		/// <summary>
		/// Carve one seam in horizontal direction.
		/// </summary>
		private void CarveHorizontalSeam()
		{
			// calculate the energy of the leftmost row relative to the rightmost row
			// memorize the path 
			(double[] firstColRelativeToRightSide, int[,] path) = CalEnergyRelativeToRight();

			// find the lowest energy seam
			int[] seamIdx = FindHorizontalLowestEnergySeam(firstColRelativeToRightSide, path);

			// remove the seam and shrink current height by 1
			RemoveHorizontalSeam(seamIdx);

			// recompute energy of the top and bottom pixels of the seam
			RecomputeEnergyHorizontally(seamIdx);
		}


		// ---------------------------- Main ----------------------------

		/// <summary>
		/// Quick carve in both directions. To improve time efficiency, only the final result is saved.
		/// </summary>
		/// <param name="targetHeight">New height in pixel</param>
		/// <param name="targetWidth">New width in pixel</param>
		public void Carve(int targetHeight, int targetWidth)
		{
			_grayImg = ImageIO.CreateGrayImage(_img);

			_edgeMap = DetectEdges(_grayImg);

			while (_currentWidth > targetWidth || _currentHeight > targetHeight)
			{
				if (_currentWidth > targetWidth)
				{
					CarveVerticalSeam();
				}
				if (_currentHeight > targetHeight)
				{
					CarveHorizontalSeam();
				}
			}
			SaveAs(_img, "Carved_" + _imgName, _currentHeight, _currentWidth);
		}


		// ------------------------------------------------------------


		public (byte, byte, byte)[,] RecreateSeams()
		{
			// seamImg will be the original image where seams replace original pixel
			(byte, byte, byte)[,] seamImg = new (byte, byte, byte)[_originalHeight, _originalWidth];

			// 2d array will be filled with false
			_markedSeam = new bool[_originalHeight, _originalWidth];

			// deep copy from original colored image
			for (int r = 0; r < _currentHeight; r++)
			{
				for (int c = 0; c < _currentWidth; c++)
				{
					seamImg[r, c] = _img[r, c];
				}
			}
			// after copying, the left portion of seamImg will be empty
			// because no pixel are copied.

			// processing each seam
			for (int seamIdx = _seamTracker.Length - 1; seamIdx >= 0; seamIdx--)
			{
				// seams[seamIdx].Length is _currentHeight
				for (int r = 0; r < _currentHeight; r++)
				{
					for (int c = _currentWidth; c > _seamTracker[seamIdx][r]; c--)
					{
						seamImg[r, c] = seamImg[r, c - 1]; // [r, c - 1] is the left neighbor
						_markedSeam[r, c] = _markedSeam[r, c - 1];
					}
					seamImg[r, _seamTracker[seamIdx][r]] = (255, 0, 0); // Red pixel
					_markedSeam[r, _seamTracker[seamIdx][r]] = true;
				}
				_currentWidth++;
			}

			if (seamImg.GetLength(0) != _originalHeight || seamImg.GetLength(1) != _originalWidth)
			{
				throw new Exception("Failed to restore original dimension (width x height). Expected (width x height): " + _originalWidth + " x " + _originalHeight + ". Actual: " + seamImg.GetLength(1) + " x " + seamImg.GetLength(0));
			}

			return seamImg;
		}



		public void CarveVertically(int targetWidth, string showSteps = "Showing middle steps", bool saveCarvedImg = true)
		{
			// Initiate seam tracker
			int numberOfSeams = _originalWidth - targetWidth;
			_seamTracker = new int[numberOfSeams][];

			// Pre-processing
			_grayImg = ImageIO.CreateGrayImage(_img);

			_edgeMap = DetectEdges(_grayImg);

			if (showSteps == "Showing middle steps")
			{
				SaveAs(_grayImg, "Grayscale_" + _imgName);
				SaveAs(_edgeMap, "Edgemap_" + _imgName);
			}



			for (int i = 0; i < numberOfSeams; i++)
			{
				// calculate the energy of the top row relative to the bottom row
				// and memorize the path 
				(double[] firstRowRelativeToBottom, int[,] path) = CalEnergyRelativeToBottom();

				// find the lowest energy seam
				int[] seamIdx = FindVerticalLowestEnergySeam(firstRowRelativeToBottom, path);

				// remove the seam and shrink current height by 1
				RemoveVerticalSeam(seamIdx);

				// recompute energy of the left and right pixels of the seam
				RecomputeEnergyVertically(seamIdx);

				// add to tracker
				_seamTracker[i] = seamIdx;
			}

			if (saveCarvedImg)
			{
				SaveAs(_img, "Carved_" + _imgName, _currentHeight, _currentWidth); // Result
			}

			(byte, byte, byte)[,] seamImg = RecreateSeams();

			if (showSteps == "Showing middle steps")
			{
				SaveAs(seamImg, "MarkedSeams_" + _imgName);
			}

		}
	}
}