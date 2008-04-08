/*
 * Copyright © 2008 Rodrigo Kumpera
*
* Permission to use, copy, modify, distribute, and sell this software and its
* documentation for any purpose is hereby granted without fee, provided that
* the above copyright notice appear in all copies and that both that
* copyright notice and this permission notice appear in supporting
* documentation, and that the name of the Authors not be used in advertising or
* publicity pertaining to distribution of the software without specific,
* written prior permission. The Authors makes no representations about the
* suitability of this software for any purpose.  It is provided "as is"
* without express or implied warranty.
*
* The AUTHORS DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE, INCLUDING ALL
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO EVENT SHALL SuSE
* BE LIABLE FOR ANY SPECIAL, INDIRECT OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
* WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION
* OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
* CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*
* Author:  Rodrigo Kumpera
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
            Console.WriteLine("HERE");
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
