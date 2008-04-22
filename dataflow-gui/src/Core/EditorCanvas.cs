/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;
using System.Text;
using C5;

using Dataflow.Core;

namespace Dataflow.Gui {

internal abstract class CanvasWidget {
	public abstract void PerformLayout (Context cc);

	public abstract void Draw (Context cc, LogHandler log);
}


public class EditorCanvas : Gtk.DrawingArea {
	ArrayList<PatchWidget> patches = new ArrayList<PatchWidget> ();
	ArrayList<PatchWidget> newPatches = new ArrayList<PatchWidget> ();
	ArrayList<ConnectionWidget> connections = new ArrayList<ConnectionWidget> ();

	bool firstRun = true;
	PatchRepository repo;

	public EditorCanvas (PatchRepository repo) {
		Realized += (obj, evnt) => SetupEvents ();
		this.repo = repo;

		var pt0 =  new PatchWidget (
			"HSL Color",
			new String[] { "Hue", "Saturation", "Luminosity"},
			new String[] { "Color" });
		pt0.X = 40;
		pt0.Y = 40;
		patches.Add (pt0);

		var pt1 = new PatchWidget (
			"Round",
			new String[] { "Value"},
			new String[] { "Rounded Value", "Floor", "Ceil Value" });
		pt1.X = 200;
		pt1.Y = 200;
		patches.Add (pt1);

		Connect (pt0, 0, pt1, 0);
	}

	void Connect (PatchWidget source, int sourcePort, PatchWidget dest, int destPort) {
		ConnectionWidget con = new ConnectionWidget (source, sourcePort, dest, destPort);
		source.SetOutletConnected (sourcePort);
		dest.SetInletConnected (destPort);
		connections.Add (con);
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

	protected override void OnDragDataReceived (Gdk.DragContext context, int x, int y, Gtk.SelectionData data, uint info, uint time) {
		var patchName = new string (Encoding.UTF8.GetChars (data.Data));
		var meta = repo.GetPatchByName (patchName);
		var ins = new string [meta.Inlets.Length];
		var outs = new string [meta.Outlets.Length];

		meta.Inlets.EachWithIndex ((metaIn, idx) => ins [idx] = metaIn.Name); 
		meta.Outlets.EachWithIndex ((metaOut, idx) => outs [idx] = metaOut.Name); 

		var pt =  new PatchWidget (meta.Name, ins, outs);
		pt.X = x;
		pt.Y = y;
		newPatches.Add (pt);

		Console.WriteLine("arg string data is {0} x {1} y {2}", patchName, x, y);
		Console.WriteLine ("--- DONE --- ");

		Gtk.Drag.Finish (context, true, false, time);
		QueueDraw ();
	}

	int sourcePort = -1;
	PatchWidget dragPatch;
	//TODO replace this crap with a Point type
	double ox, oy;
	double cx, cy;

	protected override bool OnButtonPressEvent (Gdk.EventButton ev) {
		//LogEvent ("button pressed x: " + ev.X + " y " + ev.Y);
		PointD click = new PointD (ev.X, ev.Y);
		for (var i = patches.Count - 1; i >= 0; --i) {
			var patch = patches [i];
			if (!patch.HitTest (click))
				continue; //begin draw sequences

			sourcePort = patch.OutletHitTest (click);
			dragPatch = patch;
			cx = ev.X;
			cy = ev.Y;
			ox = patch.X;
			oy = patch.Y;
			break;
		}
	
		return false;
	}

	protected override bool OnButtonReleaseEvent(Gdk.EventButton ev) {
		if (sourcePort >= 0) {
			PointD click = new PointD (cx, cy);
			foreach (var patch in this.patches) {
				var idx = -1;
				if (dragPatch == patch || !patch.HitTest (click) || (idx = patch.InletHitTest (click)) < 0)
					continue;
				Connect (dragPatch, sourcePort, patch, idx);
				break;
			}
		}

		dragPatch = null;
		sourcePort = -1;
		QueueDraw ();
		return false;
	}

	protected override bool OnMotionNotifyEvent(Gdk.EventMotion ev) {
		if (dragPatch != null) {
			if (sourcePort < 0) {
				dragPatch.X = ox + (ev.X - cx);
				dragPatch.Y = oy + (ev.Y - cy);
			} else {
				cx = ev.X;
				cy = ev.Y;
			}
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

			if (newPatches.Count > 0) {
				foreach (var patch in this.newPatches)
					patch.PerformLayout (cc);
				patches.AddAll (newPatches);
				newPatches.Clear ();
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

			if (sourcePort >= 0) {
				Color color = Colors.TEMP_WIRE_COLOR;
				PointD click = new PointD (cx, cy);
				foreach (var patch in this.patches) {
					var idx = -1;
					if (dragPatch == patch || !patch.HitTest (click) || (idx = patch.InletHitTest (click)) < 0)
						continue;
					color = Colors.WIRE_COLOR;
					break;
				}

				ConnectionWidget.DrawConnection (
					cc,
					dragPatch.GetOutletConnectionPosition (sourcePort), new PointD (cx, cy), color);
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
