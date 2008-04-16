/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Cairo;


namespace Dataflow.Gui {


public class ListView : Gtk.DrawingArea {
	Gtk.Viewport viewer;
	const int lines = 30;
	Gtk.VScrollbar scroll;

	public ListView (Gtk.VScrollbar scroll /*.Viewport viewer*/) {
		this.scroll = scroll;
		Realized += (obj, evnt) => SetupEvents ();
		//this.viewer = viewer;
	}

	public LogHandler LogEvent { get; set; }

	int x = 150;
	//int y = (lines + 1 ) * 20 + 1;
	int vsize = (lines + 1 ) * 20 + 1;
	Context ctx;

	void SetupEvents () {
		scroll.Adjustment.Lower = 0;
		scroll.Adjustment.Upper = vsize;
		scroll.Adjustment.StepIncrement = 1;
		scroll.Adjustment.PageSize = 100;
		scroll.Adjustment.PageIncrement = 10;
		scroll.ChangeValue += (obj, evt) => QueueDraw ();
	}

	public void DoStuff () {
		//y += 50;
		//SetSizeRequest (x, y);
	}

	void DrawHeader (double off, double length, string text) {
		ctx.Rectangle (off, 0, length, 20);
		ctx.Color = Colors.YELLOW;
		ctx.FillPreserve ();
		ctx.Color = Colors.BLACK;
		ctx.Stroke ();

		//TODO text should be center aligned
		ctx.MoveTo (off + 2, 16);
		ctx.SetFontSize (11);
		ctx.ShowText (text);
	}

	void DrawLine (int idx, string text, Color color) {
		ctx.Rectangle (0, (idx + 1) * 20 + 1, 151, 20);
		ctx.Color = color;
		ctx.Fill ();
	}


	void Dump (Gtk.Adjustment ad, string name) {
		string str = String.Format ("name {0} lower {1} page inc {2} page size {3} step inc {4} upper {5} value {6}", 
				name, ad.Lower, ad.PageIncrement, ad.PageSize, ad.StepIncrement, ad.Upper, ad.Value);
		LogEvent (str);
	}

    protected override bool OnExposeEvent (Gdk.EventExpose args) {
		try {
	        ctx = Gdk.CairoHelper.Create(args.Window);
			ctx.Antialias = Antialias.Subpixel;

			Dump (scroll.Adjustment, "scroll");
	
			//Dump (viewer.Vadjustment, "vertical adj");
			//Dump (viewer.Hadjustment, "horizontal adj");

			ctx.Save ();
			ctx.Translate (0, -scroll.Adjustment.Value);
			for (int i = 0; i < lines; ++i)
				DrawLine (i, "Line "+i, (i % 2 == 0) ? Colors.SOFT_MAGENTA : Colors.STRONG_MAGENTA);
			ctx.Restore ();

			DrawHeader (0, 75, "Category");
			DrawHeader (75, 75, "Name");
		


        } finally {
			((IDisposable)ctx.Target).Dispose ();
			((IDisposable)ctx).Dispose ();
			ctx = null;
		}

		//LogEvent ("Drawn");
        return true;
    }

	public static ListView CreateAndBind (Gtk.Paned split) {
	  		Gtk.Table table = new Gtk.Table(1, 2, false);
			table.RowSpacing = 0;
            table.ColumnSpacing = 0;

			Gtk.VScrollbar scroll = new Gtk.VScrollbar (null);
			ListView res = new ListView (scroll);

			table.Add (res);
			table.Add (scroll);


            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(table[scroll]));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.XOptions = ((Gtk.AttachOptions)(4));

			split.Add (table);
			return res;

            /*// Container child table1.Gtk.Table+TableChild
            this.drawingarea1 = new Gtk.DrawingArea();
            this.drawingarea1.Name = "drawingarea1";
            this.table1.Add(this.drawingarea1);
            // Container child table1.Gtk.Table+TableChild
            this.vscrollbar1 = new Gtk.VScrollbar(null);
            this.vscrollbar1.Name = "vscrollbar1";
            this.vscrollbar1.Adjustment.Upper = 100;
            this.vscrollbar1.Adjustment.PageIncrement = 10;
            this.vscrollbar1.Adjustment.PageSize = 10;
            this.vscrollbar1.Adjustment.StepIncrement = 1;
            this.table1.Add(this.vscrollbar1);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.table1[this.vscrollbar1]));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.XOptions = ((Gtk.AttachOptions)(4));
            this.Add(this.table1);
            if ((this.Child != null)) 
                this.Child.ShowAll();*/
	}

}

}
