/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Diagnostics;
using Dataflow.Core;

namespace Dataflow.Patches {

  // http://www.go-mono.com/docs/index.aspx?link=N%3ASystem.Diagnostics

[Patch(Name="Exec")]
public class Exec {

    [Inlet(Name="Arguments")]
    public Inlet<string> Args { get; set; }

    [Inlet(Name="Path")]
    public Inlet<string> Path { get; set; }

    [Inlet(Name="Input")]
    public Inlet<string> StdIn { get; set; }

    [Outlet(Name="Output")]
    public Outlet<string> StdOut { get; set; }

    [Outlet(Name="Error")]
    public Outlet<string> StdErr { get; set; }

    [Outlet(Name="Status")]
    public Outlet<int> Status { get; set; }

    public void Init(IPatchContainer container) {
        Args = container.AddInlet<string>("Arguments");
        Path = container.AddInlet<string>("Path");
        StdIn = container.AddInlet<string>("Input");

        StdOut = container.AddOutlet<string>("Output");
        StdErr = container.AddOutlet<string>("Error");
        Status = container.AddOutlet<int>("Status");

    }

    public void Execute() {
        ProcessStartInfo info = new ProcessStartInfo();
        info.Arguments = Args.Value;
        info.FileName = Path.Value;
        info.CreateNoWindow = true;
        info.UseShellExecute = false;
        info.RedirectStandardInput = true;
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;
        try {
            Process proc = Process.Start(info);

            proc.StandardInput.Write(StdIn.Value);
            proc.StandardInput.Close();

            StdErr.Value = proc.StandardError.ReadToEnd();
            StdOut.Value = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();

            Status.Value = proc.ExitCode;
        } catch {
            Status.Value = -1;
        }
    }

}

}
