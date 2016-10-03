using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using ManageImage;
using netDxf;

//using netDxf;


namespace LedFullControl
{
    public partial class frmMAP : Form
    {
        private Point StartPoint;
        private Point prevScrollPoint;
        private Point startMovePoint;
        private bool isPan;
        private bool dragging;
        public static Stack<UndoRedoModel> UndoStack = new Stack<UndoRedoModel>();
        public static Stack<UndoRedoModel> RedoStack = new Stack<UndoRedoModel>();
        public static List<Point> ListMovePoints = new List<Point>();
        public static List<Point> ListTempMovePoints = new List<Point>();
        public static List<Point> ListTempLine1 = new List<Point>();
        public static List<Point> ListSelectPoints = new List<Point>();
        public static List<Point> ListToCopy = new List<Point>();
        public static List<Point> ListPasted = new List<Point>();
        private bool isMove;
        private bool isdragmove;
        private bool isCopy;
        private bool enableMovePastedArea;
        public frmMAP()
        {
            InitializeComponent();
            SetConfig();
            MakeBackgroundGrid();
            UndoStack.Push(new UndoRedoModel());
        }

        void SetConfig()
        {
            KichThuoc.GridGap = 14;
            KichThuoc.CrossSize = 50;
            KTLamViec.ChieuRong = 500;
            KTLamViec.ChieuCao = 500;
        }

        void ReadConfig(int ChieuRong, int ChieuCao)
        {
            //	đọc dữ liệu config ở phần cấu hình
            //frmCauHinh DuLieu = new frmCauHinh();
            //KTLamViec.ChieuRong = ChieuRong;
            //KTLamViec.ChieuCao = ChieuCao;
            //MakeBackgroundGrid();
        }

        private void btnCauHinh_Click(object sender, EventArgs e)
        {
            //frmCauHinh ThietLap = new frmCauHinh();
            //ThietLap.Rong = KichThuoc.ChieuRong;
            //ThietLap.Cao = KichThuoc.ChieuCao;
            //ThietLap.TruyenConfig = new frmCauHinh.GetData(ReadConfig);		//	lấy dữ liệu từ form 2
            //ThietLap.ShowDialog();
        }

        private void frmMAP_Load(object sender, EventArgs e)
        {
            picDraw.BackColor = Color.Black;
            nbrVungRong.Value = KTLamViec.ChieuRong;
            nbrVungCao.Value = KTLamViec.ChieuCao;
        }

        //----------------------------------------------------------------------------------------------------------
        List<Point> FreeLED = new List<Point>();    //	led chưa được đi dây
                                                    //List<List<Point>> Line ;	//	Mảng led đã được đi dây
        List<Point> Line1 = new List<Point>();
        List<Point> Line2 = new List<Point>();
        //List<Point> Line3 = new List<Point>();
        List<Point> LedSelected = new List<Point>();    //	khi được chọn ta tạo ra mảng mới

        //		private int SoLine = new int();				//	Số line đã dùng
        private int[] SoLuongPhanTuList = new int[ListMax];     //	Số lượng phần tử trong mỗi mảng	
        const int ListMax = 100;
        //int LineDangVe = new int();     //	line đang làm việc

        bool isDrawing;                 //	đang ở chế độ nối led

        private int CheDoVe;            //	Chế độ vẽ
        enum CheDo
        {
            Ranh,
            ThemLed,
            XoaLed,
            NoiLed,
            XoaVung,
            ChonVung,
            VeLed,
            VeVung,
            VeVungZ,
            VeDuongThang,
            DaoChieu,
            ZZNgang,    //	dây led đi theo chiều ngang (Chế độ vẽ nhanh)
            ZZDoc,      //	Dây led đi theo chiều dọc
            ZNgang,         //	đi dây kiểu chữ Z
            ZDoc,
            MoveVung  //di chuyen ca 1 vung
        }

        private Point Kick1, MP, Kick2 = new Point(); //	Vị trí đầu , Vị trí sau
        private bool MP_OK = false; //	nếu vị trí 2 được kick

        private Point P2;   //	P2 : vị trí cuối cùng của chuột -> Mouse Move
        DinhNghia KichThuoc = new DinhNghia();
        DinhNghia KTLamViec = new DinhNghia();
        ToaDo TD = new ToaDo();

        #region Vẽ lưới cho vùng làm việc và SnapToGrid
        private void MakeBackgroundGrid()       //	Vẽ lưới nền
        {
            int ChieuRong, ChieuCao;
            ChieuRong = KichThuoc.GridGap * KTLamViec.ChieuRong;        //	Tính chiều rộng vùng làm việc
            ChieuCao = KichThuoc.GridGap * KTLamViec.ChieuCao;          //	Tính chiều cao vùng làm việc
            Bitmap bm = new Bitmap(ChieuRong, ChieuCao);                //	Tạo ra 1 cái ảnh có kích thước như trên
            for (int x = 0; x < ChieuRong; x += KichThuoc.GridGap)      //	không vẽ dấu khi x,y có tọa độ = 0
            {
                for (int y = 0; y < ChieuCao; y += KichThuoc.GridGap)
                {
                    if (x == 0 || y == 0) { bm.SetPixel(x, y, Color.Black); }
                    else { bm.SetPixel(x, y, Color.Blue); }
                }
            }
            picDraw.SizeMode = PictureBoxSizeMode.AutoSize;
            picDraw.Image = bm;
            picDraw.Refresh();
        }

        private void SnapToGrid(ref int x, ref int y)   //	Snap to the nearest grid point
        {
            x = KichThuoc.GridGap * (int)Math.Round((double)x / KichThuoc.GridGap);
            y = KichThuoc.GridGap * (int)Math.Round((double)y / KichThuoc.GridGap);
        }
        #endregion
        #region Phóng to - thu nhỏ		
        private void tbZoom_ValueChanged(object sender, EventArgs e)
        {
            rtxtThongTin.Text = " Phóng to : " + tbZoom.Value.ToString();   //	 Thông tin khi zoom
            KichThuoc.GridGap = tbZoom.Value;
            MakeBackgroundGrid();
        }
        #endregion
        private void picDraw_MouseMove(object sender, MouseEventArgs e)     //	sự kiện di chuyển chuột
        {
            if (dragging)
            {
                picDraw.Cursor = Cursors.Hand;
                //Point changePoint = new Point(e.Location.X - StartPoint.X,
                //    e.Location.Y - StartPoint.Y);
                //panelPicDraw.AutoScrollPosition = new Point(-panelPicDraw.AutoScrollPosition.X - changePoint.X/2,
                //    -panelPicDraw.AutoScrollPosition.Y - changePoint.Y/2);
                var offsetX = e.X - prevScrollPoint.X;
                var offsetY = e.Y - prevScrollPoint.Y;
                var pointX = -(panelPicDraw.AutoScrollPosition.X);
                var pointY = -(panelPicDraw.AutoScrollPosition.Y);
                pointX += -offsetX;
                pointY += -offsetY;
                panelPicDraw.AutoScrollPosition = new Point(pointX, pointY);
            }
            else if (isdragmove && ListMovePoints != null && ListMovePoints.Count > 0)
            {
                //int a = e.X;
                //int b = e.Y;
                //SnapToGrid(ref a, ref b);
                var offsetX = e.X - startMovePoint.X;
                var offsetY = e.Y - startMovePoint.Y;

                rtxtThongTin.Text = string.Format("startPoint:{0};{1}\n Offset:{2};{3}",
                    startMovePoint.X, startMovePoint.Y, offsetX, offsetY);

                var newList = ListTempMovePoints.Select(movePoint =>
                new Point(movePoint.X + offsetX / KichThuoc.GridGap, movePoint.Y + offsetY / KichThuoc.GridGap)).ToList();

                ListMovePoints.Clear();
                ListMovePoints.AddRange(newList);
                if (enableMovePastedArea)
                {
                    //Line1.AddRange(ListMovePoints);
                }
                else
                {

                    var newLine1 = ListTempLine1.Select(point => ListTempMovePoints.Contains(point) ?
                    new Point(point.X + offsetX / KichThuoc.GridGap, point.Y + offsetY / KichThuoc.GridGap) : point).ToList();

                    Line1.Clear();
                    Line1.AddRange(newLine1);

                }
            }
            else
            {
                int a = e.X;
                int b = e.Y;
                SnapToGrid(ref a, ref b);
                P2 = new Point(a, b);
                TD.TDNow = P2;
                Point ToaDoHienThi = new Point();
                ToaDoHienThi.X = P2.X / KichThuoc.GridGap;
                ToaDoHienThi.Y = P2.Y / KichThuoc.GridGap;
                //	-------------------------------------------------------
                txtToaDo.Text = (ToaDoHienThi.X.ToString() + " : " + ToaDoHienThi.Y.ToString());    //	hiển thị tọa độ đang vẽ
                txtTongLed.Text = (Line1.Count + Line2.Count).ToString();       //	Hiển thị số led tổng
                if (isDrawing)
                {
                    if (Math.Abs(Kick1.X - P2.X / KichThuoc.GridGap) > 0 && Math.Abs(Kick1.Y - P2.Y / KichThuoc.GridGap) > 0)
                    {
                        txtKichThuocVung.Text = string.Format(@"{0} : {1}", (Math.Abs(Kick1.X - P2.X / KichThuoc.GridGap) + 1).ToString(), (Math.Abs(Kick1.Y - P2.Y / KichThuoc.GridGap) + 1).ToString());
                    }
                }
                else { txtKichThuocVung.Text = " -- : -- "; }
            }
            picDraw.Invalidate();
        }

