using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chess
{
    public partial class ChessForm : Form
    {
        // The following variables encompass all of the game data
        private ChessBoard gameBoard;
        private ChessSquare selectedSquare;
        private PlayerType currentPlayer;

        // This function is provided complete as part of the starter code
        public ChessForm()
        {
            InitializeComponent();  // standard form initialization

            // create a new gameboard
            gameBoard = new ChessBoard(this);

            // initialize all of the game pieces
            initializeGame();
        }

        // The student should complete this function.
        // This method will reset all game information to the beginning configuration
        private void initializeGame()
        {
            setPlayer(PlayerType.WHITE);
            gameBoard.ClearSquares();
            gameBoard.AddNewChessPiece(0, 0, new Rook(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(1, 0, new Knight(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(2, 0, new Bishop(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(3, 0, new Queen(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(4, 0, new King(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(5, 0, new Bishop(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(6, 0, new Knight(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(7, 0, new Rook(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(0, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(1, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(2, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(3, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(4, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(5, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(6, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(7, 1, new Pawn(PlayerType.BLACK));
            gameBoard.AddNewChessPiece(0, 7, new Rook(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(1, 7, new Knight(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(2, 7, new Bishop(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(3, 7, new Queen(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(4, 7, new King(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(5, 7, new Bishop(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(6, 7, new Knight(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(7, 7, new Rook(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(0, 6, new Pawn(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(1, 6, new Pawn(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(2, 6, new Pawn(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(3, 6, new Pawn(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(4, 6, new Pawn(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(5, 6, new Pawn(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(6, 6, new Pawn(PlayerType.WHITE));
            gameBoard.AddNewChessPiece(7, 6, new Pawn(PlayerType.WHITE));


        }

        // The student should complete this function.
        // This method will set the current player and also 
        // update the display label's text, foreground, and background colors
        private void setPlayer(PlayerType player)
        {
            if (player == PlayerType.BLACK)
            {
                currentPlayer = PlayerType.BLACK;
                labelPlayer.Text = "Black's Turn";
                labelPlayer.BackColor = Color.Black;
                labelPlayer.ForeColor = Color.White;
            }
            else if (player == PlayerType.WHITE)
            {
                currentPlayer = PlayerType.WHITE;
                labelPlayer.Text = "White's Turn";
                labelPlayer.BackColor = Color.White;
                labelPlayer.ForeColor = Color.Black;
            }
        }

        // The student should complete this function.
        // This method will switch the current player from black to white
        // or vice-versa by calling the setPlayer() method.
        private void changePlayer()
        {
            if (currentPlayer == PlayerType.WHITE)
            {
                setPlayer(PlayerType.BLACK);
            }
            else
            {
                setPlayer(PlayerType.WHITE);
            }
        }


        // This function is provided complete as part of the starter code
        // This method identifies the ChessSquare clicked by using the button name.
        public void gameSquare_Click(object sender, EventArgs e)
        {
            // get the button object that the user clicked on
            ButtonBase button = (System.Windows.Forms.ButtonBase)sender;

            // identify the ChessSquare the user clicked on by using the button name
            ChessSquare clickedSquare = gameBoard.GetClickedSquare(button.Name);

            // run the game logic for this click event
            handleClick(clickedSquare);
        }


        // The student should complete this function.
        // This method contains all of the game logic to handle user click events.
        private void handleClick(ChessSquare clickedSquare)
        {
            // If there is no piece there then...
            if (selectedSquare == null)
            {
                // If the square they want to move to has a piece then...
                if (clickedSquare.ChessPiece != null)
                {
                    // If the piece is one of the players, then stop
                    if (clickedSquare.ChessPiece.Player != currentPlayer)
                    {
                        return;
                    }
                    selectedSquare = clickedSquare;
                    selectedSquare.Select();
                }
            }
            // If the click the square again, then unselect
            else if (selectedSquare == clickedSquare)
            {
                selectedSquare.Unselect();
                selectedSquare = null;
            }
            // Otherwise...
            else
            {
                // If there is a piece there...
                if (clickedSquare.ChessPiece != null)
                {
                    // If the piece is the players, then stop
                    if (clickedSquare.ChessPiece.Player == currentPlayer)
                    {
                        selectedSquare.Unselect();
                        selectedSquare = clickedSquare;
                        selectedSquare.Select();
                        return;
                    }
                }
                // If they can't move there, stop
                if (selectedSquare.ChessPiece.CanMoveToLocation(clickedSquare.Col, clickedSquare.Row, gameBoard) == false)
                {
                    return;
                }
                // Otherwise...
                else
                {
                    // If the player trys to kill themself, yell at them
                    if (gameBoard.TestMoveForCheck(currentPlayer, selectedSquare, clickedSquare))
                    {
                        MessageBox.Show("You are still in check!");
                        return;
                    }
                    AbstractChessPiece capturedPiece = gameBoard.MoveChessPiece(selectedSquare, clickedSquare);
                    selectedSquare = null;
                    // If the piece was captured, tell the player
                    if (capturedPiece != null)
                    {
                        capturedPiece.ToString();
                        MessageBox.Show(capturedPiece.ToString() + " was captured");
                    }
                    changePlayer();
                    // If the king is in danger, yell at the player
                    if (gameBoard.IsInCheck(currentPlayer))
                    {
                        MessageBox.Show("Check!");
                    }
                }
            }
        }

        // The student should complete this function.
        // This method resets the game data to the initial configuration
        private void buttonRestart_Click(object sender, EventArgs e)
        {
            initializeGame();
            if (selectedSquare != null)
            {
                selectedSquare.Unselect();
                selectedSquare = null;
            }

        }


    }
}
