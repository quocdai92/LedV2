using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LedProject;
using System.Drawing.Drawing2D;

namespace ManageImage
{
    public partial class FormEdit : Form
    {
        public class overRidePanel : Panel
        {
            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
            }
        }

        private static int interval = 50;

        public static Timer T = new Timer()
        {
            Interval = interval
        };

        public string Key = "Con mẹ thằng nào đụng vào của bố";
        private int x = 0;
        private int y = 0;
        private int width = 0;
        private int height = 0;
        private int angle = 0;
        //public static DataTable Table = new DataTable()
        //{
        //    Columns = { "FileName", "TimePlay (s)" }
        //};
        private Bitmap bitmap;
        private BufferedGraphicsContext currentContext;
        private BufferedGraphics myBuffer;
        private PointF viewPortCenter;
        private bool draging;
        private Point lastMouse;
        private List<Image> ListImage;
        private List<FileTemplate> ListFile;
        private FileTemplate CurrentFileTemplate;
        private int index = 0;
        private int widthShow;
        private int heightShow;
        private Image firstImage;
        private bool isStart;
        private int cellSize;
        //private int cellX;
        //private int cellY;
        public FormEdit(DisplayArea area)
        {
            InitializeComponent();
            cellSize = area.CellSize;
            currentContext = BufferedGraphicsManager.Current;
            T.Tick += slider;
            index = 0;
            widthShow = area.Width;
            heightShow = area.Height;
            panel1.Width = widthShow * cellSize;
            panel1.Height = heightShow * cellSize;
            dataGridView1.Columns.Add("FileName", "Tên hiệu ứng");
            dataGridView1.Columns.Add("TimePlay", "Ts");
			dataGridView1.RowHeadersWidth = 50;
            dataGridView1.Columns[0].Width = 180;
            dataGridView1.Columns[1].Width = 25;
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns[0].ReadOnly = true;
            ListFile = new List<FileTemplate>();
            if (area.ListFileTemplates != null && area.ListFileTemplates.Count > 0)
            {
                foreach (var file in area.ListFileTemplates)
                {
                    if (bitmap == null)
                    {
                        firstImage = file.ListImageReturn.ElementAt(0);
                        bitmap = (Bitmap)(firstImage);
                        setup(true);
                        //setFileInfo();
                        panel1.Invalidate();
                    }
                    x = file.X;
                    y = file.Y;
                    height = heightShow;
                    width = widthShow;
                    ListFile.Add(file);
                    //Table.Rows.Add(new object[] { fileName, 10 });
                    var idx = dataGridView1.Rows.Add(file.FileName, "10");
                    dataGridView1.Rows[idx].HeaderCell.Value = String.Format("{0}", idx + 1);
                }
                ListImage = ListFile[0].ListImageReturn;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
            //if (bitmap != null)
            //    setFileInfo();

        }

        private void setup(bool resetViewport)
        {
            if (myBuffer != null)
                myBuffer.Dispose();
            myBuffer = currentContext.Allocate(this.panel1.CreateGraphics(),
                this.panel1.DisplayRectangle);
            if (bitmap != null)
            {
                if (resetViewport)
                {
                    if (width == 0 || height == 0)
                        SetViewPort(new RectangleF(x - bitmap.Width / 2.0f, y - bitmap.Height / 2.0f, bitmap.Width,
                            bitmap.Height));
                    else
                    {
                        SetViewPort(new RectangleF(x - width / 2.0f, y - height / 2.0f, width, height));
                    }
                }
            }
            this.panel1.Focus();
            this.panel1.Invalidate();
        }

        private void SetViewPort(RectangleF worldCords)
        {
            viewPortCenter = new PointF(worldCords.X + (worldCords.Width / 2.0f),
                worldCords.Y + (worldCords.Height / 2.0f));

        }

        private void PaintImage(int wS, int hS, int w, int h)
        {
            //Rectangle drawRect = new Rectangle(
            //    (int)(viewPortCenter.X - width / 2.0f),
            //    (int)(viewPortCenter.Y - height / 2.0f),
            //    (int)(width),
            //    (int)(height));
            //this.toolStripStatusLabel1.Text = "DrawRect = " + drawRect.ToString();

            Rectangle showRect = new Rectangle((int)(panel1.Width / 2.0f - wS / 2.0f),
                (int)(panel1.Height / 2.0f - hS / 2.0f), wS, hS);

            myBuffer.Graphics.Clear(Color.White); //Clear the Back buffer
            //Draw the image, Write image to back buffer, and render back buffer              
            //Pen pen = new Pen(Color.Black);
            //pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            ////if (bitmap != null)
            ////{
            ////    myBuffer.Graphics.DrawImage(bitmap, drawRect);
            ////}
            //myBuffer.Graphics.DrawRectangle(pen, showRect);
            Brush brush;
            brush = new SolidBrush(Color.FromArgb(200, 128, 128, 128));
            Brush brushCell;

            if (bitmap != null)
            {
                if (isStart)
                {
                    myBuffer.Graphics.FillRectangle(new SolidBrush(Color.Black), showRect);
                    for (int j = 0; j < heightShow; j++)
                        for (int i = 0; i < widthShow; i++)
                        {
                            brushCell = new SolidBrush(bitmap.GetPixel(i, j));
                            myBuffer.Graphics.FillEllipse(brushCell, showRect.X + cellSize / 6 + i * cellSize,
                                showRect.Y + cellSize / 6 + j * cellSize, 2 * cellSize / 3, 2 * cellSize / 3);
                        }
                }
                else
                {
                    //myBuffer.Graphics.DrawImage(bitmap, drawRect);
                    myBuffer.Graphics.FillRectangle(new SolidBrush(Color.Black), showRect);
                    for (int j = 0; j < heightShow; j++)
                        for (int i = 0; i < widthShow; i++)
                        {
                            brushCell = new SolidBrush(Color.Gray);
                            myBuffer.Graphics.FillEllipse(brushCell, showRect.X + cellSize / 6 + i * cellSize,
                                showRect.Y + cellSize / 6 + j * cellSize, 2 * cellSize / 3, 2 * cellSize / 3);
                        }
                }
            }


            myBuffer.Graphics.FillRectangle(brush, 0, 0, panel1.Width / 2 - wS / 2, panel1.Height);
            myBuffer.Graphics.FillRectangle(brush, panel1.Width / 2 + wS / 2, 0, panel1.Width / 2 - wS / 2,
                panel1.Height);
            myBuffer.Graphics.FillRectangle(brush, panel1.Width / 2 - wS / 2, 0, wS,
                panel1.Height / 2 - hS / 2);
            myBuffer.Graphics.FillRectangle(brush, panel1.Width / 2 - wS / 2, panel1.Height / 2 + hS / 2,
                wS, panel1.Height / 2 - hS / 2);

            myBuffer.Render(this.panel1.CreateGraphics());

        }

        public List<Image> ReadFileTmp(string filename)
        {
            try
            {

                List<Image> listImg = new List<Image>();
                var fileTobyte = File.ReadAllBytes(filename);
                var readFile = EncDec.Decrypt(fileTobyte, Key);
                int from = 0;
                for (int i = 0; i < readFile.Length; i++)
                {
                    //read header:
                    if ((char)readFile[i] == 'e' && (char)readFile[i + 1] == 'h' && (char)readFile[i + 2] == 'd'
                        && (char)readFile[i + 3] == 'e' && (char)readFile[i + 4] == 'r')
                    {
                        from = i + 5;
                    }
                    //read ListImage
                    if ((char)readFile[i] == 'e' && (char)readFile[i + 1] == 'n' && (char)readFile[i + 2] == 'd')
                    {
                        var outFile = new byte[i - from];
                        var k = 0;
                        for (int j = from; j < i; j++)
                        {
                            outFile[k] = readFile[j];
                            k++;
                        }
                        var img = ByteArrayToImage(outFile);
                        listImg.Add(img);
                        from = i + 3;
                    }
                }
                return listImg;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void openFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            if (ListFile == null)
                ListFile = new List<FileTemplate>();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in openFileDialog1.FileNames)
                {
                    var listImg = ReadFileTmp(fileName);
                    if (listImg != null)
                    {
                        if (bitmap == null)
                        {
                            firstImage = listImg.ElementAt(0);
                            bitmap = (Bitmap)(firstImage);
                            setup(true);
                            //setFileInfo();
                            panel1.Invalidate();
                        }
                        x = 0;
                        y = 0;
                        height = heightShow;
                        width = widthShow;
                        ListFile.Add(new FileTemplate()
                        {
                            FileName = fileName,
                            X = x,
                            Y = y,
                            Width = width,
                            Height = height,
                            Angle = angle,
                            ListImages = listImg,
                            TimePlay = 10
                        });
                        //Table.Rows.Add(new object[] { fileName, 10 });
                        var idx = dataGridView1.Rows.Add(fileName, "10");
                        dataGridView1.Rows[idx].HeaderCell.Value = String.Format("{0}", idx + 1);
                    }
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            setup(false);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            PaintImage(widthShow * cellSize, heightShow * cellSize, width * cellSize, height * cellSize);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                draging = true;

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {

            if (draging)
            {
                viewPortCenter = new PointF(viewPortCenter.X - ((lastMouse.X - e.X)),
                    viewPortCenter.Y - ((lastMouse.Y - e.Y)));
                panel1.Invalidate();
            }
            x = (int)viewPortCenter.X;
            y = (int)viewPortCenter.Y;
            lastMouse = e.Location;
            //setFileInfo();

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
                draging = false;
        }

        private Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        private Image resizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private List<Image> createListImageCrop(List<Image> listImgOrg)
        {
            List<Image> listImgCrop = new List<Image>();
            foreach (Image img in listImgOrg)
            {
                //rotate:
                img.RotateFlip(GetRotateFlipType(angle));
                Rectangle showRect = new Rectangle((int)(panel1.Width / 2.0f - widthShow / 2.0f),
                    (int)(panel1.Height / 2.0f - heightShow / 2.0f), widthShow, heightShow);
                Rectangle drawRect = new Rectangle((int)(viewPortCenter.X - width / 2.0f),
                    (int)(viewPortCenter.Y - height / 2), width, height);
                if (drawRect.IntersectsWith(drawRect))
                {
                    var intersectRect = Rectangle.Intersect(showRect, drawRect);
                    var cropRect = new Rectangle(intersectRect.X - drawRect.X, intersectRect.Y - drawRect.Y,
                        intersectRect.Width, intersectRect.Height);

                    var cropImg = cropImage(resizeImage(img, width, height), cropRect);
                    var cellImg = resizeImage(cropImg, width / cellSize, height / cellSize);
                    listImgCrop.Add(cellImg);
                    //listImgCrop.Add(cropImg);
                }
            }
            return listImgCrop;
        }

        private List<Image> createListImage()
        {
            List<Image> listImage = new List<Image>();
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Cells[0].Value.ToString() != "")
            {
                string filename = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                int timeplay = 0;
                Int32.TryParse(dataGridView1.CurrentRow.Cells[1].Value.ToString(), out timeplay);
                List<Image> listImg = new List<Image>();
                var listImageFile = ReadFileTmp(filename);
                foreach (Image img in listImageFile)
                {
                    img.RotateFlip(GetRotateFlipType(angle));
                    Rectangle showRect = new Rectangle((int)(panel1.Width / 2.0f - widthShow / 2.0f),
                        (int)(panel1.Height / 2.0f - heightShow / 2.0f), widthShow, heightShow);
                    Rectangle drawRect = new Rectangle((int)(viewPortCenter.X - width / 2.0f),
                        (int)(viewPortCenter.Y - height / 2), width, height);
                    if (drawRect.IntersectsWith(drawRect))
                    {
                        var intersectRect = Rectangle.Intersect(showRect, drawRect);
                        var cropRect = new Rectangle(intersectRect.X - drawRect.X, intersectRect.Y - drawRect.Y,
                            intersectRect.Width, intersectRect.Height);

                        var cropImg = cropImage(resizeImage(img, width, height), cropRect);
                        var cellImg = resizeImage(cropImg, width / cellSize, height / cellSize);
                        listImg.Add(cellImg);
                    }
                }
                listImageFile.Clear();
                var coutImg = listImg.Count;
                if (coutImg > 0)
                {
                    var countLoop = timeplay * 1000 / interval;
                    for (int i = 0; i < countLoop; i++)
                    {
                        var j = i % (coutImg);
                        listImage.Add(resizeImage(listImg.ElementAt(j), widthShow, heightShow));
                    }
                }

            }
            //var cropImg = createListImageCrop(listImage);
            return listImage;
        }

        private List<Image> createListAllImage()
        {
            List<Image> listImage = new List<Image>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && !string.IsNullOrEmpty(row.Cells[0].Value.ToString()))
                {
                    string filename = row.Cells[0].Value.ToString();
                    int timeplay = 0;
                    Int32.TryParse(row.Cells[1].Value.ToString(), out timeplay);
                    List<Image> listImg = new List<Image>();
                    var listImageFile = ReadFileTmp(filename);
                    foreach (Image img in listImageFile)
                    {
                        img.RotateFlip(GetRotateFlipType(angle));
                        Rectangle showRect = new Rectangle((int)(panel1.Width / 2.0f - widthShow / 2.0f),
                            (int)(panel1.Height / 2.0f - heightShow / 2.0f), widthShow, heightShow);
                        Rectangle drawRect = new Rectangle((int)(viewPortCenter.X - width / 2.0f),
                            (int)(viewPortCenter.Y - height / 2), width, height);
                        if (drawRect.IntersectsWith(drawRect))
                        {
                            var intersectRect = Rectangle.Intersect(showRect, drawRect);
                            var cropRect = new Rectangle(intersectRect.X - drawRect.X, intersectRect.Y - drawRect.Y,
                                intersectRect.Width, intersectRect.Height);

                            var cropImg = cropImage(resizeImage(img, width, height), cropRect);
                            var cellImg = resizeImage(cropImg, width / cellSize, height / cellSize);
                            listImg.Add(cellImg);
                        }
                    }
                    listImageFile.Clear();
                    var coutImg = listImg.Count;
                    if (coutImg > 0)
                    {
                        var countLoop = timeplay * 1000 / interval;
                        for (int i = 0; i < countLoop; i++)
                        {
                            var j = i % (coutImg);
                            listImage.Add(resizeImage(listImg.ElementAt(j), widthShow, heightShow));
                        }
                    }
                }
            }
            return listImage;
        }

