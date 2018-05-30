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

        public void write(Chip8Fontset fontset) {
            write(fontset.getFontset(), Chip8Fontset.LOAD_ADDRESS);
        }

        public void write(Chip8Rom rom) {
            write(rom.getRomBytes(), Chip8Rom.LOAD_ADDRESS);
        }

        public void write(byte[] bytesToWrite, int offset) {
            Buffer.BlockCopy(bytesToWrite, 0, memory, offset, bytesToWrite.Length);
        }

        public byte read(int offset) {
            return memory[offset];
        }

    }
}
