using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageImage
{
    public class UndoRedoModel
    {
        public List<Point> FreeLed { get; set; }
        public List<Point> Line1 { get; set; }
        public List<Point> Line2 { get; set; }

        public UndoRedoModel()
        {
            FreeLed = new List<Point>();
            Line1 = new List<Point>();
            Line2 = new List<Point>();
        }
    }
}
