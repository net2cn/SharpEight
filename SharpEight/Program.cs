using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEight
{
    class Program
    {
        static Chip8 chip8 = new Chip8();
        static int Modifier = 100;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: SharpEight.exe {ROM_PATH}");
                return;
            }

            // Initialize CHIP-8 core.
            chip8.Initialize();

            // Check if there's any IO exceptions.
            if (!chip8.Load(args[0]))
            {
                Console.WriteLine("Exception caught when loading ROM");
                throw new NotSupportedException();
            }

            // TODO: User IO and emulation.
            InitializeGraphics();


            chip8.Initialize();

            ConsoleKey pressedKey = 0;

            while (true)
            {
                chip8.EmulateCycle();

                if (chip8.DrawFlag)
                {
                    DrawGraphics(chip8.Gfx);
                    chip8.DrawFlag = false;
                }

                // Only supports single key.
                GetUserInputs();

                System.Threading.Thread.Sleep(1000 / Modifier);
            }
        }

        static void InitializeGraphics()
        {
            // Clear console and set it's size (64*32 char).
            Console.Clear();
        }

        static void DrawGraphics(int[] gfxRam)
        {
            int hcount = 0; // Indicates whether we need a new line.

            Console.SetCursorPosition(0, 0);

            string gfx = "";

            foreach (int i in gfxRam){
                if(i != 0)
                {
                    gfx += "X";
                }
                else
                {
                    gfx += " ";
                }
                hcount++;
                if(hcount >= 64)
                {
                    gfx += "\n";
                    hcount = 0;
                }
            }

            byte[] buffer = Encoding.ASCII.GetBytes(gfx);

            using (var stdout = Console.OpenStandardOutput(64 * 32))
            {
                // fill
                stdout.Write(buffer, 0, buffer.Length);
                // rinse and repeat
            }
        }

        static void GetUserInputs()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey().Key;

                chip8.Keys[0x1] = (key == ConsoleKey.D1) ? 1 : 0;
                chip8.Keys[0x2] = (key == ConsoleKey.D2) ? 1 : 0;
                chip8.Keys[0x3] = (key == ConsoleKey.D3) ? 1 : 0;
                chip8.Keys[0xC] = (key == ConsoleKey.D4) ? 1 : 0;
                chip8.Keys[0x4] = (key == ConsoleKey.Q) ? 1 : 0;
                chip8.Keys[0x5] = (key == ConsoleKey.W) ? 1 : 0;
                chip8.Keys[0x6] = (key == ConsoleKey.E) ? 1 : 0;
                chip8.Keys[0xD] = (key == ConsoleKey.R) ? 1 : 0;
                chip8.Keys[0x7] = (key == ConsoleKey.A) ? 1 : 0;
                chip8.Keys[0x8] = (key == ConsoleKey.S) ? 1 : 0;
                chip8.Keys[0x9] = (key == ConsoleKey.D) ? 1 : 0;
                chip8.Keys[0xE] = (key == ConsoleKey.F) ? 1 : 0;
                chip8.Keys[0xA] = (key == ConsoleKey.Z) ? 1 : 0;
                chip8.Keys[0x0] = (key == ConsoleKey.X) ? 1 : 0;
                chip8.Keys[0xB] = (key == ConsoleKey.C) ? 1 : 0;
                chip8.Keys[0xF] = (key == ConsoleKey.V) ? 1 : 0;
            }
        }
    }
}
