using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8InstructionPointer
    {
        public const ushort START_ADDRESS = 0x200;
        public static ushort offset;

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
