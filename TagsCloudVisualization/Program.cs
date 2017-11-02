using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace TagsCloudVisualization
{
	public class Program
	{
		public static void Main()
		{
			var cloud = new CircularCloudLayouter(new Point(150, 150));
			int count = 50;
			for (int i = 0; i < count; i++)
			{
				cloud.PutNextRectangle(new Size(20, 20));
				if (i < 5)
					cloud.PutNextRectangle(new Size(40, 40));
			}

			cloud.DrawCloud("cloud");
		}
	}
}
