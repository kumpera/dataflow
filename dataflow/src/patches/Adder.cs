/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using Dataflow.Core;

namespace Dataflow.Patches {

[Patch(Name="+")]
public class Adder {
    [Inlet(Name="Left")]
    public Inlet<int> left { get; set; }

    [Inlet(Name="Right")]
    public Inlet<int> right { get; set; }

    [Outlet(Name="Result")]
    public Outlet<int> result { get; set; }

    public void Init(IPatchContainer container) {
        left = container.AddInlet<int> ("left");
        right = container.AddInlet<int> ("right");
        result = container.AddOutlet<int> ("result");
    }

    public void Execute() {
        result.Value = left.Value + right.Value;
    }
}
}
