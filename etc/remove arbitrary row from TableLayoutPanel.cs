//using Microsoft.VisualBasic;
//VB.NET example and translation site (http://www.developerfusion.com/tools/convert/vb-to-csharp/) suggested by Gray, Cody. http://stackoverflow.com/questions/15535214/removing-a-specific-row-in-tablelayoutpanel>. 20 Mar 2013 23:08. 25 July 2015.
// _with1 CHANGED TO thisTableLayoutPanel
// thisTableLayoutPanel CHANGED TO thisTableLayoutPanel
// insertPoint CHANGED TO atRowIndex

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

public class Form1
{


	private void Button1_Click(System.Object sender, System.EventArgs e)
	{
		Control c = default(Control);
		List<HoldingCell> tempHolding = new List<HoldingCell>();
		HoldingCell cell = default(HoldingCell);
		Int32 deleteIndex = 3;
		//the 4th row, zero based

		var thisTableLayoutPanel = TableLayoutPanel1;
		//Delete all controls on selected row
		for (Int32 col = 0; col <= thisTableLayoutPanel.ColumnCount - 1; col++) {
			c = thisTableLayoutPanel.GetControlFromPosition(column: col, row: deleteIndex);
			if (c != null) {
				thisTableLayoutPanel.Controls.RemoveByKey(c.Name);
				//remove it from the controls collection
				c.Dispose();
				//get rid of it
			}
		}

		//Temporarly Store the Positions
		for (Int32 row = deleteIndex + 1; row <= TableLayoutPanel1.RowCount - 1; row++) {
			for (Int32 col = 0; col <= TableLayoutPanel1.ColumnCount - 1; col++) {
				cell = new HoldingCell();
				cell.cntrl = thisTableLayoutPanel.GetControlFromPosition(col, row);
				//setup position for restore = current row -1
				cell.pos = new TableLayoutPanelCellPosition(col, row - 1);
				tempHolding.Add(cell);
			}
		}

		//delete the row
		thisTableLayoutPanel.RowStyles.RemoveAt(index: deleteIndex);
		//deletes the style only
		thisTableLayoutPanel.RowCount -= 1;

		//adjust control positions
		foreach (Form1.HoldingCell cell_loopVariable in tempHolding) {
			cell = cell_loopVariable;
			if (cell.cntrl != null) {
				thisTableLayoutPanel.SetCellPosition(cell.cntrl, cell.pos);
			}
		}
		tempHolding = null;

	}


	private void Button3_Click(System.Object sender, System.EventArgs e)
	{
		Int32 atRowIndex = 0;
		// insert as 1st row

		List<HoldingCell> tempHolding = new List<HoldingCell>();
		HoldingCell cell = default(HoldingCell);
		var thisTableLayoutPanel = TableLayoutPanel1;
		for (Int32 row = atRowIndex; row <= TableLayoutPanel1.RowCount - 1; row++) {
			for (Int32 col = 0; col <= TableLayoutPanel1.ColumnCount - 1; col++) {
				cell = new HoldingCell();
				cell.cntrl = thisTableLayoutPanel.GetControlFromPosition(col, row);
				//setup position for restore = current row + 1
				cell.pos = new TableLayoutPanelCellPosition(col, row + 1);
				tempHolding.Add(cell);
			}
		}

		//insert new row
		thisTableLayoutPanel.RowStyles.Insert(atRowIndex, new RowStyle(SizeType.Absolute, 30));
		thisTableLayoutPanel.RowCount += 1;

		//adjust control positions
		foreach (Form1.HoldingCell cell_loopVariable in tempHolding) {
			cell = cell_loopVariable;
			if (cell.cntrl != null) {
				thisTableLayoutPanel.SetCellPosition(cell.cntrl, cell.pos);
			}
		}
		tempHolding = null;
	}

	public struct HoldingCell
	{
		public Control cntrl;
		public TableLayoutPanelCellPosition pos;
	}

}