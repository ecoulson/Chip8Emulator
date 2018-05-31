using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8InstructionPointer
    {
        private const ushort START_ADDRESS = 0x200;
        private const int BYTES_PER_OPCODE = 2;

        private Chip8Memory memory;
        private ushort offset;

        public Chip8InstructionPointer(Chip8Memory memory) {
            this.offset = START_ADDRESS;
            this.memory = memory;
        }

        public Chip8Opcode readOpCode() {
            byte firstByte = memory.read(offset++);
            byte lastByte = memory.read(offset++);
            return new Chip8Opcode(firstByte, lastByte);
        }

        public void increment() {
            this.offset += BYTES_PER_OPCODE;
        }

        public void setOffset(ushort offset) {
            this.offset = offset;
        }

        public ushort getOffset() {
            return this.offset;
        }
    }
}
