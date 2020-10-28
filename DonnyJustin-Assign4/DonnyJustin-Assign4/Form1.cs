using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonnyJustin_Assign4
{
    public partial class Form1 : Form
    {
        Graphics g;
        int x = -1;
        int y = -1;
        bool moving = false;
        Pen pen;
        PointF point1;
        PointF point2;
        float penSize;


        public Form1()
        {
            InitializeComponent();
            g = Canvas.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            penSize = getPenSize();
            pen = new Pen(Color.Black, penSize);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        private void Canvas_Click(object sender, EventArgs e)
        {
            //PictureBox p = (PictureBox)sender;
            //pen.Color = p.BackColor;
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            x = e.X;
            y = e.Y;
            Canvas.Cursor = Cursors.Cross;
            point1 = new PointF(e.X, e.Y);
        }

        public float getPenSize()
        {
            if (radioButton1.Checked)
                penSize = trackBar1.Value - 4;
            else if (radioButton2.Checked)
                penSize = trackBar1.Value;

            return penSize;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving && x != -1 && y != -1 && !radioButton4.Checked)
            {
                g.DrawLine(pen, new Point(x, y), e.Location);
                x = e.X;
                y = e.Y;
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
            x = -1;
            y = -1;
            Canvas.Cursor = Cursors.Default;
            point2 = new PointF(e.X, e.Y);
            if (radioButton4.Checked)
                g.DrawLine(pen, point1, point2);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            pen.Color = p.BackColor;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                penSize = trackBar1.Value - 4;
            else if (radioButton2.Checked)
                penSize = trackBar1.Value;

            pen.Width = penSize;
        }

        // pencil
        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                pen.Width = trackBar1.Value - 4;
        }

        // brush
        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                pen.Width = trackBar1.Value;
        }

        // eraser
        private void radioButton3_Click(object sender, EventArgs e)
        {
            pen.Width = trackBar1.Value;
            pen.Color = SystemColors.Control;
        }

        // line
        private void radioButton4_Click(object sender, EventArgs e)
        {
            pen.Width = trackBar1.Value - 4;
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            if (radioButton4.Checked)
            {
                e.Graphics.DrawLine(pen, point1, point2);
            }
        }
    }
}
