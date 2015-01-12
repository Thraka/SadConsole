namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using WinConsole = System.Console;
    using Microsoft.Xna.Framework;

    class StreamConsole: Console
    {
        private System.Diagnostics.Process _process;

        public StreamConsole()
            : base(80, 25)
        {
            CanUseKeyboard = true;

            var procInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe");
            procInfo.CreateNoWindow = true;
            //procInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardInput = true;
            procInfo.RedirectStandardOutput = true;
            _process = System.Diagnostics.Process.Start(procInfo);

            _process.OutputDataReceived += proc_OutputDataReceived;
            _process.BeginOutputReadLine();

            IsVisible = false;
        }

        public override bool ProcessKeyboard(SadConsole.Input.KeyboardInfo info)
        {
            base.ProcessKeyboard(info);

            foreach (var item in info.KeysPressed)
            {

                _process.StandardInput.WriteLine("dir");
            }

            return true;
        }

        void proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            this.VirtualCursor.Print(e.Data);
            this.VirtualCursor.NewLine();
        }
    }
}
