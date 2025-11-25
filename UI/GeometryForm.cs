using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Krypton.Toolkit;

namespace JA.UI
{

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

            var page1 = new ExampleTTR(pictureBox1);
            var page2 = new EllipseCircleDistance(pictureBox2);
            var page3 = new CircleCircleDistance(pictureBox3);
            var page4 = new CircleCircleTangentLines(pictureBox4);

            SetStatusLabel();
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
                toolStripStatusLabel2.Text=$"Image ({img.Width}×{img.Height})";
            }
            else if (Clipboard.ContainsText())
            {
                var txt = new string(Clipboard.GetText().ToCharArray().Take(256).ToArray());
                toolStripStatusLabel2.Text=$"Text ({txt.Length})";
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var fn = Clipboard.GetFileDropList().OfType<string>().ToArray();
                toolStripStatusLabel2.Text=$"Files ({fn.Length})";
            }
        }
    }

}
