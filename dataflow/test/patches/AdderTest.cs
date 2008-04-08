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
public class AdderTest {
    private Mockery mocks;
    private IPatchContainer mockPatchContainer;

    [SetUp]
    public void SetUp() {
        mocks = new Mockery();
        mockPatchContainer = mocks.NewMock<IPatchContainer>();
    }

    [Test()]
    public void ShouldAddTwoInletsTogether() {

        Stub.On(mockPatchContainer).GetProperty("CurrentFrame").Will(Return.Value(0));

        Inlet<int> lhs = new Inlet<int>("left", mockPatchContainer, ActivationMode.ActivateOnMessage);
        Inlet<int> rhs = new Inlet<int>("right", mockPatchContainer, ActivationMode.ActivateOnMessage);
        Outlet<int> result = new Outlet<int>("result", mockPatchContainer);

        lhs.Value = 2;
        rhs.Value = 3;

        Expect.Once.On(mockPatchContainer).Method("AddInlet").With("left").Will(Return.Value(lhs));
        Expect.Once.On(mockPatchContainer).Method("AddInlet").With("right").Will(Return.Value(rhs));

        Expect.Once.On(mockPatchContainer).Method("AddOutlet").With("result").Will(Return.Value(result));

        Adder adder = new Adder();
        adder.Init(mockPatchContainer);
        adder.Execute();

        Assert.AreEqual(5, result.Value);
    }
}
}
