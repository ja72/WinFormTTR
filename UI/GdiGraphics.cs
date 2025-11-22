using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using JA.Numerics.Geometry;
using static System.Math;

namespace JA.UI
{
    public static class GdiGraphics
    {
        const double to_deg = 180 / PI;

        public static Pen Stroke { get; set; } = new Pen(Color.Black, 0);
        public static SolidBrush Fill { get; set; } = new SolidBrush(Color.Black);
        public static Font Font { get; set; } = SystemFonts.CaptionFont;
        public static float PointSize { get; set; } = 8f;

        #region Primitives
        /// <summary>
        /// Adds an arrow to the start of a <see cref="Pen"/> object.
        /// </summary>
        /// <remarks>Intended to be used with <see cref="Stroke"/></remarks>
        /// <param name="pen">The pen to modify.</param>
        /// <param name="arrowSize">Size of the arrow in pixels.</param>
        public static void AddStartArrow(this Pen pen, float arrowSize)
        {
            pen.CustomStartCap = new AdjustableArrowCap(0.4f * arrowSize, arrowSize);
        }
        public static void AddStartArrow(float arrowSize)
            => AddStartArrow(Stroke, arrowSize);
        /// <summary>
        /// Removes the start arrow from a <see cref="Pen"/> object.
        /// </summary>
        /// <remarks>Intended to be used with <see cref="Stroke"/></remarks>
        /// <param name="pen">The pen object to modify.</param>
        public static void RemoveStartArrow(this Pen pen)
        {
            pen.StartCap = LineCap.NoAnchor;
        }
        public static void RemoveStartArrow() => RemoveStartArrow(Stroke);
        /// <summary>
        /// Adds an arrow to the end of a <see cref="Pen"/> object.
        /// </summary>
        /// <remarks>Intended to be used with <see cref="Stroke"/></remarks>
        /// <param name="pen">The pen to modify.</param>
        /// <param name="arrowSize">Size of the arrow in pixels.</param>
        public static void AddEndArrow(this Pen pen, float arrowSize)
        {
            pen.CustomEndCap = new AdjustableArrowCap(0.4f * arrowSize, arrowSize);
        }
        public static void AddEndArrow(float arrowSize)
            => AddEndArrow(Stroke, arrowSize);
        /// <summary>
        /// Removes the end arrow from a <see cref="Pen"/> object.
        /// </summary>
        /// <remarks>Intended to be used with <see cref="Stroke"/></remarks>
        /// <param name="pen">The pen object to modify.</param>
        public static void RemoveEndArrow(this Pen pen)
        {
            pen.EndCap = LineCap.NoAnchor;
        }
        public static void RemoveEndArrow() => RemoveEndArrow(Stroke);

        public static void DrawLine(this Graphics g, PointF from, PointF to)
            => g.DrawLine(Stroke, from, to);

        public static void DrawCircle(this Graphics g, PointF center, float radius)
            => g.DrawEllipse(Stroke, new RectangleF(center.X-radius, center.Y-radius, 2*radius, 2*radius));
        public static void DrawEllipse(this Graphics g, PointF center, float majorRadius, float minorRadius)
            => g.DrawEllipse(Stroke, new RectangleF(center.X-majorRadius, center.Y-minorRadius, 2*majorRadius, 2*minorRadius));
        public static void FillCircle(this Graphics g, PointF center, float radius)
            => g.FillEllipse(Fill, new RectangleF(center.X-radius, center.Y-radius, 2*radius, 2*radius));
        public static void FillEllipse(this Graphics g, PointF center, float majorRadius, float minorRadius)
            => g.FillEllipse(Fill, new RectangleF(center.X-majorRadius, center.Y-minorRadius, 2*majorRadius, 2*minorRadius));

