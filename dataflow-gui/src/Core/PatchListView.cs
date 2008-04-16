/*
* Copyright © 2008 The Dataflow Team
*
* See AUTHORS and LICENSE for details.
*/

using System;
using System.Collections.Generic;
/*using Cairo;
using Gtk;*/

using Hyena.Gui;
using Hyena.Collections;
using Hyena.Gui.Theming;
using Hyena.Gui.Theatrics;
using Hyena.Data.Gui;
using Hyena.Data;


namespace Dataflow.Gui {


public class PatchInfo {
	public String Category { get; set; }
	public String Name { get; set; }
}

public class PatchListModel : IListModel<PatchInfo>, ISortable
{
	PatchInfo[] model;
	Selection sel = new Selection ();	

    public event EventHandler Cleared;
    public event EventHandler Reloaded;

	public PatchListModel () {
		Reload ();
	}

    public void Clear () {
		model = new PatchInfo [0];
		OnCleared ();
	}

	public void Reload () {
		model = new PatchInfo [50];
		for (int i = 0; i < 50; ++i) {
			model [i] = new PatchInfo ();
			model [i].Category = (i % 2) == 0 ? "Bar" : "Foo";
			model [i].Name = "item "+i;
		}
		OnReloaded ();
	}
  
    public PatchInfo this[int index] { get { return model [index]; } }
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
		Console.WriteLine ("--------------------- {0}", column);
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

public class PatchListView : ListView<PatchInfo> {

    private ColumnController columnController;

    public PatchListView ()
    {
        columnController = new ColumnController ();

        SortableColumn categoryColumn = new SortableColumn ("Category", new ColumnCellText ("Category", true), 0.225, "Category", true);
        SortableColumn nameColumn = new SortableColumn ("Name", new ColumnCellText ("Name", true), 0.225, "Name", true);

        columnController.AddRange (categoryColumn, nameColumn);
        
        //columnController.Load ();
        
        ColumnController = DefaultColumnController;
        ColumnController.DefaultSortColumn = categoryColumn;

        RulesHint = true;
        RowSensitivePropertyName = "Category";
       
        //ForceDragSourceSet = true;
        Reorderable = false;
    }

/*    protected override bool OnPopupMenu ()
    {
        //ServiceManager.Get<InterfaceActionService> ().TrackActions["TrackContextMenuAction"].Activate ();
        return true;
    }*/
        
#region Drag and Drop

/*    protected override void OnDragSourceSet ()
    {
        base.OnDragSourceSet ();
        Drag.SourceSetIconName (this, "audio-x-generic");
    }

    protected override void OnDragDataGet (Gdk.DragContext context, SelectionData selection_data, uint info, uint time)
    {
        if (info != (int)ListViewDragDropTarget.TargetType.ModelSelection || Selection.Count <= 0) {
            return;
        }
    }*/

#endregion
    
    public ColumnController DefaultColumnController {
        get { return columnController; }
    }
}
}

