using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LedFullControl;

namespace ManageImage
{
    static class Program
    {
        public static Main main;
        public static List<ProgramTreeView> TreeView;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TreeView = new List<ProgramTreeView>();
            TreeView.Add(new ProgramTreeView()
            {
                ProgramId = 1,
                ProgramName = "Program 1",
                ListAreas = new List<DisplayArea>(),
                GridMaps = new List<Cell>(),
                CellSize = 20
            });
            main = new Main();
            Application.Run(main);
            //Application.Run(new frmMAP());
        }

        public static void SetDoubleBuffered(Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }
    }
}
