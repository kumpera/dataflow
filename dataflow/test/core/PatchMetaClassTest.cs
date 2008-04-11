/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using NUnit.Framework;

namespace Dataflow.Core {
[TestFixture()]
public class PatchMetaClassTest {

    [Patch(Name="Example")]
    public class TestPatch {
        [Inlet(Name="input")]
        public Inlet<int> Tst {
            get;
            set;
        }

        [Outlet(Name="output")]
        public Outlet<int> Output {
            get;
            set;
        }
    }

    [Patch]
    public class TestPatchWithImplicitName {
        [Inlet]
        public Inlet<int> Tst {
            get;
            set;
        }

        [Outlet]
        public Outlet<int> TestOutput {
            get;
            set;
        }
    }

    [Test]
    public void MetaClassParsePatchName() {
        var pm = new PatchMetaClass(typeof(TestPatch));
        Assert.AreEqual("Example", pm.Name);
    }

    [Test]
    public void MetaClassEnumerateInlets() {
        var pm = new PatchMetaClass(typeof(TestPatch));

        var inlets = pm.Inlets;
        Assert.AreEqual(1, inlets.Length, "#1");
        Assert.AreEqual("input", inlets [0].Name, "#2");
    }

    [Test]
    public void MetaClassEnumerateOutlet() {
        var pm = new PatchMetaClass(typeof(TestPatch));

        var outlets = pm.Outlets;
        Assert.AreEqual(1, outlets.Length, "#1");
        Assert.AreEqual("output", outlets [0].Name, "#2");
    }

    [Test]
    public void MetaClassParseImplicitPatchName() {
        var pm = new PatchMetaClass(typeof(TestPatchWithImplicitName));
        Assert.AreEqual("TestPatchWithImplicitName", pm.Name);
    }

    [Test]
    public void MetaClassEnumerateInletsWithImplicitName() {
        var pm = new PatchMetaClass(typeof(TestPatchWithImplicitName));

        var inlets = pm.Inlets;
        Assert.AreEqual(1, inlets.Length, "#1");
        Assert.AreEqual("Tst", inlets [0].Name, "#2");
    }


    [Test]
    public void MetaClassEnumerateOutletWithImplicitName () {
        var pm = new PatchMetaClass(typeof(TestPatchWithImplicitName));

        var outlets = pm.Outlets;
        Assert.AreEqual(1, outlets.Length, "#1");
        Assert.AreEqual("TestOutput", outlets [0].Name, "#2");
    }


	[Test]
	public void NewInstanceFillInletsAndOutLets () {
		var pm = new PatchMetaClass(typeof(TestPatch));
		var patch = pm.NewInstance<TestPatch> ();

		Assert.IsNotNull (patch, "#1");
		Assert.IsNotNull (patch.Tst, "#2");
		Assert.IsNotNull (patch.Output, "#3");
	}
}
}