        private void slider(Object source, EventArgs e)
        {
            if (ListImage != null && index < ListImage.Count)
            {
                //re-draw
                bitmap = (Bitmap)ListImage[index];
                //setup(false);
                index++;
            }
            else
            {
                //isStart = false;
                //T.Stop();
                index = 0;
                //ListImage.Clear();
                //bitmap = (Bitmap)firstImage;
                //setup(true);
            }
            panel1.Invalidate();
        }



        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void tbX_TextChanged(object sender, EventArgs e)
        {
            int tempX = Convert.ToInt32(tbX.Text);

            if (tempX < widthShow)
            {
                x = tempX;
                ListImage = new List<Image>();
                if (CurrentFileTemplate != null)
                {
                    CurrentFileTemplate.X = x;
                    var lstResizeImage = ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
                        CurrentFileTemplate.Height);
                    var rectCrop = new Rectangle(CurrentFileTemplate.X, CurrentFileTemplate.Y, widthShow - CurrentFileTemplate.X,
                        heightShow - CurrentFileTemplate.Y);
                    foreach (var image in lstResizeImage)
                    {
                        ListImage.Add(cropImage(image, rectCrop));
                    }
                    ListImage = ReSizeListImage(ListImage, CurrentFileTemplate.Width,
                        CurrentFileTemplate.Height);
                }

            }
            else
            {
				MessageBox.Show(String.Format("X phải nhỏ hơn {0}!", widthShow), @"Lỗi kích thước");
                tbX.Text = x.ToString();
            }

        }

        private void tbY_TextChanged(object sender, EventArgs e)
        {
            int tempY = Convert.ToInt32(tbY.Text);
            if (tempY < heightShow)
            {
                y = tempY;
                ListImage = new List<Image>();
                if (CurrentFileTemplate != null)
                {
                    CurrentFileTemplate.Y = y;
                    var lstResizeImage = ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
                        CurrentFileTemplate.Height);
                    var rectCrop = new Rectangle(CurrentFileTemplate.X, CurrentFileTemplate.Y, widthShow - CurrentFileTemplate.X,
                        heightShow - CurrentFileTemplate.Y);
                    foreach (var image in lstResizeImage)
                    {
                        ListImage.Add(cropImage(image, rectCrop));
                    }
                    ListImage = ReSizeListImage(ListImage, CurrentFileTemplate.Width,
                        CurrentFileTemplate.Height);
                }
            }
            else
            {
				MessageBox.Show(string.Format("Y phải nhỏ hơn {0}!", heightShow), @"Lỗi kích thước");
                tbY.Text = y.ToString();
            }


        }

        private void tbWidth_TextChanged(object sender, EventArgs e)
        {
            int tempW = Convert.ToInt32(tbWidth.Text);

            if (tempW >= x)
            {
                width = tempW;
                if (width < tbWidth.Minimum)
                {
                    width = Convert.ToInt32(tbWidth.Minimum);
                }
				ListImage = new List<Image>();
				var lstImage = new List<Image>();
				if (CurrentFileTemplate != null)
				{
					CurrentFileTemplate.Width = width;
					var lstResizeImage = ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
						CurrentFileTemplate.Height);
					var rectCrop = new Rectangle(CurrentFileTemplate.X, CurrentFileTemplate.Y, widthShow - CurrentFileTemplate.X,
					   heightShow - CurrentFileTemplate.Y);
					lstImage.AddRange(lstResizeImage.Select(image => cropImage(image, rectCrop)));
					ListImage.AddRange(ReSizeListImage(lstImage, widthShow, heightShow));
					//ListImage.AddRange(ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
					//    CurrentFileTemplate.Height));
				}
            }
            else
            {
                MessageBox.Show(string.Format("Rộng phải lớn hơn hoặc bằng {0}!",x), @"Lỗi kích thước");
                tbWidth.Text = width.ToString();
            }
        }

        private void tbHeight_TextChanged(object sender, EventArgs e)
        {
            int tempH = Convert.ToInt32(tbHeight.Text);
            if (tempH >= y)
            {
                height = tempH;
                if (height < tbHeight.Minimum)
                {
                    height = Convert.ToInt32(tbHeight.Minimum);
                }
				ListImage = new List<Image>();
				var lstImage = new List<Image>();
				if (CurrentFileTemplate != null)
				{
					CurrentFileTemplate.Height = height;
					var lstResizeImage = ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
						CurrentFileTemplate.Height);
					var rectCrop = new Rectangle(CurrentFileTemplate.X, CurrentFileTemplate.Y, widthShow - CurrentFileTemplate.X,
						heightShow - CurrentFileTemplate.Y);
					lstImage.AddRange(lstResizeImage.Select(image => cropImage(image, rectCrop)));
					ListImage.AddRange(ReSizeListImage(lstImage, widthShow, heightShow));
					//ListImage.AddRange(ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
					//    CurrentFileTemplate.Height));
				}
            }
            else
            {
				MessageBox.Show(string.Format("Chiều Cao phải lớn hơn hoặc bằng {0}!", y), @"Lỗi kích thước");
                tbHeight.Text = height.ToString();
            }

        }

        private void btOpenFile_Click(object sender, EventArgs e)
        {
            openFile();
        }


        private void btSave_Click(object sender, EventArgs e)
        {
            FrmProgressbar progressbar = new FrmProgressbar();
            progressbar.progressBar1.Maximum = ListFile.Count;
            progressbar.progressBar1.Minimum = 0;
            progressbar.progressBar1.Step = 1;
            progressbar.Show();
            Main.CurrentArea.ListFileTemplates = new List<FileTemplate>();
            Main.CurrentArea.ListImages = new List<Image>();
            if (CurrentFileTemplate != null)
            {
                SaveCurrentFile();
            }
            var timePlay = 0;
            foreach (var file in ListFile)
            {
                int timeplay = file.TimePlay;
                if (file.ListImageReturn.Count == 0)
                {
                    file.ListImageReturn = ReSizeListImage(file.ListImages, widthShow, heightShow);
                }
                Main.CurrentArea.ListFileTemplates.Add(file);
                timePlay += timeplay;
                progressbar.progressBar1.PerformStep();
            }
            progressbar.Close();
            Main.CurrentArea.TimePlay = timePlay;
            //Main.CurrentArea.Angle = angle;
//			MessageBox.Show(@"Lưu thành công...", @"Save");
            T.Stop();
            this.Dispose();
            this.Close();
        }



        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ListImage != null && ListImage.Count > 0)
            {
                //ListImage = new List<Image>(CurrentFileTemplate.ListImages);
                isStart = true;
                T.Start();
                ListImage = CaculateListImage(CurrentFileTemplate.TimePlay);
            }

        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isStart = false;
            T.Stop();
            index = 0;
            //ListImage.Clear();
            bitmap = (Bitmap)firstImage;
            isStart = false;
            setup(true);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (CurrentFileTemplate != null)
            {
                SaveCurrentFile();
            }
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Cells[0].Value == null ||
                dataGridView1.CurrentRow.Cells[0].Value.ToString() == "") return;
            var filename = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            if (ListFile.Any(m => m.FileName.Equals(filename)))
            {
                CurrentFileTemplate = ListFile.FirstOrDefault(m => m.FileName.Equals(filename));
                UpdateInfoData();
            }
            ListImage = new List<Image>();
            index = 0;
            T.Stop();
            //comboBox1.SelectedIndex = 0;
            //var listImg = readFileTmp(filename);
            if (CurrentFileTemplate != null && CurrentFileTemplate.ListImages.Count > 0)
            {
                comboBox1.SelectedIndex = comboBox1.FindStringExact(CurrentFileTemplate.Angle.ToString());
                var lstResizeImage = ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
                        CurrentFileTemplate.Height);
                var rectCrop = new Rectangle(CurrentFileTemplate.X, CurrentFileTemplate.Y, widthShow - CurrentFileTemplate.X,
                    heightShow - CurrentFileTemplate.Y);
                foreach (var image in lstResizeImage)
                {
                    ListImage.Add(cropImage(image, rectCrop));
                }
                //ListImage = RotateListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Angle);
                //ListImage = ReSizeListImage(ListImage, widthShow, heightShow);
                firstImage = ListImage[0];
                bitmap = (Bitmap)(firstImage);
                setup(true);
            }
            PlayRow();
            btnPlay.PerformClick();
        }

        private void playAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isStart = true;
            var listImage = createListAllImage();
            ListImage = new List<Image>();
            ListImage.AddRange(listImage);
            if (ListImage.Count > 0)
            {
                isStart = true;
                T.Start();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            angle = Convert.ToInt32(comboBox1.SelectedItem);
            ListImage = new List<Image>();
            if (CurrentFileTemplate != null)
            {
                CurrentFileTemplate.Angle = angle;
                var listImage = new List<Image>();
                listImage.AddRange(CurrentFileTemplate.ListImages);
                var lstResizeImage = RotateListImage(listImage, angle);
                lstResizeImage = ReSizeListImage(lstResizeImage, CurrentFileTemplate.Width, CurrentFileTemplate.Height);
                var rectCrop = new Rectangle(CurrentFileTemplate.X, CurrentFileTemplate.Y, widthShow - CurrentFileTemplate.X,
                   heightShow - CurrentFileTemplate.Y);
                foreach (var image in lstResizeImage)
                {
                    ListImage.Add(cropImage(image, rectCrop));
                }
                //var lstResizeImage = ReSizeListImage(CurrentFileTemplate.ListImages, CurrentFileTemplate.Width,
                //        CurrentFileTemplate.Height);

            }
            //var listImage = createListAllImage();
            //ListImage = new List<Image>();
            //ListImage.AddRange(listImage);
        }

        public void SaveCurrentFile()
        {
            var fileTemplate = ListFile.FirstOrDefault(m => m.FileName.Equals(CurrentFileTemplate.FileName));
            if (fileTemplate != null)
            {
                fileTemplate.X = x;
                fileTemplate.Y = y;
                fileTemplate.Width = width;
                fileTemplate.Height = height;
                fileTemplate.Angle = angle;
                fileTemplate.ListImageReturn = ListImage;
            }
        }

        public List<Image> RotateListImage(List<Image> listImage, int agl)
        {
            var lstImage = new List<Image>();
            foreach (var image in listImage)
            {
                image.RotateFlip(GetRotateFlipType(angle));
                lstImage.Add(image);
            }
            return lstImage;
        }

        public List<Image> ReSizeListImage(List<Image> listImage, int newWidth, int newHeight)
        {
            var lstImageNew = new List<Image>();
            foreach (var image in listImage)
            {
                var destRect = new Rectangle(0, 0, newWidth, newHeight);
                var destImage = new Bitmap(newWidth, newHeight);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel,
                            wrapMode);
                    }
                }
                lstImageNew.Add(destImage);
            }
            return lstImageNew;
        }

        public RotateFlipType GetRotateFlipType(int agl)
        {
            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            switch (agl)
            {
                case 0:
                    rotateFlipType = (RotateFlipType.RotateNoneFlipNone);
                    break;
                case 90:
                    rotateFlipType = (RotateFlipType.Rotate90FlipNone);
                    break;
                case 180:
                    rotateFlipType = (RotateFlipType.Rotate180FlipNone);
                    break;
                case 270:
                    rotateFlipType = (RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return rotateFlipType;
        }

        public void UpdateInfoData()
        {
            if (CurrentFileTemplate != null)
            {
                tbX.Text = CurrentFileTemplate.X.ToString();
                tbY.Text = CurrentFileTemplate.Y.ToString();
                tbWidth.Text = CurrentFileTemplate.Width.ToString();
                tbWidth.Minimum = widthShow;
                tbHeight.Text = CurrentFileTemplate.Height.ToString();
                tbHeight.Minimum = heightShow;
                comboBox1.SelectedIndex = comboBox1.FindStringExact(CurrentFileTemplate.Angle.ToString());
            }
        }

        public List<Image> CaculateListImage(int timePlay)
        {
            var lstImage = new List<Image>();
            if (ListImage != null && ListImage.Count > 0)
            {
                var countImge = timePlay * 1000 / interval;
                for (int i = 0; i < countImge; i++)
                {
                    var idx = i % ListImage.Count;
                    lstImage.Add(ListImage[idx]);
                }
            }
            return lstImage;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell != null && CurrentFileTemplate != null && dataGridView1.CurrentCell.ColumnIndex == 1)
            {
                if (dataGridView1.CurrentCell.Value == null || Convert.ToInt32(dataGridView1.CurrentCell.Value) <= 0)
                {
                    dataGridView1.CurrentCell.Value = 10;
                }
                else
                {
                    CurrentFileTemplate.TimePlay = Convert.ToInt32(dataGridView1.CurrentCell.Value);
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (ListImage != null && ListImage.Count > 0)
            {
                //ListImage = new List<Image>(CurrentFileTemplate.ListImages);
                isStart = true;
                T.Start();
                ListImage = CaculateListImage(CurrentFileTemplate.TimePlay);
            }
        }

        private void PlayRow()
        {
            if (ListImage != null && ListImage.Count > 0)
            {
                //ListImage = new List<Image>(CurrentFileTemplate.ListImages);
                isStart = true;
                T.Start();
                ListImage = CaculateListImage(CurrentFileTemplate.TimePlay);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isStart = false;
            T.Stop();
            index = 0;
            //ListImage.Clear();
            bitmap = (Bitmap)firstImage;
            isStart = false;
            setup(true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
            //{
            //    dataGridView1.Rows.RemoveAt(item.Index);
            //    var fileName = item.Cells[0].Value;
            //    //ListFile = new List<FileTemplate>();
            //    var fileTemplates = ListFile.Where(m => !m.FileName.Equals(fileName)).ToList();
            //    ListFile = new List<FileTemplate>();
            //    ListFile.AddRange(fileTemplates);
            //}
            if (dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                var fileName = dataGridView1.CurrentRow.Cells[0].Value;
                //ListFile = new List<FileTemplate>();
                var fileTemplates = ListFile.Where(m => !m.FileName.Equals(fileName)).ToList();
                ListFile = new List<FileTemplate>();
                ListFile.AddRange(fileTemplates);
            }
        }

		private void FormEdit_FormClosed(object sender, FormClosedEventArgs e)
		{
			btSave.PerformClick();
		}
    }
}
