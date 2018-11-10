using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEight
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeGraphics();
            InitializeInputs();

            Chip8 chip8 = new Chip8;
            chip8.Initialize();
            chip8.LoadBinaries();

            while (1)
            {
                chip8.EmulateCycle();

                if (chip8.DrawFlag)
                {
                    DrawGraphics();
                }

                chip8.SetKeys();
            }
        }
    }
}
