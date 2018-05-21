using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
           
            StringBuilder inputSudoku = new StringBuilder("");
            StringBuilder consoleMessage = new StringBuilder("");
            consoleMessage.AppendLine("SudokuSolver is an application meant for solving sudoku puzzle.");
            consoleMessage.AppendLine("You can provide input sudoku by using terminal or providing a file.");
            consoleMessage.AppendLine("Please choose the way you want to input the puzzle(terminal/file).");
            consoleMessage.AppendLine("");
            Console.Write(consoleMessage);
            string result = Console.ReadLine();

            if(result=="terminal")
            {
                consoleMessage.Clear();
                consoleMessage.AppendLine("Please type input sudoku, unknown fields should have value of 0.");
                consoleMessage.AppendLine("To proceede to next row, please use enter, make sure to fill all 81 fields.");
                consoleMessage.AppendLine("");
                Console.WriteLine(consoleMessage);

                for (int i = 0; i < 9; i++)
                    inputSudoku.Append(Console.ReadLine());

                consoleMessage.Clear();
                consoleMessage.AppendLine("Input sudoku is:");
                consoleMessage.AppendLine(inputSudoku.ToString());
                Console.WriteLine(consoleMessage);
            }
            else if(result=="file")
            {
                consoleMessage.Clear();
                consoleMessage.AppendLine("Please provide the address of a text file with sudoku puzzle.");
                consoleMessage.AppendLine("");
                Console.WriteLine(consoleMessage);

                string sudokuFile = Console.ReadLine();
                if (File.Exists(sudokuFile))
                {
                    FileStream fs = new FileStream(sudokuFile, FileMode.Open, FileAccess.ReadWrite);
                    try
                    {
                        StreamReader sr = new StreamReader(fs);
                        inputSudoku.Append(sr.ReadToEnd());
                        sr.Close();

                        consoleMessage.Clear();
                        consoleMessage.AppendLine("Input sudoku is:");
                        consoleMessage.AppendLine(inputSudoku.ToString());
                        Console.WriteLine(consoleMessage);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                   
            }
            else
            {
                consoleMessage.Clear();
                consoleMessage.AppendLine("Incorrect value, exiting.");
                Console.WriteLine(consoleMessage);
                return;
            }

            StringBuilder puzzle = new StringBuilder("");
            //remove all new line characters from the string
            puzzle.Append(Regex.Replace(inputSudoku.ToString(), @"\t|\n|\r", ""));


            Field[][] sudoku = new Field[9][];
            int position = 0;
            for(int i=0; i<9; i++)
            {
                sudoku[i] = new Field[9];
                for (int j=0; j<9; j++)
                {
                    position = i * 9 + j;
                    sudoku[i][j].value = (int)char.GetNumericValue(puzzle[position]);
                    if(sudoku[i][j].value>0)
                        sudoku[i][j].changeable = false;
                    else
                        sudoku[i][j].changeable = true;
                }
            }
            Console.WriteLine();

            //start algorithm
            int row = 0;
            int col = 0;
            while(true)
            {
                if (sudoku[row][col].changeable == true)
                {
                    do
                    {
                        IncreaseValue(sudoku, ref row, ref col);
                    }
                    while (CheckConstraints(sudoku, row, col) == false);
                }

                if (col == 8 && row == 8)
                    break;
                else
                    NextCell(ref row, ref col);
            }

            //solution was found, display the answer
            Console.WriteLine();
            for(int i=0; i<9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(sudoku[i][j].value);
                }
                Console.WriteLine();
            }
           
        }

        static void IncreaseValue(Field[][] sudoku,ref int row,ref int col)
        {
            sudoku[row][col].value++;
            if (sudoku[row][col].value > 9)
            {
                sudoku[row][col].value = 0;
                do
                {
                    PrevCell(ref row, ref col);
                }
                while (sudoku[row][col].changeable == false);

                IncreaseValue(sudoku, ref row, ref col);
            }
        }

        static bool CheckConstraints(Field[][] sudoku, int row, int col)
        {
            return CheckBoxConstraint(sudoku, row, col) && CheckRowConstraint(sudoku, row, col) && CheckColConstraint(sudoku, row, col);
        }

        static bool CheckBoxConstraint(Field[][] sudoku, int row, int col)
        {
            int boxRow = (int)row / 3;
            int boxCol = (int)col / 3;
            int startRow = boxRow * 3;
            int startCol = boxCol * 3;
            int testedValue = sudoku[row][col].value;

            for(int i= startRow; i< startRow+3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if (i == row && j == col)
                        continue;
                    if (sudoku[i][j].value == testedValue)
                        return false;
                }
            }
            return true;
        }

        static bool CheckRowConstraint(Field[][] sudoku, int row, int col)
        {
            int testedValue = sudoku[row][col].value;

            for (int i = 0; i < 9 ; i++)
            {
                if (i == col)
                    continue;
                if (sudoku[row][i].value == testedValue)
                    return false;
            }
            return true;
        }

        static bool CheckColConstraint(Field[][] sudoku, int row, int col)
        {
            int testedValue = sudoku[row][col].value;

            for (int i = 0; i < 9; i++)
            {
                if (i == row)
                    continue;
                if (sudoku[i][col].value == testedValue)
                    return false;
            }
            return true;
        }

        static void NextCell(ref int row, ref int col)
        {
            if (col < 8)
            {
                col++;
            }
            else if (row < 8)
            {
                row++;
                col = 0;
            }
        }

        static void PrevCell(ref int row, ref int col)
        {
            if (col > 0)
            {
                col--;
            }
            else if (row > 0)
            {
                row--;
                col = 8;
            }
        }

        struct Field
        {
            public bool changeable;
            public int value;
        }

    }
}
