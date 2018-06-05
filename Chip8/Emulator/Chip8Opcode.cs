using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8Opcode
    {
        public static byte getCode(byte b1) {
            return (byte)(b1 >> 4);
        }

        public static ushort getNNN(byte b1, byte b2) {
            return (ushort)((getX(b1) << 8) + b2);
        }

        public static byte getNN(byte b2) {
            return b2;
        }

        public static byte getN(byte b2) {
            return (byte)(b2 & 0x0F);
        }

        public static byte getX(byte b1) {
            return (byte)(b1 & 0x0F);
        }

        public static byte getY(byte b2) {
            return (byte)(b2 >> 4);
        }
    }
}
