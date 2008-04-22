/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;

namespace Dataflow.Gui {

internal class ConnectionWidget  : CanvasWidget {
	PatchWidget source;
	PatchWidget dest;
	int sourcePort;
	int destPort;

	public ConnectionWidget (PatchWidget source, int sourcePort, PatchWidget dest, int destPort) {
		this.source = source;
		this.sourcePort = sourcePort;
		this.dest = dest;
		this.destPort = destPort;
	}

	public override void PerformLayout (Context ctx) {
	}

	public override void Draw (Context ctx, LogHandler log) {
		DrawConnection (ctx, 
			source.GetOutletConnectionPosition (sourcePort),
			dest.GetInletConnectionPosition (destPort), Colors.WIRE_COLOR);
	}

	public static void DrawConnection (Context ctx, PointD src, PointD dst, Color color) {
		PointD control1 = new PointD ((src.X + dst.X) / 2, src.Y);
		PointD control2 = new PointD ((src.X + dst.X) / 2, dst.Y);

		ctx.MoveTo (src);
		ctx.CurveTo (control1, control2, dst);
		ctx.Color = color;
		ctx.LineWidth = 3;
		ctx.Stroke ();
	}
}

}

