using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEight
{
    class Chip8
    {
        // Initialize variables.
        ushort ProgramCounter = 0x200;
        ushort OpCode = 0;
        byte[] Memory = new byte[4096];             // Memory. (4KB in total)
        byte[] Registers = new byte[16];            // Registers. (V0-VE, 16 registers)
        ushort IndexRegister;                       // Index register.
        byte[] Gfx = new byte[64 * 32];             // Screen. (64 * 32 pixels black and white screen)
        byte DelayTimer = 0;
        byte SoundTimer = 0;
        ushort[] Stack = new ushort[16];
        ushort StackPointer = 0;
        byte[] Keys = new byte[16];                 // HEX-based keypad (0x0 - 0xF, 16 keys);
        bool DrawFlag = false;
        readonly byte[] Chip8Fontset = new byte[]           // CHIP-8 Font set. (Each charater is 4px wide and 5px high)
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        public void Initialize()
        {
            // Reset variables.
            ProgramCounter = 0x200;
            OpCode = 0;
            IndexRegister = 0;
            stackPointer = 0;

            // Load fontset.
            for (int i = 0; i < 80; ++i)
            {
                Memory[i] = chip8Fontset[i];
            }

            // Reset timers.

        }

        public void EmulateCycle()
        {
            // Fetch opCode.
            OpCode = (ushort)(Memory[ProgramCounter] << 8 | Memory[ProgramCounter + 1]);    // Cast the vale back into ushort after shifting.
        }
    }
}
