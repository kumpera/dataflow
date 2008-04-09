/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using Dataflow.Core;

namespace Dataflow.Patches {

public class NumberKeeper : IPatch {

    public int Number {
        get {
            return input.Value;
        }
    }

    public Inlet<int> Input {
        get {
            return input;
        }
    }

    public Outlet<int> Output {
        get {
            return output;
        }
    }

    private Inlet<int> input;
    private Outlet<int> output;

    public void Init(IPatchContainer container) {
        input = container.AddInlet<int>("Number");
        output = container.AddOutlet<int>("Number");
    }

    public void Execute() {
        output.Value = input.Value;
    }
}

}