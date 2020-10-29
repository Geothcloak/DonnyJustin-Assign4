///////////////////////////////////////
/// Donny Kapic z1855273
/// Justin Roesner z1858242
/// CSCI 473 .NET programming
/// Assign 4
///////////////////////////////////////
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
        PointF point1;  // coordinate of mouse down
        PointF point2;  // coordinate of mouse up
        float penSize;  // size of pen
        Stack<Graphics> undo = new Stack<Graphics>(); // holds the lines drawn
        Stack<Graphics> redo = new Stack<Graphics>(); // holds the lines that were undone
        Stack<Point> pointStack = new Stack<Point>(); // coordinates of lines drawn
        Stack<int> history = new Stack<int>();        // length of each line
        Stack<Color> colorHistory = new Stack<Color>(); // history of colors (used for redo)

        public Form1()
        {
            InitializeComponent();
            g = Canvas.CreateGraphics();
            // anti-aliasing for smoother lines
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            penSize = getPenSize();
            pen = new Pen(Color.Black, penSize);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        // when user mouses down the coordinate is recorded
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            x = e.X;
            y = e.Y;
            Canvas.Cursor = Cursors.Cross;
            point1 = new PointF(e.X, e.Y);
        }

        // returns pen size based on which radio button is checked (pencil/brush/eraser)
        public float getPenSize()
        {
            if (radioButton1.Checked)
                penSize = trackBar1.Value - 4;
            else if (radioButton2.Checked)
                penSize = trackBar1.Value;

            return penSize;
        }

        // constantly adds Graphics objects to undo stack
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving && x != -1 && y != -1 && !radioButton4.Checked)
            {
                g.DrawLine(pen, new Point(x, y), e.Location);
                pointStack.Push(new Point(x, y));
                undo.Push(g);

                x = e.X;
                y = e.Y;
            }
        }

        // mouse-up coordinates are recorded.
        // if line button is checked, it will draw a straight line
        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
            x = -1;
            y = -1;
            Canvas.Cursor = Cursors.Default;
            point2 = new PointF(e.X, e.Y);
            if (radioButton4.Checked)
                g.DrawLine(pen, point1, point2);

            history.Push(pointStack.Count);
        }

        // color picker
        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            pen.Color = p.BackColor;
            colorHistory.Push(pen.Color);
        }

        // sets the pen width based on user input
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
            if (radioButton4.Checked)
                pen.Width = trackBar1.Value - 4;
        }

        // Undo algorithm:
        // used saved coordinates from drawn lines to redraw the lines
        // pixel for pixel in the background color
        private void button1_Click(object sender, EventArgs e)
        {
            pen.Color = SystemColors.Control;   // background color
            pen.Width = trackBar1.Value;

            // variables that store line length
            int temp;
            int temp2 = 0; 
           
            // make sure not to go out of Stack bounds if
            // there is only one line drawn
            if (history.Count == 1)
            {
                temp2 = history.Peek();
                temp = 1;
            }
            // get length of most recently drawn line
            else
            {
                temp2 = history.Pop();
                temp = history.Peek();
            }
            // draw over most recent line with background color
            while (undo.Count >= 2 && temp <= temp2)
            {
                redo.Push(undo.Peek());
                redo.Push(undo.Peek());
                g = undo.Pop();
                undo.Pop();
                g.DrawLine(pen, pointStack.Pop(), pointStack.Pop());
                temp++;
                temp++;
            }

            // resets the pen width to previous value
            if (radioButton1.Checked)
                pen.Width = trackBar1.Value - 4;
            else if (radioButton2.Checked)
                pen.Width = trackBar1.Value;
        }

        // redo
        // Couldn't get it to work
        /*
        private void button2_Click(object sender, EventArgs e)
        {
            // get the correct color
            if (colorHistory.Count != 0)
            {
                colorHistory.Pop();
                pen.Color = colorHistory.Pop();
                MessageBox.Show(pen.Color.ToString());
            }
            else
                pen.Color = Color.Black;

            pen.Width = trackBar1.Value;

            int temp;
            int temp2 = 0;

            if (history.Count == 1)
            {
                temp2 = history.Peek();
                temp = 1;
            }
            else
            {
                temp2 = history.Pop();
                temp = history.Peek();
            }
            while (undo.Count >= 2 && temp <= temp2)
            {
                g = redo.Pop();
                redo.Pop();
                g.DrawLine(pen, pointStack.Pop(), pointStack.Pop());
                temp++;
                temp++;
            }
        }*/
    }
}