using System;
using Cairo;

namespace Dataflow.Gui {

public class EditorCanvas : Gtk.DrawingArea {
    protected override bool OnExposeEvent(Gdk.EventExpose args) {
        using(Context g = Gdk.CairoHelper.Create(args.Window)) {
            int x, y, width, height, depth;
            GdkWindow.GetGeometry(out x, out y, out width, out height, out depth);
            Draw(g, x, y, width, height, depth);
        }

        return true;
    }

	static Color LIGHT_GRAY = new Color(0.8, 0.8, 0.8);
	static Color MEDIUN_GRAY = new Color(0.6, 0.6, 0.6);
	static Color DARK_GRAY = new Color(0.2, 0.2, 0.2);
	static Color BLACK = new Color (0, 0, 0);
	static Color GREEN = new Color (0.1, 0.8, 0.1);

    void Draw(Context cc, int x, int y, int width, int height, int depth) {
        //fill background
        cc.Color = LIGHT_GRAY;
        cc.Paint();

		//Patch envolope
        cc.Color = LIGHT_GRAY;
        DrawRoundedRectangle(cc, 40, 40, 120, 80, 10);
		cc.FillPreserve ();
		cc.LineWidth = 1;
		cc.Color = BLACK;
		cc.Stroke ();

		//Patch header
		cc.Color = GREEN;
        DrawPatchHeader (cc, 40, 40, 120, 20, 10);
		cc.FillPreserve ();
		cc.LineWidth = 1;
		cc.Color = BLACK;
		cc.Stroke ();

		//Header text
		cc.MoveTo (50, 55);
		cc.SetFontSize (12);
		cc.ShowText ("HSL Color");

		DrawInlet(cc, 50, 70, "Hue");

		DrawInlet(cc, 50, 84, "Saturation");

		DrawInlet(cc, 50, 98, "Luminosity");

		DrawInlet(cc, 50, 112, "Alpha");

		DrawOutlet (cc, 150, 70, "Color");
	}

	void DrawInlet (Context cc, double x, double y, string name) {
		cc.Color = DARK_GRAY;
		cc.LineWidth = 1;
		cc.SetFontSize (10);

		DrawCircle(cc, x, y, 4);
		cc.Stroke ();

		cc.MoveTo (x + 7, y + 4);
		cc.ShowText (name);
	}

	void DrawOutlet (Context cc, double x, double y, string name) {
		cc.Color = DARK_GRAY;
		cc.LineWidth = 1;
		cc.SetFontSize (10);

		DrawCircle(cc, x, y, 4);
		cc.Stroke ();

		TextExtents te = cc.TextExtents (name);
		cc.MoveTo (x - 8 - te.Width, y + 4);
		cc.ShowText (name);
	}


	void DrawCircle (Context cc, double cx, double cy, double radius) {
		cc.NewPath ();
        cc.Arc(cx, cy, radius, 0, 2 * Math.PI);

	}

	void DrawPatchHeader (Context cc, double x, double y, double width, double height, double arcRadius) {
        cc.MoveTo(x + arcRadius, y);
        cc.LineTo(x + width - arcRadius, y);
        cc.Arc(x + width - arcRadius, y + arcRadius, arcRadius, -Math.PI / 2 , 0);

        cc.LineTo(x + width, y + height);
        cc.LineTo(x,  y + height);
        cc.LineTo(x, y + arcRadius);
        cc.Arc(x + arcRadius, y + arcRadius, arcRadius, Math.PI, -Math.PI / 2);

        cc.ClosePath();
	}


    void DrawRoundedRectangle (Context cc, double x, double y, double width, double height, double arcRadius) {
        cc.MoveTo(x + arcRadius, y);
        cc.LineTo(x + width - arcRadius, y);
        cc.Arc(x + width - arcRadius, y + arcRadius, arcRadius, -Math.PI / 2 , 0);

        cc.LineTo(x + width, y + height - arcRadius);
        cc.Arc(x + width - arcRadius, y + height - arcRadius, arcRadius, 0, Math.PI / 2);

        cc.LineTo(x + arcRadius,  y + height);
        cc.Arc(x + arcRadius, y + height - arcRadius, arcRadius, Math.PI / 2, Math.PI);

        cc.LineTo(x,  y + arcRadius);
        cc.Arc(x + arcRadius, y + arcRadius, arcRadius, Math.PI, -Math.PI / 2);

        cc.ClosePath();
    }
}

}
