using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace gameCon
{
    public class Game
    {
        private const int BOARD_SIZE = 10;
        private Player player1;
        private Player player2;
        private Player currentPlayer;
        private bool gameOver = false;

        private Bombs bomb = new Bombs();

        public bool DLCEnabled;

        public void Start()
        {
            ShowWelcomeScreen();

            player1 = new Player("Player 1", BOARD_SIZE);
            player2 = new Player("Player 2", BOARD_SIZE);
            currentPlayer = player1;

            PlaceShipsManually(player1);
            PlaceShipsManually(player2);

            PlayGame();
        }

        private void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            CenterText("=== BattleShip and Bombs! ===");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            CenterText("Game for two players");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            CenterText("Rules:");
            CenterText("- Place ships on a 10x10 field");
            CenterText("- Ships cannot touch each other");
            CenterText("- Take turns entering coordinates");
            CenterText("- Receive an extra turn when you hit a ship");
            CenterText("- After 4 or more misses receive 1 bomb! (Bombs DLC required)");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            CenterText("Enable Bombs DLC? (y/n)");
            DLCEnabled = bomb.DLC();
            Console.ForegroundColor = ConsoleColor.Yellow;
            CenterText("Press any key to start...");
            Console.ReadKey();
        }

        private void PlaceShipsManually(Player player)
        {
            int[] shipSizes = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            string[] shipNames = {
                "four-deck", "three-deck", "three-deck",
                "two-deck", "two-deck", "two-deck",
                "one-deck", "one-deck", "one-deck", "one-deck"
            };

            for (int i = 0; i < shipSizes.Length; i++)
            {
                bool placed = false;
                while (!placed)
                {
                    Console.Clear();
                    DisplayPlayerBoard(player, true);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                    CenterText($"{player.Name}, place a {shipNames[i]} ship (size: {shipSizes[i]})");
                    Console.ResetColor();

                    Console.WriteLine();
                    CenterText("Enter starting coordinate (e.g., A1):");
                    string startCoord = GetInput().ToUpper();

                    Console.WriteLine();
                    if (shipSizes[i] != 1)
                    {
                        CenterText("Choose direction:");
                        CenterText("H - horizontal, V - vertical");
                        string direction = GetInput().ToUpper();

                        if (IsValidCoordinate(startCoord) && (direction == "H" || direction == "V"))
                        {
                            int row = startCoord[0] - 'A';
                            int col = int.Parse(startCoord.Substring(1)) - 1;

                            if (player.CanPlaceShip(row, col, shipSizes[i], direction == "H"))
                            {
                                player.PlaceShip(row, col, shipSizes[i], direction == "H");
                                placed = true;

                                Console.ForegroundColor = ConsoleColor.Green;
                                CenterText("Ship successfully placed!");
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(1000);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                CenterText("Cannot place ship here!");
                                CenterText("Check field boundaries and adjacent ships");
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(2000);
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            CenterText("Invalid input! Use format A1 and H/V");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        string direction = "H";
                        if (IsValidCoordinate(startCoord))
                        {
                            int row = startCoord[0] - 'A';
                            int col = int.Parse(startCoord.Substring(1)) - 1;

                            if (player.CanPlaceShip(row, col, shipSizes[i], direction == "H"))
                            {
                                player.PlaceShip(row, col, shipSizes[i], direction == "H");
                                placed = true;

                                Console.ForegroundColor = ConsoleColor.Green;
                                CenterText("Ship successfully placed!");
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(1000);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                CenterText("Cannot place ship here!");
                                CenterText("Check field boundaries and adjacent ships");
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(2000);
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            CenterText("Invalid input! Use format A1 and H/V");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                }
            }

            Console.Clear();
            DisplayPlayerBoard(player, true);
            Console.ForegroundColor = ConsoleColor.Green;
            CenterText($"{player.Name}, all ships are placed!");
            Console.ResetColor();
            Console.WriteLine();
            CenterText("Press any key to pass to the next player...");
            Console.ReadKey();
        }

        private void PlayGame()
        {
            while (!gameOver)
            {
                bool hit;
                do
                {
                    Console.Clear();
                    DisplayGameScreen();

                    hit = currentPlayer.MakeMove(GetOpponent());

                    if (GetOpponent().AllShipsSunk())
                    {
                        gameOver = true;
                        break;
                    }

                    Console.Clear();
                    DisplayGameScreen();

                    if (hit)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        CenterText("HIT! Extra turn!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        CenterText("MISS! Turn passes to opponent.");
                        Console.ResetColor();
                    }

                    if (!hit) break;

                    Console.WriteLine();
                    CenterText("Press any key to continue...");
                    Console.ReadKey();

                } while (hit && !gameOver);

                if (!gameOver && !hit)
                {
                    SwitchPlayer();
                    Console.WriteLine();
                    CenterText("Press any key to pass the turn...");
                    Console.ReadKey();
                }
            }

            ShowGameResult();
        }

        private void DisplayGameScreen()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            CenterText("=== BattleShip and Bombs ===");
            Console.ResetColor();
            Console.WriteLine();

            // display boards
            DisplayTwoBoards();
            Console.WriteLine();

            // information about current player
            Console.ForegroundColor = ConsoleColor.Yellow;
            CenterText($"Current turn: {currentPlayer.Name}");
            Console.ResetColor();

            Console.WriteLine();
            DisplayLegend();
        }

        private void DisplayTwoBoards()
        {
            string[] currentPlayerBoard = GetBoardLines(currentPlayer, true);
            string[] enemyBoard = GetBoardLines(GetOpponent(), false);


            Console.ForegroundColor = ConsoleColor.White;
            CenterText("YOUR BOARD".PadRight(25) + "OPPONENT'S BOARD".PadLeft(20));
            Console.ResetColor();
            Console.WriteLine();

            for (int i = 0; i < currentPlayerBoard.Length; i++)
            {
                int totalLength = currentPlayerBoard[i].Length + enemyBoard[i].Length + 4;
                int spaces = (80 - totalLength) / 2;
                Console.Write(new string(' ', Math.Max(0, spaces)));

                PrintBoard(currentPlayerBoard[i]);

                Console.ResetColor();
                Console.Write("    ");

                PrintBoard(enemyBoard[i]);

                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private void PrintBoard(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char currentChar = line[i];

                // coords - white
                if (i < 2)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = GetCellColor(currentChar);
                }

                Console.Write(currentChar);
            }
        }

        private string[] GetBoardLines(Player player, bool showShips)
        {
            string[] lines = new string[BOARD_SIZE + 1];

            // header with numbers
            lines[0] = "  ";
            for (int i = 1; i <= BOARD_SIZE; i++)
            {
                lines[0] += i.ToString().PadRight(2);
            }

            // board
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                char rowChar = (char)('A' + i);
                lines[i + 1] = rowChar + " ";

                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    char cell = player.OwnBoard[i, j];
                    if (!showShips && (cell == '■'))
                    {
                        lines[i + 1] += "~ ";
                    }
                    else
                    {
                        lines[i + 1] += cell + " ";
                    }
                }
            }

            return lines;
        }

        private void DisplayPlayerBoard(Player player, bool showShips)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            CenterText($"=== {player.Name} ===");
            Console.ResetColor();
            Console.WriteLine();

            string[] boardLines = GetBoardLines(player, showShips);
            foreach (string line in boardLines)
            {
                int spaces = (80 - line.Length) / 2;
                Console.Write(new string(' ', Math.Max(0, spaces)));

                foreach (char cell in line)
                {
                    Console.ForegroundColor = GetCellColor(cell);
                    Console.Write(cell);
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private ConsoleColor GetCellColor(char cell)
        {
            return cell switch
            {
                '■' => ConsoleColor.Green,           // ship
                'X' => ConsoleColor.Red,             // hit
                '*' => ConsoleColor.Gray,            // miss
                // 'B' => ConsoleColor.Magenta,         // bomb
                '~' => ConsoleColor.Cyan,            // water
                _ => ConsoleColor.White
            };
        }

        private void DisplayLegend()
        {
            Console.ForegroundColor = ConsoleColor.White;
            CenterText("Legend: ■ - ship  X - hit  * - miss  ~ - water");
            Console.ResetColor();
        }

        private Player GetOpponent()
        {
            return currentPlayer == player1 ? player2 : player1;
        }

        private void SwitchPlayer()
        {
            currentPlayer = currentPlayer == player1 ? player2 : player1;
        }

        private void ShowGameResult()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            CenterText("=== GAME OVER ===");
            Console.WriteLine();

            Player winner = player1.AllShipsSunk() ? player2 : player1;
            CenterText($"WINNER: {winner.Name}!");
            Console.ResetColor();

            Console.WriteLine();
            DisplayTwoBoards();
            Console.WriteLine();
            CenterText("Press any key to exit...");
            Console.ReadKey();
        }

        private bool IsValidCoordinate(string coord)
        {
            if (string.IsNullOrEmpty(coord) || coord.Length < 2)
                return false;

            char rowChar = coord[0];
            if (rowChar < 'A' || rowChar > 'A' + BOARD_SIZE - 1)
                return false;

            if (!int.TryParse(coord.Substring(1), out int col) || col < 1 || col > BOARD_SIZE)
                return false;

            return true;
        }

        private string GetInput()
        {
            Console.CursorVisible = true;
            string input = Console.ReadLine();
            Console.CursorVisible = false;
            return input;
        }

        private void CenterText(string text)
        {
            int spaces = (80 - text.Length) / 2;
            Console.WriteLine(new string(' ', Math.Max(0, spaces)) + text);
        }
    }

    public class Player
    {
        public string Name { get; }
        public char[,] OwnBoard { get; private set; }
        public int BoardSize { get; }
        public int Hits { get; private set; }

        private readonly List<Ship> ships;

        public Player(string name, int boardSize)
        {
            Name = name;
            BoardSize = boardSize;
            OwnBoard = new char[boardSize, boardSize];
            ships = new List<Ship>();

            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    OwnBoard[i, j] = '~';
                }
            }
        }

        public bool CanPlaceShip(int row, int col, int size, bool horizontal)
        {
            if (horizontal)
            {
                if (col + size > BoardSize) return false;
                for (int i = 0; i < size; i++)
                {
                    if (!IsCellEmpty(row, col + i)) return false;
                }
            }
            else
            {
                if (row + size > BoardSize) return false;
                for (int i = 0; i < size; i++)
                {
                    if (!IsCellEmpty(row + i, col)) return false;
                }
            }
            return true;
        }

        private bool IsCellEmpty(int row, int col)
        {
            // check the cell itself and near cells
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int r = row + i;
                    int c = col + j;
                    if (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize)
                    {
                        if (OwnBoard[r, c] == '■')
                            return false;
                    }
                }
            }
            return true;
        }

        public void PlaceShip(int row, int col, int size, bool horizontal)
        {
            var shipCells = new List<(int, int)>();

            if (horizontal)
            {
                for (int i = 0; i < size; i++)
                {
                    OwnBoard[row, col + i] = '■';
                    shipCells.Add((row, col + i));
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    OwnBoard[row + i, col] = '■';
                    shipCells.Add((row + i, col));
                }
            }

            ships.Add(new Ship(shipCells));
        }

        public bool MakeMove(Player opponent)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            CenterText("Enter coordinates for shot (e.g., A1):");
            Console.ResetColor();

            while (true)
            {
                string input = GetInput().ToUpper().Trim();

                if (IsValidCoordinate(input))
                {
                    int row = input[0] - 'A';
                    int col = int.Parse(input.Substring(1)) - 1;

                    if (opponent.OwnBoard[row, col] == '~' || opponent.OwnBoard[row, col] == '■')
                    {
                        bool hit = opponent.ReceiveShot(row, col);

                        if (hit)
                        {
                            Hits++;
                            return true;
                        }
                        else
                        {
                            // Bombs++;
                            return false;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        CenterText("You already shot at this cell! Try again:");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterText("Invalid format! Use letter and number (e.g., A1):");
                    Console.ResetColor();
                }
            }
        }

        public bool ReceiveShot(int row, int col)
        {
            if (OwnBoard[row, col] == '■')
            {
                OwnBoard[row, col] = 'X';

                // mark the ship as hit
                var ship = ships.FirstOrDefault(s => s.Cells.Contains((row, col)));
                if (ship != null)
                {
                    ship.Hit((row, col));
                }

                return true;
            }

            if (OwnBoard[row, col] == '~')
            {
                OwnBoard[row, col] = '*';
            }

            return false;
        }

        public bool AllShipsSunk()
        {
            return ships.All(ship => ship.IsSunk);
        }

        private bool IsValidCoordinate(string coord)
        {
            if (string.IsNullOrEmpty(coord) || coord.Length < 2)
                return false;

            char rowChar = coord[0];
            if (rowChar < 'A' || rowChar > 'A' + BoardSize - 1)
                return false;

            return int.TryParse(coord.Substring(1), out int col) && col >= 1 && col <= BoardSize;
        }

        private string GetInput()
        {
            Console.CursorVisible = true;
            string input = Console.ReadLine();
            Console.CursorVisible = false;
            return input;
        }

        private void CenterText(string text)
        {
            int spaces = (80 - text.Length) / 2;
            Console.WriteLine(new string(' ', Math.Max(0, spaces)) + text);
        }
    }

    public class Ship
    {
        public List<(int, int)> Cells { get; }
        public bool IsSunk => Cells.All(cell => hitCells.Contains(cell));

        private HashSet<(int, int)> hitCells;

        public Ship(List<(int, int)> cells)
        {
            Cells = cells;
            hitCells = new HashSet<(int, int)>();
        }

        public void Hit((int, int) cell)
        {
            hitCells.Add(cell);
        }
    }

    public class Bombs
    {
        public bool DLCEnabled = false;
        public bool DLC()
        {
            bool DLCflag = false;
            while (!DLCflag)
            {
                string DLCopt = GetInput().ToLower().Trim();
                if (DLCopt != "y" && DLCopt != "n")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterText("Wrong format. Please, type 'y' or 'n'");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }

                if (DLCopt == "y")
                {
                    DLCEnabled = true;
                }

                DLCflag = true;
            }

            return DLCEnabled;
        }

        private string GetInput()
        {
            Console.CursorVisible = true;
            string input = Console.ReadLine();
            Console.CursorVisible = false;
            return input;
        }

        private void CenterText(string text)
        {
            int spaces = (80 - text.Length) / 2;
            Console.WriteLine(new string(' ', Math.Max(0, spaces)) + text);
        }
    }
}