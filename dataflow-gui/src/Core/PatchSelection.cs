/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/
using System;
using Gtk;


namespace Dataflow.Gui {


[TreeNode (ListOnly=true)]
public class PatchNode : Gtk.TreeNode {
    string category;
	string name;

    public PatchNode (string category, string name)
    {
        this.category = category;
        this.name = name;
    }

    [Gtk.TreeNodeValue (Column=0)]
    public string Category { get { return category; } }

    [Gtk.TreeNodeValue (Column=1)]
    public string Name {get { return name; } }
}


public class PatchSelection {
	NodeStore store;
	NodeView nodeView;

	public LogHandler LogEvent { get; set; }

	public PatchSelection () {

	}

	void SetupEvents () {
		nodeView.DragBegin += (obj, evt) => LogEvent ("DragBegin");
	}

	public void Init (Container panel) {
        store = new NodeStore (typeof (PatchNode));
        for (int i = 0; i  < 40; ++i) {
		    store.AddNode (new PatchNode ("The Beatles", "Yesterday"));
		    store.AddNode (new PatchNode ("Peter Gabriel", "In Your Eyes"));
		    store.AddNode (new PatchNode ("Rush", "Fly By Night"));
        }		
	

		nodeView = new NodeView (store);

		CellRendererText render = new Gtk.CellRendererText ();


		nodeView.HeadersClickable = true;
		nodeView.Reorderable = true;
        nodeView.AppendColumn ("Category", render, "text", 0);
        nodeView.AppendColumn ("Name", render, "text", 1);

		nodeView.Realized += (obj, evnt) => SetupEvents ();

        Gtk.ScrolledWindow scroll = new Gtk.ScrolledWindow ();
        scroll.ShadowType = Gtk.ShadowType.In;
		scroll.Add (nodeView);
		panel.Add (scroll);
	}
}

}
