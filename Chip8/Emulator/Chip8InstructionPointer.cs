using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8InstructionPointer
    {
        private const ushort START_ADDRESS = 0x200;
        public static ushort offset = START_ADDRESS;

        public static void increment() {
            offset += 2;
        }
    }
}