        /// <summary>
        /// Draws a small circle representing a point on the screen.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="coordinates">The point coordinates.</param>
        /// <param name="size">The point size in pixels.</param>
        public static void DrawPoint(this Graphics g, PointF coordinates, float size = -1)
        {
            size = size < 0 ? PointSize : size;
            var rect = new RectangleF(
                coordinates.X - size / 2,
                coordinates.Y - size / 2, size, size);
            g.DrawEllipse(Stroke, rect);
        }

        /// <summary>
        /// Fills a small circle representing a point on the screen.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="coordinates">The point coordinates.</param>
        /// <param name="size">The point size in pixels.</param>
        public static void FillPoint(this Graphics g, PointF coordinates, float size = -1)
        {
            size = size < 0 ? PointSize : size;
            var rect = new RectangleF(
                coordinates.X - size / 2,
                coordinates.Y - size / 2, size, size);
            g.FillEllipse(Fill, rect);
        }
        /// <summary>
        /// Draws a set of points (nodes)
        /// </summary>
        /// <remarks>There must be more than one node to draw anything.</remarks>
        /// <param name="g">The graphics object to draw on.</param>
        /// <param name="scale">The drawing scale converting (x, y) to <see cref="PointF"/>.</param>
        /// <param name="nodes">The array of locations defining the points</param>
        /// <remarks>Uses <see cref="Stroke"/> to draw the shape.</remarks>
        public static void DrawPoints(this Graphics g, PointF[] nodes, float size = -1)
        {
            size = size < 0 ? PointSize : size;
            for (var i = 0; i < nodes.Length; i++)
            {
                g.DrawPoint(nodes[i], size);
            }
        }
        /// <summary>
        /// Fills a set of points (nodes)
        /// </summary>
        /// <remarks>There must be more than one node to draw anything.</remarks>
        /// <param name="g">The graphics object to draw on.</param>
        /// <param name="scale">The drawing scale converting (x, y) to <see cref="PointF"/>.</param>
        /// <param name="nodes">The array of locations defining the points</param>
        /// <remarks>Uses <see cref="Stroke"/> to draw the shape.</remarks>
        public static void FillPoints(this Graphics g, PointF[] nodes, float size = -1)
        {
            size = size < 0 ? PointSize : size;
            for (var i = 0; i < nodes.Length; i++)
            {
                g.FillPoint(nodes[i], size);
            }
        }
        /// <summary>
        /// Draws an ellipse
        /// </summary>
        /// <param name="g">The graphics object to draw on.</param>
        /// <param name="scale">The drawing scale converting (x, y) to <see cref="PointF"/>.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="semiMajor">The semi-major axis.</param>
        /// <param name="semiMinor">The semi-minor axis.</param>
        /// <param name="tiltAngleDegrees">The tilt angle degrees CCW from horizontal.</param>
        /// <remarks>Uses <see cref="Stroke"/> to draw lines.</remarks>
        public static void DrawEllipse(this Graphics g, PointF center, float semiMajor, float semiMinor, float tiltAngleDegrees = 0)
        {
            float x = center.X, y = center.Y;
            float dx = semiMajor, dy = semiMinor;
            var save = g.Save();
            g.RotateTransform(-tiltAngleDegrees);
            g.DrawEllipse(Stroke, x - dx, y - dy, 2 * dx, 2 * dy);
            g.Restore(save);
        }
        /// <summary>
        /// Fills an ellipse
        /// </summary>
        /// <param name="g">The graphics object to draw on.</param>
        /// <param name="scale">The drawing scale converting (x, y) to <see cref="PointF"/>.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="semiMajor">The semi-major axis.</param>
        /// <param name="semiMinor">The semi-minor axis.</param>
        /// <param name="tiltAngleDegrees">The tilt angle degrees CCW from horizontal.</param>
        /// <remarks>Uses <see cref="Fill"/> to fill shapes.</remarks>
        public static void FillEllipse(this Graphics g, PointF center, float semiMajor, float semiMinor, float tiltAngleDegrees = 0)
        {
            float x = center.X, y = center.Y;
            float dx = semiMajor, dy = semiMinor;
            var save = g.Save();
            g.RotateTransform(-tiltAngleDegrees);
            g.FillEllipse(Fill, x - dx, y - dy, 2 * dx, 2 * dy);
            g.Restore(save);
        }

