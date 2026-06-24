using System.Diagnostics;

namespace ContentAwareImageResizing
{
	public class Program
	{
		static void Main()
		{
			// -------- INPUT FILENAME | UNCOMMENT ONE LINE BELOW --------

			//string imgFileName = "BroadwayTower.jpg";
			//string imgFileName = "ThePersistenceOfMemory.jpg";
			//string imgFileName = "SchoolOfFish_JeremyBishop.jpg";
			//string imgFileName = "SilhouetteOfTrees_DaveHoefler.jpg";
			//string imgFileName = "SnowFoxOnSnowField_JonatanPie.jpg";
			//string imgFileName = "LakeAndTrees_AliceTriquet.jpg";
			string imgFileName = "HerdOfCows_Yang.jpg";



			// -------- END OF INPUT FILENAME --------


			ImageHandler img = new(imgFileName);

			img.PrintDimension();


			// ---------- measure time ---------- 
			Stopwatch sw = new();
			sw.Start();

			// -------- IMAGE MODIFIER | UNCOMMENT ONE LINE BELOW --------
			//img.ShrinkTo("60%", "50%");
			//img.ShrinkTo("100%", "60%"); // preserve height, change width
			img.ExpandWidthTo("150%");
			//img.ExpandWidthTo("200%");

			// -------- END OF IMAGE MODIFIER --------


			sw.Stop();
			Console.WriteLine($"\n\nTime processing: {Math.Round(sw.Elapsed.TotalSeconds, 3)} s");

			// ---------- end of measuring time ---------- 
		}
	}
}