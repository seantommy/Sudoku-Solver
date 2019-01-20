using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Solver
{
    public partial class Form1 : Form
    {
        private int[,] gridContents = new int[9, 9];
        private int[,] mandatoryNumbers = new int[9, 9];
        private bool[,] numbersAvailable = new bool[10, 10];
        private bool solved = false;
        private BackgroundWorker puzzleSolver = new BackgroundWorker();

        public Form1()
        {
            InitializeComponent();
            BuildGrid();

            puzzleSolver.DoWork += puzzleSolver_DoWork;
            puzzleSolver.RunWorkerCompleted += puzzleSolver_Complete;
            puzzleSolver.WorkerSupportsCancellation = true;
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
            if (puzzleSolver != null && puzzleSolver.IsBusy)
            {
                return;
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (gridContents[i, j] != 0)
                    {
                        mandatoryNumbers[i, j] = gridContents[i, j];
                    }
                    else
                    {
                        mandatoryNumbers[i, j] = 0;
                        dataGrid[i, j].Style.ForeColor = Color.Blue;
                    }
                }
            }
            dataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            InitializeNumbersAvailable();
            noSolutionLabel.Visible = false;
            puzzleSolver.RunWorkerAsync();
        }

        private void ClearButtonClicked(object sender, EventArgs e)
        {
            if (puzzleSolver != null && puzzleSolver.IsBusy)
            {
                return;
            }

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    dataGrid[x, y].Value = null;
                    dataGrid[x, y].Style.ForeColor = SystemColors.ControlText;
                }
            }
        }

        private void StopButtonClicked(object sender, EventArgs e)
        {
            if (puzzleSolver.IsBusy)
            {
                puzzleSolver.CancelAsync();
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
            if (puzzleSolver != null && puzzleSolver.IsBusy)
            {
                return;
            }

            DataGridViewCell cell = dataGrid.CurrentCell;
            string key = e.KeyChar.ToString();
            if (key == "\b")
            {
                cell.Value = null;
            }
        }

        private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (puzzleSolver != null && puzzleSolver.IsBusy)
            {
                dataGrid.CancelEdit();
            }
        }

        private void puzzleSolver_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SolvePuzzle();
        }

        private void puzzleSolver_Complete(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            dataGrid.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            if (!solved)
            {
                noSolutionLabel.Visible = true;
            }
        }

        private void SolvePuzzle()
        {
            solved = AttemptToSolve(0, 0);
        }

        private bool AttemptToSolve(int x, int y)
        {
            if (puzzleSolver.CancellationPending)
            {
                return false;
            }
            bool numberPlaced = false;

            if (mandatoryNumbers[x, y] == 0)
            {
                for (int z = 1; z < 10; z++)
                {
                    if (numbersAvailable[GetCellBlockNumber(x, y), z])
                    {
                        if (CheckRowsAndColumns(x, y, z))
                        {
                            dataGrid[x, y].Value = z;
                            numbersAvailable[GetCellBlockNumber(x, y), z] = false;
                            numberPlaced = true;
                            if (x < 8)
                            {
                                numberPlaced = AttemptToSolve(x + 1, y);
                            }
                            else if (y < 8)
                            {
                                numberPlaced = AttemptToSolve(0, y + 1);
                            }

                            if (!numberPlaced)
                            {
                                numbersAvailable[GetCellBlockNumber(x, y), z] = true;
                                dataGrid[x, y].Value = null;
                            }
                        }
                    }
                    if (puzzleSolver.CancellationPending)
                    {
                        return false;
                    }
                }
            }
            else
            {
                numberPlaced = true;
                if (x < 8)
                {
                    numberPlaced = AttemptToSolve(x + 1, y);
                }
                else if (y < 8)
                {
                    numberPlaced = AttemptToSolve(0, y + 1);
                }
            }

            return numberPlaced;
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
            for (int x = 1; x < 10; x++)
            {
                for (int y = 1; y < 10; y++)
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

            for (int i = 0; i < 9; i++)
            {
                if (gridContents[x, i] == value)
                {
                    isValid = false;
                    return isValid;
                }
            }

            for (int j = 0; j < 9; j++)
            {
                if (gridContents[j, y] == value)
                {
                    isValid = false;
                    return isValid;
                }
            }

            return isValid;
        }
    }
}
