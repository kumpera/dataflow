/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;
using System.Text;

using Dataflow.Core;

namespace Dataflow.Gui {

internal abstract class CanvasWidget {
	public abstract void PerformLayout (Context cc);

	public abstract void Draw (Context cc, LogHandler log);

	public abstract bool HitTest (PointD p);
}


public class EditorCanvas : Gtk.DrawingArea {
	PatchWidget[] patches;
	ConnectionWidget[] connections;
	bool firstRun = true;
	PatchRepository repo;

	public EditorCanvas (PatchRepository repo) {
		Realized += (obj, evnt) => SetupEvents ();
		this.repo = repo;

		patches = new PatchWidget[2];
		patches[0] = new PatchWidget (
			"HSL Color",
			new String[] { "Hue", "Saturation", "Luminosity"},
			new String[] { "Color" });
		patches[0].X = 40;
		patches[0].Y = 40;

		patches[1] = new PatchWidget (
			"Round",
			new String[] { "Value"},
			new String[] { "Rounded Value", "Floor", "Ceil Value" });
		patches[1].X = 200;
		patches[1].Y = 200;

		connections = new ConnectionWidget [1];
		connections [0] = new ConnectionWidget (patches[0], 0, patches[1], 0);
		patches [0].SetOutletConnected (0);
		patches [1].SetInletConnected (0);

	}

	void SetupEvents () {
		GdkWindow.Events |= Gdk.EventMask.ButtonMotionMask | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask;
		//TODO move drop flavor to somewhere else
		Console.WriteLine ("SETUP");
		Gtk.Drag.DestSet (
			this,
			Gtk.DestDefaults.Drop | Gtk.DestDefaults.Motion,
			Hyena.Data.Gui.ListView<PatchInfo>.DragDropDestEntries, 
			Gdk.DragAction.Move);
	}

	PatchWidget dragPatch;
	//TODO replace this crap with a Pointer type
	double ox, oy;
	double cx, cy;

	protected override bool OnButtonPressEvent (Gdk.EventButton ev) {
		//LogEvent ("button pressed x: " + ev.X + " y " + ev.Y);

		for (var i = patches.Length - 1; i >= 0; --i) {
			var patch = patches [i];
			if (patch.HitTest (new PointD (ev.X, ev.Y))) { //begin draw sequences
				dragPatch = patch;
				cx = ev.X;
				cy = ev.Y;
				ox = patch.X;
				oy = patch.Y;
				break;
			}
		}
	
		return false;
	}

	protected override void OnDragDataReceived (Gdk.DragContext context, int x, int y, Gtk.SelectionData data, uint info, uint time) {
		Console.WriteLine("arg string data is {0}", new string (Encoding.UTF8.GetChars (data.Data)));
		Console.WriteLine ("--- DONE --- ");
		Gtk.Drag.Finish (context, true, false, time);
	}


	protected override bool OnButtonReleaseEvent(Gdk.EventButton ev) {
		//LogEvent ("button release " + ev);
		dragPatch = null;
		return false;
	}

	protected override bool OnMotionNotifyEvent(Gdk.EventMotion ev) {
		//LogEvent ("motion moved " + ev);
		if (dragPatch != null) {
			dragPatch.X = ox + (ev.X - cx);
			dragPatch.Y = oy + (ev.Y - cy);
			QueueDraw ();
		}
		return false;
	}


    protected override bool OnExposeEvent (Gdk.EventExpose args) {
		Context cc = null;
		try {
	        cc = Gdk.CairoHelper.Create(args.Window);
			cc.Antialias = Antialias.Subpixel;

			//clear background
			cc.Color = Colors.LIGHT_GRAY;
			cc.Paint();

			if (firstRun) {
				foreach (var patch in this.patches)
					patch.PerformLayout (cc);
				firstRun = false;
			}

			foreach (var con in this.connections) {
				cc.Save ();
				con.Draw (cc, LogEvent);
				cc.Restore ();
			}

			foreach (var patch in this.patches) {
				cc.Save ();
				patch.Draw (cc, LogEvent);
				cc.Restore ();
			}
        } finally {
			((IDisposable)cc.Target).Dispose ();
			((IDisposable)cc).Dispose ();
		}

		//LogEvent ("Drawn");
        return true;
    }

	public LogHandler LogEvent { get; set; }
}

}
