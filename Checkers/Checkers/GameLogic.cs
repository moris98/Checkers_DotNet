using System;
using System.Text.RegularExpressions;

namespace Checkers
{
    internal class GameLogic
    {
        private readonly Player[] r_Players = { new Player(Player.ePlayerIndex.First), new Player(Player.ePlayerIndex.Second) }; // Initialize players
        private readonly UI r_UIManager;
        private int m_BoardSize;
        private Piece[,] m_Board;

        internal GameLogic()
        {
            this.r_UIManager = new UI(this.r_Players);
        }
        
        internal void startGame()
        {
            bool rematch = true;
            getUserName(this.r_Players[0]); // Get the first player name
            getBoardSize(); // Get the desired board size
            GetTypeOfGame(); // Get the type of game - multi player / vs computer
            while (rematch)
            { // Iterate while rematch is requested
                rematch = false;
                initializeBoard(); // Initialize board
                foreach (Player p in this.r_Players)
                {
                    p.UpdatePossibleMoves(this.m_Board); // Update initial moves possible for each player
                }

                //Ex02.ConsoleUtils.Screen.Clear();
                this.r_UIManager.PrintBoard(this.m_Board); // Print initial state of board
                playGame(ref rematch); // Start game logic
            }

            this.r_UIManager.ShowGoodByeMessage(this.r_Players[0], this.r_Players[1]); // Show quitting message when the user has not chosen a rematch
        }

        private void getUserName(Player i_CurrentPlayer)
        {
            string playerName = this.r_UIManager.GetUserInput(UI.eRequestType.UserName); // Ask for user name
            if(!Regex.Match(playerName, "^[A-Za-z]{1,10}$").Success)
            { // Check if user name contain anything else than english letters or has more than 10 letters
                this.r_UIManager.WrongInput(); // Alert user that the input is invalid
                getUserName(i_CurrentPlayer); // Ask again for user name
            }
            else
            {
                i_CurrentPlayer.PlayerName = playerName;
            }
        }

        private void getBoardSize()
        {
            string boardSizeStr = this.r_UIManager.GetUserInput(UI.eRequestType.BoardSize); // Ask for board size
            bool isInteger = int.TryParse(boardSizeStr, out int boardSize);
            if (!isInteger || (boardSize != 6 && boardSize != 8 && boardSize != 10))
            {
                this.r_UIManager.WrongInput(); // Alert user that the input is invalid
                getBoardSize(); // Ask again for board size
            }
            else
            {
                this.m_BoardSize = boardSize;
            }
        }

        private void initializeBoard()
        {
            Piece pieceToBeAdded;
            this.m_Board = new Piece[this.m_BoardSize, this.m_BoardSize]; // Initialize board as 2D Piece array
            int linesPerPlayer = (int)((this.m_BoardSize - 2) / 2);
            for(int i = 0; i < linesPerPlayer; i++)
            {
                for(int j = 0; j < this.m_BoardSize; j++)
                {
                    if(i % 2 == 0)
                    {
                        if(j % 2 == 0)
                        {
                            pieceToBeAdded = new Piece(this.m_BoardSize - i - 1, j, this.r_Players[1]); // Second player piece location
                            this.m_Board[this.m_BoardSize - i - 1, j] = pieceToBeAdded;
                            this.r_Players[1].AddPiece(pieceToBeAdded); // Add piece to second player piece list
                        }
                        else
                        {
                            pieceToBeAdded = new Piece(i, j, this.r_Players[0]); // First player piece location
                            this.m_Board[i, j] = pieceToBeAdded;
                            this.r_Players[0].AddPiece(pieceToBeAdded); // Add piece to first player piece list
                        }
                    }
                    else
                    {
                        if (j % 2 == 0)
                        {
                            pieceToBeAdded = new Piece(i, j, this.r_Players[0]); // First player piece location
                            this.m_Board[i, j] = pieceToBeAdded;
                            this.r_Players[0].AddPiece(pieceToBeAdded); // Add piece to first player piece list
                        }
                        else
                        {
                            pieceToBeAdded = new Piece(this.m_BoardSize - i - 1, j, this.r_Players[1]); // Second player piece location
                            this.m_Board[this.m_BoardSize - i - 1, j] = pieceToBeAdded;
                            this.r_Players[1].AddPiece(pieceToBeAdded); // Add piece to second player piece list
                        }
                    }
                }
            }
        }

        private void GetTypeOfGame()
        {
            string gameTypeRequested = this.r_UIManager.GetUserInput(UI.eRequestType.GameType); // Ask for game type - 'p' for multi player and 'c' to play versus the computer
            if (gameTypeRequested.Equals("c"))
            {
                this.r_Players[1].PlayerType = Player.ePlayerType.Computer;
                this.r_Players[1].PlayerName = "Computer";
            }
            else if (gameTypeRequested.Equals("p"))
            {
                getUserName(this.r_Players[1]); // In case of multi player, get second player's name
            }
            else
            {
                this.r_UIManager.WrongInput(); // Alert user that the input is invalid
                GetTypeOfGame(); // Ask for game type
            }
        }

