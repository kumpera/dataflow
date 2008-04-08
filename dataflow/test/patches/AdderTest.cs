/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using NUnit.Framework;
using NMock2;
using Dataflow.Core;

namespace Dataflow.Patches {

[TestFixture()]
public class AdderTest : SimpleArithmeticTestBase {

    [Test()]
    public void ShouldAddTwoPositiveInletsTogether() {
        LeftValueIs(2);
        RightValueIs(3);
        ResultShouldBe(5);
    }

    [Test()]
    public void ShouldAddTwoNegativeInletsTogether() {
        LeftValueIs(-2);
        RightValueIs(-3);
        ResultShouldBe(-5);
    }

    [Test()]
    public void ShouldAddZerosTogether() {
        LeftValueIs(0);
        RightValueIs(0);
        ResultShouldBe(0);
    }

    private void ResultShouldBe(int value) {
        Outlet<int> result = Result();

        Adder adder = new Adder();
        adder.Init(mockPatchContainer);
        adder.Execute();

        Assert.AreEqual(value, result.Value);
    }
}
}
