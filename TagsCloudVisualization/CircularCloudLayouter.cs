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
	class CircularCloudLayouter
	{
		private static Point Center;

		CircularCloudLayouter(Point center)
		{
			Center = center;
		}

		Rectangle PutNextRectangle(Size rectangleSize)
		{
			return new Rectangle();
		}
	}

	[TestFixture]
	class CircularCloudLayouter_Should
	{
		
	}
}
