using System.Drawing;

namespace TagsCloudVisualization
{
	public class Program
	{
		public static void Main()
		{
			var cloud = new CircularCloudLayouter(new Point(350, 350));
			const int count = 90;
			for (var i = 0; i < count; i++)
			{
				cloud.PutNextRectangle(new Size(70, 20));
				if (i < 5)
					cloud.PutNextRectangle(new Size(30, 30));
			}

			cloud.DrawCloud("cloud");
		}
	}
}
