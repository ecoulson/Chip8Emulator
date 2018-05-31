using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8Opcode
    {
        public byte code;
        public byte x;
        public byte y;
        public ushort NNN;
        public byte NN;
        public byte N;
        public ushort instruction;

        public Chip8Opcode(byte firstByte, byte lastByte)
        {
            this.instruction = (byte)((firstByte << 8) + lastByte);
            this.code = (byte)(firstByte >> 4);
            this.x = (byte)(firstByte & 0x0F);
            this.y = (byte)(lastByte >> 4);
            this.NNN = (ushort)((x << 8) + lastByte);
            this.NN = (byte)lastByte;
            this.N = (byte)(lastByte & 0x0F);
        }
    }
}
