using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Image im; Size imSize; Color[,] c;
        int h, w; Color empty = Color.FromArgb(0, 0, 0, 0);
        private void button1_Click(object sender, EventArgs e)
        {
            counter = 0;   
            Bitmap bitmap;
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image(*.jpg, *.png)|*.jpg;*.png;";
            string filePath = textBox1.Text;
            if (opf.ShowDialog() == DialogResult.OK)
                im = Image.FromFile(opf.FileName);

            opf.Dispose();
            h = im.Height;
            w = im.Width;
            Width = w + 100;
            Height = h + 100;
            imSize = new Size(w, h);
            bitmap = new Bitmap(im, imSize);
            BitmapToColorMatrix(bitmap);

            pictureBox1.Show();
            pictureBox1.Size = new Size(w,h);
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            
            for (int i = 50; i < h; i += 60)
            {
                int max = -1;
                int maxr = i;
                for (int q = 0; q < 45; q++)
                    if (i + q < h)
                    {
                        int count = 0;
                        bool emp = true;
                        for (int j = 0; j < w; j++)
                            if (emp && c[i + q, j] != empty) { count++; emp = false; }
                            else if (!emp && c[i+q, j] == empty) { emp = true; }
                        
                        if(max < count) { max = count; maxr = q; }
                    }

                i += maxr;
                for (int j = 0; j < w; j += 2)
                    if (c[i ,j] != empty)
                        CreateAndExportBitmap(i, j, filePath);
            }
            for (int i = 0; i < h; i ++)
                for (int j = 0; j < w; j++)
                    if (c[i, j] != empty)
                        CreateAndExportBitmap(i, j, filePath);
        }
        int counter = 0;
        private void CreateAndExportBitmap(int i, int j, string path)
        {
            List<(Point, Color)> Points = new List<(Point, Color)>();
            DFS(ref Points, i, j);
            int t = - 1, b = 9999999, l= 99999999, r = -1;
            foreach((Point, Color) p in Points)
            {
                int x = p.Item1.X;
                int y = p.Item1.Y;
                if (x > r) r = x;
                if (x < l) l = x;
                if (y > t) t = y;
                if (y < b) b = y;
            }
            int width = r - l;
            int height = t - b;
            Bitmap tBm = new Bitmap(width + 1, height + 1);
            for(int i2 = 0; i2 < height; i2++)
                for(int j2 = 0; j2 < width; j2++)
                    tBm.SetPixel(j2, i2, Color.FromArgb(0, 0, 0, 0));
            
            foreach ((Point, Color) p in Points)    
            {
                int width2 = p.Item1.X - l;
                int height2 = p.Item1.Y - b;
                tBm.SetPixel(width2, height2, p.Item2);
            }
            counter++;
            string npath = path + "\\" + Convert.ToString(counter) + ".png";
            textBox1.Text = path;
            tBm.Save(npath);
            tBm.Dispose();
            if (checkBox1.Checked)
            {
                Bitmap cmap = new Bitmap(w, h);
                for (int i2 = 0; i2 < h; i2++)
                    for (int j2 = 0; j2 < w; j2++)
                        cmap.SetPixel(j2, i2, c[i2, j2]);

                pictureBox1.Image = cmap;
                pictureBox1.Refresh();
                cmap.Dispose();
            }
        }
        private void DFS(ref List<(Point, Color)> Points, int i, int j)
        {
            Queue<Point> toGo = new Queue<Point>();
            Point curP = new Point(j, i);
            toGo.Enqueue(curP);
            Points.Add((curP, c[i, j]));
            while(toGo.Count > 0)
            {
                Point temp = toGo.Dequeue();
                int x = temp.X, y = temp.Y;
                if(x >= 0 && x < w && y >= 0 && y < h && c[y,x] != empty)
                {
                    Points.Add((temp, c[y, x]));
                    c[y, x] = empty;
                    int kek;
                    if (y > h * 2 / 3) kek = 11;
                    else kek = 2;

                    for (int p = -kek; p <= kek; p++)
                        for (int q = -kek; q <= kek; q++)
                            if (p != 0 || q != 0)
                                toGo.Enqueue(new Point(x + p, y + q));
                }
            }
        }
        private void BitmapToColorMatrix(Bitmap map)
        {
            c = new Color[h,w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    c[i,j] = map.GetPixel(j, i);//x, y i = y, j = x
        }
    }
}
