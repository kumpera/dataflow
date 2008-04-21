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

    protected void Run() {
        Exec exec = new Exec();
        exec.Init(mockPatchContainer);
        
        exec.Execute();
    }

    [Test()]
    public void ShouldSetStatusToOneWhenExecutableNotFound() {
        PathIs("/non/existant/executable/file");
        ArgumentsAre("");
        InputIs("");
        Outlet<int> status = StatusOutlet();
        Outlet<string> output = OutputOutlet();
        Outlet<string> error = ErrorOutlet();

        Run();
        
        Assert.AreEqual(-1, status.Value);
    }
    
    [Test()]
    public void ShouldExecuteShellCommandGracefully() {
        PathIs("/usr/bin/true");
        ArgumentsAre("");
        InputIs("");
        
        Outlet<int> status = StatusOutlet();
        Outlet<string> output = OutputOutlet();
        Outlet<string> error = ErrorOutlet();

        Run();
        
        Assert.AreEqual(0, status.Value);
    }

    [Test()]
    public void ShouldExecuteShellCommandWithErrorExitCodeGracefully() {
        PathIs("/usr/bin/false");
        ArgumentsAre("");
        InputIs("");
        
        Outlet<int> status = StatusOutlet();
        Outlet<string> output = OutputOutlet();
        Outlet<string> error = ErrorOutlet();

        Run();
        
        Assert.AreEqual(1, status.Value);
    }
    
    [Test()]
    public void ShouldExecuteShellCommandWithOutputToStdOut() {
        PathIs("/bin/echo");
        ArgumentsAre("hello");
        InputIs("");
        
        Outlet<int> status = StatusOutlet();
        Outlet<string> output = OutputOutlet();
        Outlet<string> error = ErrorOutlet();

        Run();
        
        Assert.AreEqual(0, status.Value);
        Assert.AreEqual("hello\n", output.Value);
    }

    [Test()]
    public void ShouldExecuteShellCommandWithInputFromStdIn() {
        PathIs("/bin/cat");
        ArgumentsAre("");
        InputIs("hello");
        
        Outlet<int> status = StatusOutlet();
        Outlet<string> output = OutputOutlet();
        Outlet<string> error = ErrorOutlet();

        Run();
        
        Assert.AreEqual(0, status.Value);
        Assert.AreEqual("hello", output.Value);
    }
    
}
}