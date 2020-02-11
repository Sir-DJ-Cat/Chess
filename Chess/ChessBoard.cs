using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// All classes and methods in this file are provided complete to the 
// student in order to aid the game development.  There are two main classes:

// ChessSquare - represents one square on the ChessBoard, which may or may not contain
//               an AbstractChessPiece.

// ChessBoard - represents the entire gameboard in an 8x8 array of ChessSquares.  Also
//              provides utility methods to aid in path-building for moving pieces.
namespace Chess
{
    //////////////////////////////////////////////////////
    // This class represents one square on the ChessBoard.
    public class ChessSquare
    {
        //////////// private properties

        // define some colors for the light and dark squares
        private System.Drawing.Color DARK_SQUARE_BACKGROUND = Color.CornflowerBlue;
        private System.Drawing.Color LIGHT_SQUARE_BACKGROUND = Color.Cyan;

        // define the font we will use for all chess pieces except the King
        private Font pieceFont = new Font("Arial", 16, FontStyle.Bold);

        // define the font we will use for the king (add underline style to
        // distinguish the King from the Knight).
        private Font kingFont = new Font("Arial", 16, FontStyle.Bold | FontStyle.Underline);

        // define the size of a square, in pixels
        private int SQUARE_SIZE = 40;

        //////////// public properties

        public int Col; // the col index 0-7 identifying this square
        public int Row; // the row index 0-7 identifying this square

        // Each square is actually represented by a Button on the screen.  This will
        // allow us to capture click events on the squares easily.
        public System.Windows.Forms.Button button = new Button();

        // identify the current chess piece, if any, on the square
        public AbstractChessPiece ChessPiece;

        // This function is the object's constructor.  We require that the row
        // and column of the square be set initially (it never changes later!).
        public ChessSquare(int r, int c)
        {
            Row = r;
            Col = c;

            // configure the button for the appearance we want

            // set the border to black
            button.FlatAppearance.BorderColor = System.Drawing.Color.Black;

            // set the style to Flat to get rid of the 3D button look
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            // establish a checkerboard pattern of light and dark squares
            if ((Row % 2) == 0)  // if even Row
            {
                if ((Col % 2) == 0)  // if even Col
                {
                    button.BackColor = LIGHT_SQUARE_BACKGROUND;
                }
                else  // odd Col
                {
                    button.BackColor = DARK_SQUARE_BACKGROUND;
                }
            }
            else  // odd Row
            {
                if ((Col % 2) == 0)  // if even Col
                {
                    button.BackColor = DARK_SQUARE_BACKGROUND;
                }
                else  // odd Col
                {
                    button.BackColor = LIGHT_SQUARE_BACKGROUND;
                }
            }

            // put the button in the right spot in the screen
            // (upper-left location is determined by the Col and Row index)
            button.Location = new System.Drawing.Point((Col + 1) * SQUARE_SIZE, (Row + 1)* SQUARE_SIZE);
            
            // name the button something systematic that we'll be able to parse later
            // to identify button clicks
            button.Name = "square" + Col.ToString() + Row.ToString();

            // size the button according to our defined square size
            button.Size = new System.Drawing.Size(SQUARE_SIZE, SQUARE_SIZE);

            // TODO ??
//            button.TabIndex = Col * 8 + Row;
//            button.UseVisualStyleBackColor = false;
        }

        // The Select method is called when the user clicks on the square.  We want to 
        // Select it by establishing a thick yellow border
        public void Select()
        {
            button.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            button.FlatAppearance.BorderSize = 3;
        }

        // The Unselect method is called when the a square is no longer selected.  We want to 
        // revert back to the thin black border
        public void Unselect()
        {
            button.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            button.FlatAppearance.BorderSize = 1;
        }

