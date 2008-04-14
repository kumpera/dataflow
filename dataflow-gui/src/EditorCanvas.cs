using System;
using Cairo;

namespace Dataflow.Gui {

public delegate void LogHandler (String msg);

static class ArrayExtensions {
	public static void EachWithIndex<T> (this T[] array, Action <T, int> closuse) {
		for (int i = 0; i < array.Length; ++i)
			closuse (array [i], i);
	}
}

internal static class Constants {
	public static readonly Color LIGHT_GRAY = new Color(0.8, 0.8, 0.8);
	public static readonly Color MEDIUN_GRAY = new Color(0.6, 0.6, 0.6);
	public static readonly Color DARK_GRAY = new Color(0.2, 0.2, 0.2);
	public static readonly Color BLACK = new Color (0, 0, 0);
	public static readonly Color GREEN = new Color (0.1, 0.8, 0.1);


	public static readonly Color WIRE_COLOR = new Color (0, 0.5, 1);
}

internal static class DrawingPrimitives {
	internal static void DrawCircle (Context cc, double cx, double cy, double radius) {
		cc.NewPath ();
        cc.Arc(cx, cy, radius, 0, 2 * Math.PI);
	}

    internal static void DrawRoundedRectangle (Context cc, double x, double y, double width, double height, double arcRadius) {
        cc.MoveTo (x + arcRadius, y);
        cc.LineTo (x + width - arcRadius, y);
        cc.Arc (x + width - arcRadius, y + arcRadius, arcRadius, -Math.PI / 2 , 0);

        cc.LineTo (x + width, y + height - arcRadius);
        cc.Arc (x + width - arcRadius, y + height - arcRadius, arcRadius, 0, Math.PI / 2);

        cc.LineTo (x + arcRadius,  y + height);
        cc.Arc (x + arcRadius, y + height - arcRadius, arcRadius, Math.PI / 2, Math.PI);

        cc.LineTo (x, y + arcRadius);
        cc.Arc (x + arcRadius, y + arcRadius, arcRadius, Math.PI, -Math.PI / 2);

        cc.ClosePath();
    }


	internal static void DrawUpRoundedRectangle (Context cc, double x, double y, double width, double height, double arcRadius) {
        cc.MoveTo(x + arcRadius, y);
        cc.LineTo(x + width - arcRadius, y);
        cc.Arc(x + width - arcRadius, y + arcRadius, arcRadius, -Math.PI / 2 , 0);

        cc.LineTo(x + width, y + height);
        cc.LineTo(x,  y + height);
        cc.LineTo(x, y + arcRadius);
        cc.Arc(x + arcRadius, y + arcRadius, arcRadius, Math.PI, -Math.PI / 2);

        cc.ClosePath();
	}
}

public class PatchWidget {
	const int HEADER_HEIGHT = 20;
	const int FIRST_LINE = HEADER_HEIGHT + 10;

	const int BORDER_ROUND_SIZE = 10;
	const int HEADER_FONT_SIZE = 12;

	const int PORT_FONT_SIZE = 10;
	const int PORT_HEIGHT = 14;
	const int PORT_BORDER_SPACING = 17;
	const int PORT_INNER_SPACING = 8;

	String name;
	String[] inlets;
	String[] outlets;
	bool[] inletConnected;
	bool[] outletConnected;

	Context ctx;

	public double X { get; set; }
	public double Y { get; set; }

	public double Width { get; set; }
	public double Height { get; set; }

	public PatchWidget (String name, String[] inlets, String[] outlets) {
		this.name = name;
		this.inlets = inlets;
		this.outlets = outlets;
		this.inletConnected = new bool [inlets.Length];
		this.outletConnected = new bool [outlets.Length];
	}

	public void SetInletConnected (int idx) {
		inletConnected [idx] = true;
	}

	public void SetOutletConnected (int idx) {
		outletConnected [idx] = true;
	}

	public void Draw (Context ctx, LogHandler log) {
		this.ctx = ctx;
		//We must first calculate the layout width based on title size and inlet/outlet sizes
		DrawEnvelope ();

		DrawHeader ();

		inlets.EachWithIndex ((n, idx) => DrawInlet (idx, n));
		outlets.EachWithIndex ((n, idx) => DrawOutlet (idx, n));
	}

	public bool HitTest (double px, double py) {
		return 	px >= X && px < (X + Width) &&
				py >= Y && py < (Y + Height);
	}

	/*Returns in user space coordinates*/
	public PointD GetInletConnectionPosition (int idx) {
		return new PointD (X + 10, Y + FIRST_LINE + idx * PORT_HEIGHT);
	}

	/*Returns in user space coordinates*/
	public PointD GetOutletConnectionPosition (int idx) {
		return new PointD (X + Width - 10, Y + FIRST_LINE + idx * PORT_HEIGHT);
	}

