/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Collections.Generic;
using System.Text;

using Hyena.Gui;
using Hyena.Collections;
using Hyena.Gui.Theming;
using Hyena.Gui.Theatrics;
using Hyena.Data.Gui;
using Hyena.Data;

using Dataflow.Core;

namespace Dataflow.Gui {


public class PatchInfo {
	public String Category { get; set; }
	public String Name { get; set; }
}

public class PatchListModel : IListModel<PatchInfo>, ISortable
{
	PatchInfo[] model;
	Selection sel = new Selection ();
	PatchRepository repo;

    public event EventHandler Cleared;
    public event EventHandler Reloaded;

	public PatchListModel (PatchRepository repo) {
		this.repo = repo;
		Reload ();
	}

    public void Clear () {
		model = new PatchInfo [0];
		OnCleared ();
	}

	public void Reload () {
		repo.Init ();
		model = new PatchInfo [repo.Count];
		var i = 0;
		foreach (var patch in repo) {
			model [i] = new PatchInfo ();
			model [i].Name = patch.Name;
			model [i].Category = patch.Category;
			++i;
		}

		/*for (i = 0; i < 50; ++i) {
			model [i] = new PatchInfo ();
			model [i].Category = (i % 2) == 0 ? "Bar" : "Foo";
			model [i].Name = "item "+i;
		}*/
		OnReloaded ();
	}
  
    public PatchInfo this [int index] { get { return model [index]; } }
    public int Count { get { return model.Length; } }

    protected virtual void OnCleared ()
    {
        EventHandler handler = Cleared;
        if(handler != null)
        	handler(this, EventArgs.Empty);
    }
    
    protected virtual void OnReloaded ()
    {
        EventHandler handler = Reloaded;
        if(handler != null) 
            handler(this, EventArgs.Empty);
    }

	public Selection Selection { get { return sel; } }

   	public void Sort (ISortableColumn column) {
		Array.Sort (model, new PatchInfoComparer (column.SortKey, column.SortType == SortType.Ascending));
	}

    public ISortableColumn SortColumn { get; private set; }
}

public class PatchInfoComparer : IComparer<PatchInfo> {
	string column;
	bool asc;

	public PatchInfoComparer (string column, bool asc) {
		this.column = column;
		this.asc = asc;
	}

	public int Compare (PatchInfo a, PatchInfo b) {
		string sa, sb;
		if (column == "Name") {
			sa = a.Name;
			sb = b.Name;
		} else {
			sa = a.Category;
			sb = b.Category;
		}

		if (!asc) {
			string tmp = sa;
			sa = sb;
			sb = sa;
		}
		return sa.CompareTo (sb);
	}
}

}
