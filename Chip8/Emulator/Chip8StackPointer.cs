using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8StackPointer
    {
        private const ushort START_ADDRESS = 0xEA0;
        public static ushort offset = START_ADDRESS;
    }
}
