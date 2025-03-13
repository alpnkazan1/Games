using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games
{
    public abstract class ChessPiece
    {
        protected readonly char Color;
        protected readonly char Type;
        public int StartX, StartY;
        protected List<int[]> LegalMoves { get; } = new List<int[]>();
        public string Name
        { 
            get { return $"{Color}{Type}"; }
        }
        public abstract bool Move(int targetX, int targetY, ChessPiece?[,] board);
        public abstract void GenerateLegalMoves(ChessPiece?[,] board);
        protected ChessPiece(char color, char type, int startX, int startY)
        {
            if(color != 'W' && color != 'B') throw new ArgumentException("Color must be 'W' (White) or 'B' (Black).");
            Color = color;
            Type = type;
            StartX = startX;
            StartY = startY;
        }
    }

    public class Pawn : ChessPiece
    {
        public bool promotion;
        public Pawn(char color, int startX, int startY) : base(color, 'P',  startX, startY) { }
        
        public override bool Move(int targetX, int targetY, ChessPiece?[,] board)
        {   
            GenerateLegalMoves(board);
            if(LegalMoves.Any(move => move[0] == targetX && move[1] == targetY))
            {
                if(promotion)
                {
                    ChessPiece chessPiece = PromotePawn(targetX, targetY);
                    board[StartX, StartY] = null;
                    board[targetX,targetY] = chessPiece;
                    return true;
                }
                else
                {
                    board[StartX, StartY] = null;
                    StartX = targetX;
                    StartY = targetY;
                    board[targetX,targetY] = this;
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Piece cannot move there!");
                return false;
            }
        }
        
        public ChessPiece PromotePawn(int targetX, int targetY)
        {
            Console.WriteLine("Choose what to promote Q (Queen), R (Rook), B (Bishop), N (Knight): ");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);
            ChessPiece newPiece;
            switch(choice)
            {
            case 'Q':
                newPiece = new Queen(Color, targetX, targetY); // Replace Pawn with Queen
                break;
            case 'R':
                newPiece = new Rook(Color, targetX, targetY); // Replace Pawn with Rook
                break;
            case 'B':
                newPiece = new Bishop(Color, targetX, targetY); // Replace Pawn with Bishop
                break;
            case 'N':
                newPiece = new Knight(Color, targetX, targetY); // Replace Pawn with Knight
                break;
            default:
                Console.WriteLine("Invalid choice. Defaulting to Queen.");
                newPiece = new Queen(Color, targetX, targetY); // Default to Queen
                break;
            }
            return newPiece;
        }

        public override void GenerateLegalMoves(ChessPiece?[,] board)
        {
            // Move only up if color is white and move only down if color is black
            if(Color == 'W' && StartY < 7)
            {
                if(board[StartX,StartY+1].Name[0] == 'B' || board[StartX,StartY+1] == null)   LegalMoves.Add(new int[] {StartX,StartY+1});
                if(board[StartX-1,StartY+1].Name[0] == 'B')   LegalMoves.Add(new int[] {StartX-1,StartY+1});
                if(board[StartX+1,StartY+1].Name[0] == 'B')   LegalMoves.Add(new int[] {StartX+1,StartY+1});
            }
            else if(Color == 'B' && StartY > 0)
            {
                if(board[StartX,StartY-1].Name[0] == 'B' || board[StartX,StartY-1] == null)   LegalMoves.Add(new int[] {StartX,StartY-1});
                if(board[StartX-1,StartY-1].Name[0] == 'B')   LegalMoves.Add(new int[] {StartX-1,StartY-1});
                if(board[StartX+1,StartY-1].Name[0] == 'B')   LegalMoves.Add(new int[] {StartX+1,StartY-1});
            }
        }

    }

    public class Rook : ChessPiece
    {
        public Rook(char color, int startX, int startY) : base(color, 'R',  startX, startY) { }

        public override void GenerateLegalMoves(ChessPiece?[,] board)
        {
            //Check all the squares on the column from the StartX till the ends of the board to see till where we can go
            for(int i=StartX+1;i<=7;i++)
            {
                if(board[i,StartY] != null) // If we came across a piece, lets check if it is an opposite piece. 
                                            // If so, we can capture
                {
                    if(board[i,StartY].Name[0] != Color) LegalMoves.Add(new int[] {i,StartY});
                    break;
                }
                LegalMoves.Add(new int[] {i,StartY});
            }
            for(int i=StartX-1;i>=0;i--)
            {                
                if(board[i,StartY] != null)
                {
                    if(board[i,StartY].Name[0] != Color) LegalMoves.Add(new int[] {i,StartY});
                    break;
                }
                LegalMoves.Add(new int[] {i,StartY});
            }
            // Do the same on the same row
            for(int j=StartY+1;j<=7;j++)
            {
                if(board[StartX,j] != null)
                {
                    if(board[StartX,j].Name[0] != Color) LegalMoves.Add(new int[] {StartX,j});
                    break;
                }
                LegalMoves.Add(new int[] {StartX,j});
            }
            for(int j=StartY-1;j>=0;j--)
            {
                if(board[StartX,j] != null)
                {
                    if(board[StartX,j].Name[0] != Color) LegalMoves.Add(new int[] {StartX,j});
                    break;
                }
                LegalMoves.Add(new int[] {StartX,j});
            }            
        }

        public override bool Move(int targetX, int targetY, ChessPiece?[,] board)
        {
            GenerateLegalMoves(board);
            if(LegalMoves.Any(move => move[0] == targetX && move[1] == targetY))
            {
                board[StartX, StartY] = null;
                StartX = targetX;
                StartY = targetY;
                board[targetX,targetY] = this;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Bishop : ChessPiece
    {
        public Bishop(char color, int startX, int startY) : base(color, 'B',  startX, startY) { }

        public override void GenerateLegalMoves(ChessPiece?[,] board)
        {
            // Check diagonals
            // Top right
            for (int i = StartX + 1, j = StartY + 1; i <= 7 && j <= 7; i++, j++)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }
            // Top left
            for (int i = StartX - 1, j = StartY + 1; i >= 0 && j <= 7; i--, j++)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }
            // Bottom left
            for (int i = StartX - 1, j = StartY - 1; i >= 0 && j >= 0; i--, j--)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }
            // Bottom right
            for (int i = StartX + 1, j = StartY - 1; i <= 7 && j >= 0; i++, j--)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }          
        }

        public override bool Move(int targetX, int targetY, ChessPiece?[,] board)
        {
            GenerateLegalMoves(board);
            if(LegalMoves.Any(move => move[0] == targetX && move[1] == targetY))
            {
                board[StartX, StartY] = null;
                StartX = targetX;
                StartY = targetY;
                board[targetX,targetY] = this;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(char color, int startX, int startY) : base(color, 'Q',  startX, startY) { }

        public override void GenerateLegalMoves(ChessPiece?[,] board)
        {
            // Check diagonals
            // Top right
            for (int i = StartX + 1, j = StartY + 1; i <= 7 && j <= 7; i++, j++)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }
            // Top left
            for (int i = StartX - 1, j = StartY + 1; i >= 0 && j <= 7; i--, j++)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }
            // Bottom left
            for (int i = StartX - 1, j = StartY - 1; i >= 0 && j >= 0; i--, j--)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }
            // Bottom right
            for (int i = StartX + 1, j = StartY - 1; i <= 7 && j >= 0; i++, j--)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].Name[0] != Color) LegalMoves.Add(new int[] {i,j});
                    break;
                }
                LegalMoves.Add(new int[] {i,j});
            }          
            // Check vertical and horizontal
            // Check for elements on the same column 
            for(int i=StartX+1;i<=7;i++)
            {
                if(board[i,StartY] != null)
                {
                    if(board[i,StartY].Name[0] != Color) LegalMoves.Add(new int[] {i,StartY});
                    break;
                }
                LegalMoves.Add(new int[] {i,StartY});
            }
            for(int i=StartX-1;i>=0;i--)
            {                
                if(board[i,StartY] != null)
                {
                    if(board[i,StartY].Name[0] != Color) LegalMoves.Add(new int[] {i,StartY});
                    break;
                }
                LegalMoves.Add(new int[] {i,StartY});
            }
            // Do the same on the same row
            for(int j=StartY+1;j<=7;j++)
            {
                if(board[StartX,j] != null)
                {
                    if(board[StartX,j].Name[0] != Color) LegalMoves.Add(new int[] {StartX,j});
                    break;
                }
                LegalMoves.Add(new int[] {StartX,j});
            }
            for(int j=StartY-1;j>=0;j--)
            {
                if(board[StartX,j] != null)
                {
                    if(board[StartX,j].Name[0] != Color) LegalMoves.Add(new int[] {StartX,j});
                    break;
                }
                LegalMoves.Add(new int[] {StartX,j});
            }
        }

        public override bool Move(int targetX, int targetY, ChessPiece?[,] board)
        {
            GenerateLegalMoves(board);
            if(LegalMoves.Any(move => move[0] == targetX && move[1] == targetY))
            {
                board[StartX, StartY] = null;
                StartX = targetX;
                StartY = targetY;
                board[targetX,targetY] = this;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Knight : ChessPiece
    {
        public Knight(char color, int startX, int startY) : base(color, 'N',  startX, startY) { }

        public override void GenerateLegalMoves(ChessPiece?[,] board)
        {
            // Top-Right side
            if(
                StartY+1 <= 7 && StartX+2 <= 7 && 
                (board[StartX+2, StartY+1] == null || board[StartX+2, StartY+1].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartY+2 <= 7 && StartX+1 <= 7 && 
                (board[StartX+1, StartY+2] == null || board[StartX+1, StartY+2].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+1,StartY+2});
            }
            // Top-Left side
            if(
                StartY+1 <= 7 && StartX-2 >= 0 && 
                (board[StartX-2, StartY+1] == null || board[StartX-2, StartY+1].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX-2,StartY+1});
            }
            if(
                StartY+2 <= 7 && StartX-1 >= 0 && 
                (board[StartX-1, StartY+2] == null || board[StartX-1, StartY+2].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX-1,StartY+2});
            }
            // Bottom-Left
            if(
                StartY-1 >= 0 && StartX-2 >= 0 && 
                (board[StartX-2, StartY-1] == null || board[StartX-2, StartY-1].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX-2,StartY-1});
            }
            if(
                StartY-2 >= 0 && StartX-1 >= 0 && 
                (board[StartX-1, StartY-2] == null || board[StartX-1, StartY-2].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX-1,StartY-2});
            }
            // Bottom-Right
            if(
                StartY-1 >= 0 && StartX+2 <= 7 && 
                (board[StartX+2, StartY-1] == null || board[StartX+2, StartY-1].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY-1});
            }
            if(
                StartY-2 >= 0 && StartX+1 <= 7 && 
                (board[StartX+1, StartY-2] == null || board[StartX+1, StartY-2].Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+1,StartY-2});
            }
        }

        public override bool Move(int targetX, int targetY, ChessPiece?[,] board)
        {
            GenerateLegalMoves(board);
            if(LegalMoves.Any(move => move[0] == targetX && move[1] == targetY))
            {
                board[StartX, StartY] = null;
                StartX = targetX;
                StartY = targetY;
                board[targetX,targetY] = this;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class King : ChessPiece
    {
        public King(char color, int startX, int startY) : base(color, 'K', startX, startY) { }

        public override void GenerateLegalMoves(ChessPiece?[,] board)
        {
            // Check 1 element to all sides for availability
            if(
                StartY+1 <= 7 && StartX+1 <= 7 && 
                (board[StartX+1, StartY+1] == null || board[StartX+1, StartY+1]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartY+1 <= 7 && 
                (board[StartX, StartY+1] == null || board[StartX, StartY+1]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartY+1 <= 7 && StartX-1 >= 0 && 
                (board[StartX-1, StartY+1] == null || board[StartX-1, StartY+1]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartX-1 >= 0 && 
                (board[StartX-1, StartY] == null || board[StartX-1, StartY]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartX-1 >= 0 && StartY-1 >= 0 &&
                (board[StartX-1, StartY-1] == null || board[StartX-1, StartY-1]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartY-1 >= 0 && 
                (board[StartX, StartY-1] == null || board[StartX, StartY-1]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartX+1 <= 7 && StartY-1 >= 0 &&
                (board[StartX+1, StartY-1] == null || board[StartX+1, StartY-1]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
            if(
                StartX+1 <= 7 && 
                (board[StartX+1, StartY] == null || board[StartX+1, StartY]?.Name[0] != Color)
            )
            {
                LegalMoves.Add(new int[] {StartX+2,StartY+1});
            }
        }

        public override bool Move(int targetX, int targetY, ChessPiece?[,] board)
        {
            GenerateLegalMoves(board);
            if(LegalMoves.Any(move => move[0] == targetX && move[1] == targetY))
            {
                board[StartX, StartY] = null;
                StartX = targetX;
                StartY = targetY;
                board[targetX,targetY] = this;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Chess
    {
        private ChessPiece?[,] chessboard;

        public Chess()
        {
            chessboard = new ChessPiece[8,8];
        }

        private void BuildBoard()
        {
            // Initialize the board so that white starts from Y=0 and Black starts from Y=8
            char color = 'W';
            chessboard[0,0] = new Rook(color, 0,0);
            chessboard[1,0] = new Knight(color, 1,0);
            chessboard[2,0] = new Bishop(color, 2,0);
            chessboard[3,0] = new Queen(color, 3,0);
            chessboard[4,0] = new King(color, 4,0);
            chessboard[5,0] = new Bishop(color, 5,0);
            chessboard[6,0] = new Knight(color, 6,0);
            chessboard[7,0] = new Rook(color, 7,0);
            for(int i=0;i<8;i++)
            {
                chessboard[i,1] = new Pawn(color, i,1);
            }

            color = 'B';
            chessboard[0,7] = new Rook(color, 0,7);
            chessboard[1,7] = new Knight(color, 1,7);
            chessboard[2,7] = new Bishop(color, 2,7);
            chessboard[3,7] = new Queen(color, 3,7);
            chessboard[4,7] = new King(color, 4,7);
            chessboard[5,7] = new Bishop(color, 5,7);
            chessboard[6,7] = new Knight(color, 6,7);
            chessboard[7,7] = new Rook(color, 7,7);
            for(int i=0;i<8;i++)
            {
                chessboard[i,6] = new Pawn(color, i,6);
            }

            for(int j=2;j<6;j++)
            {
                for(int i=0;i<8;j++)
                {
                    chessboard[i,j] = null;
                }
            }
        }

        public void Play()
        {
            BuildBoard();

            Console.WriteLine("White's turn!");
            while(true)
            {
                int startX, startY;
                // Select a piece to move
                while(true)
                {
                    Console.WriteLine("Select a piece to move by typing the coordinates with a comma(x,y):");
                    string? input = Console.ReadLine();
                    if (input != null)
                    {
                        string[] parts = input.Split(','); // Split input by comma

                        if (parts.Length == 2 && int.TryParse(parts[0], out startX) && int.TryParse(parts[1], out startY))
                        {
                            Console.WriteLine($"You selected the piece '{chessboard[startX,startY]?.Name[..2]}' at ({startX}, {startY}).");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input! Please enter coordinates in the format: x,y");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You have to type something!");
                    }
                }

                int targetX, targetY;
                // Select a square to move
                while(true)
                {
                    Console.WriteLine("Select a square to move into by typing the coordinates with a comma(x,y):");
                    string? target = Console.ReadLine();
                    if (target != null)
                    {
                        string[] parts = target.Split(','); // Split input by comma

                        if (parts.Length == 2 && int.TryParse(parts[0], out targetX) && int.TryParse(parts[1], out targetY))
                        {
                            Console.WriteLine($"You try to move into ({targetX}, {targetY}).");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input! Please enter coordinates in the format: x,y");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You have to type something!");
                    }
                }

                if(MoveLegalityChecker(targetX, targetY))
                {
                    if(chessboard[startX,startY].Move(targetX,targetY,chessboard))
                    {
                        break;
                    }
                    else
                    {

                    }
                }
            }
        }

        public bool MoveLegalityChecker(int targetX, int targetY)
        {
            /*
            Return true if move is legal and false if move is illegal
                - Moves that cause self-check are illegal
                - Moves that try to move outside the table are illegal
                - Moves that take enemy queen are illegal
                + Moves that try to take your own piece or 
                    try to move a piece in a way the piece cannot move are already considered 
                    inside the ChessPiece.Move() code
            */
            if(targetX > 7 || targetX < 0 || targetY > 7 || targetY < 0)
            {
                Console.WriteLine("Choose a point inside the board!");
                return false;
            }

            return true;
        }
    }
}