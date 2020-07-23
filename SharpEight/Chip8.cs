using System;
using System.IO;

namespace SharpEight
{
    /// <summary>
    /// This class implemented a whole CHIP-8 system.
    /// </summary>
    class Chip8
    {
        // Initialize variables.
        int ProgramCounter = 0x200;
        int Opcode = 0;
        int[] Memory = new int[4096];             // Memory. (4KB in total)
        int[] Registers = new int[16];            // Registers. (V0-VE, 16 registers) V.
        int IndexRegister;                       // Index register. I.
        public int[] Gfx = new int[64 * 32];             // Screen. (64 * 32 pixels black and white screen)
        int DelayTimer = 0;
        int SoundTimer = 0;
        int[] Stack = new int[16];
        int StackPointer = 0;
        public int[] Keys = new int[16];                 // HEX-based keypad (0x0 - 0xF, 16 keys);
        public bool DrawFlag = false;                    // Decide whether we need to update graphics to user.

        readonly int[] CHIP_8_FONTSET = new int[]           // CHIP-8 Font set. (Each charater is 4px wide and 5px high)
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
            ProgramCounter = 0x200;     // Start address program 0x200.
            Opcode = 0;
            IndexRegister = 0;
            StackPointer = 0;

            // Clear screen once.
            DrawFlag = true;

            // Load fontset.
            Array.ConstrainedCopy(CHIP_8_FONTSET, 0, Memory, 0, CHIP_8_FONTSET.Length);
        }