        // This function will establish the specified chess piece on the square.
        // (Or, if the piece is null, just clear the square of any prior piece!)
        public void SetChessPiece(AbstractChessPiece piece)
        {
            // clear the prior abbreviation
            button.Text = "";

            // set our chess piece to the input
            if (piece == null) // if the input was null
            {
                RemoveChessPiece(); // clear off any prior piece
                return;             // nothing else to do!
            }

            // set the new chess piece to the input parameter
            ChessPiece = piece;

            // update the Chess Piece so it knows what row and column it now resides on
            ChessPiece.Col = Col;
            ChessPiece.Row = Row;

            // change the text color of the button according to the piece's player
            if (ChessPiece.Player == PlayerType.WHITE)
                button.ForeColor = Color.White;
            else
                button.ForeColor = Color.Black;

            // set the button's text from the piece's 1-character abbreviation
            button.Text = piece.Abbreviation;
            
            // set the font based on the type of piece
            if (piece.Name.Equals("King"))
            {
                button.Font = kingFont;
            }
            else
            {
                button.Font = pieceFont;
            }
            
        }

        // this method will clear out any trace of the existing chess piece, if any. 
        public void RemoveChessPiece()
        {
            ChessPiece = null;
            button.Text = "";
        }

    }

    //////////////////////////////////////////////////////
    // This class represents the entire game board.
    public class ChessBoard
    {
        //////////// private properties

        private ChessSquare[,] squares;


        //////////// public properties

