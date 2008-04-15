/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Gtk;


namespace Dataflow.Gui {

public delegate void LogHandler (String msg);


public class EditorWindow : Gtk.Window {
	Gtk.TextView logArea;
	EditorCanvas canvas;
	PatchSelection patchSelection;
		

    void SetResize(Gtk.Paned paned, Gtk.Widget child, bool resize) {
        ((Gtk.Paned.PanedChild)(paned[child])).Resize = resize;
    }

    void AddOkButton(Gtk.HPaned leftPanel) {
        Gtk.ScrolledWindow scroll = new Gtk.ScrolledWindow ();
        scroll.ShadowType = Gtk.ShadowType.In;

		patchSelection = new PatchSelection ();
		patchSelection.Init (scroll);

		leftPanel.Add (scroll);
        SetResize(leftPanel, scroll, false);
    }

    void AddLogArea(Gtk.VPaned middlePanel) {
        Gtk.ScrolledWindow scroll = new Gtk.ScrolledWindow ();
        scroll.ShadowType = Gtk.ShadowType.In;

        logArea = new Gtk.TextView ();
		logArea.Editable = false;
        scroll.Add (logArea);
        middlePanel.Add (scroll);
    }

    void AddDrawingArea(Gtk.VPaned middlePanel) {
        canvas = new EditorCanvas ();
        middlePanel.Add (canvas);
        SetResize(middlePanel, canvas, true);
    }

    public EditorWindow (): base("Dataflow Editor") {
        DefaultWidth = 800;
        DefaultHeight = 600;

        Gtk.HPaned leftPanel = new Gtk.HPaned();
        leftPanel.Position = 180;

        AddOkButton(leftPanel);

        Gtk.VPaned middlePanel = new Gtk.VPaned();
        middlePanel.Position = 400;

        AddDrawingArea(middlePanel);
        AddLogArea(middlePanel);

        leftPanel.Add(middlePanel);

        this.Add(leftPanel);

		canvas.LogEvent = (text) => logArea.Buffer.InsertAtCursor (text +"\n");
		patchSelection.LogEvent = (text) => logArea.Buffer.InsertAtCursor (text +"\n");
        DeleteEvent += (obj, arg) => Application.Quit ();
 

   }
}
}
