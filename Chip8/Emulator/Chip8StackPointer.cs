using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8StackPointer
    {
        private const ushort START_ADDRESS = 0xEA0;

        private Chip8Memory memory;
        private ushort offset;

        public Chip8StackPointer(Chip8Memory memory) {
            this.offset = START_ADDRESS;
            this.memory = memory;
        }

        public Chip8Opcode readOpCode() {
            byte firstByte = memory.read(--offset);
            byte lastByte = memory.read(--offset);
            return new Chip8Opcode(firstByte, lastByte);
        }

        public void storeInstructionPointer(Chip8InstructionPointer instructionPointer) {
            byte lastByte = (byte)(instructionPointer.getOffset() & 0xFF);
            byte firstByte = (byte)(instructionPointer.getOffset() >> 8);
            memory.write(lastByte, offset++);
            memory.write(firstByte, offset++);
        }

        public void setOffset(ushort offset) {
            this.offset = offset;
        }

        public ushort getOffset()
        {
            return this.offset;
        }
    }
}