        // This constructor needs a reference to the parent Form so it
        // can configure the individual buttons that make up the squares.
        public ChessBoard(ChessForm myForm)
        {
            // create a new 8x8 array of squares
            squares = new ChessSquare[8,8];

            // iterate over each column and row
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    // create a new ChessSquare object
                    squares[col,row] = new ChessSquare(row,col);

                    // add the ChessSquare's button control to the Form
                    myForm.Controls.Add(squares[col,row].button);

                    // link the button's click event to the Form's click handler function
                    squares[col,row].button.Click += new System.EventHandler(myForm.gameSquare_Click);
                }
            }
        }

        // This utility method will remove all pieces from all squares
        public void ClearSquares()
        {
            // iterate over each column and row
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    // remove any piece that may exist on this square
                    squares[col,row].RemoveChessPiece();
                }
            }
        }

        // This method is called when initializing a new game.  It assigns
        // the specified piece to the ChessSquare identified by the col and row.
        public void AddNewChessPiece(int col, int row, AbstractChessPiece piece)
        {
            // get the target ChessSquare
            ChessSquare square = GetSquare(col, row);

            // assign the piece to the square
            square.SetChessPiece(piece);
        }

        // this function will return the ChessSquare at the indicated column and row
        public ChessSquare GetSquare(int col, int row)
        {
            return squares[col, row];
        }

        // this function will return the ChessSquare identified by the specified button name
        public ChessSquare GetClickedSquare(String buttonName)
        {
            // extract col and row from the button name like "squareXY"
            String strCol = buttonName.Substring(6, 1);
            String strRow = buttonName.Substring(7, 1);

            // convert the string to numbers
            int col;
            int row;

            Int32.TryParse(strCol, out col);
            Int32.TryParse(strRow, out row);

            // return the square at the specified column and row index
            return squares[col,row];

        }

        // this utility function will return a path of diagonal squares from the starting 
        // location (col1/row1) to the target location (col2/row2).
        // If the squares are not on a diagonal to each other, the returned list is empty!
        public LinkedList<ChessSquare> GetDiagonalSquares(int col1, int row1, int col2, int row2)
        {
            // create the LinkedList that contains the path
            LinkedList<ChessSquare> path = new LinkedList<ChessSquare>();

            // if destination is the same as the origin
            if ((col1 == col2) && (row1 == row2))
                return path;       // return the empty path...not valid

            // if we are going down and to the right
            if ((col1 < col2) && (row1 < row2))
            {
                // while we haven't passed by our target square yet
                while ((col1 < col2) && (row1 < row2))
                {
                    col1++; // move diagonally one square down & right
                    row1++;

                    // add this square to the path
                    path.AddLast(squares[col1, row1]);
                }
            }
            // if we are going down and to the left
            else if ((col1 > col2) && (row1 < row2))
            {
                // while we haven't passed by our target square yet
                while ((col1 > col2) && (row1 < row2))
                {
                    col1--; // move diagonally one square down & left
                    row1++;

                    // add this square to the path
                    path.AddLast(squares[col1, row1]);
                }
            }
            // if we are going up and to the left
            else if ((col1 > col2) && (row1 > row2))
            {
                // while we haven't passed by our target square yet
                while ((col1 > col2) && (row1 > row2))
                {
                    col1--; // move diagonally one square up & left
                    row1--;

                    // add this square to the path
                    path.AddLast(squares[col1, row1]);
                }
            }
            // if we are going up and to the right
            else if ((col1 < col2) && (row1 > row2))
            {
                // while we haven't passed by our target square yet
                while ((col1 < col2) && (row1 > row2))
                {
                    col1++; // move diagonally one square up & right
                    row1--;

                    // add this square to the path
                    path.AddLast(squares[col1, row1]);
                }
            }

            // finally...important!  We need to make sure our ending column/row is 
            // exactly equal to the target column/row.  Otherwise the starting and ending
            // points are NOT on a diagonal relationship to each other, and the resulting
            // path should therefore be cleared out and left empty.
            if ((col1 != col2) || (row1 != row2))
                path.Clear();       // not diagonals!

            // now return whatever we found to the calling function
            return path;

        }

        // this utility function will return a path of straight squares from the starting 
        // location (col1/row1) to the target location (col2/row2).
        // If the squares are not on a straight line with each other, the returned list is empty!
        public LinkedList<ChessSquare> GetStraightSquares(int col1, int row1, int col2, int row2)
        {
            // create the LinkedList that contains the path
            LinkedList<ChessSquare> path = new LinkedList<ChessSquare>();

            // if destination is the same as the origin
            if ((col1 == col2) && (row1 == row2))
                return path;       // return the empty path...not valid

            // destination row and column are different, cannot make straight line to target
            if ((col1 != col2) && (row1 != row2))
                return path;       // return the empty path...not valid

            // if we are moving down
            if ((col1 == col2) && (row1 < row2))
            {
                // while we haven't passed by our target square yet
                while (row1 < row2)
                {
                    row1++; // move one square down
                    path.AddLast(squares[col1, row1]);
                }
            }
            // if we are moving up
            else if ((col1 == col2) && (row1 > row2))
            {
                // while we haven't passed by our target square yet
                while (row1 > row2)
                {
                    row1--; // move one square up
                    path.AddLast(squares[col1, row1]);
                }
            }
            // if we are moving left
            else if ((col1 > col2) && (row1 == row2))
            {
                // while we haven't passed by our target square yet
                while (col1 > col2)
                {
                    col1--; // move one square left
                    path.AddLast(squares[col1, row1]);
                }
            }
            // if we are moving right
            else if ((col1 < col2) && (row1 == row2))
            {
                // while we haven't passed by our target square yet
                while (col1 < col2)
                {
                    col1++; // move one square right
                    path.AddLast(squares[col1, row1]);
                }
            }

            // now return whatever we found to the calling function
            return path;
        }

        // this utility function will return a path of straight squares UP from the starting 
        // location (col1/row1) to the target location (col2/row2).
        // If the squares are not on a straight line with each other, the returned list is empty!
        public LinkedList<ChessSquare> GetSquaresUp(int col1, int row1, int col2, int row2)
        {
            // create the LinkedList that contains the path
            LinkedList<ChessSquare> path = new LinkedList<ChessSquare>();

            // if target row is the same as the origin or below
            if (row1 <= row2)
                return path;       // return the empty path...not valid

            // if target column is not the same
            if (col1 != col2)
                return path;       // return the empty path...not valid

            // going up...while we haven't passed our target square
            while (row1 > row2)
            {
                row1--; // move one square up
                path.AddLast(squares[col1, row1]);
            }

            // now return whatever we found to the calling function
            return path;
        }

        // this utility function will return a path of straight squares UP from the starting 
        // location (col1/row1) to the target location (col2/row2).
        // If the squares are not on a straight line with each other, the returned list is empty!
        public LinkedList<ChessSquare> GetSquaresDown(int col1, int row1, int col2, int row2)
        {
            // create the LinkedList that contains the path
            LinkedList<ChessSquare> path = new LinkedList<ChessSquare>();

            // if target row is the same as the origin or above
            if (row1 >= row2)
                return path;       // return the empty path...not valid

            // if target column is not the same
            if (col1 != col2)
                return path;       // return the empty path...not valid

            // going down...while we haven't passed our target square
            while (row1 < row2)
            {
                row1++; // move one square down
                path.AddLast(squares[col1, row1]);
            }

            // now return whatever we found to the calling function
            return path;
        }

        // This utility function will move a piece from the selected square to the target square.
        // It will return any chess piece that was captured from the target square.
        public AbstractChessPiece MoveChessPiece(ChessSquare selectedSquare, ChessSquare clickedSquare)
        {
            // save any piece that is on the target square
            AbstractChessPiece capturedPiece = clickedSquare.ChessPiece;

            // move the piece from the original square to the target square
            clickedSquare.SetChessPiece(selectedSquare.ChessPiece);

            // mark the chess piece as having been moved, in case the piece needs to know (e.g. Pawn)
            selectedSquare.ChessPiece.HasMoved = true;

            // remove the chess piece from the original square
            selectedSquare.RemoveChessPiece();

            // Unselect the currently selected square
            selectedSquare.Unselect();

            // return the captured piece, if any (or null if no piece was captured)
            return capturedPiece;

        }

        // This utility function will look at a proposed move from the selectedSquare to
        // the clickedSquare and determine if the specified player would be in check after
        // the move.  The actual gameboard itself is left unchanged.
        public bool TestMoveForCheck(PlayerType currentPlayer, ChessSquare selectedSquare, ChessSquare clickedSquare)
        {
            // save the piece that may be on the target square
            AbstractChessPiece capturedPiece = clickedSquare.ChessPiece;

            // temporarily put the original piece on the target square
            clickedSquare.SetChessPiece(selectedSquare.ChessPiece);
            // temporarily remove the original piece from the original square
            selectedSquare.RemoveChessPiece();
            
            // see if the specified player is in check
            bool isInCheck = IsInCheck(currentPlayer);

            // restore the moved piece to the original square
            selectedSquare.SetChessPiece(clickedSquare.ChessPiece);

            // restore the original contents of the target square
            clickedSquare.SetChessPiece(capturedPiece);

            // return true if the current player would still be in check, or false otherwise
            return isInCheck;
        }

        // This utility function will return true if the specified player is in check, or
        // false otherwise.
        public bool IsInCheck(PlayerType player)
        {
            // to see if player is in check, walk through all opposing player pieces and see if any of
            // them can move to this player's King position.

            // first, find this player's king
            AbstractChessPiece king = null;

            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    // see if there is a piece on this square
                    AbstractChessPiece piece = squares[col, row].ChessPiece;

                    // if there is a piece here, and it belongs to this player, and it's the King
                    if ((piece != null) && (piece.Player == player) && (piece.Name.Equals("King")))
                    {
                        king = piece;   // save the king piece, we found it!
                    }
                }
            }

            // now, walk through all opposing pieces to see if they can capture this player's king
            PlayerType opposingPlayer = PlayerType.WHITE;
            if (player == PlayerType.WHITE)
                opposingPlayer = PlayerType.BLACK;

            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    // see if there is a piece on this square
                    AbstractChessPiece piece = squares[col, row].ChessPiece;

                    // if there is a piece here, and it belongs to the opposing player
                    if ((piece != null) && (piece.Player == opposingPlayer))
                    {
                        // see if opposing piece can capture our King
                        if (piece.CanMoveToLocation(king.Col, king.Row, this))
                            return true;    // yikes, player is in check!
                    }
                }
            }

            // if we get here then no opposing piece can capture player's king, so
            // player's king is not in check.
            return false;
        }
    }

}
