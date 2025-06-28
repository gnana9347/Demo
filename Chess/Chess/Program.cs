using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Safe initialization
            try
            {
                Application.Run(new FrmHome());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start application: {ex.Message}");
            }
        }
    }
}
