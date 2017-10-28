using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace TagsCloudVisualization
{
	public class CircularCloudLayouter
	{
		private static Point Center;
		private static List<Rectangle> cloudOfRectangles;

		public CircularCloudLayouter(Point center)
		{
			Center = center;
			cloudOfRectangles = new List<Rectangle>();
		}

		public Rectangle PutNextRectangle(Size rectangleSize)
		{
			if (cloudOfRectangles.Count == 0)
			{
				cloudOfRectangles.Add(GetFirstRectangle(rectangleSize));
				return GetFirstRectangle(rectangleSize);
			}
			return new Rectangle();
		}

		private Rectangle GetFirstRectangle(Size rectangleSize)
		{
			return new Rectangle(Center.X - rectangleSize.Width / 2, 
				Center.Y - rectangleSize.Height / 2, 
				rectangleSize.Width, 
				rectangleSize.Height);
		}
	}

	[TestFixture]
	class CircularCloudLayouter_Should
	{
		[Test]
		public void CenterOfFirstRactangle_IsSutuated_InCenterOfCloud()
		{
			var cloud = new CircularCloudLayouter(new Point(12, 12));
			var rectangle = new Rectangle(10, 10, 4, 4);
			cloud.PutNextRectangle(new Size(4, 4)).ShouldBeEquivalentTo(rectangle);

		}
	}
}
