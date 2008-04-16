/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;

namespace Dataflow.Gui {

internal class PatchWidget : CanvasWidget {
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

	public override void Draw (Context ctx, LogHandler log) {
		this.ctx = ctx;
		ctx.Translate (X, Y);
		DrawEnvelope ();
		DrawHeader ();

		inlets.EachWithIndex ((n, idx) => DrawInlet (idx, n));
		outlets.EachWithIndex ((n, idx) => DrawOutlet (idx, n));
	}

	public override bool HitTest (PointD p) {
		return 	p.X >= X && p.X < (X + Width) &&
				p.Y >= Y && p.Y < (Y + Height);
	}


	public override void PerformLayout (Context ctx) {
		this.ctx = ctx;
		Width = CalcWidth ();
		Height = HEADER_HEIGHT + Math.Max (inlets.Length, outlets.Length) * PORT_HEIGHT + BORDER_ROUND_SIZE;
	}


	/*Returns in user space coordinates*/
	public PointD GetInletConnectionPosition (int idx) {
		return new PointD (X + 10, Y + FIRST_LINE + idx * PORT_HEIGHT);
	}

	/*Returns in user space coordinates*/
	public PointD GetOutletConnectionPosition (int idx) {
		return new PointD (X + Width - 10, Y + FIRST_LINE + idx * PORT_HEIGHT);
	}

	void DrawEnvelope () {
        ctx.Color = new Color (0.8, 0.8, 0.8, 0.8);
        DrawingPrimitives.DrawRoundedRectangle(ctx, 0, 0, Width, Height, BORDER_ROUND_SIZE);
		ctx.FillPreserve ();
		ctx.LineWidth = 1;
		ctx.Color = Colors.BLACK;
		ctx.Stroke ();	
	}

	void DrawHeader () {
		//Box
		ctx.Color = new Color (0.1, 0.8, 0.1, 0.6);
        DrawingPrimitives.DrawUpRoundedRectangle (ctx, 0, 0, Width, HEADER_HEIGHT, BORDER_ROUND_SIZE);
		ctx.FillPreserve ();
		ctx.LineWidth = 1;
		ctx.Color = Colors.BLACK;
		ctx.Stroke ();

		//text
		ctx.MoveTo (BORDER_ROUND_SIZE, 15);
		ctx.SetFontSize (HEADER_FONT_SIZE);
		ctx.ShowText (this.name);
	}

	void DrawInlet (int pos, string inlet) {
		double y = FIRST_LINE + pos * PORT_HEIGHT;

		ctx.Color = Colors.DARK_GRAY;
		ctx.LineWidth = 1;
		ctx.SetFontSize (PORT_FONT_SIZE);

		DrawingPrimitives.DrawCircle(ctx, 10, y, 4);
		if (inletConnected [pos]) {
			ctx.Color = Colors.WIRE_COLOR;
			ctx.FillPreserve ();
			ctx.Color = Colors.DARK_GRAY;
		}
		ctx.Stroke ();

		ctx.MoveTo (PORT_BORDER_SPACING, y + 4);
		ctx.ShowText (inlet);
	}

	void DrawOutlet (int pos, string outlet) {
		double y = FIRST_LINE + pos * PORT_HEIGHT;

		ctx.Color = Colors.DARK_GRAY;
		ctx.LineWidth = 1;
		ctx.SetFontSize (PORT_FONT_SIZE);

		DrawingPrimitives.DrawCircle(ctx, Width - 10, y, 4);
		if (outletConnected [pos]) {
			ctx.Color = Colors.WIRE_COLOR;
			ctx.FillPreserve ();
			ctx.Color = Colors.DARK_GRAY;
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

}