        /// <summary>
        /// Draws an arc of an ellipse (partial ellipse)
        /// </summary>
        /// <param name="g">The graphics object to draw on.</param>
        /// <param name="scale">The drawing scale converting (x, y) to <see cref="PointF"/>.</param>
        /// <param name="center">The center of the ellipse.</param>
        /// <param name="semiMajor">The semi-major axis.</param>
        /// <param name="semiMinor">The semi-minor axis.</param>
        /// <param name="startAngleDegrees">The start angle in degrees CCW from 3 o'clock.</param>
        /// <param name="sweepAngleDegrees">The sweep angle in degrees CCW.</param>
        /// <param name="tiltAngleDegrees">The tilt angle degrees CCW from horizontal.</param>
        /// <remarks>Uses <see cref="Stroke"/> to draw lines.</remarks>
        public static void DrawEllipseArc(this Graphics g, PointF center, float semiMajor, float semiMinor, float startAngleDegrees, float sweepAngleDegrees, float tiltAngleDegrees = 0)
        {
            float x = center.X, y = center.Y;
            float dx = semiMajor, dy = semiMinor;
            var save = g.Save();
            g.RotateTransform(-tiltAngleDegrees);
            g.DrawArc(Stroke, x - dx, y - dy, 2 * dx, 2 * dy, 360 - startAngleDegrees, -sweepAngleDegrees);
            g.Restore(save);
        }
        /// <summary>
        /// Draw a line with an arrow head.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="p1">The start point.</param>
        /// <param name="p2">The end point.</param>
        /// <param name="arrowSize">Size of the arrow head.</param>
        /// <param name="bothSides">True to draw arrows on both ends</param>
        public static void DrawLineArrow(this Graphics g, PointF p1, PointF p2, float arrowSize, bool bothSides = false)
        {
            Stroke.AddEndArrow(arrowSize);
            if (bothSides)
            {
                Stroke.AddStartArrow(arrowSize);
            }
            g.DrawLine(Stroke, p1, p2);
            Stroke.RemoveEndArrow();
            if (bothSides)
            {
                Stroke.RemoveStartArrow();
            }
        }
        /// <summary>
        /// Draw a line with an arrow head.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="pnt">The start point.</param>
        /// <param name="dx">The horizontal offset.</param>
        /// <param name="dy">The vertical offset.</param>
        /// <param name="arrowSize">Size of the arrow head.</param>
        /// <param name="bothSides">True to draw arrow heads on both sides.</param>
        public static void DrawLineArrow(this Graphics g, PointF pnt, float offset_x, float offset_y, float arrowSize, bool bothSides = false)
        {
            g.DrawLineArrow(pnt, new PointF(pnt.X + offset_x, pnt.Y + offset_y), arrowSize, bothSides);
        }

        /// <summary>
        /// Draws an arc between two points.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="pen">The line color.</param>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="radius">The arc radius.</param>
        /// <param name="flip">if set to <c>true</c> switches side of the arc.</param>
        public static void DrawArcBetweenTwoPoints(this Graphics g, PointF a, PointF b, float radius, bool flip = false)
        {
            if (flip)
            {
                (a, b) = (b, a);
            }

            // get distance components
            double x = b.X - a.X, y = b.Y - a.Y;
            // get orientation angle
            var θ = Atan2(y, x);
            // length between A and B
            var l = Sqrt(x * x + y * y);
            if (2 * radius >= l)
            {
                // find the sweep angle (actually half the sweep angle)
                var φ = Asin(l / (2 * radius));
                // triangle height from the chord to the center
                var h = radius * Cos(φ);
                // get center point. 
                // Use sin(θ)=y/l and cos(θ)=x/l
                var C = new PointF(
                    (float)(a.X + x / 2 - h * (y / l)),
                    (float)(a.Y + y / 2 + h * (x / l)));


                // Draw arc based on square around center and start/sweep angles
                g.DrawArc(Stroke, C.X - radius, C.Y - radius, 2 * radius, 2 * radius,
                    (float)((θ - φ) * to_deg) - 90, (float)(2 * φ * to_deg));
            }
        }

