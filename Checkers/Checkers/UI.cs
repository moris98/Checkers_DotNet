using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers
{
    internal class UI
    {
        private static readonly Dictionary<eRequestType, string> sr_RequestTranslator = new Dictionary<eRequestType, string>() // Map eRequestType to a suitable message to show the user
              {
                  { eRequestType.UserName, "Enter player name" },
                  { eRequestType.GameType, "Enter 'c' to play against the computer and 'p' for multi player" },
                  { eRequestType.BoardSize, "Enter '6', '8', or '10' as the board size" },
                  { eRequestType.NextMove, "'s Turn" },
                  { eRequestType.Rematch, "Press 'r' for a rematch or anything else to quit" }
              };

        private static readonly Dictionary<eEndGameType, string> sr_EndGameTranslator = new Dictionary<eEndGameType, string>() // Map eEndGameType to a suitable message to show the user
              {
                  { eEndGameType.Win, "Game ended, and the winner is: " },
                  { eEndGameType.Tie, "Game ended, and it's a tie" }
              };

        private readonly Dictionary<Player, char[]> r_symbolTranslator = new Dictionary<Player, char[]>(); // Visual representations of pieces for each player

        internal UI(Player[] players)
        {
            this.r_symbolTranslator[players[0]] = new char[] { 'O', 'Q' }; // Set symbols to represent pieces of the first player
            this.r_symbolTranslator[players[1]] = new char[] { 'X', 'Z' }; // Set symbols to represent pieces of the second player
        }

        internal string GetUserInput(eRequestType i_requestType, Player i_currentPlayer = null)
        {
            if(i_currentPlayer == null)
            {
                // Generic request
                Console.WriteLine(sr_RequestTranslator[i_requestType]);
            }
            else
            {
                Console.WriteLine(string.Format("{0}{1} ({2}) : ", i_currentPlayer.PlayerName, sr_RequestTranslator[i_requestType], this.r_symbolTranslator[i_currentPlayer][0]));
            }

            string userAnswer = Console.ReadLine();
            return userAnswer;
        }

        internal void WrongInput()
        {
            Console.WriteLine("wrong input, try again");
        }

        internal void PrintBoard(Piece[,] i_Board)
        {
            int numOfRows = i_Board.GetLength(0);
            int numOfColumns = i_Board.GetLength(1);
            string rowSeperator = new string('=', 4 * numOfColumns);
            StringBuilder columnTitlesStr = new StringBuilder();
            for(int i = 0; i < numOfColumns; i++)
            {
                char currentLetter = (char)('A' + i);
                columnTitlesStr.Append(string.Format("   {0}", currentLetter));
            }

            Console.WriteLine(columnTitlesStr);
            
            for(int i = 0; i < i_Board.GetLength(0); i++)
            {
                Console.WriteLine(string.Format(" {0}=", rowSeperator));
                char rowLetter = (char)('a' + i); 
                StringBuilder rowContentStr = new StringBuilder();
                rowContentStr.Append(rowLetter + "|");
                for(int j = 0; j < i_Board.GetLength(1); j++)
                {
                    if(i_Board[i, j] != null)
                    {
                        Piece currentPiece = i_Board[i, j];
                        int indexSymbol = currentPiece.PieceType == Piece.ePieceType.Soldier ? 0 : 1;
                        rowContentStr.Append(string.Format(" {0} |", this.r_symbolTranslator[currentPiece.PieceOwner][indexSymbol])); // Get symbol by the type of piece
                    }
                    else
                    {
                        rowContentStr.Append(string.Format(" {0} |", '\0'));
                    }
                }

                Console.WriteLine(rowContentStr);
            }

            Console.WriteLine(string.Format(" {0}=", rowSeperator));
        }

        internal void PrintRivalMove(Player i_RivalPlayer, string i_RivalMove)
        {
            Console.WriteLine(string.Format("{0}'s move was ({1}) : {2}", i_RivalPlayer.PlayerName, this.r_symbolTranslator[i_RivalPlayer][0], i_RivalMove));
        }

        internal void ShowEndMessage(eEndGameType i_EndType, string i_PlayerName = "")
        {
            Console.WriteLine(string.Format("{0}{1}", sr_EndGameTranslator[i_EndType], i_PlayerName));
        }

        internal void ShowPoints(Player i_Player1, Player i_Player2)
        {
            Console.WriteLine(string.Format("{0} has {1} points", i_Player1.PlayerName, i_Player1.CurrentPoints));
            Console.WriteLine(string.Format("{0} has {1} points", i_Player2.PlayerName, i_Player2.CurrentPoints));
        }

        internal void ShowGoodByeMessage(Player i_Player1, Player i_Player2)
        {
            Console.WriteLine("Bye Bye, thanks for playing");
            ShowPoints(i_Player1, i_Player2);
            Console.ReadLine();
        }

        internal enum eRequestType
        {
            UserName,
            GameType,
            BoardSize,
            NextMove,
            Rematch
        }

        internal enum eEndGameType
        {
            Win,
            Tie
        }
    }
}
