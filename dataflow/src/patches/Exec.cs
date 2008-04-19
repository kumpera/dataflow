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
    public Inlet<string> args { get; set; }

    [Inlet(Name="Path")]
    public Inlet<string> path { get; set; }

    [Inlet(Name="Input")]
    public Inlet<string> stdin { get; set; }

    [Outlet(Name="Output")]
    public Outlet<string> stdout { get; set; }

    [Outlet(Name="Error")]
    public Outlet<string> stderr { get; set; }

    [Outlet(Name="Status")]
    public Outlet<int> status { get; set; }
    
    public void Init(IPatchContainer container) {
        args = container.AddInlet<string>("Arguments");
        path = container.AddInlet<string>("Path");
        stdin = container.AddInlet<string>("Input");

        stdout = container.AddOutlet<string>("Output");
        stderr = container.AddOutlet<string>("Error");
        status = container.AddOutlet<int>("Status");
                
    }

    public void Execute() {
    }
  
}

}
