using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8InstructionPointer
    {
        private Chip8Memory memory;
        private ushort offset;

        public Chip8InstructionPointer(Chip8Memory memory, ushort offset) {
            this.offset = offset;
            this.memory = memory;
        }

        public Chip8Opcode readOpCode() {
            byte firstByte = memory[instructionPointer++];
            byte lastByte = memory[instructionPointer++];
            return new Chip8Opcode(firstByte, lastByte);
        }

        public void setOffset(ushort offset) {
            this.offset = offset;
        }

        public ushort getOffset() {
            return this.offset;
        }
    }
}
