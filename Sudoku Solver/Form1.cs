using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Solver
{
    public partial class Form1 : Form
    {
        private int[,] gridContents = new int[9, 9];

        public Form1()
        {
            InitializeComponent();
            BuildGrid();
        }

        private void BuildGrid()
        {
            dataGrid.BackgroundColor = SystemColors.Control;
            dataGrid.AutoSize = true;
            for (int x = 0; x < 9; x++)
            {
                DataGridViewTextBoxColumn newColumn = new DataGridViewTextBoxColumn();
                newColumn.Width = 70;
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.Height = 70;

                dataGrid.Columns.Add(newColumn);
                dataGrid.Rows.Add(newRow);

            }
        }

        private void SolveButtonClicked(object sender, EventArgs e)
        {

        }

        private void ClearButtonClicked(object sender, EventArgs e)
        {

        }

        private void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell changedCell = dataGrid.CurrentCell;
            bool numberInput = false;

            for (int x = 1; x < 10; x++)
            {
                if (changedCell.Value.ToString() == x.ToString())
                {
                    numberInput = true;
                }
            }

            if (numberInput)
            {
                gridContents[changedCell.ColumnIndex, changedCell.RowIndex] = Int16.Parse(changedCell.Value.ToString());
            }
            else
            {
                if (gridContents[changedCell.ColumnIndex, changedCell.RowIndex] != 0)
                {
                    changedCell.Value = gridContents[changedCell.ColumnIndex, changedCell.RowIndex];
                }
                else
                {
                    changedCell.Value = "";
                }
            }

        }
    }
}
