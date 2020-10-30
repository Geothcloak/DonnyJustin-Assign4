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
using System.IO;
using System.Drawing.Imaging;


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
        string currentFile = null; //keep track of the current path of the file being worked on

        public Form1()
        {
            InitializeComponent();
            g = Canvas.CreateGraphics();
            // anti-aliasing for smoother lines
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            penSize = getPenSize();
            pen = new Pen(Color.Black, penSize);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            //Canvas.Image = Image.FromFile("../../blankcanvas.png"); //load a blank canvas
            updateRecentHistory(); //load the recent history
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

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //messagebox do you want to save first?
            string caption = "Warning";
            string message = "Do you want to save before starting a new canvas?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            //show messagebox
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                //if yes use save as code
                //run saveas function
                save();
            }
            //back to new default state 
            Canvas.Image = Image.FromFile("../../blankcanvas.png");
            currentFile = null;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //open .png image
            //fill canvas don't worry about resize 
            string fileName = "";
            openFileDialog1.InitialDirectory = "C:";
            openFileDialog1.Title = "Select a png image";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "PNG Images|*.png";

            //openFileDialog1.ShowDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                //if cancel opening do nothing
            }
            else
            {
                fileName = openFileDialog1.FileName;
                Canvas.Image = Image.FromFile(fileName);
                currentFile = fileName;
                addRecentHistory(fileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }
        private void save()
        {
            //if drawing isnt associated with an existing file use save as code
            //if drawing is associated with an existing file the update
            if (currentFile == null)
            {
                //no current file name so run as save as
                saveAs();
            }
            else
            {
                //save code goes here
                //Canvas.Image.Save(currentFile);
                //
                /* SAVE NOT QUITE WORKING 
                Bitmap pic = new Bitmap(Canvas.Image);
                Canvas.DrawToBitmap(pic, Canvas.ClientRectangle);
                pic.Save(currentFile, ImageFormat.Png);
                */
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAs();
        }
        private void saveAs()
        {
            //always ask where and what to save the file as 
            string fileName = "";
            saveFileDialog1.InitialDirectory = "C:";
            saveFileDialog1.Title = "Save .png as";
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "PNG Images|*.png";

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                //if cancel opening do nothing
            }
            else
            {
                fileName = saveFileDialog1.FileName;
                /* SAVE NOT QUITE WORKING 
                //
                Bitmap pic = new Bitmap(Canvas.Image);
                Canvas.DrawToBitmap(pic, Canvas.ClientRectangle);
                pic.Save(fileName, ImageFormat.Png);
                */

                //update last worked file
                currentFile = fileName;
            }
        }
        private void addRecentHistory(string fileName)
        {
            //add the path to the history file
            StreamWriter recentHistory = new StreamWriter("../../recentHistory.txt", true);
            recentHistory.WriteLine(fileName);
            recentHistory.Close();
            updateRecentHistory();
        }
        private void updateRecentHistory()
        {
            //load the recent history from file into the menu strip
            StreamReader recentHistory = new StreamReader("../../recentHistory.txt");
            string history = recentHistory.ReadLine();
            recentHistory.Close();

            string[] historyLines = File.ReadAllLines("../../recentHistory.txt");

            toolStripMenuItem2.Text = historyLines[historyLines.Length - 1];
            toolStripMenuItem3.Text = historyLines[historyLines.Length - 2];
            toolStripMenuItem4.Text = historyLines[historyLines.Length - 3];
            toolStripMenuItem5.Text = historyLines[historyLines.Length - 4];
            toolStripMenuItem6.Text = historyLines[historyLines.Length - 5];
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            goToHistory(sender);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            goToHistory(sender);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            goToHistory(sender);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            goToHistory(sender);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            goToHistory(sender);
        }
        private void goToHistory(object menuName)
        {
            //go to the path that is listed on the menu strip
            string fileName = menuName.ToString();
            Canvas.Image = Image.FromFile(fileName);
            currentFile = fileName;
            addRecentHistory(fileName);
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