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
			var cloud = new CircularCloudLayouter(new Point(350, 350));
			int count = 50;
			for (int i = 0; i < count; i++)
			{
				cloud.PutNextRectangle(new Size(40, 40));
			}

			cloud.DrawCloud("cloud");
		}
	}
}
