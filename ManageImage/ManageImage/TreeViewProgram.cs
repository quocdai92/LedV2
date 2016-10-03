using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LedFullControl;

namespace ManageImage
{
    public class TreeViewProgram
    {
        public string Name { get; set; }
        public List<Cell> ListGrid { get; set; }
        public frmMAP Map { get; set; }
        public List<DisplayArea> ListChilNode { get; set; } 
    }
}
