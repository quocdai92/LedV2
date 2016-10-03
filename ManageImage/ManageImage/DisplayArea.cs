using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LedProject;

namespace ManageImage
{
    public class DisplayArea
    {
        public DisplayArea()
        {
            ListGrid = new List<Cell>();
            ListImages = new List<Image>();
            ListFileTemplates = new List<FileTemplate>();
        }
        public int AreaId { get; set; }
        public string Name { get; set; }
        public List<Cell> ListGrid { get; set; } 
        public string Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle DisplayRectangle { get; set; }
        public List<Image> ListImages { get; set; }
        public List<FileTemplate> ListFileTemplates { get; set; }
        public int TimePlay { get; set; }
        public int Angle { get; set; }
        public int CellSize { get; set; }
    }

    public class Cell
    {
        public int Size { get; set; }
        public Point StartPosition { get; set; }
        public Color Color { get; set; }
        //pixel location x
        public int X { get; set; }
        //pixel location y
        public int Y { get; set; }
        public bool IsEmpty { get; set; }
    }
}
