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
        Status.Value = -1;
    }
  
}

}