        /// <summary>
        /// Draws the arc from point, given a center and a sweep angle.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="pen">The arc color.</param>
        /// <param name="center">The center point.</param>
        /// <param name="point">The start point.</param>
        /// <param name="sweepAngleDegrees">The sweep angle.</param>
        public static void DrawArcFromCenter(this Graphics g, PointF center, PointF point, float sweepAngleDegrees)
        {
            // get distance components
            double x = center.X - point.X, y = center.Y - point.Y;
            // get orientation angle
            var θ = Atan2(y, x);
            // length between A and C
            var r = (float)Sqrt(x * x + y * y);

            g.DrawArc(Stroke,
                center.X - r, center.Y - r,
                2 * r, 2 * r,
                (float)(θ * to_deg), (float)(sweepAngleDegrees));
        }

        /// <summary>
        /// Draws text around a specific point with specific alignment.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="anchor">The reference point.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="font">The font.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="offset_x">(optional) Horizontal offset in pixes.</param>
        /// <param name="offset_y">(optional) Vertical offset in pixels.</param>
        public static void DrawText(this Graphics g, string text, PointF anchor, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
        {
            var x = anchor.X;
            var y = anchor.Y;
            var size = g.MeasureString(text, Font);
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    x = x - size.Width - padding;
                    y = y - size.Height - padding;
                    break;
                case ContentAlignment.TopCenter:
                    x = x - size.Width / 2;
                    y = y - size.Height - padding;
                    break;
                case ContentAlignment.TopRight:
                    x = x + padding;
                    y = y - size.Height - padding;
                    break;
                case ContentAlignment.MiddleLeft:
                    x = x - size.Width - padding;
                    y = y - size.Height / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    x = x - size.Width / 2;
                    y = y - size.Height / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    x = x + padding;
                    y = y - size.Height / 2;
                    break;
                case ContentAlignment.BottomLeft:
                    x = x - size.Width - padding;
                    y = y + padding;
                    break;
                case ContentAlignment.BottomCenter:
                    x = x - size.Width / 2;
                    y = y + padding;
                    break;
                case ContentAlignment.BottomRight:
                    x = x + padding;
                    y = y + padding;
                    break;
                default:
                    throw new NotSupportedException();
            }
            g.DrawString(text, Font, Fill, x + offset_x, y - offset_y);
        }
        public static void DrawTextBetweenTwoPoints(this Graphics g, string text, PointF start, PointF end, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
        {
            if (start == end)
            {
                DrawText(g, text, start, alignment, offset_x, offset_y, padding);
                return;
            }

            var size = g.MeasureString(text, Font);

            float x_min = g.ClipBounds.Left, x_max = x_min + g.ClipBounds.Width;
            float y_min = g.ClipBounds.Top, y_max = y_min + g.ClipBounds.Height;
            float dx = Abs(end.X - start.X), dy = Abs(end.Y - start.Y);
            float sx = Sign(end.X - start.X), sy = Sign(end.Y - start.Y);
            float xm = (end.X + start.X) / 2, ym = (end.Y + start.Y) / 2;
            var fx = Min(2*(xm - x_min) / dx, 2*(x_max - xm) / dx);
            var fy = Min(2*(ym - y_min) / dy, 2*(y_max - ym) / dy);
            var f = Min(1, Min(fx, fy));
            dx *= f;
            dy *= f;
            var length = (float)Sqrt((dx * dx) + (dy * dy));
            float ex = sx*dx / length, ey = sy*dy / length;
            float nx = -ey, ny = ex;
            float x = xm, y = ym;
            float h = size.Height/2, w = size.Width/2;
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    x += -(length / 2) * ex - h * ey;
                    y += -(length / 2) * ey + h * ex;
                    alignment = ContentAlignment.BottomRight;
                    break;
                case ContentAlignment.TopCenter:
                    x += -h * ey;
                    y += h * ex;
                    alignment = ContentAlignment.BottomCenter;
                    break;
                case ContentAlignment.TopRight:
                    x += (length / 2) * ex + h * ey;
                    y += (length / 2) * ey + h * ex;
                    alignment = ContentAlignment.BottomLeft;
                    break;
                case ContentAlignment.MiddleLeft:
                    x += -(length / 2) * ex;
                    y += -(length / 2) * ey;
                    alignment = ContentAlignment.MiddleRight;
                    break;
                case ContentAlignment.MiddleCenter:
                    break;
                case ContentAlignment.MiddleRight:
                    x += (length / 2) * ex;
                    y += (length / 2) * ey;
                    alignment = ContentAlignment.MiddleLeft;
                    break;
                case ContentAlignment.BottomLeft:
                    x += -(length / 2) * ex - h * ey;
                    y += -(length / 2) * ey - h * ex;
                    alignment = ContentAlignment.TopRight;
                    break;
                case ContentAlignment.BottomCenter:
                    x += -h * ey;
                    y += -h * ex;
                    alignment = ContentAlignment.TopCenter;
                    break;
                case ContentAlignment.BottomRight:
                    x += (length / 2) * ex + h * ey;
                    y += (length / 2) * ey - h * ex;
                    alignment = ContentAlignment.TopLeft;
                    break;
                default:
                    throw new NotSupportedException();
            }
            Debug.Assert(!float.IsNaN(x), "x is NaN");
            Debug.Assert(!float.IsNaN(y), "y is NaN");
            var point = new PointF(x, y);
            DrawText(g, text, point, alignment, offset_x, offset_y, padding);
        }

