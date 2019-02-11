using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MakeTxGui
{
    static class ProcessHelper
    {
        public static int StartProcess(out List<string> stdout, out List<string> errout, string filename, string arguments, string workingDirectory, int timeout)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    WorkingDirectory = workingDirectory,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                },
                EnableRaisingEvents = true,
            };

            List<string> listStdOut = new List<string>();
            List<string> listErrOut = new List<string>();

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        try
                        {
                            outputWaitHandle.Set();
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        listStdOut.Add(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        try
                        {
                            errorWaitHandle.Set();
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        listErrOut.Add(e.Data);
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (process.WaitForExit(timeout) &&
                    outputWaitHandle.WaitOne(timeout) &&
                    errorWaitHandle.WaitOne(timeout))
                {
                    stdout = listStdOut;
                    errout = listErrOut;
                    return process.ExitCode;
                }
                else
                {
                    stdout = listStdOut;
                    errout = listErrOut;
                    return -1;
                }
            }
        }
    }
}
