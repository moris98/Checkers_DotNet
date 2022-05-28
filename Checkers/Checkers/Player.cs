using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Checkers
{
    internal class Player
    {
        private readonly ePlayerIndex r_PlayerIndex;
        private readonly List<Piece> r_Pieces = new List<Piece>();
        private List<string> m_PossibleMoves = new List<string>();
        private string m_PlayerName;
        private ePlayerType m_PlayerType = ePlayerType.Human;
        private int m_CurrentPoints = 0;

        internal Player(ePlayerIndex i_PlayerIndex)
        {
            this.r_PlayerIndex = i_PlayerIndex;
        }

        internal string PlayerName
        {
            get { return this.m_PlayerName; }
            set { this.m_PlayerName = value; }
        }

        internal ePlayerType PlayerType
        {
            get { return this.m_PlayerType; }
            set { this.m_PlayerType = value; }
        }

        internal int PossibleMovesLength
        {
            get { return this.m_PossibleMoves.Count; }
        }

        internal int CurrentPoints
        {
            get { return this.m_CurrentPoints; }
            set { this.m_CurrentPoints = value; }
        }

        internal ePlayerIndex PlayerIndex
        {
            get { return this.r_PlayerIndex; }
        }

        internal void AddPiece(Piece i_Piece)
        {
            this.r_Pieces.Add(i_Piece); // Add i_Piece to this players' piece list
        }

        internal bool RemovePiece(Piece i_Piece)
        { 
            return this.r_Pieces.Remove(i_Piece); // Remove i_Piece from this players' piece list
        }

        internal bool IsValidMove(string i_Move)
        {
            return this.m_PossibleMoves.Contains(i_Move); // Returns true if the given move is part of this player's current possible moves
        }

        internal void UpdatePossibleMoves(Piece[,] i_Board)
        {
            int currentDirection = this.r_PlayerIndex == ePlayerIndex.Second ? -1 : 1; // Set valid move direction to -1 if it's the second player, 1 otherwise

            if (this.m_PossibleMoves.Count != 0)
            {
                this.m_PossibleMoves.Clear();
            }

            foreach(Piece currentPiece in this.r_Pieces)
            {
                getPossibleMovesInDirection(i_Board, currentDirection, currentPiece.Row, currentPiece.Column); // Get all possible moves of current piece in given direction
                if (currentPiece.PieceType == Piece.ePieceType.King)
                {
                    getPossibleMovesInDirection(i_Board, currentDirection * -1, currentPiece.Row, currentPiece.Column);  // Get all possible moves of king piece in opposite direction
                }
            }
        }

        private bool isDestinationOccupied(Piece[,] i_Board, int i_Row, int i_Column)
        {
            return i_Board[i_Row, i_Column] != null;
        }

        private bool isOccupiedRival(Piece[,] i_Board, int i_Row, int i_Column)
        {
            return i_Board[i_Row, i_Column] != null && i_Board[i_Row, i_Column].PieceOwner.r_PlayerIndex != this.r_PlayerIndex; // Returns true if rival piece is in given [row, column]
        }

        private void getPossibleMovesInDirection(Piece[,] i_Board, int i_Direction, int i_Row, int i_Column)
        {
            int boardSize = i_Board.GetLength(0);
            if(i_Row + i_Direction < boardSize && i_Row + i_Direction >= 0)
            {
                if(i_Column + 1 < boardSize && !isDestinationOccupied(i_Board, i_Row + i_Direction, i_Column + 1))
                {
                    this.m_PossibleMoves.Add(string.Format("{0}{1}>{2}{3}", (char)(i_Column + 'A'), (char)(i_Row + 'a'), (char)(i_Column + 1 + 'A'), (char)(i_Row + i_Direction + 'a')));
                }

                if(i_Column - 1 >= 0 && !isDestinationOccupied(i_Board, i_Row + i_Direction, i_Column - 1))
                {
                    this.m_PossibleMoves.Add(string.Format("{0}{1}>{2}{3}", (char)(i_Column + 'A'), (char)(i_Row + 'a'), (char)(i_Column - 1 + 'A'), (char)(i_Row + i_Direction + 'a')));
                }

                if(i_Row + (2 * i_Direction) < boardSize && i_Row + (2 * i_Direction) >= 0)
                {
                    if (i_Column + 2 < boardSize && !isDestinationOccupied(i_Board, i_Row + (2 * i_Direction), i_Column + 2) && isOccupiedRival(i_Board, i_Row + i_Direction, i_Column + 1))
                    {
                        this.m_PossibleMoves.Add(string.Format("{0}{1}>{2}{3}", (char)(i_Column + 'A'), (char)(i_Row + 'a'), (char)(i_Column + 2 + 'A'), (char)(i_Row + (2 * i_Direction) + 'a')));
                    }

                    if (i_Column - 2 >= 0 && !isDestinationOccupied(i_Board, i_Row + (2 * i_Direction), i_Column - 2) && isOccupiedRival(i_Board, i_Row + i_Direction, i_Column - 1))
                    {
                        this.m_PossibleMoves.Add(string.Format("{0}{1}>{2}{3}", (char)(i_Column + 'A'), (char)(i_Row + 'a'), (char)(i_Column - 2 + 'A'), (char)(i_Row + (2 * i_Direction) + 'a')));
                    }
                }
            }
        }

        internal string GetRandomMove()
        {
            int randomIndex = new Random().Next(0, this.m_PossibleMoves.Count);
            return this.m_PossibleMoves[randomIndex]; // Return a random move from all possible moves
        }

        internal bool CheckFurtherJumps(string i_Location)
        {
            List<string> furtherMoves = new List<string>();
            foreach(string possibleMove in this.m_PossibleMoves)
            {
                string originInPossibleMove = possibleMove.Substring(0, 2); // Get origin location in string representation
                string destinationInPossibleMove = possibleMove.Substring(3); // Get destination location in string representation
                if (originInPossibleMove.Equals(i_Location) && Math.Abs(originInPossibleMove[0] - destinationInPossibleMove[0]) == 2)
                {
                    furtherMoves.Add(possibleMove); // // Check if a move starts in i_Location and ends 2 steps away. If so, add move to possible moves
                }
            }

            if(furtherMoves.Count != 0)
            {
                this.m_PossibleMoves.Clear(); // Clear all other moves
                this.m_PossibleMoves = furtherMoves; // Only keep moves that are 2 jumps away
            }

            return furtherMoves.Count != 0;
        }

        internal enum ePlayerIndex
        {
            First,
            Second
        }

        internal enum ePlayerType
        {
            Human,
            Computer
        }
    }
}