        private void playGame(ref bool o_Rematch)
        {
            int indexOfCurrentPlayer = 0;
            Player currentPlayer = this.r_Players[0];
            bool quitGame = false;
            bool forfeitedGame = false;
            while (!quitGame)
            {
                bool ateRival = false;
                currentPlayer = this.r_Players[indexOfCurrentPlayer];
                string currentMove = string.Empty;
                makeNextStep(currentPlayer, ref currentMove, ref forfeitedGame, ref ateRival);
                if(!forfeitedGame)
                {
                    //Ex02.ConsoleUtils.Screen.Clear();
                    this.r_UIManager.PrintBoard(this.m_Board);
                    this.r_UIManager.PrintRivalMove(currentPlayer, currentMove);
                    this.r_Players[0].UpdatePossibleMoves(this.m_Board);
                    this.r_Players[1].UpdatePossibleMoves(this.m_Board);
                    if(!(ateRival && currentPlayer.CheckFurtherJumps(currentMove.Substring(3))))
                    {
                        indexOfCurrentPlayer = (indexOfCurrentPlayer + 1) % 2;
                    }
                }

                quitGame = forfeitedGame || checkIfGameEnded();
            }

            this.calcPoints(forfeitedGame, currentPlayer); // Calc points for each player by current board state
            this.r_UIManager.ShowPoints(r_Players[0], r_Players[1]); // Present points of each user
            string userRematchReply = this.r_UIManager.GetUserInput(UI.eRequestType.Rematch); // Ask for rematch user input
            if(userRematchReply.Equals("r"))
            {
                o_Rematch = true;
            }
        }

        private bool checkIfGameEnded()
        {
            bool gameEnded = false;
            if(this.r_Players[0].PossibleMovesLength == 0)
            {
                if(this.r_Players[1].PossibleMovesLength == 0)
                {
                    // It's a tie, both players don't have valid moves
                    gameEnded = true;
                    this.r_UIManager.ShowEndMessage(UI.eEndGameType.Tie);
                }
                else
                {
                    // Player 2 won - player 1 doesn't have valid moves
                    gameEnded = true;
                    this.r_UIManager.ShowEndMessage(UI.eEndGameType.Win, this.r_Players[1].PlayerName);
                }
            }
            else if(r_Players[1].PossibleMovesLength == 0)
            {
                // Player 1 won - player 2 doesn't have valid moves
                gameEnded = true;
                this.r_UIManager.ShowEndMessage(UI.eEndGameType.Win, this.r_Players[0].PlayerName);
            }

            return gameEnded;
        }

        private void makeNextStep(Player i_CurrentPlayer, ref string o_LastMove, ref bool o_QuitGame, ref bool o_AteRival)
        {
            string nextStep = string.Empty;
            if (i_CurrentPlayer.PlayerType == Player.ePlayerType.Computer)
            {
                nextStep = i_CurrentPlayer.GetRandomMove(); // If players is computer, get random move from possible moves
            }
            else
            {
                nextStep = this.r_UIManager.GetUserInput(UI.eRequestType.NextMove, i_CurrentPlayer); // Get next move from user
            }

            if(nextStep == "Q")
            {
                o_QuitGame = true; // The user has chosen to forfeit the game
                return;
            }

            if (i_CurrentPlayer.IsValidMove(nextStep))
            {
                string initialSpot = nextStep.Substring(0, 2); 
                string nextSpot = nextStep.Substring(3);
                int[] initialSpotArray = { initialSpot[1] - 'a', initialSpot[0] - 'A' }; // [row, column]
                int[] destinationSpotArray = { nextSpot[1] - 'a', nextSpot[0] - 'A' }; // [row, column]
                this.m_Board[initialSpotArray[0], initialSpotArray[1]].Row = destinationSpotArray[0];
                this.m_Board[initialSpotArray[0], initialSpotArray[1]].Column = destinationSpotArray[1];
                this.m_Board[destinationSpotArray[0], destinationSpotArray[1]] = this.m_Board[initialSpotArray[0], initialSpotArray[1]];

                if ((destinationSpotArray[0] == this.m_BoardSize - 1 && i_CurrentPlayer.PlayerIndex == Player.ePlayerIndex.First) || (destinationSpotArray[0] == 0 && i_CurrentPlayer.PlayerIndex == Player.ePlayerIndex.Second))
                {
                    this.m_Board[destinationSpotArray[0], destinationSpotArray[1]].CrownKing();
                }

                this.m_Board[initialSpotArray[0], initialSpotArray[1]] = null;
                if(Math.Abs(destinationSpotArray[0] - initialSpotArray[0]) == 2)
                {
                    // Stepped over
                    Player rival = i_CurrentPlayer.PlayerIndex == Player.ePlayerIndex.First ? this.r_Players[1] : this.r_Players[0];
                    Piece pieceToDelete = this.m_Board[(destinationSpotArray[0] + initialSpotArray[0]) / 2, (destinationSpotArray[1] + initialSpotArray[1]) / 2];
                    rival.RemovePiece(pieceToDelete);
                    this.m_Board[(destinationSpotArray[0] + initialSpotArray[0]) / 2, (destinationSpotArray[1] + initialSpotArray[1]) / 2] = null;
                    o_AteRival = true;
                }

                o_LastMove = nextStep;
            }
            else
            {
                this.r_UIManager.WrongInput();
                makeNextStep(i_CurrentPlayer, ref o_LastMove, ref o_QuitGame, ref o_AteRival);
            }
        }

        private void calcPoints(bool i_ForfeitedGame, Player i_CurrentPlayer)
        {
            Piece currentPiece;

            for(int i = 0; i < this.m_Board.GetLength(0); i++)
            {
                for(int j = 0; j < this.m_Board.GetLength(1); j++)
                {
                    if(this.m_Board[i, j] != null)
                    {
                        currentPiece = this.m_Board[i, j];

                        if(!(i_ForfeitedGame && i_CurrentPlayer == currentPiece.PieceOwner))
                        {
                            if(currentPiece.PieceType == Piece.ePieceType.Soldier)
                            {
                                currentPiece.PieceOwner.CurrentPoints += 1;
                            }
                            else
                            {
                                currentPiece.PieceOwner.CurrentPoints += 4;
                            }
                        }
                    }
                }
            }
        }
    }
}
