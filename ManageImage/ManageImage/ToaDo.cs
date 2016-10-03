using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace LedFullControl
{
	public class ToaDo
	{
		private Point _ToaDo1 = new Point();
		private Point _ToaDo2 = new Point();
		private Point _ToaDo3 = new Point();
		private Point _ToaDoVuaKick = new Point();
		private Point _ToaDoHienTai = new Point();

		public ToaDo()
		{ 
		}
		/// <summary>
		/// Tọa độ kick lần 1
		/// </summary>
		public Point TD1
		{
			get { return _ToaDo1; }
			set { _ToaDo1 = value; }
		}
		/// <summary>
		/// Tọa độ kick lần 2
		/// </summary>
		public Point TD2
		{
			get { return _ToaDo2; }
			set { _ToaDo2 = value; }
		}
		/// <summary>
		/// Tọa độ kick lần 3
		/// </summary>
		public Point TD3
		{
			get { return _ToaDo3; }
			set { _ToaDo3 = value; }
		}
		/// <summary>
		/// Tọa độ kick lần cuối
		/// </summary>
		public Point VuaKick
		{
			get { return _ToaDoVuaKick; }
			set { _ToaDoVuaKick = value; }
		}
		/// <summary>
		/// Tọa độ hiện tại của chuột
		/// </summary>
		public Point TDNow
		{
			get { return _ToaDoHienTai; }
			set { _ToaDoHienTai = value; }
		}

	}	
}
