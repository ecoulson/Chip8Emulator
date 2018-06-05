using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8Memory
    {
        private const int MEMORY_SIZE = 4096;
        public static byte[] memory = new byte[MEMORY_SIZE];

        public static void writeBlock(byte[] bytesToWrite, int offset) {
            Buffer.BlockCopy(bytesToWrite, 0, memory, offset, bytesToWrite.Length);
        }
    }
}
