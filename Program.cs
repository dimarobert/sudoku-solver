using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sudoku_solver {
    class Program {
        static void Main(string[] args) {

            var inputLines = File.ReadAllLines("input.txt");

            var data = new int[9, 9];

            int pos = 0;
            foreach (var line in inputLines) {
                foreach (var chr in line) {
                    if (chr == '-')
                        data[pos / 9, pos % 9] = 0;
                    else
                        data[pos / 9, pos % 9] = int.Parse(chr.ToString());
                    pos++;
                }
            }

            var cellOptions = new List<int>[9, 9];

            var stack = new Stack<(int pos, int[,] data)>();
            var cellsFilled = 0;
            while (true) {
                UpdateCellOptions(data, cellOptions);
                var min = GetMinCellOption(cellOptions);
                var cell = cellOptions[min.i, min.j];
                if (cell.Count == 0) {
                    if (IsValid(data))
                        break;

                    (pos, data) = stack.Pop();
                    UpdateCellOptions(data, cellOptions);
                    min = GetMinCellOption(cellOptions);
                    cell = cellOptions[min.i, min.j];
                    if (cell.Count > pos + 1)
                        stack.Push((pos + 1, Clone(data)));
                    data[min.i, min.j] = cell[pos];
                    cellsFilled++;
                    continue;
                }

                if (cell.Count == 1) {
                    data[min.i, min.j] = cell.First();
                    cellsFilled++;
                    continue;
                }

                stack.Push((1, Clone(data)));
                data[min.i, min.j] = cell.First();
                cellsFilled++;
            }

            Console.Clear();
            Print(data, -1, -1);
            Console.WriteLine($"Result found! Cells filled {cellsFilled}");
            Console.ReadKey();
        }

        private static bool IsValid(int[,] data) {
            var lineOptions = Enumerable.Range(1, 9).Select(_ => Enumerable.Range(1, 9).ToArray()).ToArray();
            var cellOptions = Enumerable.Range(1, 9).Select(_ => Enumerable.Range(1, 9).ToArray()).ToArray();
            var squareOptions = Enumerable.Range(1, 9).Select(_ => Enumerable.Range(1, 9).ToArray()).ToArray();
            for (int i = 0; i < 9; i++) {

                for (int j = 0; j < 9; j++) {
                    if (data[i, j] == 0)
                        return false;

                    if (lineOptions[i][data[i, j] - 1] == 0)
                        return false;
                    else lineOptions[i][data[i, j] - 1] = 0;

                    if (cellOptions[j][data[i, j] - 1] == 0)
                        return false;
                    else cellOptions[j][data[i, j] - 1] = 0;

                    if (squareOptions[(i / 3 * 3) + (j % 3)][data[i, j] - 1] == 0)
                        return false;
                    else squareOptions[(i / 3 * 3) + (j % 3)][data[i, j] - 1] = 0;
                }
            }
            return true;
        }

        private static void UpdateCellOptions(int[,] data, List<int>[,] cellOptions) {
            for (var i = 0; i < 9; i++) {
                for (var j = 0; j < 9; j++) {
                    cellOptions[i, j] = new List<int>();
                    if (data[i, j] == 0)
                        cellOptions[i, j] = GetCellOptions(data, i, j);
                }
            }
        }

        private static T[,] Clone<T>(T[,] data) {
            var dataClone = new T[9, 9];

            for (var i = 0; i < 9; i++) {
                for (var j = 0; j < 9; j++) {
                    dataClone[i, j] = data[i, j];
                }
            }
            return dataClone;
        }

        private static List<int> GetCellOptions(int[,] data, int line, int col) {
            var result = Enumerable.Range(1, 9).ToArray();

            var squareStartLine = (line / 3) * 3;
            var squareStartCol = (col / 3) * 3;
            for (var i = 0; i < 9; i++) {
                if (data[line, i] > 0)
                    result[data[line, i] - 1] = 0;
                if (data[i, col] > 0)
                    result[data[i, col] - 1] = 0;

                if (data[squareStartLine + i / 3, squareStartCol + i % 3] > 0)
                    result[data[squareStartLine + i / 3, squareStartCol + i % 3] - 1] = 0;
            }

            return result.Where(i => i > 0).ToList();
        }

        private static (int i, int j) GetMinCellOption(List<int>[,] options) {
            int i = 0, j = 0;

            for (var _i = 0; _i < 9; _i++) {
                for (var _j = 0; _j < 9; _j++) {
                    if (options[_i, _j].Count == 0)
                        continue;

                    if (options[_i, _j].Count == 1)
                        return (_i, _j);

                    if (options[i, j].Count == 0) {
                        i = _i;
                        j = _j;
                    }

                    if (options[_i, _j].Count < options[i, j].Count) {
                        i = _i;
                        j = _j;
                    }
                }
            }

            return (i, j);
        }

        private static void Print(int[,] data, int lastChangeI, int lastChangeJ) {
            for (var i = 0; i < 9; i++) {
                for (var j = 0; j < 9; j++) {
                    if (i == lastChangeI && j == lastChangeJ)
                        Console.ForegroundColor = ConsoleColor.Red;
                    if (data[i, j] == 0)
                        Console.Write($"- ");
                    else
                        Console.Write($"{data[i, j]} ");
                    Console.ResetColor();
                    if (j % 3 == 2)
                        Console.Write(" ");
                }
                Console.WriteLine();
                if (i % 3 == 2)
                    Console.WriteLine();
            }
        }
    }
}
