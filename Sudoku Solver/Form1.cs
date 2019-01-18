using System;
using System.Collections;
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
        private int[,] mandatoryNumbers = new int[9, 9];
        private bool[,] numbersAvailable = new bool[10, 10];

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

        private void SolvePuzzle()
        {
            InitializeNumbersAvailable();
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (mandatoryNumbers[x, y] == 0)
                    {
                        for (int z = 1; z < 10; z++)
                        {
                            if (numbersAvailable[GetCellBlockNumber(x,y), z] == true)
                            {
                                if (CheckRowsAndColumns(x, y, z))
                                {
                                    dataGrid[x, y].Value = z;
                                    numbersAvailable[GetCellBlockNumber(x, y), z] = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }

        private int GetCellBlockNumber(int x, int y)
        {
            int cellBlock;

            if (x < 3)
            {
                if (y < 3)
                {
                    cellBlock = 1;
                }
                else if (y < 6)
                {
                    cellBlock = 2;
                }
                else
                {
                    cellBlock = 3;
                }
            }
            else if (x < 6)
            {
                if (y < 3)
                {
                    cellBlock = 4;
                }
                else if (y < 6)
                {
                    cellBlock = 5;
                }
                else
                {
                    cellBlock = 6;
                }
            }
            else
            {
                if (y < 3)
                {
                    cellBlock = 7;
                }
                else if (y < 6)
                {
                    cellBlock = 8;
                }
                else
                {
                    cellBlock = 9;
                }

            }

            return cellBlock;
        }

        private void InitializeNumbersAvailable()
        {
            for(int x = 1; x < 10; x++)
            {
                for(int y = 1; y < 10; y++)
                {
                    numbersAvailable[x, y] = true;
                }
            }

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    int cellBlock = GetCellBlockNumber(x, y);

                    if (mandatoryNumbers[x, y] != 0)
                    {
                        numbersAvailable[cellBlock, mandatoryNumbers[x, y]] = false;
                    }
                }
            }
        }

        private bool CheckRowsAndColumns(int x, int y, int value)
        {
            bool isValid = true;

            for(int i = 0; i < 9; i++)
            {
                if(gridContents[x,i] == value)
                {
                    isValid = false;
                    return isValid;
                }    
            }
            
            for(int j = 0; j < 9; j++)
            {
                if(gridContents[j,y] == value)
                {
                    isValid = false;
                    return isValid;
                }
            }

            return isValid;
        }

        private void SolveButtonClicked(object sender, EventArgs e)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (gridContents[x, y] != 0)
                    {
                        mandatoryNumbers[x, y] = gridContents[x, y];
                    }
                    else
                    {
                        mandatoryNumbers[x, y] = 0;
                        dataGrid[x, y].Style.ForeColor = Color.Blue;
                    }
                }
            }

            SolvePuzzle();
        }

        private void ClearButtonClicked(object sender, EventArgs e)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    dataGrid[x, y].Value = null;
                }
            }
        }

        private void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell changedCell = dataGrid[e.ColumnIndex, e.RowIndex];
            bool numberInput = false;
            if (changedCell.Value == null)
            {
                gridContents[changedCell.ColumnIndex, changedCell.RowIndex] = 0;
            }
            else
            {
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

        private void KeyPressed(object sender, KeyPressEventArgs e)
        {
            DataGridViewCell cell = dataGrid.CurrentCell;
            string key = e.KeyChar.ToString();
            if (key == "\b")
            {
                cell.Value = null;
            }
        }
    }
}
