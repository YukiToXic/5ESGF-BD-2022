using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.DancingLinks.SparkSolver.MatrixClass
{
    class MatrixNodeHead : MatrixNode
    {
        internal string name;
        internal new MatrixNode item;
        internal int size;

        public MatrixNodeHead(string s) : base(null)
        {
            name = s;
        }

    }
}
