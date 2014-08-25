using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aert_csharp_extensions;

namespace aert_csharp_helpers
{
    /// <summary>
    /// Classe utilitaire gérant les appels vers le programme Robocopy.
    /// </summary>
    public class RobocopyWrapper
    {
        public bool IsKilled { get; private set; }
        private const string CMD = "robocopy.exe";
        private const string CMD_OPTIONS = "/E /R:2 /W:15 /NS /NC /NJH /MIR";

        public StringBuilder StandardOutputBuilder { get; private set; }
        public StringBuilder StandardErrorBuilder { get; private set; }
        public Process CopyProcess { get; private set; }

        public object LockObject = new object();

        #region Evénement à gérer

        public event EventHandler OnOutputDataReceived;
        public event EventHandler OnErrorDataReceived;

        #endregion

        public RobocopyWrapper()
        {
            StandardOutputBuilder = new StringBuilder();
            StandardErrorBuilder = new StringBuilder();
        }

        #region Méthodes publiques

        /// <summary>
        /// Démarre une copie.
        /// </summary>
        public bool Copy(string src, string dest)
        {
            src = "\"{0}\"".HelpFormat(src);
            dest = "\"{0}\"".HelpFormat(dest);

            string args = "{0} {1} {2}".HelpFormat(src, dest, CMD_OPTIONS);

            return StartProcess(args);
        }

        #endregion

        public void Kill()
        {
            if (CopyProcess != null)
                CopyProcess.Kill();
            IsKilled = true;
        }

        #region Méthodes privées

        private bool StartProcess(string args)
        {

            ProcessStartInfo ps = new ProcessStartInfo
            {
                FileName = CMD,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = args,
                StandardOutputEncoding = Encoding.GetEncoding(437),
                StandardErrorEncoding = Encoding.GetEncoding(437),
            };

            try
            {
                using (Process p = Process.Start(ps))
                {
                    CopyProcess = p;

                    p.OutputDataReceived += process_OutputDataReceived;
                    p.ErrorDataReceived += process_ErrorDataReceived;

                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    CopyProcess = null;
                    return !IsKilled && (p.ExitCode >= 0 || p.ExitCode <= 2);
                }
            }
            catch (Exception ex)
            {
                lock (LockObject)
                {
                    StandardErrorBuilder.Append(ex.Message);
                }

                RoboDataReceivedEventArgs eventArgs = new RoboDataReceivedEventArgs(ex.Message);
                OnErrorDataReceived(this, eventArgs);
                return false;
            }
        }

        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            RoboDataReceivedEventArgs eventArgs = new RoboDataReceivedEventArgs(e.Data ?? string.Empty);

            lock (LockObject)
            {
                if (IntProgress(eventArgs.Data) == null)
                    StandardErrorBuilder.AppendLine(e.Data);
            }

            if (OnErrorDataReceived != null)
                OnErrorDataReceived(sender, eventArgs);
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            RoboDataReceivedEventArgs eventArgs = new RoboDataReceivedEventArgs(e.Data ?? string.Empty);

            lock (LockObject)
            {
                if (IntProgress(eventArgs.Data) == null)
                    StandardOutputBuilder.AppendLine(eventArgs.Data);
            }

            if (OnOutputDataReceived != null)
                OnOutputDataReceived(sender, eventArgs);
        }

        #endregion

        public float? IntProgress(string roboOutput)
        {
            roboOutput = roboOutput.Trim();
            if (!roboOutput.EndsWith("%")) return null;

            roboOutput = roboOutput.Replace("%", "").Replace(".", ",");
            float rv;
            if (float.TryParse(roboOutput, out rv))
                return rv;
            return null;
        }
    }

    internal class RoboDataReceivedEventArgs : EventArgs
    {

        public string Data { get; set; }


        public RoboDataReceivedEventArgs(string data)
        {
            Data = data;
        }

        public RoboDataReceivedEventArgs(DataReceivedEventArgs source)
        {
            Data = source.Data;
        }
    }
}
