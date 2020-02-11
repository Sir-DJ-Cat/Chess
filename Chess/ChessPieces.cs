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
    // the following enum allows us to identify a chess piece as black or white
    public enum PlayerType
    {
        BLACK,
        WHITE
    }

    public abstract class AbstractChessPiece
    {
        // the following variables define the AbstractChessPiece

        public String Name;             // "Pawn", "Rook", etc.
        public String Abbreviation;     // "P", "R", etc.

        public PlayerType Player;       // BLACK or WHITE
        public int Col;                 // 0 - 7
        public int Row;                 // 0 - 7
        public bool HasMoved;           // true or false

        // This function should be completed by the student.
        // The AbstractChessPiece initializes the member variables with the
        // provided information.
        public AbstractChessPiece(string newName, string newAbbreviation, PlayerType newPlayer)
        {
            // TODO BY STUDENT
            Name = newName;
            Abbreviation = newAbbreviation;
            Player = newPlayer;
            HasMoved = false;
        }

        // This abstract method is defined but not implemented by AbstractChessPiece.
        // Each derived class will have to implement their own version.
        abstract public bool CanMoveToLocation(int targetCol, int targetRow, ChessBoard gameBoard);

        // This method will be implemented by the student.
        // Given a listed of ChessSquares in the path, determine if the indicated
        // targetRow and column can be reached within the number of steps allowed.
        protected bool CanFollowPath(int targetCol, int targetRow, 
                                     LinkedList<ChessSquare> path, int stepsAllowed)
        {
            // If you there are no steps allowed, stop
            if (path.Count == 0)
            {
                return false;
            }
            // For every chesssquare in the path, excute this logic
            foreach (ChessSquare square in path)
            {
                stepsAllowed -= 1;
                // If there are no more steps allowed, stop
                if (stepsAllowed < 0)
                {
                    return false;
                }
                // If there is a chess piece on the square, excute this logic
                if (square.ChessPiece != null)
                {
                    // If the chess piece is the players, then stop
                    if (square.ChessPiece.Player == Player)
                    {
                        return false;
                    }
                    // If the capture rules align, then you can capture
                    else if (square.ChessPiece.Col == targetCol && square.ChessPiece.Row == targetRow)
                    {
                        return true;
                    }
                    // Otherwise, stop
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // This function will be implemented by the student.
        // Override the ToString() method to return some nicer description of the piece.
        override public String ToString()
        {
            // TODO BY STUDENT
            return Player + " " + Name;
        }
    }

    // This class will be implemented by the student.
    // Pawn Capture rules
    public class Pawn : AbstractChessPiece
    {
        public Pawn(PlayerType player) : base("Pawn", "P", player)
        {
            
        }

        private bool canCaptureLocation(int targetCol, int targetRow, ChessBoard gameBoard)
        {
            ChessSquare targetSquare = gameBoard.GetSquare(targetCol, targetRow);
            // If there is no piece on the square, stop
            if (targetSquare.ChessPiece == null)
            {
                return false;
            }
            // If the piece is the players, stop
            if (targetSquare.ChessPiece.Player == Player)
            {
                return false;
            }
            bool validMove = false;
            // If the player is white, make sure they can only go up
            if (Player == PlayerType.WHITE)
            {
                if (targetRow == Row - 1 && targetCol >- Col)
                {
                    validMove = true;
                }
            }
            // If the player is black (don't disciminate the chess pieces because of their color) then make sure they can only go down
            if (Player == PlayerType.BLACK)
            {
                if (targetRow == Row + 1 && targetCol >- Col)
                {
                    validMove = true;
                }
            }
            return validMove;
        }

        // Pawn movement rules
        public override bool CanMoveToLocation(int targetCol, int targetRow, ChessBoard gameBoard)
        {
            // If the capture location is valid, then capture
            if (canCaptureLocation(targetCol, targetRow, gameBoard) == true)
            {
                return true;
            }
            LinkedList<ChessSquare> path = new LinkedList<ChessSquare>();
            // If the player is white, make sure they are going up
            if (Player == PlayerType.WHITE)
            {
                path = gameBoard.GetSquaresUp(Col, Row, targetCol, targetRow);
            }
            // If the player is black, make sure they are going down
            else if (Player == PlayerType.BLACK)
            {
                path = gameBoard.GetSquaresDown(Col, Row, targetCol, targetRow);
            }
            // If there are no steps allowed, stop
            if (path.Count == 0)
            {
                return false;
            }
            int maxMoves = 2;
            // If the pawn has moved, reduce the amount of steps they can take to one
            if (HasMoved == true)
            {
                maxMoves = 1;
            }
            for (int i = 0; i < maxMoves; i++)
            {
                ChessSquare step = path.ElementAt(i);
                // If there is a chess piece there then stop moving
                if (step.ChessPiece != null)
                {
                    return false;
                }
                // If you can capture, capture!
                if (step.Col == targetCol && step.Row == targetRow)
                {
                    return true;
                }
            }
            return false;
        }
    }

    // This class will be implemented by the student.
    // Rook movement and capture rules
    public class Rook : AbstractChessPiece
    {
        public Rook(PlayerType player) : base("Rook" , "R", player)
        {

        }

        public override bool CanMoveToLocation(int targetCol, int targetRow, ChessBoard gameBoard)
        {
            LinkedList<ChessSquare> path = gameBoard.GetStraightSquares(Col, Row, targetCol, targetRow);
            return CanFollowPath(targetCol, targetRow, path, path.Count);
        }
    }

    // This class will be implemented by the student.
    // Knight movement and caputre rules
    public class Knight : AbstractChessPiece
    {
        public Knight(PlayerType player) : base("Knight", "K", player)
        {

        }

        public override bool CanMoveToLocation(int targetCol, int targetRow, ChessBoard gameBoard)
        {
            bool validMove = false;
            int moveColumn = Math.Abs(Col - targetCol);
            int moveRow = Math.Abs(Row - targetRow);
            if ((moveColumn == 2 & moveRow == 1) || (moveColumn == 1 & moveRow == 2))
            {
                validMove = true;
            }
            
            if (validMove)
            {
                ChessSquare targetSquare = gameBoard.GetSquare(targetCol, targetRow);
                if (targetSquare.ChessPiece != null)
                {
                    if (targetSquare.ChessPiece.Player == Player)
                    {
                        return false;
                    }
                }
            }
            return validMove;
        }
    }

    // This class will be implemented by the student.
    // Bishop movement and capture rules
    public class Bishop : AbstractChessPiece
    {
        public Bishop(PlayerType player) : base("Bishop", "B", player)
        {
            return;
        }

        public override bool CanMoveToLocation(int targetCol, int targetRow, ChessBoard gameBoard)
        {
            LinkedList<ChessSquare> path = gameBoard.GetDiagonalSquares(Col, Row, targetCol, targetRow);
            return CanFollowPath(targetCol, targetRow, path, path.Count);
        }
    }

    // This class will be implemented by the student.
    // Queen movement and capture rules
    public class Queen : AbstractChessPiece
    {
        public Queen(PlayerType player) : base("Queen" , "Q", player)
        {

        }

        public override bool CanMoveToLocation(int targetCol, int targetRow, ChessBoard gameBoard)
        {
            LinkedList<ChessSquare> straightPath = gameBoard.GetStraightSquares(Col, Row, targetCol, targetRow);
            LinkedList<ChessSquare> diagonalPath = gameBoard.GetDiagonalSquares(Col, Row, targetCol, targetRow);
            if (CanFollowPath(targetCol, targetRow, straightPath, straightPath.Count) || CanFollowPath(targetCol, targetRow, diagonalPath, diagonalPath.Count))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // This class will be implemented by the student.
    // King movement and capture rules
    public class King : AbstractChessPiece
    {
        public King(PlayerType player) : base("King", "K", player)
        {

        }

        public override bool CanMoveToLocation(int targetCol, int targetRow, ChessBoard gameBoard)
        {
            LinkedList<ChessSquare> straightPath = gameBoard.GetStraightSquares(Col, Row, targetCol, targetRow);
            LinkedList<ChessSquare> diagonalPath = gameBoard.GetDiagonalSquares(Col, Row, targetCol, targetRow);
            if (CanFollowPath(targetCol, targetRow, straightPath, 1) || CanFollowPath(targetCol, targetRow, diagonalPath, 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
