using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using gameCon;

namespace BattleShip
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "BattleShip and Bombs!";
            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 25);
            Console.CursorVisible = false;

            Game game = new Game();
            game.Start();
        }
    }
}