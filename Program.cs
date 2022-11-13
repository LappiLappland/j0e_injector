using System;
using System.Windows.Forms;

namespace WindowsForms_revive
{
    static class Program
    {
        /// <summary>
        /// Entrance.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }
}
