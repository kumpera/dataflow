/*
* Copyright © 2008 Rodrigo Kumpera, Carlos Villela
*
* Permission to use, copy, modify, distribute, and sell this software and its
* documentation for any purpose is hereby granted without fee, provided that
* the above copyright notice appear in all copies and that both that
* copyright notice and this permission notice appear in supporting
* documentation, and that the name of SuSE not be used in advertising or
* publicity pertaining to distribution of the software without specific,
* written prior permission.  SuSE makes no representations about the
* suitability of this software for any purpose.  It is provided "as is"
* without express or implied warranty.
*
* SuSE DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE, INCLUDING ALL
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO EVENT SHALL SuSE
* BE LIABLE FOR ANY SPECIAL, INDIRECT OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
* WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION
* OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
* CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*
* Author:  Carlos Villela <cv@lixo.org>
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
