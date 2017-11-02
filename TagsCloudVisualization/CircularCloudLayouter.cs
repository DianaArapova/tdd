﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using NUnit.Framework.Interfaces;

namespace TagsCloudVisualization
{
	public class CircularCloudLayouter
	{
		private readonly Point center;
		private readonly List<Rectangle> cloudOfRectangles;

		public IReadOnlyList<Rectangle> CloudOfRectangles => cloudOfRectangles;

		private int radius;
		
		public CircularCloudLayouter(Point center)
		{
			if (center.X < 0 || center.Y < 0)
				throw new ArgumentException("Coordinat of center is negative");
			this.center = center;
			cloudOfRectangles = new List<Rectangle>();
		}

		public Rectangle PutNextRectangle(Size rectangleSize)
		{
			if (rectangleSize.Height < 0 || rectangleSize.Width < 0)
				throw new ArgumentException("Size of rectangle is negative");

			var rectangle = FindPositionForRectangle(rectangleSize);
			cloudOfRectangles.Add(rectangle);
			return rectangle;
		}

		private static Rectangle GetRectangle(Size rectangleSize, Point centerOfRectangle)
		{
			return new Rectangle(centerOfRectangle.X - rectangleSize.Width / 2,
				centerOfRectangle.Y - rectangleSize.Height / 2,
				rectangleSize.Width,
				rectangleSize.Height);
		}

		private bool ValidateRectangle(Rectangle rectangle)
		{
			if (rectangle.X < 0 || rectangle.Y < 0)
				return false;
			return cloudOfRectangles.All(r => !r.IntersectsWith(rectangle));
		}

		private Rectangle FindPositionForRectangle(Size rectangleSize)
		{
			var isAnswerFind = false;
			var rectangle = new Rectangle();
			while (!isAnswerFind)
			{
				for (double i = 0; i < 2 * Math.PI; i += Math.PI / 30)
				{
					var locationOfRectangle = 
						new DoublePoint(radius * Math.Cos(i), radius * Math.Sin(i))
						.ShiftPoint(center);

					rectangle = GetRectangle(rectangleSize, locationOfRectangle);
					if (ValidateRectangle(rectangle))
					{
						isAnswerFind = true;
						break;
					}
				}
				radius += 1;
			}
			radius -= (int) Math.Sqrt(rectangle.Height * rectangle.Height +
			                     rectangle.Width * rectangle.Width);
			radius = Math.Max(0, radius);
			return rectangle;
		}


		public void DrawCloud(string nameOfFile)
		{
			var width = 0;
			var height = 0;
			foreach (var rectangle in cloudOfRectangles)
			{
				width = Math.Max(width, rectangle.Location.X + rectangle.Width);
				height = Math.Max(height, rectangle.Location.Y + rectangle.Height);
			}

			var bitmap = new Bitmap(width + 100, height + 100);
			var graphics = Graphics.FromImage(bitmap);
			var centerRect = new Rectangle(center, new Size(1, 1));
			graphics.DrawRectangle(new Pen(Color.Brown), centerRect);
			foreach (var rectangle in cloudOfRectangles)
				graphics.DrawRectangle(new Pen(Color.Brown), rectangle);
			graphics.Dispose();
			bitmap.Save(nameOfFile + ".bmp");
		}
	}

	[TestFixture]
	class CircularCloudLayouter_Should
	{
		private CircularCloudLayouter cloud = new CircularCloudLayouter(new Point(30, 30));

		[TearDown]
		public void TearDown()
		{
			if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Failed))
			{
				cloud.DrawCloud(TestContext.CurrentContext.TestDirectory +
					TestContext.CurrentContext.Test.Name);
				
				Console.WriteLine("Tag cloud visualization saved to file " + 
					TestContext.CurrentContext.TestDirectory + 
					TestContext.CurrentContext.Test.Name + ".bmp");
			}
		}

		[TestCase(10, 10)]
		[TestCase(11, 11)]
		[TestCase(10, 8)]
		public void PutNextRectangle_CenterOfFirstRectangle_IsSutuated_InCenterOfCloud
			(int width, int height)
		{
			var center = new Point(120, 120);
			cloud = new CircularCloudLayouter(center);
			var x = center.X - width / 2;
			var y = center.Y - height / 2;
			var rectangle = new Rectangle(x, y, width, height);
			cloud.PutNextRectangle(new Size(width, height)).ShouldBeEquivalentTo(rectangle);
		}

		[Test]
		public void PutNextRectangle_ReturnNotFirstRectangleWithRigthSize()
		{
			cloud = new CircularCloudLayouter(new Point(12, 12));
			var sizeOfRectangle = new Size(4, 4);
			cloud.PutNextRectangle(sizeOfRectangle);
			cloud.PutNextRectangle(sizeOfRectangle).Size.
				ShouldBeEquivalentTo(sizeOfRectangle);
		}

		[TestCase(2, 4, 4)]
		[TestCase(50, 40, 40)]
		[TestCase(100, 4, 4)]
		[TestCase(100, 4, 2)]
		[TestCase(500, 2, 2)]
		public void PutNextRectangle_ReturnRectangles_DoNotHaveIntersection(int count, int width, int height)
		{
			cloud = new CircularCloudLayouter(new Point(150, 150));
			var listOfRectangles = new List<Rectangle>();
			for (int i = 0; i < count; i++)
			{
				listOfRectangles.Add(cloud.PutNextRectangle(new Size(width, height)));	
			}
			for (int i = 0; i < count; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					listOfRectangles[i].IntersectsWith(listOfRectangles[j]).
						Should().BeFalse();
				}
			}
		}

		[Test]
		public void PutNextRectangle_CenterWithNegativeCoordinates_ThrowException()
		{
			Assert.Throws<ArgumentException>(() => 
			new CircularCloudLayouter(new Point(-1, 7)));
		}

		[Test]
		public void PutNextRectangle_GetRectangleWithNegativeSize_ThrowException()
		{
			cloud = new CircularCloudLayouter(new Point(6, 7));
			Assert.Throws<ArgumentException>(() => cloud.PutNextRectangle(new Size(4, -4)));
		}

		private double DistanceBetweenTwoPoint(Point point1, Point point2)
		{
			var point = new Point(point1.X - point2.X, point1.Y - point2.Y);
			return Math.Sqrt(point.X * point.X + point.Y * point.Y);
		}

		[TestCase(1, 2, 2)]
		[TestCase(10, 10, 10)]
		[TestCase(100, 6, 4)]
		[TestCase(100, 10, 10)]
		[TestCase(500, 1, 1)]
		[TestCase(5000, 1, 1)]
		public void PutRectangles_RadiusOfCloud_IsLessThenDoubleRadiusOfCircle_WithAreaSumOfAreasOfRectangles
			(int count, int width, int height)
		{
			var center = new Point(5000, 5000);
			cloud = new CircularCloudLayouter(center);
			double area = 0;
			double radiusOfCloud = 0;
			for (int i = 0; i < count; i++)
			{
				var rectangle = cloud.PutNextRectangle(new Size(height, width));
				area += rectangle.Height * rectangle.Width;
				radiusOfCloud = Math.Max(radiusOfCloud,
					DistanceBetweenTwoPoint(center, rectangle.Location));
			}
			var radiusOfCircle = Math.Sqrt(area / Math.PI);
			radiusOfCloud.Should().BeLessThan(4 * radiusOfCircle);
		}
	}
}
