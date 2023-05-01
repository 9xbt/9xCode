using System;
using System.Threading;
using System.Windows.Forms;
using static System.ConsoleColor;

// Beta 2.2
// Licensed under the MIT license

namespace _9xCode
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Console.Title = "9xCode";
            Console.ForegroundColor = White;
            Application.EnableVisualStyles();

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "9xCode Files (.9xc)|*.9xc";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {

            Run:
                Interpreter.Run(openDialog.FileName);

                Thread.Sleep(500);
                Console.Title = "9xCode";
                if (MessageBox.Show("Run again?", "9xCode", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) goto Run;
            }
        }
    }
}
