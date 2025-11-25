using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Math;

namespace JA.UI
{
    using JA.Numerics.Geometry;

    public class Frame
    {
        public Frame(Control target, float scale = 48)
        {
            Target=target??throw new ArgumentNullException(nameof(target));
            Scale=scale;
            SetOriginOnCenter();

            target.SizeChanged += (s, ev) =>
            {
                SetOriginOnCenter();
            };
        }

        public Control Target { get; }
        public float Scale { get; set; }
        public PointF Origin { get; set; }

        public void SetOriginOnCenter(float sx = 0.5f, float sy = 0.5f)
        {
            Origin = new PointF(
                Target.ClientRectangle.Left + Target.ClientSize.Width*sx,
                Target.ClientRectangle.Top + Target.ClientSize.Height*sy);
        }

        public void FitModelSize(float size)
        {
            var pixels = Min(Target.ClientSize.Height, Target.ClientSize.Width);

            Scale = pixels/size;
        }

        public Point2 TopLeft => Point2.FromVector(GetVector(new PointF(Target.ClientRectangle.Left, Target.ClientRectangle.Top)));
        public Point2 TopRight => Point2.FromVector(GetVector(new PointF(Target.ClientRectangle.Left + Target.ClientRectangle.Width-1, Target.ClientRectangle.Top)));
        public Point2 BottomLeft => Point2.FromVector(GetVector(new PointF(Target.ClientRectangle.Left, Target.ClientRectangle.Top + Target.ClientRectangle.Height-1)));
        public Point2 BottomRight => Point2.FromVector(GetVector(new PointF(Target.ClientRectangle.Left + Target.ClientRectangle.Width-1, Target.ClientRectangle.Top + Target.ClientRectangle.Height-1)));

        #region Conversions
        public PointF GetPixel(Point2 point)
            => GetPixel(point.AsVector());

        public PointF GetPixel(Vector2 coordinate)
        {
            var origin = Origin;

            return new PointF(
                origin.X + Scale * coordinate.X,
                origin.Y - Scale * coordinate.Y);
        }

        public PointF[] GetPixels(params Point2[] points)
            => GetPixels(points.Select(pt => pt.AsVector()).ToArray());

        public PointF[] GetPixels(params Vector2[] coordinates)
        {
            var origin = Origin;

            var points = new PointF[coordinates.Length];
            for (var i = 0; i < points.Length; i++)
            {
                points[i] = new PointF(
                origin.X + Scale * coordinates[i].X,
                origin.Y - Scale * coordinates[i].Y);
            }
            return points;
        }
        public Point2 GetPoint(PointF pixel)
        {
            var origin = Origin;

            return new Point2(
                (pixel.X - origin.X)/Scale,
                -(pixel.Y - origin.Y)/Scale,
                1);
        }
        public Vector2 GetVector(PointF pixel)
        {
            var origin = Origin;

            return new Vector2(
                (pixel.X - origin.X)/Scale,
                -(pixel.Y - origin.Y)/Scale);
        }
        #endregion
    }
    public class Canvas : Frame
    {
        public Canvas(Control target, Graphics graphics, float scale = 48)
            : base(target, scale) 
        {
            Graphics = graphics;
        }

        public Graphics Graphics {get; }
        public Pen Stroke
        {
            get => GdiGraphics.Stroke;
            set => GdiGraphics.Stroke= value;
        }
        public SolidBrush Fill
        {
            get => GdiGraphics.Fill;
            set => GdiGraphics.Fill = value;
        }
        public Font Font
        {
            get => GdiGraphics.Font;
            set => GdiGraphics.Font =value;
        }

        public float PointSize
        {
            get => GdiGraphics.PointSize; 
            set => GdiGraphics.PointSize =value;
        }

        public void AddStartArrow(float arrowSize)
        {
            GdiGraphics.AddStartArrow(arrowSize);
        }
        public void RemoveStartArrow()
        {
            GdiGraphics.RemoveStartArrow();
        }
        public void AddEndArrow(float arrowSize)
        {
            GdiGraphics.AddEndArrow(arrowSize);
        }
        public void RemoveEndArrow()
        {
            GdiGraphics.RemoveEndArrow();
        }

        #region Rendering

        public void DrawLine(Line2 line)
        {
            //var top = Line2.Join(TopRight, TopLeft);
            //var left = Line2.Join(TopLeft, BottomLeft);
            //var bottom = Line2.Join(BottomLeft, BottomRight);
            //var right = Line2.Join(BottomRight, TopRight);

            //var A = Point2.Meet(line, top);
            //var B = Point2.Meet(line, left);
            //var C = Point2.Meet(line, bottom);
            //var D = Point2.Meet(line, right);

            //var pnts = new Point2[] { A, B, C, D };
            //pnts = pnts.Where((pt) => pt.IsFinite).ToArray();

            //if (pnts.Length>0)
            //{
            //    float t_min = line.ParallelDistanceTo(pnts[0]);
            //    float t_max = t_min;

            //    for (int i = 1; i < pnts.Length; i++)
            //    {
            //        var t = line.ParallelDistanceTo(pnts[i]);
            //        t_min = Min(t_min, t);
            //        t_max = Max(t_max, t);
            //    }

            //    var from = line.GetPointAlong(t_min);
            //    var to = line.GetPointAlong(t_max);

            //    Graphics.DrawLine(GetPixel(from), GetPixel(to));
            //}
            //else
            //{
            //    // Now what?
            //}

            float size = (float)Sqrt( Target.ClientSize.Width*Target.ClientSize.Width
                + Target.ClientSize.Height*Target.ClientSize.Height )/Scale;

            float d = line.ParallelDistanceTo(line.GetCenter());

            var from = line.GetPointAlong(d - size);
            var to = line.GetPointAlong(d + size);

            Graphics.DrawLine(GetPixel(from), GetPixel(to));


        }

