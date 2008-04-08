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
public class MultiplierTest : SimpleArithmeticTestBase {

    [Test()]
    public void ShouldMultiplyTwoPositiveInlets() {
        LeftValueIs(2);
        RightValueIs(3);
        ResultShouldBe(6);
    }

    [Test()]
    public void ShouldMultiplyTwoNegativeInlets() {
        LeftValueIs(-2);
        RightValueIs(-3);
        ResultShouldBe(6);
    }

    [Test()]
    public void ShouldMultiplyZeros() {
        LeftValueIs(0);
        RightValueIs(0);
        ResultShouldBe(0);
    }

    private void ResultShouldBe(int value) {
        Outlet<int> result = Result();

        Multiplier multiplier = new Multiplier();
        multiplier.Init(mockPatchContainer);
        multiplier.Execute();

        Assert.AreEqual(value, result.Value);
    }
}
}
