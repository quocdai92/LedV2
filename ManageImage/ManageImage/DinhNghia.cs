using System;
using System.Collections.Generic;
using System.Text;

namespace LedFullControl
{

	class DinhNghia
	{
		private int _ChieuRong;
		private int _ChieuCao;
		private int _GridGap;
		private int _CrossSize;

		public DinhNghia()
		{ 
		
		}

		public int LedSize
		{
			get { return _GridGap*3/4; }
		}

		public int BanKinhLed
		{
			get { return _GridGap *3/8; }
		}
		public int ChieuRong
		{
			get { return _ChieuRong; }
			set { _ChieuRong = value; }
		}

		public int ChieuCao
		{
			get { return _ChieuCao; }
			set { _ChieuCao = value; }
		}

		public int GridGap
		{
			get { return _GridGap; }
			set { _GridGap = value; }
		}

		public int CrossSize		//	kích thước dấu chữ thập
		{
			get { return _CrossSize; }
			set { _CrossSize = value; }
		}


	}
}
