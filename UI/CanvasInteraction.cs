using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace JA.UI
{
    using System.Drawing;
    using System.Numerics;

    using JA.Numerics;
    using JA.Numerics.Geometry;

    using static GdiGraphics;

    public abstract class CanvasInteraction
    {
        protected PictureBox control;
        protected Canvas canvas;
        public CanvasInteraction(PictureBox control, float scale)
        {
            this.control=control;
            control.Paint+=(s, ev) =>
            {
                ev.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
                canvas=new Canvas(control, ev.Graphics, scale);
                OnPaint();
            };
            control.MouseDown+=(s, ev) =>
            {
                if (canvas==null) return;
                var point=canvas.GetPoint(ev.Location);
                OnMouseDown(ev.Button, point);
                control.Invalidate();
            };
            control.MouseMove+=(s, ev) =>
            {
                if (canvas==null) return;
                var point=canvas.GetPoint(ev.Location);
                if (ev.Button!=MouseButtons.None)
                {
                    OnMouseDrag(ev.Button, point);
                }
                else
                {
                    OnMouseMove(point);

                }
                control.Invalidate();
            };
            control.MouseUp+=(s, ev) =>
            {
                if (canvas==null) return;
                var point=canvas.GetPoint(ev.Location);
                OnMouseUp(ev.Button, point);
                control.Invalidate();
            };
            control.FindForm().MouseWheel+=(s, ev) =>
            {
                if (canvas==null) return;
                canvas.Scale*=(float)Math.Pow(1.2, ev.Delta/120);
                control.Invalidate();
            };
        }

        protected abstract void OnPaint();
        protected virtual void OnMouseDown(MouseButtons buttons, Point2 mousePoint) { }
        protected virtual void OnMouseMove(Point2 mousePoint) { }   
        protected virtual void OnMouseDrag(MouseButtons buttons, Point2 mousePoint) { }
        protected virtual void OnMouseUp(MouseButtons buttons, Point2 mousePoint) { }

    }

    public class ExampleTTR : CanvasInteraction
    {
        Point2 apex;
        public ExampleTTR(PictureBox control) : base(control, 32f)
        {
            apex=Point2.Origin;
        }

        protected override void OnPaint()
        {
            var P1 = apex + 3*Vector2.UnitX;
            var P2 = apex + 2*Vector2.One;
            float radius = 1;

            Stroke.Color=Color.White;
            Fill.Color=Color.Wheat;
            var side1 = Line2.Join(apex, P1);
            var side2 = Line2.Join(apex, P2);

            Stroke.Color=Color.DarkGray;
            canvas.DrawLine(
                side1.GetPointAlong(-5),
                side1.GetPointAlong(5));
            canvas.DrawLine(
                side2.GetPointAlong(-5),
                side2.GetPointAlong(5));

            canvas.DrawText(
                "(0,0)", apex, ContentAlignment.BottomLeft);
            canvas.DrawText(
                "(3,0)", P1, ContentAlignment.BottomRight);
            canvas.DrawText(
                "(2,2)", P2, ContentAlignment.TopRight);
            //Canvas.FillPoint(GetPoint(apex));
            canvas.FillPoint(P1);
            canvas.FillPoint(P2);
            Stroke.Color=Color.Cyan;
            canvas.DrawLineArrow(apex, P1, 6);
            canvas.DrawLineArrow(apex, P2, 6);

            Fill.Color=Color.Yellow;

            var sides = (CornerSolution[])Enum.GetValues(typeof(CornerSolution));
            foreach (var side in sides)
            {

                var arccen = Geometry.TanTanRadius(apex, P1, P2, radius, side);

                Stroke.Color=Color.FromKnownColor(KnownColor.Magenta+(int)side+1);
                Fill.Color=Stroke.Color;

                canvas.FillPoint(arccen);
                canvas.DrawCircle(arccen, radius);
                canvas.DrawText(side.ToString(), arccen, ContentAlignment.BottomRight);
            }

        }

        protected override void OnMouseDrag(MouseButtons buttons, Point2 point)
        {
            if (buttons==MouseButtons.Left)
            {
                apex=point;
            }
            base.OnMouseDrag(buttons, point);
        }
    }
    public class EllipseCircleDistance : CanvasInteraction
    {
        Point2 center;
        Ellipse2 ellipse;
        public EllipseCircleDistance(PictureBox control) : base(control, 24f)
        {
            center=Point2.FromCoordinates(8, 4);
            ellipse=new Ellipse2(7, 1);
        }
        protected override void OnPaint()
        {
            canvas.Stroke.Color=Color.Maroon;

            canvas.DrawEllipse(ellipse);

            float radius = 1;

            var circle = new Circle2(center, radius);

            canvas.Stroke.Color=Color.Aquamarine;

            canvas.DrawCircle(circle);
            var dir = Geometry.Polar(1, 0);

            var contact = ellipse.GetClosestPoint(circle, NumericalMethods.LooseTolerance).Normalized();
            var target = circle.GetClosestPoint(contact);
            var distance = circle.DistanceTo(contact);
            canvas.Fill.Color=Color.Yellow;
            canvas.DrawText($"({contact.U:g3},{contact.V:g3})", contact, ContentAlignment.BottomRight, 4, 4);
            canvas.FillPoint(contact);
            canvas.Stroke.Color=Color.Cyan;
            canvas.DrawLineArrow(contact, target, 6);
            canvas.DrawTextBetweenTwoPoints($"{distance:g3}", contact, target, ContentAlignment.BottomCenter);
        }
        protected override void OnMouseDrag(MouseButtons buttons, Point2 point)
        {
            if (buttons==MouseButtons.Left)
            {
                center=point;
            }
            base.OnMouseDrag(buttons, point);
        }
    }
    public class CircleCircleDistance : CanvasInteraction
    {
        Point2 center1;
        Point2 center2;
        public CircleCircleDistance(PictureBox control) : base(control, 24f)
        {
            center1=Point2.FromCoordinates(4, 4);
            center2=Point2.FromCoordinates(10, 4);
        }
        protected override void OnPaint()
        {
            float radius1 = 2;
            float radius2 = 3;
            var circle1 = new Circle2(center1, radius1);
            var circle2 = new Circle2(center2, radius2);
            canvas.Stroke.Color=Color.Aquamarine;
            canvas.DrawCircle(circle1);
            canvas.DrawCircle(circle2);
            var contact1 = circle1.GetClosestPoint(circle2);
            var contact2 = circle2.GetClosestPoint(circle1);
            var distance = circle1.DistanceTo(circle2);
            canvas.Fill.Color=Color.Yellow;
            canvas.DrawText($"({contact1.U:g3},{contact1.V:g3})", contact1, ContentAlignment.BottomRight, 4, 4);
            canvas.FillPoint(contact1);
            canvas.DrawText($"({contact2.U:g3},{contact2.V:g3})", contact2, ContentAlignment.BottomLeft, 4, 4);
            canvas.FillPoint(contact2);
            canvas.Stroke.Color=Color.Cyan;
            canvas.DrawLineArrow(contact1, contact2, 6);
            canvas.DrawTextBetweenTwoPoints($"{distance:g3}", contact1, contact2, ContentAlignment.BottomCenter);
        }
        protected override void OnMouseDrag(MouseButtons buttons, Point2 point)
        {
            if (buttons==MouseButtons.Left)
            {
                center1=point;
            }
            else if (buttons==MouseButtons.Right)
            {
                center2=point;
            }
            base.OnMouseDrag(buttons, point);
        }
    }
    public class CircleCircleTangentLines : CanvasInteraction
    {
        Point2 center;
        Circle2 circle;
        Point2 point;
        public CircleCircleTangentLines(PictureBox control) : base(control, 24f)
        {
            center  = Point2.FromCoordinates(0, 0);
            float radius   = 3.5f;
            circle = new Circle2(center, radius);
            point   = Point2.FromCoordinates(10, 4);
        }
        protected override void OnPaint()
        {
            canvas.Stroke.Color=Color.Maroon;
            //var ellipse = new Ellipse2(7, 1);
            //canvas.DrawEllipse(ellipse);

            canvas.Fill.Color=Color.Cyan;
            canvas.Stroke.Color=Color.Aquamarine;
            canvas.DrawCircle(circle);
            canvas.FillPoint(center);

            canvas.Stroke.Color=Color.Yellow;
            canvas.Fill.Color=Color.DarkGoldenrod;
            canvas.FillPoint(point);

            Circle2 diagCircle = Circle2.FromDiagonals(center, point);
            canvas.Stroke.Color=Color.DarkOrange;
            canvas.Stroke.DashStyle=DashStyle.Dash;
            canvas.DrawCircle(diagCircle);
            canvas.Stroke.DashStyle=DashStyle.Solid;

            if (circle.CommonLine(diagCircle, out var line))
            {
                canvas.Stroke.Color=Color.Lime;
                canvas.DrawLine(line);
            }

            if (circle.Intersect(diagCircle, out var intersections))
            {
                canvas.Stroke.Color=Color.LimeGreen;
                foreach (var ip in intersections)
                {
                    Line2 tangent = Line2.Join(point, ip);
                    canvas.Stroke.Color=Color.Yellow;
                    canvas.DrawLine(tangent);
                    canvas.FillPoint(ip);
                }
            }
        }
        protected override void OnMouseDrag(MouseButtons buttons, Point2 point)
        {
            if (buttons==MouseButtons.Left)
            {
                this.point=point;
            }
            base.OnMouseDrag(buttons, point);
        }
    }
}
