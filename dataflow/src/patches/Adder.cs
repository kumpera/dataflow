/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using Dataflow.Core;

namespace Dataflow.Patches {

public class Adder : IPatch {
    Inlet<int> left;
    Inlet<int> right;
    Outlet<int> result;

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