        /// <summary>
        /// Draws text around a rectangle with specific alingment.
        /// </summary>
        /// <param name="g">The Graphics object.</param>
        /// <param name="area">The reference rectangle.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="alingment">The alignment.</param>
        /// <param name="font">The font.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="offset_x">(optional) Horizontal offset in pixes.</param>
        /// <param name="offset_y">(optional) Vertical offset in pixels.</param>
        public static void DrawCaption(this Graphics g, PointF anchor, Size area, string text, ContentAlignment alingment, float offset_x = 0, float offset_y = 0)
        {
            var size = g.MeasureString(text, Font);
            var p = PointF.Empty;
            StringFormatFlags f = 0;
            switch (alingment)
            {
                case ContentAlignment.BottomCenter:
                    p = new PointF(
                        area.Width / 2f - size.Width / 2f,
                        area.Height - size.Height);
                    break;
                case ContentAlignment.BottomLeft:
                    p = new PointF(
                        0f,
                        area.Height - size.Height);
                    break;
                case ContentAlignment.BottomRight:
                    p = new PointF(
                        area.Width - size.Width,
                        area.Height - size.Height);
                    break;
                case ContentAlignment.MiddleCenter:
                    p = new PointF(
                        area.Width / 2f - size.Width / 2f,
                        area.Height / 2f - size.Height / 2f);
                    break;
                case ContentAlignment.MiddleLeft:
                    p = new PointF(
                        0f,
                        area.Height / 2f - size.Width / 2f);
                    f = StringFormatFlags.DirectionVertical;
                    break;
                case ContentAlignment.MiddleRight:
                    p = new PointF(
                        area.Width - size.Height,
                        area.Height / 2f - size.Width / 2f);
                    f = StringFormatFlags.DirectionVertical;
                    break;
                case ContentAlignment.TopCenter:
                    p = new PointF(
                        area.Width / 2f - size.Width / 2f,
                        0f);
                    break;
                case ContentAlignment.TopLeft:
                    p = new PointF(
                        0f,
                        0f);
                    break;
                case ContentAlignment.TopRight:
                    p = new PointF(
                        area.Width - size.Width,
                        0f);
                    break;
                default:
                    break;
            }
            p = new PointF(anchor.X + p.X + offset_x, anchor.Y + p.Y + offset_y);
            g.DrawString(text, Font, Fill, new RectangleF(p, area), new StringFormat(f));
        }

