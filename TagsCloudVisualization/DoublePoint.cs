﻿using System.Drawing;

namespace TagsCloudVisualization
{
	class DoublePoint
	{
		public double X;
		public double Y;

		public DoublePoint(double x, double y)
		{
			X = x;
			Y = y;
		}

		public Point ShiftPoint(Point point)
		{
			return new Point((int)(point.X + X), (int)(point.Y + Y));
		}
	}
}
