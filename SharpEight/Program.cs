using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEight
{
    class Program
    {
        static readonly int SCREEN_WIDTH = 64;
        static readonly int SCREEN_HEIGHT = 32;

        static Chip8 chip8 = new Chip8();
        static int Modifier = 100;
        static bool pressingFlag = false;

        static string gfxBuffer = "";

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
                pressedKey = ReadKeys(ref pressingFlag);
                
                if (pressingFlag)
                {
                    KeyboardDown(pressedKey);
                }
                else
                {
                    KeyboardUp(pressedKey);
                    pressedKey = 0;
                }

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

            gfxBuffer = gfx;

            Console.WriteLine(gfx);
        }

        static ConsoleKey ReadKeys(ref bool flag)
        {
            flag = Console.KeyAvailable;
            if (flag)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                return key.Key;
            }

            return 0;
        }

        static void KeyboardDown(ConsoleKey key)
        {
            if(key == ConsoleKey.Escape)
            {
                System.Environment.Exit(0);
            }

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    chip8.Keys[0x1] = 1;
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    chip8.Keys[0x2] = 1;
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    chip8.Keys[0x3] = 1;
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    chip8.Keys[0xC] = 1;
                    break;
                case ConsoleKey.Q:
                    chip8.Keys[0x4] = 1;
                    break;
                case ConsoleKey.W:
                    chip8.Keys[0x5] = 1;
                    break;
                case ConsoleKey.E:
                    chip8.Keys[0x6] = 1;
                    break;
                case ConsoleKey.R:
                    chip8.Keys[0xD] = 1;
                    break;
                case ConsoleKey.A:
                    chip8.Keys[0x7] = 1;
                    break;
                case ConsoleKey.S:
                    chip8.Keys[0x8] = 1;
                    break;
                case ConsoleKey.D:
                    chip8.Keys[0x9] = 1;
                    break;
                case ConsoleKey.F:
                    chip8.Keys[0xE] = 1;
                    break;
                case ConsoleKey.Z:
                    chip8.Keys[0xA] = 1;
                    break;
                case ConsoleKey.X:
                    chip8.Keys[0x0] = 1;
                    break;
                case ConsoleKey.C:
                    chip8.Keys[0xB] = 1;
                    break;
                case ConsoleKey.V:
                    chip8.Keys[0xF] = 1;
                    break;
            }
        }

        static void KeyboardUp(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    chip8.Keys[0x1] = 0;
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    chip8.Keys[0x2] = 0;
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    chip8.Keys[0x3] = 0;
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    chip8.Keys[0xC] = 0;
                    break;
                case ConsoleKey.Q:
                    chip8.Keys[0x4] = 0;
                    break;
                case ConsoleKey.W:
                    chip8.Keys[0x5] = 0;
                    break;
                case ConsoleKey.E:
                    chip8.Keys[0x6] = 0;
                    break;
                case ConsoleKey.R:
                    chip8.Keys[0xD] = 0;
                    break;
                case ConsoleKey.A:
                    chip8.Keys[0x7] = 0;
                    break;
                case ConsoleKey.S:
                    chip8.Keys[0x8] = 0;
                    break;
                case ConsoleKey.D:
                    chip8.Keys[0x9] = 0;
                    break;
                case ConsoleKey.F:
                    chip8.Keys[0xE] = 0;
                    break;
                case ConsoleKey.Z:
                    chip8.Keys[0xA] = 0;
                    break;
                case ConsoleKey.X:
                    chip8.Keys[0x0] = 0;
                    break;
                case ConsoleKey.C:
                    chip8.Keys[0xB] = 0;
                    break;
                case ConsoleKey.V:
                    chip8.Keys[0xF] = 0;
                    break;
            }
        }
    }
}