        /// <summary>
        /// Vẽ 1 bóng led
        /// </summary>
        /// <param name="TD">Tọa độ vẽ</param>
        void VeLed(Point TD)    //	tọa độ của điểm / GridGap
        {
            Graphics Ve = picDraw.CreateGraphics();
            Brush FillLed = new SolidBrush(Color.Blue);
            Ve.FillEllipse(FillLed, TD.X * KichThuoc.GridGap - KichThuoc.BanKinhLed, TD.Y * KichThuoc.GridGap - KichThuoc.BanKinhLed, KichThuoc.LedSize, KichThuoc.LedSize);
            Ve.Dispose();
            FillLed.Dispose();
        }

        #region Toàn bộ phần hiển thị trên màn hình PicDraw nằm ở đây
        private void picDraw_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (!isPan)
            {
                e.Graphics.DrawEllipse(Pens.White, P2.X - KichThuoc.BanKinhLed, P2.Y - KichThuoc.BanKinhLed, KichThuoc.LedSize, KichThuoc.LedSize);
            }
            //---------------------------------------
            Pen GrenPen = new Pen(Color.Green, 2);
            Brush FillLed = new SolidBrush(Color.Blue);

            foreach (Point pt in FreeLED)   //	Vẽ led free 
            {
                Rectangle rect = new Rectangle(pt.X * KichThuoc.GridGap - KichThuoc.BanKinhLed, pt.Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
                                                KichThuoc.LedSize, KichThuoc.LedSize);
                e.Graphics.FillEllipse(Brushes.White, rect);
            }
            #region Vẽ led line1,2 và đường nối led
            Pen P = new Pen(Color.Red, 2);
            for (int i = 0; i < Line1.Count - 1; i++)
            {
                Point pt1, pt2 = new Point();
                pt1 = Line1[i];
                pt2 = Line1[i + 1];
                pt1.X *= KichThuoc.GridGap;
                pt1.Y *= KichThuoc.GridGap;
                pt2.X *= KichThuoc.GridGap;
                pt2.Y *= KichThuoc.GridGap;

                e.Graphics.DrawLine(P, pt1, pt2);
            }
            foreach (Point pt in Line1)
            {
                Rectangle rect = new Rectangle(pt.X * KichThuoc.GridGap - KichThuoc.BanKinhLed, pt.Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
                                                KichThuoc.LedSize, KichThuoc.LedSize);
                e.Graphics.FillEllipse(Brushes.Blue, rect);
            }
            if (Line1.Count > 0)        //	Vẽ led đầu màu đỏ
            {
                Rectangle Frist = new Rectangle(Line1[0].X * KichThuoc.GridGap - KichThuoc.BanKinhLed, Line1[0].Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
                                    KichThuoc.LedSize, KichThuoc.LedSize);
                e.Graphics.FillEllipse(Brushes.Green, Frist);
            }
            //-----------------------------------------------------
            //	Vẽ led line2 và đường nối led Line 2
            //			Pen P = new Pen(Color.Red,2);
            for (int i = 0; i < Line2.Count - 1; i++)
            {
                Point pt1, pt2 = new Point();
                pt1 = Line2[i];
                pt2 = Line2[i + 1];
                pt1.X *= KichThuoc.GridGap;
                pt1.Y *= KichThuoc.GridGap;
                pt2.X *= KichThuoc.GridGap;
                pt2.Y *= KichThuoc.GridGap;

                e.Graphics.DrawLine(P, pt1, pt2);
            }
            foreach (Point pt in Line2)
            {
                Rectangle rect = new Rectangle(pt.X * KichThuoc.GridGap - KichThuoc.BanKinhLed, pt.Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
                                                KichThuoc.LedSize, KichThuoc.LedSize);
                e.Graphics.FillEllipse(Brushes.Blue, rect);
            }
            if (Line2.Count > 0)        //	Vẽ led đầu màu đỏ
            {
                Rectangle Frist = new Rectangle(Line2[0].X * KichThuoc.GridGap - KichThuoc.BanKinhLed, Line2[0].Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
                                    KichThuoc.LedSize, KichThuoc.LedSize);
                e.Graphics.FillEllipse(Brushes.Red, Frist);
            }

