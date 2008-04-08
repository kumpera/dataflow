/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using NUnit.Framework;

namespace Dataflow.Core {
[TestFixture]
public class PatchTests {
    public class CheckInletChanged : IPatch {
        public bool hasChanged;
        public Inlet<int> input;

        public CheckInletChanged() {
        }

        public void Init(IPatchContainer container) {
            input = container.AddInlet<int>("InPort");
        }

        public void Execute() {
            hasChanged = input.HasChanged;
        }
    }

    [Test]
    public void PatchDetectInletValueChangeBetweenExecutions() {
        CheckInletChanged patch = new CheckInletChanged();
        PatchContainer pc = new PatchContainer(patch);
        Assert.IsFalse(patch.hasChanged, "#1");

        pc.ExecutePatch();
        Assert.IsFalse(patch.hasChanged, "#2");

        patch.input.Value = 20;
        pc.ExecutePatch();
        Assert.IsTrue(patch.hasChanged, "#3");

        pc.ExecutePatch();
        Assert.IsFalse(patch.hasChanged, "#4");
    }

    public class ChangeOutlet : IPatch {
        public Outlet<int> output;

        public ChangeOutlet() {
        }

        public void Init(IPatchContainer container) {
            output = container.AddOutlet<int>("OutPut");
        }

        public void Execute() {
            output.Value = 99;
        }
    }

    [Test]
    public void PatchDetectOutletValueChangeDuringExecution() {
        ChangeOutlet patch = new ChangeOutlet();
        PatchContainer pc = new PatchContainer(patch);
        Assert.IsFalse(patch.output.HasChanged, "#1");
        pc.ExecutePatch();
        Assert.IsTrue(patch.output.HasChanged, "#2");
    }
}
}
