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
public class ExecTest {

    protected Mockery mocks;
    protected IPatchContainer mockPatchContainer;

    [SetUp]
    public void SetUp() {
        mocks = new Mockery();
        mockPatchContainer = mocks.NewMock<IPatchContainer>();
        Stub.On(mockPatchContainer).GetProperty("CurrentFrame").Will(Return.Value(0));
    }

    protected Inlet<string> ExpectInletToBeConnectedWith(string name, string value) {
        Inlet<string> inlet = new Inlet<string>("Arguments", mockPatchContainer, ActivationMode.ActivateOnMessage);
        Expect.Once.On(mockPatchContainer).Method("AddInlet").With(name).Will(Return.Value(inlet));
        inlet.Value = value;
        return inlet;
    }

    protected Inlet<string> PathIs(string value) {
        return ExpectInletToBeConnectedWith("Path", value);
    }

    protected Inlet<string> ArgumentsAre(string value) {
        return ExpectInletToBeConnectedWith("Arguments", value);
    }

    protected Inlet<string> InputIs(string value) {
        return ExpectInletToBeConnectedWith("Input", value);
    }

    protected Outlet<int> StatusOutlet() {
        Outlet<int> result = new Outlet<int>("Status", mockPatchContainer);
        Expect.Once.On(mockPatchContainer).Method("AddOutlet").With("Status").Will(Return.Value(result));
        return result;
    }

    protected Outlet<string> OutputOutlet() {
        Outlet<string> result = new Outlet<string>("Status", mockPatchContainer);
        Expect.Once.On(mockPatchContainer).Method("AddOutlet").With("Output").Will(Return.Value(result));
        return result;
    }

    protected Outlet<string> ErrorOutlet() {
        Outlet<string> result = new Outlet<string>("Error", mockPatchContainer);
        Expect.Once.On(mockPatchContainer).Method("AddOutlet").With("Error").Will(Return.Value(result));
        return result;
    }

    [Test()]
    public void ShouldSetStatusToMinusOneWhenExecutableNotFound() {
        PathIs("/not/existant/executable/file");
        ArgumentsAre("");
        InputIs("");
        Outlet<int> status = StatusOutlet();
        Outlet<string> output = OutputOutlet();
        Outlet<string> error = ErrorOutlet();

        Exec exec = new Exec();
        exec.Init(mockPatchContainer);
        
        exec.Execute();
        
        Assert.AreEqual(-1, status.Value);
    }

}
}