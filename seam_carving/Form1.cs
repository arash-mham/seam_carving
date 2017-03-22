using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace HaarImage
{
    public partial class Form1 : Form
    {
        Bitmap Original;
        int initialHeight;
        int initialWidth;
        Bitmap second;




        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        private void Save_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog save_image = new SaveFileDialog())
            {
                save_image.ShowDialog();
                string name = save_image.FileName;
                pictureBox1.Image.Save(name, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            int changeHeight = Height - initialHeight;
            int changeWidth = Width - initialWidth;
            initialWidth = Width;
            initialHeight = Height;
            if (changeWidth < 0)
            {
                ImageSeam I = new ImageSeam((Bitmap)pictureBox1.Image);
                pictureBox1.Image = I.DeletSeam_Vertical_List((Bitmap)pictureBox1.Image, -changeWidth);
            }
            if (changeHeight < 0)
            {
                ImageSeam I = new ImageSeam((Bitmap)pictureBox1.Image);
                pictureBox1.Image = I.DeletSeam_Horizontal_List((Bitmap)pictureBox1.Image, -changeHeight);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initialHeight = Height;
            initialWidth = Width;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "bmp files (*.png)|*.png";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = new Bitmap(dlg.FileName);
                    second = new Bitmap(dlg.FileName);
                }
            }

        }
        private Bitmap binary_reconstruct()
        {
            Bitmap OriginalBitmap = new Bitmap(pictureBox1.Image);
            int OriginWidth = (OriginalBitmap.Width) * 2;
            int OriginHeight = (OriginalBitmap.Height) * 2;
            return OriginalBitmap;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            ImageSeam I = new ImageSeam((Bitmap)pictureBox1.Image);
            switch (e.KeyData)
            {
                case Keys.V:
                    I = new ImageSeam((Bitmap)pictureBox1.Image);
                    second = (Bitmap)pictureBox1.Image;
                    pictureBox1.Image = I.MarkSeam_vertical((Bitmap)pictureBox1.Image);
                    break;
                case Keys.H:
                    I = new ImageSeam((Bitmap)pictureBox1.Image);
                    pictureBox1.Image = I.MarkSeam_Horizontal((Bitmap)pictureBox1.Image);
                    break;
                case Keys.Q:
                    I = new ImageSeam((Bitmap)pictureBox1.Image);
                    pictureBox1.Image = I.DeletSeam_Vertical_List((Bitmap)pictureBox1.Image, 30);
                    break;
                case Keys.R:
                    I = new ImageSeam((Bitmap)pictureBox1.Image);
                    pictureBox1.Image = I.DeletSeam_Horizontal_List((Bitmap)pictureBox1.Image, 10);
                    break;
            }


        }
    }

}
