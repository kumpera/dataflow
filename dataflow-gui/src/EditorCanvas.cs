/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;

namespace Dataflow.Gui {

internal static class Constants {
	public static readonly Color LIGHT_GRAY = new Color(0.8, 0.8, 0.8);
	public static readonly Color MEDIUN_GRAY = new Color(0.6, 0.6, 0.6);
	public static readonly Color DARK_GRAY = new Color(0.2, 0.2, 0.2);
	public static readonly Color BLACK = new Color (0, 0, 0);
	public static readonly Color GREEN = new Color (0.1, 0.8, 0.1);


	public static readonly Color WIRE_COLOR = new Color (0, 0.5, 1);
}

internal abstract class CanvasWidget {
	public abstract void PerformLayout (Context cc);

	public abstract void Draw (Context cc, LogHandler log);

	public abstract bool HitTest (PointD p);
}


public class EditorCanvas : Gtk.DrawingArea {
	PatchWidget[] patches;
	ConnectionWidget[] connections;
	bool firstRun = true;

	public EditorCanvas () {
		Realized += (obj, evnt) => SetupEvents ();

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
			cc.Color = Constants.LIGHT_GRAY;
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
				cc.Translate (patch.X, patch.Y);
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
