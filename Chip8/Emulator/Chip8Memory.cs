using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8Memory
    {
        private const int MEMORY_SIZE = 4096;
        private byte[] memory;

        public Chip8Memory() {
            this.memory = new byte[MEMORY_SIZE];
        }

        public void writeBlock(Chip8Fontset fontset) {
            writeBlock(fontset.getFontset(), Chip8Fontset.LOAD_ADDRESS);
        }

        public void writeBlock(Chip8Rom rom) {
            writeBlock(rom.getRomBytes(), Chip8Rom.LOAD_ADDRESS);
        }

        public void writeBlock(byte[] bytesToWrite, int offset) {
            Buffer.BlockCopy(bytesToWrite, 0, memory, offset, bytesToWrite.Length);
        }

        public void write(byte byteToWrite, int offset) {
            memory[offset] = byteToWrite;
        }

        public byte read(int offset) {
            return memory[offset];
        }

    }
}
