using System;
using System.Numerics;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

using static System.Math;

namespace JA.UI
{
    using JA.Numerics;
    using JA.Numerics.Geometry;
    using Krypton.Toolkit;
    using static GdiGraphics;

    public partial class GeometryForm : KryptonForm
    {
        public GeometryForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetStyle(ControlStyles.ResizeRedraw, true);

            SetupDrawPictureBox1(pictureBox1);
            SetupDrawPictureBox2(pictureBox2);
            SetupDrawPictureBox3(pictureBox3);
            SetupDrawPictureBox4(pictureBox4);
            SetStatusLabel();
        }

        void SetupDrawPictureBox1(PictureBox control)
        {
            const float scale = 32;

            control.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var canvas = new Canvas(control, ev, scale);
                var apex = Point2.Origin;
                var P1 = apex + 3*Vector2.UnitX;
                var P2 = apex + 2*Vector2.One;
                float radius = 1;

                Stroke.Color = Color.White;
                Fill.Color = Color.Wheat;
                var side1 = Line2.Join(apex, P1);
                var side2 = Line2.Join(apex, P2);

                Stroke.Color = Color.DarkGray;
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
                Stroke.Color = Color.Cyan;
                canvas.DrawLineArrow(apex, P1, 6);
                canvas.DrawLineArrow(apex, P2, 6);

                Fill.Color = Color.Yellow;

                var sides = (CornerSolution[])Enum.GetValues(typeof(CornerSolution));
                foreach (var side in sides)
                {

                    var arccen = Geometry.TanTanRadius(apex, P1, P2, radius, side);

                    Stroke.Color = Color.FromKnownColor(KnownColor.Magenta + (int)side + 1);
                    Fill.Color = Stroke.Color;

                    canvas.FillPoint(arccen);
                    canvas.DrawCircle(arccen, radius);
                    canvas.DrawText(side.ToString(), arccen, ContentAlignment.BottomRight);
                }
            };
        }

        void SetupDrawPictureBox2(PictureBox control)
        {
            Point2 center = Point2.FromCoordinates(8, 4);

            float scale = 24;
            control.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var canvas = new Canvas(control, ev, scale);

                var ellipse = new Ellipse2(7, 1);

                canvas.Stroke.Color = Color.Maroon;

                canvas.DrawEllipse(ellipse);

                float radius = 1;

                var circle = new Circle2(center, radius);

                canvas.Stroke.Color = Color.Aquamarine;

                canvas.DrawCircle(circle);
                var dir = Geometry.Polar(1, 0);

                var contact = ellipse.GetClosestPoint(circle, NumericalMethods.LooseTolerance).Normalized();
                var target = circle.GetClosestPoint(contact);
                var distance = circle.DistanceTo(contact);
                canvas.Fill.Color = Color.Yellow;
                canvas.DrawText($"({contact.U:g3},{contact.V:g3})", contact, ContentAlignment.BottomRight, 4, 4);
                canvas.FillPoint(contact);
                canvas.Stroke.Color = Color.Cyan;
                canvas.DrawLineArrow(contact, target, 6);
                canvas.DrawTextBetweenTwoPoints($"{distance:g3}", contact, target, ContentAlignment.BottomCenter);
            };

            control.MouseMove += (s, ev) =>
            {
                var canvas = new Frame(control, scale);

                if (ev.Button == MouseButtons.Left)
                {
                    center = canvas.GetPoint(ev.Location);
                    control.Invalidate();
                }
            };

            control.FindForm().MouseWheel += (s, ev) =>
            {
                scale *= (float)Math.Pow(1.2, ev.Delta/120);
                control.Invalidate();
            };
        }

        void SetupDrawPictureBox3(PictureBox control)
        {
            Point2 center = Point2.FromCoordinates(8, 4);

            float scale = 32;
            control.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var canvas = new Canvas(control, ev, scale);

                var ellipse = new Ellipse2(7, 1);

                canvas.Stroke.Color = Color.Maroon;

                canvas.DrawEllipse(ellipse);

                float radius = 1;

                var circle = new Circle2(center, radius);

                canvas.Stroke.Color = Color.Aquamarine;

                // canvas.DrawCircle(circle);
                var dir = Geometry.Polar(1, 0);

                var line = Line2.ThroughPointAwayFromOrigin(center);

                canvas.DrawLine(line);

                //var contact = ellipse.GetClosestPoint(line, NumericalMethods.LooseTolerance).Normalized();
                var contact = ellipse.GetClosestPoint(line, SolutionSet.First).Normalized();
                var target = line.GetClosestPoint(contact);
                var distance = line.DistanceTo(contact);
                canvas.Stroke.Color = Color.DarkGray;
                canvas.DrawLine(Point2.Origin, line.GetCenter());

                canvas.Fill.Color = Color.Yellow;
                canvas.DrawText($"({contact.U:g3},{contact.V:g3})", contact, ContentAlignment.BottomRight, 4, 4);
                canvas.FillPoint(contact);
                canvas.Stroke.Color = Color.Cyan;
                canvas.DrawLineArrow(contact, target, 6);
                canvas.DrawTextBetweenTwoPoints($"{distance:g3}", contact, target, ContentAlignment.BottomCenter);
            };

            control.MouseMove += (s, ev) =>
            {
                var canvas = new Frame(control, scale);

                if (ev.Button == MouseButtons.Left)
                {
                    center = canvas.GetPoint(ev.Location);
                    control.Invalidate();
                }
            };

            control.FindForm().MouseWheel += (s, ev) =>
            {
                scale *= (float)Math.Pow(1.2, ev.Delta/120);
                control.Invalidate();
            };
        }

        void SetupDrawPictureBox4(PictureBox control)
        {
            Point2 center = Point2.FromCoordinates(0, 0);
            float radius = 3.5f;
            Circle2 circle = new Circle2(center, radius);
            Point2 point = Point2.FromCoordinates(10, 4);

            float scale = 24;
            control.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var canvas = new Canvas(control, ev, scale);

                canvas.Stroke.Color = Color.Maroon;
                //var ellipse = new Ellipse2(7, 1);
                //canvas.DrawEllipse(ellipse);

                canvas.Fill.Color = Color.Cyan;
                canvas.Stroke.Color = Color.Aquamarine;
                canvas.DrawCircle(circle);
                canvas.FillPoint(center);

                canvas.Stroke.Color = Color.Yellow;
                canvas.Fill.Color = Color.DarkGoldenrod;
                canvas.FillPoint(point);

                Circle2 diagCircle = Circle2.FromDiagonals(center, point);
                canvas.Stroke.Color = Color.DarkOrange;
                canvas.Stroke.DashStyle = DashStyle.Dash;
                canvas.DrawCircle(diagCircle);
                canvas.Stroke.DashStyle = DashStyle.Solid;

                if( circle.CommonLine(diagCircle, out var line))
                {
                    canvas.Stroke.Color = Color.Lime;
                    canvas.DrawLine(line);
                }

                if ( circle.Intersect(diagCircle,  out var intersections))
                {
                    canvas.Stroke.Color = Color.LimeGreen;
                    foreach (var ip in intersections)
                    {
                        Line2 tangent = Line2.Join(point, ip);
                        canvas.Stroke.Color = Color.Yellow;
                        canvas.DrawLine(tangent);
                        canvas.FillPoint(ip);
                    }
                }
            };

            control.MouseMove += (s, ev) =>
            {
                var canvas = new Frame(control, scale);

                if (ev.Button == MouseButtons.Left)
                {
                    point = canvas.GetPoint(ev.Location);
                    control.Invalidate();
                }
            };

            control.FindForm().MouseWheel += (s, ev) =>
            {
                scale *= (float)Math.Pow(1.2, ev.Delta/120);
                control.Invalidate();
            };
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = kryptonNavigator1.SelectedPage;
            if (page!=null)
            {
                Bitmap bmp = new Bitmap(page.ClientSize.Width, page.ClientSize.Height);
                page.DrawToBitmap(bmp, page.ClientRectangle);
                Clipboard.SetImage(bmp);

                SetStatusLabel();
            }
        }

        private void SetStatusLabel()
        {
            if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();
                toolStripStatusLabel2.Text = $"Image ({img.Width}×{img.Height})";
            }
            else if (Clipboard.ContainsText())
            {
                var txt = new string(Clipboard.GetText().ToCharArray().Take(256).ToArray());
                toolStripStatusLabel2.Text = $"Text ({txt.Length})";
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var fn = Clipboard.GetFileDropList().OfType<string>().ToArray();
                toolStripStatusLabel2.Text = $"Files ({fn.Length})";
            }
        }
    }
}



