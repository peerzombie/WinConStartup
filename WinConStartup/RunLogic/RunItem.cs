using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinConStartup.RunLogic
{
    public class RunItem
    {
        public string Name;
        public string Executable;
        public bool RunInBackground;
        public bool RunAsAdmin;
        public string Args;

		public RunItem(string name, string executable, string arguments = "", bool background = false, bool asadmin = false)
        {
            Name = name;
            Executable = executable;
            RunInBackground = background;
            RunAsAdmin = asadmin;
            Args = arguments;
        }

        // ReSharper disable once InconsistentNaming
        public void run()
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = Executable;
                    if (RunAsAdmin) p.StartInfo.Verb = "runas";
                    p.StartInfo.Arguments = Args;
                    if (RunInBackground)
                    {
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.StartInfo.CreateNoWindow = true;
                    }
					Console.Write("Starting " + Name + " [ .... ] ");
                    p.Start();
                }
                else
                {
                    MessageBox.Show(
                        "Alert !\nThe Windows Version you are using is not Supported.\nPlease upgrade to Windows Vista or Later to use this Program !",
                        "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Win32Exception)
            {
                MessageBox.Show("Error\nUnable to access File !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Error\nInvalid Operation !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}