        public void DrawLine(Point2 point1, Point2 point2) 
            => Graphics.DrawLine(GetPixel(point1), GetPixel(point2));
        public void DrawPoint(Point2 point, float size = -1)
            => Graphics.DrawPoint(GetPixel(point), size);
        public void FillPoint(Point2 point, float size = -1)
            => Graphics.FillPoint(GetPixel(point), size);
        public void DrawPoints(Point2[] nodes, float size = -1)
            => Graphics.DrawPoints(GetPixels(nodes), size);
        public void FillPoints(Point2[] nodes, float size = -1)
            => Graphics.FillPoints(GetPixels(nodes), size);
        public void DrawCircle(Circle2 circle)
            => DrawCircle(circle.Center, circle.Radius);
        public void DrawCircle(Point2 center, float radius)
            => Graphics.DrawCircle(GetPixel(center), Scale*radius);
        public void DrawEllipse(Ellipse2 ellipse, float tiltAngleDegrees = 0)
            => DrawEllipse(ellipse.Center, ellipse.MajorAxis, ellipse.MinorAxis);
        public void DrawEllipse(Point2 center, float radius1, float radius2,float tiltAngleDegrees = 0)
            => Graphics.DrawEllipse(GetPixel(center), Scale*radius1, Scale*radius2, tiltAngleDegrees);
        public void DrawCircleArc(Point2 center, float radius, float startAngle, float sweepAngleDegrees)
            => Graphics.DrawEllipseArc(GetPixel(center), Scale*radius, Scale*radius, startAngle, sweepAngleDegrees);
        public void DrawEllipseArc(Point2 center, float radius1, float radius2, float startAngle, float sweepAngleDegrees, float tiltAngleDegrees = 0)
            => Graphics.DrawEllipseArc(GetPixel(center), Scale*radius1, Scale*radius2, startAngle, sweepAngleDegrees, tiltAngleDegrees);
        public void FillCircle(Point2 center, float radius)
            => Graphics.FillCircle(GetPixel(center), Scale*radius);
        public void FillEllipse(Point2 center, float radius1, float radius2,float tiltAngleDegrees = 0)
            => Graphics.FillEllipse(GetPixel(center), Scale*radius1, Scale*radius2, tiltAngleDegrees);
        public void DrawArcBetweenTwoPoints(Point2 a, Point2 b, float radius, bool flip = false)
            => Graphics.DrawArcBetweenTwoPoints(GetPixel(a), GetPixel(b), Scale*radius, flip);
        public void DrawArcFromCenter(Point2 center, Point2 point, float sweepAngleDegrees)
            => Graphics.DrawArcFromCenter(GetPixel(center), GetPixel(point), sweepAngleDegrees);
        public void DrawLineArrow(Point2 from, Vector2 delta, float arrowSize, bool bothSides = false)
            => Graphics.DrawLineArrow(GetPixel(from), GetPixel(from + delta), arrowSize, bothSides);
        public void DrawLineArrow(Point2 from, Point2 to, float arrowSize, bool bothSides = false)
            => Graphics.DrawLineArrow(GetPixel(from), GetPixel(to), arrowSize, bothSides);
        public void DrawLineArrow(Point2 from, float offset_x, float offset_y, float arrowSize, bool bothSides = false)
            => Graphics.DrawLineArrow(GetPixel(from), offset_y, offset_y, arrowSize, bothSides);
        public void DrawText(string text, Point2 anchor, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => Graphics.DrawText(text, GetPixel(anchor), alignment, offset_x, offset_y, padding);
        public void DrawTextBetweenTwoPoints(string text, Point2 start, Point2 end, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => Graphics.DrawTextBetweenTwoPoints(text, GetPixel(start), GetPixel(end), alignment, offset_x, offset_y, padding);
        public void DrawCaption(Point2 anchor, Size area, string text, ContentAlignment alingment, float offset_x = 0, float offset_y = 0)
            => Graphics.DrawCaption(GetPixel(anchor), area, text, alingment, offset_x, offset_y);
        public void HorizontalArrow(Point2 start, float length, string text, bool bothSides = false)
            => Graphics.HorizontalArrow(GetPixel(start), Scale*length, text, bothSides);
        public void VerticalArrow(Point2 start, float length, string text, bool bothSides = false)
            => Graphics.VerticalArrow(GetPixel(start), Scale*length, text, bothSides);
        #endregion
    }
}
