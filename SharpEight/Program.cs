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
        static int Modifier = 10;
        static bool pressingFlag = false;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: SharpEight.exe {Binary Path}");
                return;
            }

            // Initialize CHIP-8 core.
            chip8.Initialize();

            // Check if there's any IO exceptions.
            if (!chip8.Load(args[1]))
            {
                return;
            }

            // TODO: User IO and emulation.
            InitializeGraphics();


            chip8.Initialize();

            int pressedKey = 0;

            while (true)
            {
                chip8.EmulateCycle();

                if (chip8.DrawFlag)
                {
                    DrawGraphics();
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
                }
            }
        }

        static void InitializeGraphics()
        {
            // Clear console and set it's size (64*32 char).
            Console.Clear();
            throw new NotImplementedException();
        }

        static void DrawGraphics()
        {
            // Draw screen to console,
            throw new NotImplementedException();
        }

        static int ReadKeys(ref bool flag)
        {
            // Read keys from console while not blocking the program.
            // Return a integar keycode.
            throw new NotImplementedException();
        }

        static void KeyboardDown(int key)
        {
            if(key == (int)ConsoleKey.Escape)
            {
                System.Environment.Exit(0);
            }

            switch (key)
            {
                case 1:
                    chip8.Keys[0x1] = 1;
                    break;
                case 2:
                    chip8.Keys[0x2] = 1;
                    break;
                case 3:
                    chip8.Keys[0x3] = 1;
                    break;
                case 4:
                    chip8.Keys[0xC] = 1;
                    break;
                case 'q':
                    chip8.Keys[0x4] = 1;
                    break;
                case 'w':
                    chip8.Keys[0x5] = 1;
                    break;
                case 'e':
                    chip8.Keys[0x6] = 1;
                    break;
                case 'r':
                    chip8.Keys[0xD] = 1;
                    break;
                case 'a':
                    chip8.Keys[0x7] = 1;
                    break;
                case 's':
                    chip8.Keys[0x8] = 1;
                    break;
                case 'd':
                    chip8.Keys[0x9] = 1;
                    break;
                case 'f':
                    chip8.Keys[0xE] = 1;
                    break;
                case 'z':
                    chip8.Keys[0xA] = 1;
                    break;
                case 'x':
                    chip8.Keys[0x0] = 1;
                    break;
                case 'c':
                    chip8.Keys[0xB] = 1;
                    break;
                case 'v':
                    chip8.Keys[0xF] = 1;
                    break;
            }
        }

        static void KeyboardUp(int key)
        {
            switch (key)
            {
                case 1:
                    chip8.Keys[0x1] = 0;
                    break;
                case 2:
                    chip8.Keys[0x2] = 0;
                    break;
                case 3:
                    chip8.Keys[0x3] = 0;
                    break;
                case 4:
                    chip8.Keys[0xC] = 0;
                    break;
                case 'q':
                    chip8.Keys[0x4] = 0;
                    break;
                case 'w':
                    chip8.Keys[0x5] = 0;
                    break;
                case 'e':
                    chip8.Keys[0x6] = 0;
                    break;
                case 'r':
                    chip8.Keys[0xD] = 0;
                    break;
                case 'a':
                    chip8.Keys[0x7] = 0;
                    break;
                case 's':
                    chip8.Keys[0x8] = 0;
                    break;
                case 'd':
                    chip8.Keys[0x9] = 0;
                    break;
                case 'f':
                    chip8.Keys[0xE] = 0;
                    break;
                case 'z':
                    chip8.Keys[0xA] = 0;
                    break;
                case 'x':
                    chip8.Keys[0x0] = 0;
                    break;
                case 'c':
                    chip8.Keys[0xB] = 0;
                    break;
                case 'v':
                    chip8.Keys[0xF] = 0;
                    break;
            }
        }
    }
}