        public void EmulateCycle()
        {
            // Fetch OpCode.
            Opcode = Memory[ProgramCounter] << 8 | Memory[ProgramCounter + 1];

            // Decode & process Opcode.
            // OpCode Reference: https://en.wikipedia.org/wiki/CHIP-8
            switch (Opcode & 0xF000) // Calculates the logical bitwise AND of its operands. (e.g., 0b_1001 & 0b_1100 = 0b_1000)
            {
                case 0x0000:
                    switch(Opcode & 0x000F)
                    {
                        case 0x0000:    // 0x00E0: Clears the screen.
                            Array.Clear(Gfx,0,Gfx.Length);  // Clear screen.
                            DrawFlag = true;
                            ProgramCounter += 2;
                            break;
                        case 0x000E:    // 0x00EE: Returns from subroutine.
                            --StackPointer;
                            ProgramCounter = Stack[StackPointer];
                            ProgramCounter += 2;
                            break;
                        default:
                            Console.WriteLine($"Unknown OpCode [0x0000]: 0x{Opcode:X}");
                            throw new InvalidOperationException();
                    }
                    break;
                case 0x1000:    // 0x1NNN: Jumps to address NNN
                    ProgramCounter = Opcode & 0x0FFF;
                    break;
                case 0x2000:    // 0x2NNN: Calls subroutine at NNN.
                    Stack[StackPointer] = ProgramCounter;
                    ++StackPointer;
                    ProgramCounter = Opcode & 0x0FFF;
                    break;
                case 0x3000:    // 0x3XNN: Skips the next instruction if VX equals NN.
                    ProgramCounter += Registers[(Opcode & 0x0F00) >> 8] == (Opcode & 0x00FF) ? 4 : 2;
                    break;
                case 0x4000:    // 0x4NNN: Skips the next instruction if VX doesn't equal NN.
                    ProgramCounter += Registers[(Opcode & 0x0F00) >> 8] != (Opcode & 0x00FF) ? 4 : 2;
                    break;
                case 0x5000:    // 0x5XY0: Skips the next instruction if VX equals VY.
                    ProgramCounter += Registers[(Opcode & 0x0F00) >> 8] == Registers[(Opcode & 0x00F0) >> 4] ? 4 : 2;
                    break;
                case 0x6000:    // 0x6NNN: Sets VX to NN.
                    Registers[(Opcode & 0x0F00) >> 8] = Opcode & 0x00FF;
                    ProgramCounter += 2;
                    break;
                case 0x7000:    // 0x7NNN: Adds NN to VX.
                    Registers[(Opcode & 0x0F00) >> 8] += Opcode & 0x00FF;
                    ProgramCounter += 2;
                    break;
                case 0x8000:
                    switch (Opcode & 0x000F)
                    {
                        case 0x0000:    // 0x8XY0: Sets VX to the value of VY.
                            Registers[(Opcode & 0x0F00) >> 8] = Registers[(Opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0001:    // 0x8XY1: Sets VX to "VX OR VY".
                            Registers[(Opcode & 0x0F00) >> 8] |= Registers[(Opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0002:    // 0x8XY2: Sets VX to "VX AND VY".
                            Registers[(Opcode & 0x0F00) >> 8] &= Registers[(Opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0003:    // 0x8XY3: Sets VX to "VX XOR VY".
                            Registers[(Opcode & 0x0F00) >> 8] ^= Registers[(Opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0004:    // 0x8XY4: Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
                            Registers[0xF] = Registers[(Opcode & 0x00F0) >> 4] > (0xFF - Registers[(Opcode & 0x0F00) >> 8]) ? 1 : 0;
                            Registers[(Opcode & 0x0F00) >> 8] += Registers[(Opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0005:    // 0x8XY5: VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                            Registers[0xF] = Registers[(Opcode & 0x00F0) >> 4] > Registers[(Opcode & 0x0F00) >> 8] ? 0 : 1;
                            Registers[(Opcode & 0x0F00) >> 8] -= Registers[(Opcode & 0x00F0) >> 4];
                            ProgramCounter += 2;
                            break;
                        case 0x0006:    // 0x8XY6: Stores the least significant bit of VX in VF and then shifts VX to the right by 1.
                            Registers[0xF] = Registers[(Opcode & 0x0F00) >> 8] & 0x1;
                            Registers[(Opcode & 0x0F00) >> 8] >>= 1;
                            ProgramCounter += 2;
                            break;
                        case 0x0007:    // 0x8XY7: Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                            Registers[0xF] = Registers[(Opcode & 0x0F00) >> 8] > Registers[(Opcode & 0x00F0) >> 4] ? 1 : 0;
                            Registers[(Opcode & 0x0F00) >> 8] = Registers[(Opcode & 0x00F0) >> 4] - Registers[(Opcode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;
                        case 0x000E:    // 0x8XYE: Stores the most significant bit of VX in VF and then shifts VX to the left by 1.
                            Registers[0xF] = Registers[(Opcode & 0x0F00) >> 8] >> 7;
                            Registers[(Opcode & 0x0F00) >> 8] <<= 1;
                            ProgramCounter += 2;
                            break;
                        default:
                            Console.WriteLine($"Unknown Opcode [0x8000]: 0x{Opcode:X}");
                            throw new InvalidOperationException();
                    }
                    break;
                case 0x9000:    // 0x9XY0: Skips the next instruction if VX doesn't equal VY. (Usually the next instruction is a jump to skip a code block)
                    ProgramCounter += Registers[(Opcode & 0x0F00) >> 8] != Registers[(Opcode & 0x00F0) >> 4] ? 4 : 2;
                    break;
                case 0xA000:    // 0xANNN: Sets I to the address NNN.
                    IndexRegister = Opcode & 0x0FFF;
                    ProgramCounter += 2;
                    break;
                case 0xB000:    // 0xBNNN: Jumps to the address NNN plus V0.
                    ProgramCounter = (Opcode & 0x0FFF) + Registers[0];
                    break;
                case 0xC000:    // 0xCXNN: Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.
                    Registers[(Opcode & 0x0F00) >> 8] = (new Random().Next() % 0xFF) & (Opcode & 0x00FF);
                    ProgramCounter += 2;
                    break;
                case 0xD000:    // 0xDXYN: 	Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels.
                                //          Each row of 8 pixels is read as bit-coded starting from memory location I;
                                //          I value doesn’t change after the execution of this instruction.
                                //          As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn,
                                //          and to 0 if that doesn’t happen
                    int x = Registers[(Opcode & 0x0F00) >> 8];
                    int y = Registers[(Opcode & 0x00F0) >> 4];
                    int height = Opcode & 0x000F;
                    int pixel = 0;

                    Registers[0xF] = 0;
                    for (int yi = 0; yi < height; yi++)
                    {
                        pixel = Memory[IndexRegister + yi];
                        for (int xi = 0; xi < 8; xi++)
                        {
                            if ((pixel & (0x80 >> xi)) != 0)
                            {
                                if (Gfx[(x + xi + ((y + yi) * 64))] == 1)
                                {
                                    Registers[0xF] = 1;
                                }
                                Gfx[x + xi + ((y + yi) * 64)] ^= 1;
                            }
                        }
                    }
                    DrawFlag = true;
                    ProgramCounter += 2;
                    break;
                case 0xE000:
                    switch (Opcode & 0x00FF)
                    {
                        case 0x009E:    // 0xEX9E: Skips the next instruction if the key stored in VX is pressed. (Usually the next instruction is a jump to skip a code block)
                            ProgramCounter += (Keys[Registers[(Opcode & 0x0F00) >> 8]] != 0) ? 4 : 2;
                            break;
                        case 0x00A1:    // 0xEXA1: Skips the next instruction if the key stored in VX isn't pressed. (Usually the next instruction is a jump to skip a code block)
                            ProgramCounter += (Keys[Registers[(Opcode & 0x0F00) >> 8]] == 0) ? 4 : 2;
                            break;
                        default:
                            Console.WriteLine($"Unknown Opcode [0xE000]: 0x{Opcode:X}");
                            throw new InvalidOperationException();
                    }
                    break;
                case 0xF000:
                    switch(Opcode & 0x00FF)
                    {
                        case 0x0007:    // 0xFX07: Sets VX to the value of the delay timer.
                            Registers[(Opcode & 0x0F00) >> 8] = DelayTimer;
                            ProgramCounter += 2;
                            break;
                        case 0x000A:    // 0xFX0A: A key press is awaited, and then stored in VX. (Blocking Operation. All instruction halted until next key event)
                            bool keyPressed = false;
                            for(int i = 1; i < 16; ++i)
                            {
                                if (Keys[i] != 0)
                                {
                                    Registers[(Opcode & 0x0F00) >> 8] = i;
                                    keyPressed = true;
                                }
                            }

                            // If no key was pressed, skip this cycle and goto next cycle.
                            if (!keyPressed)
                            {
                                return;
                            }
                            ProgramCounter += 2;
                            break;
                        case 0x0015:    // 0xFX15: Sets the delay timer to VX.
                            DelayTimer = Registers[(Opcode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;
                        case 0x0018:    // 0xFX18: Sets the sound timer to VX.
                            SoundTimer = Registers[(Opcode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;
                        case 0x001E:    // 0xFX1E: Adds VX to I.
                            Registers[0xF] = (IndexRegister + Registers[(Opcode & 0x0F00) >> 8] > 0xFFF) ? 1 : 0;
                            IndexRegister += Registers[(Opcode & 0x0F00) >> 8];
                            ProgramCounter += 2;
                            break;
                        case 0x0029:    // 0xFX29: Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                            IndexRegister = Registers[(Opcode & 0x0F00) >> 8] * 0x5;
                            ProgramCounter += 2;
                            break;
                        case 0x0033:    // 0xFX33: Stores the binary-coded decimal representation of VX,
                                        //         with the most significant of three digits at the address in I,
                                        //         the middle digit at I plus 1, and the least significant digit at I plus 2.
                                        //         (In other words, take the decimal representation of VX, place the hundreds digit in memory at location in I,
                                        //         the tens digit at location I+1, and the ones digit at location I+2.)
                            Memory[IndexRegister] = Registers[(Opcode & 0x0F00) >> 8] / 100;
                            Memory[IndexRegister + 1] = (Registers[(Opcode & 0x0F00) >> 8] / 10) % 10;
                            Memory[IndexRegister + 2] = (Registers[(Opcode & 0x0F00) >> 8] % 100) % 10;
                            ProgramCounter += 2;
                            break;
                        case 0x0055:    // 0xFX55: Stores V0 to VX (including VX) in memory starting at address I.
                                        //         The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                            for (int i = 0; i <= ((Opcode & 0x0F00) >> 8); ++i)
                            {
                                Memory[IndexRegister + i] = Registers[i];
                            }
                            IndexRegister += ((Opcode & 0x0F00) >> 8) + 1;
                            ProgramCounter += 2;
                            break;
                        case 0x0065:    // 0xFX65: Fills V0 to VX (including VX) with values from memory starting at address I.
                                        //         The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                            for(int i = 0; i <= ((Opcode & 0x0F00) >> 8); ++i)
                            {
                                Registers[i] = Memory[IndexRegister + i];
                            }
                            IndexRegister += ((Opcode & 0x0F00) >> 8) + 1;
                            ProgramCounter += 2;
                            break;
                        default:
                            Console.WriteLine($"Unknown Opcode [0xF000]: 0x{Opcode:X}");
                            throw new InvalidOperationException();
                    }
                    break;
                default:
                    Console.WriteLine($"Unkonwn Opcode 0x{Opcode:X}");
                    throw new InvalidOperationException();
            }

            //Update timers.
            if (DelayTimer > 0)
            {
                --DelayTimer;
            }

            if (SoundTimer > 0)
            {
                --SoundTimer;
            }
        }

        public bool Load(string path)
        {
            byte[] binaries;
            Initialize();
            Console.WriteLine($"Loading binaries: {Path.GetFileName(path)}");

            // Try loading binaries.
            try
            {
                binaries = File.ReadAllBytes(path);
            }
            catch(Exception ex)
            {
                // Handle IOException.
                Console.WriteLine($"WARNING: Load binaries ERROR! Printing stack trace...\n{ex.StackTrace}");
                return false;
            }

            Console.WriteLine($"Binaries size: {binaries.Length}");
            if (binaries.Length > (4096 - 512))
            {
                Console.WriteLine($"WARNING: Binaries are too big for CHIP-8 system's memory!");
                return false;
            }
            else
            {
                Array.Copy(binaries, 0, Memory, 0 + 512,binaries.Length);
                Console.WriteLine("Success!");
                return true;
            }
        }
    }
}
