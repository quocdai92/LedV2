using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageImage
{
    public class ProgramTreeView
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int CellSize { get; set; }
        public List<DisplayArea> ListAreas { get; set; }
        public List<Cell> GridMaps { get; set; }
    }
}
