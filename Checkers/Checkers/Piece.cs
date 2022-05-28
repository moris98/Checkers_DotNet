using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers
{
    internal class Piece
    {
        private readonly Player r_PieceOwner;
        private int m_Row;
        private int m_Column;
        private ePieceType m_PieceType = ePieceType.Soldier;

        internal Piece(int i_InitialRow, int i_InitialColumn, Player i_PieceOwner)
        {
            this.m_Row = i_InitialRow;
            this.m_Column = i_InitialColumn;
            this.r_PieceOwner = i_PieceOwner;
        }

        internal int Row
        {
            get { return this.m_Row; }
            set { this.m_Row = value; }
        }

        internal int Column
        {
            get { return this.m_Column; }
            set { this.m_Column = value; }
        }

        internal ePieceType PieceType
        {
            get { return this.m_PieceType; }
        }

        internal Player PieceOwner
        {
            get { return this.r_PieceOwner; }
        }

        internal void CrownKing()
        {
            this.m_PieceType = ePieceType.King;
        }

        internal enum ePieceType
        {
            Soldier,
            King
        }
    }
}