        /// <summary>
        /// Draws a horizontal arrow with leader text either mid length, or near the end point
        /// </summary>
        /// <param name="g">The graphics object</param>
        /// <param name="start">The starting location of the arrow.</param>
        /// <param name="length">The length of the arrow in pixels. Positive goes right, and negative goes left.</param>
        /// <param name="text">The text to display or null</param>
        /// <param name="pen">The line pen</param>
        /// <param name="fill">The text fill</param>
        /// <param name="font">The text font</param>
        /// <param name="bothSides">True to draw arrows on both ends</param>
        public static void HorizontalArrow(this Graphics g, PointF start, float length, string text, bool bothSides = false)
        {
            var sz = new SizeF(8, 8);

            if (!string.IsNullOrEmpty(text))
            {
                sz = g.MeasureString(text, Font);
            }
            float x1 = start.X, x2 = x1 + length, y = start.Y;
            g.DrawLineArrow(new PointF(x1, y), new PointF(x2, y), 0.4f * sz.Height, bothSides);

            if (!string.IsNullOrEmpty(text))
            {
                var xm = (x1 + x2) / 2;

                if (Abs(x2 - x1) >= sz.Height)
                {
                    //middle text
                    g.DrawString(
                        text,
                        Font,
                        Fill,
                        xm - sz.Width / 2, y - sz.Height - 2);
                }
                else
                {
                    var text_offset = x1 > x2 ?
                        -sz.Width - 2 :
                        2;

                    g.DrawString(
                        text,
                        Font,
                        Fill,
                        x2 + text_offset, y - sz.Height / 2);
                }

            }
        }

        /// <summary>
        /// Draws a vertical arrow with leader text either mid length, or near the end point
        /// </summary>
        /// <param name="g">The graphics object</param>
        /// <param name="start">The starting location of the arrow.</param>
        /// <param name="length">The length of the arrow in pixels. Positive goes down, and negative goes up.</param>
        /// <param name="text">The text to display or null</param>
        /// <param name="pen">The line pen</param>
        /// <param name="fill">The text fill</param>
        /// <param name="font">The text font</param>
        /// <param name="bothSides">True to draw arrows on both ends</param>
        public static void VerticalArrow(this Graphics g, PointF start, float length, string text, bool bothSides = false)
        {
            var sz = new SizeF(8, 8);


            if (!string.IsNullOrEmpty(text))
            {
                sz = g.MeasureString(text, Font);
            }

            float x = start.X, y1 = start.Y, y2 = y1 + length;

            g.DrawLineArrow(new PointF(x, y1), new PointF(x, y2), 0.4f * sz.Height, bothSides);

            if (!string.IsNullOrEmpty(text))
            {
                var ym = (y1 + y2) / 2;

                if (Abs(y2 - y1) >= sz.Width)
                {
                    //vertical text
                    var sf = new StringFormat(StringFormatFlags.DirectionVertical);
                    g.DrawString(
                        text,
                        Font,
                        Fill,
                        x, ym - sz.Width / 2, sf);
                }
                else
                {
                    var text_offset = y1 > y2 ?
                        -sz.Height - 2 :
                        2;

                    g.DrawString(
                        text,
                        Font,
                        Fill,
                        x - sz.Width / 2, y2 + text_offset);
                }
            }

        }

        #endregion

    }

}

