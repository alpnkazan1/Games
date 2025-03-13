using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games
{
    public class TicTacToe
    {
        private char[,] table;
        private Random random = new Random();
        private bool turn;
        public TicTacToe()
        {
            table = new char[3,3];
        }

        public void Play()
        {
            turn = random.NextDouble() >= 0.5;
            if(turn) Console.WriteLine("X starts!");
            else Console.WriteLine("O starts!");

            int column = 0;
            int row = 0;

            while(true)
            {

                if(turn) Console.WriteLine("Player X's turn!");
                else Console.WriteLine("Player O's turn!");
                Console.WriteLine("-------------------------------------------------------");
                while(true)
                {
                    Console.WriteLine("Select row number from top to bottom(0,1,2): ");
                    row = int.Parse(Console.ReadLine()); // Need to handle possible errors later on
                    if(!(row == 0 || row == 1 || row == 2))
                    {
                        Console.WriteLine("Only rows 0, 1 and 2 can be used!");
                        continue;
                    }
                    Console.WriteLine("Select column number from left to right(0,1,2): ");
                    column = int.Parse(Console.ReadLine()); // Need to handle possible errors later on
                    if(!(column == 0 || column == 1 || column == 2))
                    {
                        Console.WriteLine("Only columns 0, 1 and 2 can be used!");
                        continue;
                    }


                    if(table[row,column] == '\0') break;
                    Console.WriteLine("That place is not empty!");
                }
                
                if(turn) table[row,column] = 'X';
                else table[row,column] = 'O';

                PrintTable();

                if(CheckVictor()) 
                {
                    table = new char[3,3];
                    break;
                }

                if(table.Cast<char>().All(c => c!= '\0'))
                {
                    Console.WriteLine("Draw!");
                    table = new char[3,3];
                    break;
                }

                turn = !turn;
            }
        }
        
        private bool CheckVictor()
        {
            char checkValue = turn ? 'X' : 'O';

            // Check if any of the rows are all the same elements
            for(int i=0;i<3;i++)
            {
                for(int j=0;j<3;j++)
                {
                    if(table[i,j] != checkValue) break;
                    if(j==2) // Found the winner!
                    {
                        DeclareWinner(checkValue);
                        return true;
                    }
                }
            }

            // Check if any of the columns are all the same elements
            for(int i=0;i<3;i++) 
            {
                for(int j=0;j<3;j++)
                {
                    if(table[j,i] != checkValue) break;
                    if(j==2) // Found the winner!
                    {
                        DeclareWinner(checkValue);
                        return true;
                    }
                }
            }
        
            // Check diagonals
            if(table[0,0] == table[1,1] && table[0,0] == table[2,2] && table[0,0] == checkValue)
            {
                DeclareWinner(checkValue);
                return true;
            }

            if(table[0,2] == table[1,1] && table[0,2] == table[2,0] && table[0,2] == checkValue)
            {
                DeclareWinner(checkValue);
                return true;
            }

            return false;
        }

        private void DeclareWinner(char current)
        {
            if(current == 'X')
            {
                Console.WriteLine("Winner is Player X!");
            }
            else
            {
                Console.WriteLine("Winner is Player O!");
            }  
        }


        private void PrintTable()
        {
            Console.WriteLine("_______");
            Console.WriteLine("|" + (table[0,0] == '\0' ? " " : table[0,0].ToString()) + "|" + 
                                    (table[0,1] == '\0' ? " " : table[0,1].ToString()) + "|" + 
                                    (table[0,2] == '\0' ? " " : table[0,2].ToString()) + "|");
            Console.WriteLine("_______");
            Console.WriteLine("|" + (table[1,0] == '\0' ? " " : table[1,0].ToString()) + "|" + 
                                    (table[1,1] == '\0' ? " " : table[1,1].ToString()) + "|" + 
                                    (table[1,2] == '\0' ? " " : table[1,2].ToString()) + "|");
            Console.WriteLine("_______");
            Console.WriteLine("|" + (table[2,0] == '\0' ? " " : table[2,0].ToString()) + "|" + 
                                    (table[2,1] == '\0' ? " " : table[2,1].ToString()) + "|" + 
                                    (table[2,2] == '\0' ? " " : table[2,2].ToString()) + "|");
            Console.WriteLine("_______");
        }
    }
}