	public void PerformLayout (Context ctx) {
		this.ctx = ctx;
		Width = CalcWidth ();
		Height = HEADER_HEIGHT + Math.Max (inlets.Length, outlets.Length) * PORT_HEIGHT + BORDER_ROUND_SIZE;
	}

	void DrawEnvelope () {
        ctx.Color = new Color(0.8, 0.8, 0.8, 0.8);
        DrawingPrimitives.DrawRoundedRectangle(ctx, 0, 0, Width, Height, BORDER_ROUND_SIZE);
		ctx.FillPreserve ();
		ctx.LineWidth = 1;
		ctx.Color = Constants.BLACK;
		ctx.Stroke ();	
	}

	void DrawHeader () {
		//Box
		ctx.Color = new Color (0.1, 0.8, 0.1, 0.6);
        DrawingPrimitives.DrawUpRoundedRectangle (ctx, 0, 0, Width, HEADER_HEIGHT, BORDER_ROUND_SIZE);
		ctx.FillPreserve ();
		ctx.LineWidth = 1;
		ctx.Color = Constants.BLACK;
		ctx.Stroke ();

		//text
		ctx.MoveTo (BORDER_ROUND_SIZE, 15);
		ctx.SetFontSize (HEADER_FONT_SIZE);
		ctx.ShowText (this.name);
	}

	void DrawInlet (int pos, string inlet) {
		double y = FIRST_LINE + pos * PORT_HEIGHT;

		ctx.Color = Constants.DARK_GRAY;
		ctx.LineWidth = 1;
		ctx.SetFontSize (PORT_FONT_SIZE);

		DrawingPrimitives.DrawCircle(ctx, 10, y, 4);
		if (inletConnected [pos]) {
			ctx.Color = Constants.WIRE_COLOR;
			ctx.FillPreserve ();
			ctx.Color = Constants.DARK_GRAY;
		}
		ctx.Stroke ();

		ctx.MoveTo (PORT_BORDER_SPACING, y + 4);
		ctx.ShowText (inlet);
	}

	void DrawOutlet (int pos, string outlet) {
		double y = FIRST_LINE + pos * PORT_HEIGHT;

		ctx.Color = Constants.DARK_GRAY;
		ctx.LineWidth = 1;
		ctx.SetFontSize (PORT_FONT_SIZE);

		DrawingPrimitives.DrawCircle(ctx, Width - 10, y, 4);
		if (outletConnected [pos]) {
			ctx.Color = Constants.WIRE_COLOR;
			ctx.FillPreserve ();
			ctx.Color = Constants.DARK_GRAY;
		}
		ctx.Stroke ();

		TextExtents te = ctx.TextExtents (outlet);
		ctx.MoveTo (Width - (PORT_BORDER_SPACING + 1) - te.Width, y + 4);
		ctx.ShowText (outlet);
	}

	//Layout stuff
	double CalcWidth () {
		double width = 0;
		//Header text must not enter the round border area

		ctx.SetFontSize (HEADER_FONT_SIZE);
		width = Math.Max (width, TextWidth (this.name) + 2 * BORDER_ROUND_SIZE);

		ctx.SetFontSize (PORT_FONT_SIZE);
		double left = 0;
		double right = 0;
		for (int i = 0; i < Math.Max (inlets.Length, outlets.Length); ++i) {
			left = Math.Max (left, CalcTextSize (inlets, i));
			right = Math.Max (right, CalcTextSize (outlets, i));
		}

		return Math.Max (width, left + right + 2 * PORT_BORDER_SPACING + PORT_INNER_SPACING);
	}

	double CalcTextSize (String[] array, int idx) {
		if (idx >= array.Length) 
			return 0;
		return TextWidth (array [idx]);
	}

	double TextWidth (String str) {
		TextExtents te = ctx.TextExtents (str);
		return te.Width;
	}
}

public class ConnectionWidget {
	PatchWidget source;
	PatchWidget dest;
	int sourcePort;
	int destPort;

	public  ConnectionWidget (PatchWidget source, int sourcePort, PatchWidget dest, int destPort) {
		this.source = source;
		this.sourcePort = sourcePort;
		this.dest = dest;
		this.destPort = destPort;
	}

	public void Draw (Context ctx, LogHandler log) {
		PointD src = source.GetOutletConnectionPosition (sourcePort);
		PointD dst = dest.GetInletConnectionPosition (destPort);

		PointD control1 = new PointD ((src.X + dst.X) / 2, src.Y);
		PointD control2 = new PointD ((src.X + dst.X) / 2, dst.Y);

		ctx.MoveTo (src);
		ctx.CurveTo (control1, control2, dst);
		ctx.Color = Constants.WIRE_COLOR;
		ctx.LineWidth = 3;
		ctx.Stroke ();
	}
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
			if (patch.HitTest (ev.X, ev.Y)) { //begin draw sequences
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
        using(Context cc = Gdk.CairoHelper.Create(args.Window)) {
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
        }

		//LogEvent ("Drawn");
        return true;
    }

	public LogHandler LogEvent { get; set; }
}

}
