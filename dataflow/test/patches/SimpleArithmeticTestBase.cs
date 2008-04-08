using System;
using NUnit.Framework;
using NMock2;
using Dataflow.Core;

namespace Dataflow.Patches {

public class SimpleArithmeticTestBase {
    protected Mockery mocks;
    protected IPatchContainer mockPatchContainer;

    [SetUp]
    public void SetUp() {
        mocks = new Mockery();
        mockPatchContainer = mocks.NewMock<IPatchContainer>();
        Stub.On(mockPatchContainer).GetProperty("CurrentFrame").Will(Return.Value(0));
    }

    protected Inlet<int> LeftValueIs(int value) {
        Inlet<int> lhs = new Inlet<int>("left", mockPatchContainer, ActivationMode.ActivateOnMessage);
        Expect.Once.On(mockPatchContainer).Method("AddInlet").With("left").Will(Return.Value(lhs));
        lhs.Value = value;
        return lhs;
    }

    protected Inlet<int> RightValueIs(int value) {
        Inlet<int> rhs = new Inlet<int>("right", mockPatchContainer, ActivationMode.ActivateOnMessage);
        Expect.Once.On(mockPatchContainer).Method("AddInlet").With("right").Will(Return.Value(rhs));
        rhs.Value = value;
        return rhs;
    }

    protected Outlet<int> Result() {
        Outlet<int> result = new Outlet<int>("result", mockPatchContainer);
        Expect.Once.On(mockPatchContainer).Method("AddOutlet").With("result").Will(Return.Value(result));
        return result;
    }

}
}