            #endregion
            //------------------------------------------------
            #region Vẽ đường kẻ đứt khi nối led
            Point DiemCuoiLine = new Point();
            if (CheDoVe == (int)CheDo.NoiLed || CheDoVe == (int)CheDo.VeLed)
            {
                Pen NetDut = new Pen(Color.White);

                if (isDrawing)
                {
                    if (Line1.Count > 0)
                    {
                        DiemCuoiLine = Line1[Line1.Count - 1];
                        DiemCuoiLine.X *= KichThuoc.GridGap;
                        DiemCuoiLine.Y *= KichThuoc.GridGap;
                        NetDut.DashPattern = new float[] { 3f, 3f };
                        e.Graphics.DrawLine(NetDut, DiemCuoiLine, P2);
                    }
                }

            }
            #endregion
            //------------------------------------------------
            #region Vẽ hình chữ nhật biểu thị đang chọn vùng
            else if (CheDoVe == (int)CheDo.ChonVung || CheDoVe == (int)CheDo.XoaVung || CheDoVe == (int)CheDo.ZZNgang
                    || CheDoVe == (int)CheDo.ZZDoc || CheDoVe == (int)CheDo.ZDoc || CheDoVe == (int)CheDo.ZNgang || CheDoVe == (int)CheDo.MoveVung)
            {
                if (isDrawing)
                {
                    e.Graphics.DrawLine(GrenPen, Kick1.X * KichThuoc.GridGap, Kick1.Y * KichThuoc.GridGap, Kick1.X * KichThuoc.GridGap, P2.Y);   //	X
                    e.Graphics.DrawLine(GrenPen, Kick1.X * KichThuoc.GridGap, Kick1.Y * KichThuoc.GridGap, P2.X, Kick1.Y * KichThuoc.GridGap);
                    e.Graphics.DrawLine(GrenPen, P2.X, Kick1.Y * KichThuoc.GridGap, P2.X, P2.Y);
                    e.Graphics.DrawLine(GrenPen, Kick1.X * KichThuoc.GridGap, P2.Y, P2.X, P2.Y);
                }
            }
            #endregion
            //------------------------------------------------
            #region Vẽ line biểu thị khi chọn vẽ vùng
            else if (CheDoVe == (int)CheDo.VeVung)
            {
                if (isDrawing)
                {
                    if (!MP_OK)
                    {
                        int CX, CY, KQ;     //	kiểm tra xem vẽ theo chiều nào
                        CX = P2.X - Kick1.X * KichThuoc.GridGap;
                        CY = P2.Y - Kick1.Y * KichThuoc.GridGap;
                        KQ = Math.Abs(CX) - Math.Abs(CY);
                        if (KQ > 0) //	Theo chiều X
                        {
                            e.Graphics.DrawLine(Pens.Cyan, Kick1.X * KichThuoc.GridGap, Kick1.Y * KichThuoc.GridGap, P2.X, Kick1.Y * KichThuoc.GridGap);
                        }
                        else
                        {
                            e.Graphics.DrawLine(Pens.Cyan, Kick1.X * KichThuoc.GridGap, Kick1.Y * KichThuoc.GridGap, Kick1.X * KichThuoc.GridGap, P2.Y);
                        }
                    }
                    else if (MP_OK)
                    {
                        bool Tren = false;
                        #region Theo chiều X
                        if (MP.Y == Kick1.Y)       //	Theo chiều X
                        {
                            if (P2.Y / KichThuoc.GridGap > MP.Y)
                            {
                                for (int i = 0; i <= P2.Y / KichThuoc.GridGap - MP.Y; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y + i) * KichThuoc.GridGap,
                                                                    (MP.X) * KichThuoc.GridGap, (MP.Y + i) * KichThuoc.GridGap);
                                }
                                for (int i = 0; i < P2.Y / KichThuoc.GridGap - MP.Y; i++)
                                {
                                    Tren = !Tren;
                                    if (Tren)
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (MP.X) * KichThuoc.GridGap, (MP.Y + i) * KichThuoc.GridGap,
                                                                        (MP.X) * KichThuoc.GridGap, (MP.Y + i + 1) * KichThuoc.GridGap);
                                    }
                                    else
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y + i) * KichThuoc.GridGap,
                                                                        (Kick1.X) * KichThuoc.GridGap, (Kick1.Y + i + 1) * KichThuoc.GridGap);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i <= MP.Y - P2.Y / KichThuoc.GridGap; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y - i) * KichThuoc.GridGap,
                                                                    (MP.X) * KichThuoc.GridGap, (MP.Y - i) * KichThuoc.GridGap);
                                }
                                for (int i = 0; i < MP.Y - P2.Y / KichThuoc.GridGap; i++)
                                {
                                    Tren = !Tren;
                                    if (Tren)
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (MP.X) * KichThuoc.GridGap, (MP.Y - i) * KichThuoc.GridGap,
                                                                        (MP.X) * KichThuoc.GridGap, (MP.Y - i - 1) * KichThuoc.GridGap);
                                    }
                                    else
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y - i) * KichThuoc.GridGap,
                                                                        (Kick1.X) * KichThuoc.GridGap, (Kick1.Y - i - 1) * KichThuoc.GridGap);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Theo chiều Y
                        else if (MP.X == Kick1.X)  //	Theo chiều Y
                        {
                            if (P2.X / KichThuoc.GridGap > MP.X)
                            {
                                for (int i = 0; i <= P2.X / KichThuoc.GridGap - MP.X; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X + i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                                                    (MP.X + i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                }
                                Tren = true;
                                for (int i = 0; i < P2.X / KichThuoc.GridGap - MP.X; i++)
                                {
                                    Tren = !Tren;
                                    if (Tren)
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (Kick1.X + i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                                                        (Kick1.X + i + 1) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap);
                                    }
                                    else
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (MP.X + i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap,
                                                                        (MP.X + i + 1) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i <= MP.X - P2.X / KichThuoc.GridGap; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X - i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                                                    (MP.X - i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                }
                                Tren = true;
                                for (int i = 0; i < MP.X - P2.X / KichThuoc.GridGap; i++)
                                {
                                    Tren = !Tren;
                                    if (Tren)
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (Kick1.X - i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                                                        (Kick1.X - i - 1) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap);
                                    }
                                    else
                                    {
                                        e.Graphics.DrawLine(Pens.Cyan, (MP.X - i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap,
                                                                        (MP.X - i - 1) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion
            //------------------------------------------------
            #region Vẽ Vùng Z
            else if (CheDoVe == (int)CheDo.VeVungZ)
            {
                if (isDrawing)
                {
                    if (!MP_OK)
                    {
                        int CX, CY, KQ;     //	kiểm tra xem vẽ theo chiều nào
                        CX = P2.X - Kick1.X * KichThuoc.GridGap;
                        CY = P2.Y - Kick1.Y * KichThuoc.GridGap;
                        KQ = Math.Abs(CX) - Math.Abs(CY);
                        if (KQ > 0) //	Theo chiều X
                        {
                            e.Graphics.DrawLine(Pens.Cyan, Kick1.X * KichThuoc.GridGap, Kick1.Y * KichThuoc.GridGap, P2.X, Kick1.Y * KichThuoc.GridGap);
                        }
                        else
                        {
                            e.Graphics.DrawLine(Pens.Cyan, Kick1.X * KichThuoc.GridGap, Kick1.Y * KichThuoc.GridGap, Kick1.X * KichThuoc.GridGap, P2.Y);
                        }
                    }
                    else if (MP_OK)
                    {
                        //						bool Tren = false;
                        #region Theo chiều X
                        if (MP.Y == Kick1.Y)       //	Theo chiều X
                        {
                            if (P2.Y / KichThuoc.GridGap > MP.Y)
                            {
                                for (int i = 0; i <= P2.Y / KichThuoc.GridGap - MP.Y; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y + i) * KichThuoc.GridGap,
                                                                    (MP.X) * KichThuoc.GridGap, (MP.Y + i) * KichThuoc.GridGap);
                                }
                                //for (int i = 0; i < P2.Y / KichThuoc.GridGap - MP.Y; i++)
                                //{
                                //	Tren = !Tren;
                                //	if (Tren)
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (MP.X) * KichThuoc.GridGap, (MP.Y+i) * KichThuoc.GridGap,
                                //										(MP.X) * KichThuoc.GridGap, (MP.Y+i+1) * KichThuoc.GridGap);
                                //	}
                                //	else
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y+i) * KichThuoc.GridGap,
                                //										(Kick1.X) * KichThuoc.GridGap, (Kick1.Y+i+1) * KichThuoc.GridGap);
                                //	}
                                //}
                            }
                            else
                            {
                                for (int i = 0; i <= MP.Y - P2.Y / KichThuoc.GridGap; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y - i) * KichThuoc.GridGap,
                                                                    (MP.X) * KichThuoc.GridGap, (MP.Y - i) * KichThuoc.GridGap);
                                }
                                //for (int i = 0; i < MP.Y - P2.Y / KichThuoc.GridGap; i++)
                                //{
                                //	Tren = !Tren;
                                //	if (Tren)
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (MP.X) * KichThuoc.GridGap, (MP.Y-i) * KichThuoc.GridGap,
                                //										(MP.X) * KichThuoc.GridGap, (MP.Y-i-1) * KichThuoc.GridGap);
                                //	}
                                //	else
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (Kick1.X) * KichThuoc.GridGap, (Kick1.Y-i) * KichThuoc.GridGap,
                                //										(Kick1.X) * KichThuoc.GridGap, (Kick1.Y-i-1) * KichThuoc.GridGap);
                                //	}
                                //}								
                            }
                        }
                        #endregion
                        #region Theo chiều Y
                        else if (MP.X == Kick1.X)  //	Theo chiều Y
                        {
                            if (P2.X / KichThuoc.GridGap > MP.X)
                            {
                                for (int i = 0; i <= P2.X / KichThuoc.GridGap - MP.X; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X + i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                                                    (MP.X + i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                }
                                //Tren = true;
                                //for (int i = 0; i < P2.X / KichThuoc.GridGap - MP.X; i++)
                                //{
                                //	Tren = !Tren;
                                //	if (Tren)
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (Kick1.X+i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                //										(Kick1.X+i+1) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap);
                                //	}
                                //	else
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (MP.X+i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap,
                                //										(MP.X+i+1) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                //	}
                                //}
                            }
                            else
                            {
                                for (int i = 0; i <= MP.X - P2.X / KichThuoc.GridGap; i++)
                                {
                                    e.Graphics.DrawLine(Pens.Cyan, (Kick1.X - i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                                                    (MP.X - i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                }
                                //Tren = true;
                                //for (int i = 0; i < MP.X - P2.X / KichThuoc.GridGap; i++)
                                //{
                                //	Tren = !Tren;
                                //	if (Tren)
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (Kick1.X-i) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap,
                                //										(Kick1.X-i-1) * KichThuoc.GridGap, (Kick1.Y) * KichThuoc.GridGap);
                                //	}
                                //	else
                                //	{
                                //		e.Graphics.DrawLine(Pens.Cyan, (MP.X-i) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap,
                                //										(MP.X-i-1) * KichThuoc.GridGap, (MP.Y) * KichThuoc.GridGap);
                                //	}
                                //}
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region Vẽ vùng chọn để move

            if (ListMovePoints != null && ListMovePoints.Count > 0)
            {
                foreach (Point pt in ListMovePoints)
                {
                    Rectangle rect = new Rectangle(pt.X * KichThuoc.GridGap - KichThuoc.BanKinhLed, pt.Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
                                                    KichThuoc.LedSize, KichThuoc.LedSize);
                    e.Graphics.FillEllipse(Brushes.LightSeaGreen, rect);
                }
            }
            #endregion

            #region Ve vung chon de copy
            if (ListSelectPoints != null && ListSelectPoints.Count > 0)
            {
                foreach (Point pt in ListSelectPoints)
                {
                    Rectangle rect = new Rectangle(pt.X * KichThuoc.GridGap - KichThuoc.BanKinhLed, pt.Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
                                                    KichThuoc.LedSize, KichThuoc.LedSize);
                    e.Graphics.FillEllipse(Brushes.LightSeaGreen, rect);
                }
            }
            //if (ListPasted != null && ListPasted.Count > 0)
            //{
            //    foreach (Point pt in ListPasted)
            //    {
            //        Rectangle rect = new Rectangle(pt.X * KichThuoc.GridGap - KichThuoc.BanKinhLed, pt.Y * KichThuoc.GridGap - KichThuoc.BanKinhLed,
            //                                        KichThuoc.LedSize, KichThuoc.LedSize);
            //        e.Graphics.FillEllipse(Brushes.LightGreen, rect);
            //    }
            //}
            #endregion
            FillLed.Dispose();
            GrenPen.Dispose();
        }
        #endregion 
        #region Kiểm tra xem led ở line nào ?		
        private bool LedInLine1(Point pt)
        {
            foreach (Point item in Line1)
            {
                if (item == pt)
                {
                    return true;
                }
            }
            return false;
        }
        private bool LedInLine2(Point pt)
        {
            foreach (Point item in Line2)
            {
                if (item == pt)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        //	Sự kiện kick chuột
        private void picDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (isPan || e.Button == MouseButtons.Middle)
            {
                prevScrollPoint = e.Location;
                StartPoint = e.Location;
                dragging = true;
            }
            if (e.Button == MouseButtons.Right)     //	Nếu là kick chuột phải thì thoát các chế độ
            {
                if (CheDoVe == (int)CheDo.MoveVung)
                {
                    if (isdragmove)
                    {
                        isMove = false;
                        isdragmove = false;
                        ListMovePoints.Clear();
                    }
                    else
                    {
                        isMove = true;
                        if (ListMovePoints.Count > 0)
                        {
                            ListTempMovePoints.AddRange(ListMovePoints);
                            ListTempLine1.AddRange(Line1);
                        }
                    }
                    CheDoVe = (int)CheDo.Ranh;
                }
                else if (isMove)
                {
                    isMove = false;
                    isdragmove = false;



                    var newLine1 = new List<Point>();
                    foreach (var point in ListTempLine1)
                    {
                        if (!ListTempMovePoints.Contains(point) && !ListMovePoints.Contains(point))
                        {
                            newLine1.Add(point);
                        }
                        else if (ListTempMovePoints.Contains(point))
                        {
                            var idx = ListTempMovePoints.IndexOf(point);
                            newLine1.Add(ListMovePoints.ElementAt(idx));
                        }
                    }
                    //var newLine1 = Line1.Except(ListMovePoints).ToList();
                    //newLine1.AddRange(ListMovePoints);

                    //var newline = newLine1.OrderBy(m => m.Y).ThenBy(m => m.X);
                    if (enableMovePastedArea)
                    {
                        Line1.AddRange(ListMovePoints);
                    }
                    else
                    {
                        Line1.Clear();
                        Line1.AddRange(newLine1);
                    }

                    UndoStack.Push(new UndoRedoModel()
                    {
                        FreeLed = new List<Point>(FreeLED),
                        Line1 = new List<Point>(Line1),
                        Line2 = new List<Point>(Line2)
                    });
                    RedoStack.Clear();
                    ListMovePoints.Clear();
                    ListTempMovePoints.Clear();
                    ListTempLine1.Clear();
                }
                else
                {
                    CheDoVe = (int)CheDo.Ranh;
                    isDrawing = false;
                    //	Cái này dành cho chế độ chèn LED
                    EnableButton();
                    foreach (Point item in Line2)
                    {
                        Line1.Add(item);
                    }
                    Line2.Clear();
                    MP_OK = false;
                }

            }
            else if (e.Button == MouseButtons.Left)
            {
                if (isMove)
                {
                    isdragmove = true;
                    //int a = e.X;
                    //int b = e.Y;
                    //SnapToGrid(ref a, ref b);
                    startMovePoint = e.Location;
                    picDraw.Cursor = Cursors.SizeAll;
                }
                //	Tính vị trí điểm
                Point NewP1 = new Point();
                float x, y;     //	Lấy giá trị nhảy lưới	
                x = e.Location.X % KichThuoc.GridGap;
                y = e.Location.Y % KichThuoc.GridGap;
                if (x > KichThuoc.GridGap / 2) { NewP1.X = e.Location.X / KichThuoc.GridGap + 1; }  //	căn lại tọa độ trỏ chuột.
                else { NewP1.X = e.Location.X / KichThuoc.GridGap; }                                //	khi ở giữa 2 điểm grid

                if (y > KichThuoc.GridGap / 2) { NewP1.Y = e.Location.Y / KichThuoc.GridGap + 1; }  //	nó sẽ chọn grid gần nhất để vẽ
                else { NewP1.Y = e.Location.Y / KichThuoc.GridGap; }

                TD.VuaKick = NewP1;
                StartPoint = NewP1;
                if (NewP1.X > 0 && NewP1.Y > 0)     //	thêm led ở chỗ này
                {
                    switch (CheDoVe)
                    {
                        #region Rảnh						
                        case (int)CheDo.Ranh:
                            break;
                        #endregion

                        #region Thêm FreeLED
                        case (int)CheDo.ThemLed:    //	Thêm led đơn
                            if (!LedInFreeLED(NewP1))
                            {
                                FreeLED.Add(NewP1);
                                UndoStack.Push(new UndoRedoModel()
                                {
                                    FreeLed = new List<Point>(FreeLED),
                                    Line1 = new List<Point>(Line1),
                                    Line2 = new List<Point>(Line2)
                                });
                            }
                            break;
                        #endregion

                        #region Xóa LED						
                        case (int)CheDo.XoaLed:
                            if (LedInFreeLED(NewP1))
                            {
                                FreeLED.Remove(NewP1);
                            }
                            if (LedInLine1(NewP1))
                            {
                                Line1.Remove(NewP1);
                            }
                            UndoStack.Push(new UndoRedoModel()
                            {
                                FreeLed = new List<Point>(FreeLED),
                                Line1 = new List<Point>(Line1),
                                Line2 = new List<Point>(Line2)
                            });
                            break;
                        #endregion

                        #region Nối led
                        case (int)CheDo.NoiLed:
                            int ViTriLED = 0;   //	vị trí của led nằm trong list 1
                            if (!isDrawing)  //	nếu là chế độ vẽ 
                            {
                                isDrawing = true;
                                Line2.Clear();
                                //	Xác định điểm vừa kick ở vị trí nào trong line 1
                                for (int i = 0; i < Line1.Count; i++)
                                {
                                    if (Line1[i] == TD.VuaKick)
                                    {
                                        ViTriLED = i;
                                        break;
                                    }
                                }

                                int DaiLine1 = Line1.Count;
                                for (int i = ViTriLED + 1; i < DaiLine1; i++)   //	Copy Toàn bộ sang Line2
                                {
                                    Line2.Add(Line1[ViTriLED + 1]);
                                    Line1.Remove(Line1[ViTriLED + 1]);
                                }

                            }
                            else
                            {
                                if (!LedInLine1(TD.VuaKick) && !LedInLine2(TD.VuaKick))
                                {
                                    Line1.Add(TD.VuaKick);
                                }
                            }
                            UndoStack.Push(new UndoRedoModel()
                            {
                                FreeLed = new List<Point>(FreeLED),
                                Line1 = new List<Point>(Line1),
                                Line2 = new List<Point>(Line2)
                            });
                            break;
                        #endregion

                        #region Vẽ Led						
                        case (int)CheDo.VeLed:
                            if (isDrawing)  //	nếu là chế độ vẽ 
                            {
                                Kick1 = NewP1;
                                TD.VuaKick = NewP1;
                                //--------------------------------------------------------------
                                if (LedInFreeLED(NewP1))
                                {
                                    Line1.Add(NewP1);
                                    FreeLED.Remove(NewP1);
                                }
                                else
                                {
                                    #region Vẽ nhanh khi ấn phím Ctrl + Kick
                                    if (Control.ModifierKeys == Keys.Control)   //	vẽ nhanh với phím Control
                                    {
                                        Point Moi = new Point();

                                        if (Line1[Line1.Count - 1].Y == NewP1.Y)    //	Nếu là chiều ngang
                                        {
                                            if (Line1[Line1.Count - 1].X > NewP1.X) //	Lùi
                                            {
                                                int X2 = Line1[Line1.Count - 1].X;
                                                for (int i = 1; i <= X2 - NewP1.X; i++)
                                                {
                                                    Moi.X = X2 - i;
                                                    Moi.Y = NewP1.Y;
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else if (Line1[Line1.Count - 1].X < NewP1.X)    //	Tiến
                                            {
                                                int X2 = Line1[Line1.Count - 1].X;
                                                for (int i = 1; i <= NewP1.X - X2; i++)
                                                {
                                                    Moi.X = X2 + i;
                                                    Moi.Y = NewP1.Y;
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                        else if (Line1[Line1.Count - 1].X == NewP1.X)   //	Nếu là chiều Dọc
                                        {
                                            if (Line1[Line1.Count - 1].Y > NewP1.Y) //	Lùi
                                            {
                                                int Y2 = Line1[Line1.Count - 1].Y;
                                                for (int i = 1; i <= Y2 - NewP1.Y; i++)
                                                {
                                                    Moi.Y = Y2 - i;
                                                    Moi.X = NewP1.X;
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else if (Line1[Line1.Count - 1].Y < NewP1.Y)    //	Tiến
                                            {
                                                int Y2 = Line1[Line1.Count - 1].Y;
                                                for (int i = 1; i <= NewP1.Y - Y2; i++)
                                                {
                                                    Moi.Y = Y2 + i;
                                                    Moi.X = NewP1.X;
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                        else    //	Vẽ led theo đường chéo
                                        {
                                            if (true)
                                            {

                                            }
                                        }
                                    }
                                    #endregion
                                    else
                                    {
                                        if (!LedInLine1(NewP1))
                                        {
                                            Line1.Add(NewP1);
                                        }
                                    }
                                }

                            }
                            else    //	Not Drawing
                            {
                                Line1.Add(NewP1);
                                isDrawing = true;
                            }
                            UndoStack.Push(new UndoRedoModel()
                            {
                                FreeLed = new List<Point>(FreeLED),
                                Line1 = new List<Point>(Line1),
                                Line2 = new List<Point>(Line2)
                            });
                            break;
                        #endregion

                        #region Xóa vùng
                        case (int)CheDo.XoaVung:
                            if (!isDrawing)
                            {
                                Kick1 = NewP1;     //	Lấy tọa độ điểm đầu
                                isDrawing = true;
                            }
                            else
                            {
                                Kick2 = NewP1;     //	Lấy tọa độ điểm sau
                                isDrawing = false;
                                //	Lọc tất cả các led có tọa độ nằm trong khoảng đã chọn . điểm nào trùng thì xóa
                                if (Kick2.X > Kick1.X && Kick2.Y > Kick1.Y)
                                {
                                    for (int xx = Kick1.X; xx <= Kick2.X; xx++)
                                    {
                                        for (int yy = Kick1.Y; yy <= Kick2.Y; yy++)
                                        {
                                            Point DiemCheck = new Point(xx, yy);
                                            if (LedInLine1(DiemCheck))
                                            {
                                                Line1.Remove(DiemCheck);
                                            }
                                            else if (LedInLine2(DiemCheck))
                                            {
                                                Line2.Remove(DiemCheck);
                                            }
                                        }
                                    }
                                }
                                else if (Kick2.X < Kick1.X && Kick2.Y > Kick1.Y)
                                {
                                    for (int xx = Kick2.X; xx <= Kick1.X; xx++)
                                    {
                                        for (int yy = Kick1.Y; yy <= Kick2.Y; yy++)
                                        {
                                            Point DiemCheck = new Point(xx, yy);
                                            if (LedInLine1(DiemCheck))
                                            {
                                                Line1.Remove(DiemCheck);
                                            }
                                            else if (LedInLine2(DiemCheck))
                                            {
                                                Line2.Remove(DiemCheck);
                                            }
                                        }
                                    }
                                }
                                else if (Kick2.X > Kick1.X && Kick2.Y < Kick1.Y)
                                {
                                    for (int xx = Kick1.X; xx <= Kick2.X; xx++)
                                    {
                                        for (int yy = Kick2.Y; yy <= Kick1.Y; yy++)
                                        {
                                            Point DiemCheck = new Point(xx, yy);
                                            if (LedInLine1(DiemCheck))
                                            {
                                                Line1.Remove(DiemCheck);
                                            }
                                            else if (LedInLine2(DiemCheck))
                                            {
                                                Line2.Remove(DiemCheck);
                                            }
                                        }
                                    }
                                }
                                else if (Kick2.X < Kick1.X && Kick2.Y < Kick1.Y)
                                {
                                    for (int xx = Kick2.X; xx <= Kick1.X; xx++)
                                    {
                                        for (int yy = Kick2.Y; yy <= Kick1.Y; yy++)
                                        {
                                            Point DiemCheck = new Point(xx, yy);
                                            if (LedInLine1(DiemCheck))
                                            {
                                                Line1.Remove(DiemCheck);
                                            }
                                            else if (LedInLine2(DiemCheck))
                                            {
                                                Line2.Remove(DiemCheck);
                                            }
                                        }
                                    }
                                }
                                UndoStack.Push(new UndoRedoModel()
                                {
                                    FreeLed = new List<Point>(FreeLED),
                                    Line1 = new List<Point>(Line1),
                                    Line2 = new List<Point>(Line2)
                                });
                            }

                            break;
                        #endregion

                        #region Ziczac Ngang
                        case (int)CheDo.ZZNgang:    //	dây đi theo chiều ngang

                            if (!isDrawing)
                            {
                                Kick1 = NewP1;     //	Lấy tọa độ điểm đầu
                                isDrawing = true;
                            }
                            else
                            {
                                bool Xuoi = false;
                                isDrawing = false;
                                Kick2 = NewP1;
                                if (Kick2.X - Kick1.X > 0)          //	------->>>
                                {
                                    #region Chạy xuống
                                    if (Kick2.Y - Kick1.Y > 0)      //	↓
                                    {
                                        for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int xx = Kick2.X - Kick1.X; xx >= 0; xx--)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Chạy lên
                                    else        //	↑
                                    {
                                        for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int xx = Kick2.X - Kick1.X; xx >= 0; xx--)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else                            //	<<<-------
                                {
                                    #region Chạy xuống
                                    if (Kick2.Y - Kick1.Y > 0)      //	↓
                                    {
                                        for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int xx = Kick1.X - Kick2.X; xx >= 0; xx--)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Chạy lên
                                    else        //	↑
                                    {
                                        for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int xx = Kick1.X - Kick2.X; xx >= 0; xx--)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                }
                                UndoStack.Push(new UndoRedoModel()
                                {
                                    FreeLed = new List<Point>(FreeLED),
                                    Line1 = new List<Point>(Line1),
                                    Line2 = new List<Point>(Line2)
                                });
                            }
                            break;
                        #endregion

                        #region Ziczac Dọc
                        case (int)CheDo.ZZDoc:
                            if (!isDrawing)
                            {
                                Kick1 = NewP1;     //	Lấy tọa độ điểm đầu
                                isDrawing = true;
                            }
                            else
                            {
                                bool Xuoi = false;
                                isDrawing = false;
                                Kick2 = NewP1;
                                #region	Xuống	
                                if (Kick2.Y - Kick1.Y > 0)          //	|
                                {                                   //	V
                                    if (Kick2.X - Kick1.X > 0)
                                    {
                                        for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int yy = Kick2.Y - Kick1.Y; yy >= 0; yy--)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    else //	(Kick2.X - Kick1.X < 0)
                                    {
                                        for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int yy = Kick2.Y - Kick1.Y; yy >= 0; yy--)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                                #endregion
                                #region Lên
                                else
                                {
                                    if (Kick2.X - Kick1.X > 0)
                                    {
                                        for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int yy = Kick1.Y - Kick2.Y; yy >= 0; yy--)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    else //	(Kick2.X - Kick1.X < 0)
                                    {
                                        for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                        {
                                            Xuoi = !Xuoi;
                                            if (Xuoi)
                                            {
                                                for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int yy = Kick1.Y - Kick2.Y; yy >= 0; yy--)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                    if (!LedInLine1(Moi))
                                                    {
                                                        Line1.Add(Moi);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                                #endregion
                                UndoStack.Push(new UndoRedoModel()
                                {
                                    FreeLed = new List<Point>(FreeLED),
                                    Line1 = new List<Point>(Line1),
                                    Line2 = new List<Point>(Line2)
                                });
                            }
                            break;
                        #endregion

                        #region Z Dọc
                        case (int)CheDo.ZDoc:
                            if (!isDrawing)
                            {
                                Kick1 = NewP1;     //	Lấy tọa độ điểm đầu
                                isDrawing = true;
                            }
                            else
                            {
                                isDrawing = false;
                                Kick2 = NewP1;
                                #region	Xuống
                                if (Kick2.Y - Kick1.Y > 0)          //	|
                                {                                   //	V
                                    if (Kick2.X - Kick1.X > 0)
                                    {
                                        for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                        {

                                            for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                    else //	(Kick2.X - Kick1.X < 0)
                                    {
                                        for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                        {
                                            for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Lên
                                else
                                {
                                    if (Kick2.X - Kick1.X > 0)
                                    {
                                        for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                        {
                                            for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }

                                        }
                                    }
                                    else //	(Kick2.X - Kick1.X < 0)
                                    {
                                        for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                        {
                                            for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                UndoStack.Push(new UndoRedoModel()
                                {
                                    FreeLed = new List<Point>(FreeLED),
                                    Line1 = new List<Point>(Line1),
                                    Line2 = new List<Point>(Line2)
                                });
                            }
                            break;
                        #endregion

                        #region Z Ngang
                        case (int)CheDo.ZNgang: //	dây đi theo chiều ngang

                            if (!isDrawing)
                            {
                                Kick1 = NewP1;     //	Lấy tọa độ điểm đầu
                                isDrawing = true;
                            }
                            else
                            {
                                isDrawing = false;
                                Kick2 = NewP1;
                                if (Kick2.X - Kick1.X > 0)          //	------->>>
                                {
                                    #region Chạy xuống
                                    if (Kick2.Y - Kick1.Y > 0)      //	↓
                                    {
                                        for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                        {
                                            for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                            {
                                                Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Chạy lên
                                    else        //	↑
                                    {
                                        for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                        {
                                            for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                            {
                                                Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else                            //	<<<-------
                                {
                                    #region Chạy xuống
                                    if (Kick2.Y - Kick1.Y > 0)      //	↓
                                    {
                                        for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                        {
                                            for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                            {
                                                Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Chạy lên
                                    else        //	↑
                                    {
                                        for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                        {
                                            for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                            {
                                                Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                if (!LedInLine1(Moi))
                                                {
                                                    Line1.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                }
                                UndoStack.Push(new UndoRedoModel()
                                {
                                    FreeLed = new List<Point>(FreeLED),
                                    Line1 = new List<Point>(Line1),
                                    Line2 = new List<Point>(Line2)
                                });
                            }
                            break;
                        #endregion

                        #region Di Chuyen Vung
                        case (int)CheDo.MoveVung:
                            if (!isDrawing)
                            {
                                Kick1 = NewP1;     //	Lấy tọa độ điểm đầu
                                isDrawing = true;
                            }
                            else
                            {
                                if (!ModifierKeys.HasFlag(Keys.Control))
                                {
                                    ListMovePoints = new List<Point>();
                                }
                                isDrawing = false;
                                Kick2 = NewP1;
                                #region	Xuống
                                if (Kick2.Y - Kick1.Y > 0)          //	|
                                {                                   //	V
                                    if (Kick2.X - Kick1.X > 0)
                                    {
                                        for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                        {

                                            for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                if (LedInLine1(Moi))
                                                {
                                                    ListMovePoints.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                    else //	(Kick2.X - Kick1.X < 0)
                                    {
                                        for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                        {
                                            for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                if (LedInLine1(Moi))
                                                {
                                                    ListMovePoints.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Lên
                                else
                                {
                                    if (Kick2.X - Kick1.X > 0)
                                    {
                                        for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                        {
                                            for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                if (LedInLine1(Moi))
                                                {
                                                    ListMovePoints.Add(Moi);
                                                }
                                            }

                                        }
                                    }
                                    else //	(Kick2.X - Kick1.X < 0)
                                    {
                                        for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                        {
                                            for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                            {
                                                Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                if (LedInLine1(Moi))
                                                {
                                                    ListMovePoints.Add(Moi);
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            break;
                        #endregion

                        #region Chon vung
                        case (int)CheDo.ChonVung:
                            {
                                if (!isDrawing)
                                {
                                    Kick1 = NewP1;     //	Lấy tọa độ điểm đầu
                                    isDrawing = true;
                                }
                                else
                                {
                                    if (!ModifierKeys.HasFlag(Keys.Control))
                                    {
                                        ListSelectPoints = new List<Point>();
                                    }
                                    isDrawing = false;
                                    Kick2 = NewP1;
                                    #region	Xuống
                                    if (Kick2.Y - Kick1.Y > 0)          //	|
                                    {                                   //	V
                                        if (Kick2.X - Kick1.X > 0)
                                        {
                                            for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                            {

                                                for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y + yy);
                                                    if (LedInLine1(Moi))
                                                    {
                                                        ListSelectPoints.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                        else //	(Kick2.X - Kick1.X < 0)
                                        {
                                            for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                            {
                                                for (int yy = 0; yy <= Kick2.Y - Kick1.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y + yy);
                                                    if (LedInLine1(Moi))
                                                    {
                                                        ListSelectPoints.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Lên
                                    else
                                    {
                                        if (Kick2.X - Kick1.X > 0)
                                        {
                                            for (int xx = 0; xx <= Kick2.X - Kick1.X; xx++)
                                            {
                                                for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X + xx, Kick1.Y - yy);
                                                    if (LedInLine1(Moi))
                                                    {
                                                        ListSelectPoints.Add(Moi);
                                                    }
                                                }

                                            }
                                        }
                                        else //	(Kick2.X - Kick1.X < 0)
                                        {
                                            for (int xx = 0; xx <= Kick1.X - Kick2.X; xx++)
                                            {
                                                for (int yy = 0; yy <= Kick1.Y - Kick2.Y; yy++)
                                                {
                                                    Point Moi = new Point(Kick1.X - xx, Kick1.Y - yy);
                                                    if (LedInLine1(Moi))
                                                    {
                                                        ListSelectPoints.Add(Moi);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    //var lstSelect = new List<Point>();
                                    //for (int i = 0; i < Line1.Count; i++)
                                    //{
                                    //    if (ListSelectPoints.Contains(Line1[i]))
                                    //    {
                                    //        lstSelect.Add(Line1[i]);
                                    //    }
                                    //}
                                    //ListSelectPoints.Clear();
                                    //ListSelectPoints.AddRange(lstSelect);
                                }
                                break;
                            }
                            #endregion
                    }
                    RedoStack.Clear();
                }

            }
        }

        /// <summary>
        /// kiểm tra xem led vừa được thêm vào có nằm trong list đã có hay không ( mảng FreeLed )
        /// </summary>
        /// <param name="pt">Tọa độ kiểm tra</param>
        /// <returns>Kết quả</returns>
        private bool LedInFreeLED(Point pt) //	đè led. Led mới đè led cũ ( cái này chỉ tính ở mảng FreeLed )
        {
            foreach (Point item in FreeLED)
            {
                if (item == pt)
                {
                    return true;
                }
            }
            return false;
        }

        #region Xử lý các nút ấn ở đây
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnVe_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.NoiLed;
        }
        private void btnThemLed_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.ThemLed;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Thêm led trống. \n----------------------------\nLED không được nối vào đâu cả, chỉ có tác dụng căn trong việc vẽ";
        }
        private void btnXoaLed_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.XoaLed;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Xóa led. \n ----------------------------\nChọn LED cần xóa và Kick chuột trái để xóa LED";
        }
        #region Cái này dành cho nút CHÈN LED
        void DisableButton()
        {
            btnClear.Enabled = false;
            btnDaoChieu.Enabled = false;
            btnOkVung.Enabled = false;
            btnOpen.Enabled = false;
            btnSave.Enabled = false;
            btnThemLed.Enabled = false;
            btnVeLed.Enabled = false;
            btnXoaLed.Enabled = false;
            btnChen.Enabled = false;
            btnZDoc.Enabled = false;
            btnZNgang.Enabled = false;
            btnZZDoc.Enabled = false;
            btnZZNgang.Enabled = false;
            btnXoaVung.Enabled = false;     //				
        }

        void EnableButton()
        {
            btnClear.Enabled = true;
            btnDaoChieu.Enabled = true;
            btnOkVung.Enabled = true;
            btnOpen.Enabled = true;
            btnSave.Enabled = true;
            btnThemLed.Enabled = true;
            btnVeLed.Enabled = true;
            btnXoaLed.Enabled = true;
            btnChen.Enabled = true;
            btnZDoc.Enabled = true;
            btnZNgang.Enabled = true;
            btnZZDoc.Enabled = true;
            btnZZNgang.Enabled = true;
            btnXoaVung.Enabled = true;      //				
        }


        #endregion
        private void btnNoiLed_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.NoiLed;
            DisableButton();

            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Chèn LED.\n ----------------------------\nKick chọn vị trí muốn chèn, sau khi thực hiện chèn LED xong. kick chuột phải để trở lại bình thường";
        }
        private void btnVeLed_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.VeLed;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Vẽ LED thủ công.\n ----------------------------\nKick chuột trái để thêm LED\n Vẽ nhanh theo đường thẳng bằng cách ấn giữ phím Ctrl + kick";
        }
        private void btnXoaVung_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.XoaVung;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Xóa vùng.\n ----------------------------\nKick chọn 2 điểm để xóa LED. Tất cả các LED nằm trong vùng được chọn sẽ bị xóa";
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            isPan = false;
            DialogResult TraLoi;
            TraLoi = MessageBox.Show("Tùy chọn sẽ xóa toàn bộ LED.  \nChắc chán xóa ?", "Thông báo", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (TraLoi == DialogResult.Yes)
            {
                FreeLED.Clear();
                Line1.Clear();
                Line2.Clear();
                picDraw.Invalidate();
                CheDoVe = (int)CheDo.XoaVung;
                rtxtThongTin.Clear();
                rtxtThongTin.Text = "Xóa toàn bộ.\n ";
            }
        }
        private void btnZiczacNgang_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.ZZNgang;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Vẽ đường Ziczac ngang.\n ----------------------------\nKick chọn 2 điểm để vẽ tự động";
        }
        private void btnZZDoc_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.ZZDoc;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Vẽ đường Ziczac dọc.\n ----------------------------\nKick chọn 2 điểm để vẽ tự động";
        }
        private void btnZDoc_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.ZDoc;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Vẽ đường chữ Z dọc.\n ----------------------------\nKick chọn 2 điểm để vẽ tự động";
        }
        private void btnZNgang_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.ZNgang;
            rtxtThongTin.Clear();
            rtxtThongTin.Text = "Vẽ đường chữ Z ngang.\n ----------------------------\nKick chọn 2 điểm để vẽ tự động";
        }

        protected bool SaveDataToFile(string filename, byte[] Data, int length)
        {
            BinaryWriter writer = null;
            try
            {
                writer = new BinaryWriter(File.OpenWrite(filename));
                writer.Write(Data, 0, length);
                writer.Flush();
                writer.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            isPan = false;
            string TenFile;
            saveFile.Filter = "File cấu hình đi dây (.map) | *.map";
            if (saveFile.ShowDialog() == DialogResult.OK)       //	
            {
                TenFile = saveFile.FileName;
                int DataSize = Line1.Count * 4 + Line2.Count * 4 + FreeLED.Count * 4 + 20;  //	1 point có 4 byte ( 2byte x, 2byte y )
                byte[] Data = new byte[DataSize];

                #region Lấy thuộc tính vùng làm việc		
                Data[0] = (byte)(KTLamViec.ChieuRong / 256);
                Data[1] = (byte)(KTLamViec.ChieuRong % 256);
                Data[2] = (byte)(KTLamViec.ChieuCao / 256);
                Data[3] = (byte)(KTLamViec.ChieuCao % 256);
                Data[4] = (byte)(Line1.Count / 256);
                Data[5] = (byte)(Line1.Count % 256);
                Data[6] = (byte)(Line2.Count / 256);
                Data[7] = (byte)(Line2.Count % 256);
                Data[8] = (byte)(FreeLED.Count / 256);
                Data[9] = (byte)(FreeLED.Count % 256);  //	FreeLed
                #endregion
                //-------------------------------------------------------------
                #region Line 1			
                int Dai1 = Line1.Count;
                byte[] buf1 = new byte[Dai1 * 4];

                for (int i = 0; i < Dai1; i++)
                {
                    int H = Line1[i].X / 256;
                    int L = Line1[i].X % 256;
                    buf1[i * 4] = (byte)H;
                    buf1[i * 4 + 1] = (byte)L;

                    H = Line1[i].Y / 256;
                    L = Line1[i].Y % 256;
                    buf1[i * 4 + 2] = (byte)H;
                    buf1[i * 4 + 3] = (byte)L;
                }
                for (int i = 0; i < Dai1 * 4; i++)
                {
                    Data[i + 20] = buf1[i];
                }
                #endregion
                //--------------------------------------------------------------
                #region Line 2				
                int Dai2 = Line2.Count;
                byte[] buf2 = new byte[Dai2 * 4];

                for (int i = 0; i < Dai2; i++)
                {
                    int H = Line2[i].X / 256;
                    int L = Line2[i].X % 256;
                    buf2[i * 4] = (byte)H;
                    buf2[i * 4 + 1] = (byte)L;

                    H = Line2[i].Y / 256;
                    L = Line2[i].Y % 256;
                    buf2[i * 4 + 2] = (byte)H;
                    buf2[i * 4 + 3] = (byte)L;
                }
                for (int i = 0; i < Dai2 * 4; i++)
                {
                    Data[i + 20 + Dai1 * 4] = buf2[i];
                }
                #endregion
                //--------------------------------------------------------------
                #region FreeLED		
                int DaiFree = FreeLED.Count;
                byte[] buf0 = new byte[DaiFree * 4];

                for (int i = 0; i < DaiFree; i++)
                {
                    int H = FreeLED[i].X / 256;
                    int L = FreeLED[i].X % 256;
                    buf0[i * 4] = (byte)H;
                    buf0[i * 4 + 1] = (byte)L;

                    H = FreeLED[i].Y / 256;
                    L = FreeLED[i].Y % 256;
                    buf0[i * 4 + 2] = (byte)H;
                    buf0[i * 4 + 3] = (byte)L;
                }
                for (int i = 0; i < DaiFree * 4; i++)
                {
                    Data[i + 20 + Dai1 * 4 + Dai2 * 4] = buf0[i];
                }
                #endregion

                SaveDataToFile(TenFile, Data, DataSize);    //	ghi dữ liệu ra file
            }
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            isPan = false;
            ofdFile.Filter = "File cấu hình đi dây (*.map)|*.map|All files (*.*)|*.*";
            if (ofdFile.ShowDialog() == DialogResult.OK)
            {
                FileStream F = new FileStream(ofdFile.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                byte[] Data = new byte[F.Length];
                F.Position = 0; //	Vị trí bắt đầu đọc dữ liệu
                for (int i = 0; i < F.Length; i++)
                {
                    Data[i] = (byte)F.ReadByte();
                }
                F.Close();  //	Đóng file khi đọc xong

                KTLamViec.ChieuRong = Data[0] * 256 + Data[1];      //	kích thước làm việc
                KTLamViec.ChieuCao = Data[2] * 256 + Data[3];
                MakeBackgroundGrid();   //	Vẽ lại kích thước vùng làm việc

                //	Lấy dữ liệu tọa độ cho các line 
                Line1.Clear();
                Line2.Clear();
                FreeLED.Clear();    //	làm trống các line
                int[] Dai = new int[3]; //	Chiều dài các line
                Dai[0] = Data[8] * 256 + Data[9];
                Dai[1] = Data[4] * 256 + Data[5];
                Dai[2] = Data[6] * 256 + Data[7];
                //	lấy dữ liệu Line 1
                Point P = new Point();
                for (int i = 0; i < Dai[1]; i++)
                {
                    P.X = Data[i * 4 + 20] * 256 + Data[i * 4 + 21];
                    P.Y = Data[i * 4 + 22] * 256 + Data[i * 4 + 23];
                    Line1.Add(P);
                }
                for (int i = 0; i < Dai[2]; i++)
                {
                    P.X = Data[i * 4 + 20 + Dai[1] * 4] * 256 + Data[i * 4 + 21 + Dai[1] * 4];
                    P.Y = Data[i * 4 + 22 + Dai[1] * 4] * 256 + Data[i * 4 + 23 + Dai[1] * 4];
                    Line2.Add(P);
                }

                for (int i = 0; i < Dai[0]; i++)
                {
                    P.X = Data[i * 4 + 20 + Dai[1] * 4 + Dai[2] * 4] * 256 + Data[i * 4 + 21 + Dai[1] * 4 + Dai[2] * 4];
                    P.Y = Data[i * 4 + 22 + Dai[1] * 4 + Dai[2] * 4] * 256 + Data[i * 4 + 23 + Dai[1] * 4 + Dai[2] * 4];
                    FreeLED.Add(P);
                }
            }
        }
        #endregion

        #region Nút thoát form và lưu lại data
        private void frmMAP_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                int DataSize = Line1.Count * 4 + Line2.Count * 4 + FreeLED.Count * 4 + 20;  //	1 point có 4 byte ( 2byte x, 2byte y )
                byte[] Data = new byte[DataSize];
                //cal width/height:
                if (Line1.Count > 0)
                {
                    Program.main.maxX = Line1.Max(m => m.X);
                    Program.main.maxY = Line1.Max(m => m.Y);
                    Program.main.minX = Line1.Min(m => m.X);
                    Program.main.minY = Line1.Min(m => m.Y);
                    Program.main.line1 = Line1;
                    Program.main.line2 = Line2;
                    Program.main.ReSizePanel();
                }
                else
                {
                    Program.main.ClearMap();
                }
                #region Lấy thuộc tính vùng làm việc
                Data[0] = (byte)(KTLamViec.ChieuRong / 256);
                Data[1] = (byte)(KTLamViec.ChieuRong % 256);
                Data[2] = (byte)(KTLamViec.ChieuCao / 256);
                Data[3] = (byte)(KTLamViec.ChieuCao % 256);
                Data[4] = (byte)(Line1.Count / 256);
                Data[5] = (byte)(Line1.Count % 256);
                Data[6] = (byte)(Line2.Count / 256);
                Data[7] = (byte)(Line2.Count % 256);
                Data[8] = (byte)(FreeLED.Count / 256);
                Data[9] = (byte)(FreeLED.Count % 256);  //	FreeLed
                #endregion
                //-------------------------------------------------------------
                #region Line 1
                int Dai1 = Line1.Count;
                byte[] buf1 = new byte[Dai1 * 4];

                for (int i = 0; i < Dai1; i++)
                {
                    int H = Line1[i].X / 256;
                    int L = Line1[i].X % 256;
                    buf1[i * 4] = (byte)H;
                    buf1[i * 4 + 1] = (byte)L;

                    H = Line1[i].Y / 256;
                    L = Line1[i].Y % 256;
                    buf1[i * 4 + 2] = (byte)H;
                    buf1[i * 4 + 3] = (byte)L;
                }
                for (int i = 0; i < Dai1 * 4; i++)
                {
                    Data[i + 20] = buf1[i];
                }
                #endregion
                //--------------------------------------------------------------
                #region Line 2
                int Dai2 = Line2.Count;
                byte[] buf2 = new byte[Dai2 * 4];

                for (int i = 0; i < Dai2; i++)
                {
                    int H = Line2[i].X / 256;
                    int L = Line2[i].X % 256;
                    buf2[i * 4] = (byte)H;
                    buf2[i * 4 + 1] = (byte)L;

                    H = Line2[i].Y / 256;
                    L = Line2[i].Y % 256;
                    buf2[i * 4 + 2] = (byte)H;
                    buf2[i * 4 + 3] = (byte)L;
                }
                for (int i = 0; i < Dai2 * 4; i++)
                {
                    Data[i + 20 + Dai1 * 4] = buf2[i];
                }
                #endregion
                //--------------------------------------------------------------
                #region FreeLED
                int DaiFree = FreeLED.Count;
                byte[] buf0 = new byte[DaiFree * 4];

                for (int i = 0; i < DaiFree; i++)
                {
                    int H = FreeLED[i].X / 256;
                    int L = FreeLED[i].X % 256;
                    buf0[i * 4] = (byte)H;
                    buf0[i * 4 + 1] = (byte)L;

                    H = FreeLED[i].Y / 256;
                    L = FreeLED[i].Y % 256;
                    buf0[i * 4 + 2] = (byte)H;
                    buf0[i * 4 + 3] = (byte)L;
                }
                for (int i = 0; i < DaiFree * 4; i++)
                {
                    Data[i + 20 + Dai1 * 4 + Dai2 * 4] = buf0[i];
                }
                #endregion

                SaveDataToFile(@"SmartFull Data.lon", Data, DataSize);  //	ghi dữ liệu ra file
            }
            catch
            {
                MessageBox.Show(@"Éo kết chuyển data được", @"Có lỗi không rõ");
                throw;
            }
            StreamWriter str = File.AppendText(@"C:\\Exception.txt");
            str.WriteLine("ghi gi tuy thich");
            str.Close();
        }
        #endregion

        #region Trình xử lý phím tắt với ToolStripMenu
        private void btnDaoChieu_Click(object sender, EventArgs e)
        {
            List<Point> Buffer = new List<Point>();
            isPan = false;
            int SoLed = Line1.Count;

            for (int i = 0; i < SoLed; i++)  //	copy toàn bộ mảng led sang 1 mảng mới
            {
                Buffer.Add(Line1[i]);       //	copy
            }
            Line1.Clear();      //	Xóa List

            for (int i = SoLed; i > 0; i--) //	copy lại
            {
                Line1.Add(Buffer[i - 1]);
            }
            picDraw.Invalidate();           //	vẽ lại

        }
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnOpen_Click(sender, e);
        }
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
        }
        private void addFreeLedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.ThemLed;
        }
        private void connectLedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.NoiLed;
        }
        private void drawLedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.VeLed;
        }
        private void drawAreaLedZicZacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.VeVung;
        }
        private void drawAreaLedChữZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.VeVungZ;
        }
        private void removeLedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.XoaLed;
        }
        private void removeAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheDoVe = (int)CheDo.XoaVung;
        }
        private void RemoveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void InvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnDaoChieu_Click(sender, e);
        }
        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (KichThuoc.GridGap < 20)
            {
                KichThuoc.GridGap += 2;
                tbZoom.Value = KichThuoc.GridGap;
                rtxtThongTin.Text = " Phóng to : " + tbZoom.Value.ToString();   //	 Thông tin khi zoom
                MakeBackgroundGrid();
            }
        }
        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (KichThuoc.GridGap > 4)
            {
                KichThuoc.GridGap -= 2;
                tbZoom.Value = KichThuoc.GridGap;
                rtxtThongTin.Text = " Thu nhỏ : " + tbZoom.Value.ToString();    //	 Thông tin khi zoom
                MakeBackgroundGrid();
            }
        }
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thanks for use ", "upgrading ...");
        }
        private void NewFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult TraLoi;
            TraLoi = MessageBox.Show("Tùy chọn sẽ xóa hết dữ liệu file.  \nBạn có muốn lưu lại công việc không ?", "Thông báo", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (TraLoi == DialogResult.Yes)
            {
                btnSave_Click(sender, e);
            }
            else if (TraLoi == DialogResult.No)
            {
                btnClear_Click(sender, e);
            }
        }
        private void importDXFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "File DXF (*.dxf)|*.dxf|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string TenFile = ofd.FileName;
                DxfDocument dxfLoad = DxfDocument.Load(TenFile);
                FreeLED.Clear();
                var listCircle = dxfLoad.Circles;
                if (listCircle.Count > 0)
                {
                    for (int i = 0; i < listCircle.Count; i++)
                    {
                        FreeLED.Add(new Point(Convert.ToInt32(listCircle[i].Center.X), Convert.ToInt32(listCircle[i].Center.Y)));
                    }
                }
                var listSpline = dxfLoad.Splines;
                if (listSpline.Count > 0)
                {
                    for (int i = 0; i < listSpline.Count; i++)
                    {
                        var avgX = listSpline[i].ControlPoints.Average(p => p.Position.X);
                        var avgY = listSpline[i].ControlPoints.Average(p => p.Position.Y);
                        FreeLED.Add(new Point(Convert.ToInt32(avgX), Convert.ToInt32(avgY)));
                    }
                }
                var listElip = dxfLoad.Ellipses;
                if (listElip.Count > 0)
                {
                    for (int i = 0; i < listElip.Count; i++)
                    {
                        var avgX = listElip[i].Center.X;
                        var avgY = listElip[i].Center.Y;
                        FreeLED.Add(new Point(Convert.ToInt32(avgX), Convert.ToInt32(avgY)));
                    }
                }
                var minX = FreeLED.Min(m => m.X);
                var minY = FreeLED.Min(m => m.Y);
                var list = FreeLED.Select(point => new Point()
                {
                    X = (point.X - minX) / KichThuoc.GridGap + 1,
                    Y = (point.Y - minY) / KichThuoc.GridGap + 1
                }).ToList();
                FreeLED.Clear();
                FreeLED.AddRange(list);
                //txtFileInfo.AppendText(string.Format("So luong circle: {0} \n", x.Count));
                picDraw.Invalidate();
            }
        }
        #endregion

        private void btnChen_Click(object sender, EventArgs e)
        {
            var ledRong = nbrLedRong.Value;
            var ledCao = nbrLedCao.Value;
            Line1 = new List<Point>();
            if (StartPoint.X > 0 && StartPoint.Y > 0)
            {
                for (int i = StartPoint.Y; i < ledCao + StartPoint.Y; i++)
                {
                    for (int j = StartPoint.X; j < ledRong + StartPoint.X; j++)
                    {
                        Line1.Add(new Point(j, i));
                    }
                }
            }

        }

        private void btnOkVung_Click(object sender, EventArgs e)
        {
            KTLamViec.ChieuRong = Convert.ToInt32(nbrVungRong.Value);
            KTLamViec.ChieuCao = Convert.ToInt32(nbrVungCao.Value);
            MakeBackgroundGrid();
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            isPan = true;
            picDraw.Cursor = Cursors.Hand;
            isDrawing = false;
            CheDoVe = (int)CheDo.Ranh;
        }

        private void picDraw_MouseUp(object sender, MouseEventArgs e)
        {
            if (isPan || e.Button == MouseButtons.Middle)
            {
                prevScrollPoint = e.Location;
                dragging = false;
                picDraw.Cursor = Cursors.Default;
                /*if (e.Button == MouseButtons.Left)
                {
                    prevScrollPoint = e.Location;
                    dragging = false;
                }*/
            }
            if (e.Button == MouseButtons.Left && isMove)
            {
                if (isdragmove)
                {
                    ListTempMovePoints.Clear();
                    ListTempMovePoints.AddRange(ListMovePoints);
                }
                isdragmove = false;
                picDraw.Cursor = Cursors.Default;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            isPan = false;
            CheDoVe = (int)CheDo.ChonVung;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            isPan = false;
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            isPan = false;
            if (CheDoVe == (int)CheDo.MoveVung)
            {
                CheDoVe = (int)CheDo.Ranh;
            }
            else
            {
                CheDoVe = (int)CheDo.MoveVung;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UndoStack.Count > 1)
            {
                var tmp1 = UndoStack.Pop();
                RedoStack.Push(tmp1);
                var tmp2 = UndoStack.Peek();
                Line1 = new List<Point>(tmp2.Line1);
                FreeLED = new List<Point>(tmp2.FreeLed);
                Line2 = new List<Point>(tmp2.Line2);
                picDraw.Invalidate();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RedoStack.Count > 0)
            {
                var tmp = RedoStack.Pop();
                UndoStack.Push(tmp);
                Line1 = new List<Point>(tmp.Line1);
                FreeLED = new List<Point>(tmp.FreeLed);
                Line2 = new List<Point>(tmp.Line2);
                picDraw.Invalidate();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ListToCopy != null && ListToCopy.Count > 0)
            {
                
                ListSelectPoints.Clear();
                ListMovePoints = new List<Point>();
                for (int i = 0; i < Line1.Count; i++)
                {
                    if (ListToCopy.Contains(Line1[i]))
                    {
                        ListMovePoints.Add(new Point(Line1[i].X, Line1[i].Y));
                    }
                }
                //ListMovePoints.AddRange(ListToCopy.Select(m => new Point(m.X + 2, m.Y + 2)));
                enableMovePastedArea = true;
                CheDoVe = (int)CheDo.MoveVung;
            }
            picDraw.Invalidate();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCopy = true;
            if (ListSelectPoints != null && ListSelectPoints.Count > 0)
            {
                ListToCopy = new List<Point>();
                ListToCopy.AddRange(ListSelectPoints);
            }
        }

        private void nbrLedRong_Enter(object sender, EventArgs e)
        {
            nbrLedRong.Select(0, nbrLedRong.Text.Length);
        }
        private void nbrLedCao_Enter(object sender, EventArgs e)
        {
            nbrLedCao.Select(0, nbrLedCao.Text.Length);

        }

    }
}







