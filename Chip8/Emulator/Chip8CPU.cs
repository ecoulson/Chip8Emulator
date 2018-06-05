using System;
using Microsoft.Xna.Framework.Input;

namespace Chip8.Desktop.Emulator
{
    public class Chip8CPU
    {
        private const ushort I_ADDRESS = 0;
        private const int REGISTER_COUNT = 16;

        private byte[] registers;
        private ushort I;

        private Random random;
        private Chip8Timer delayTimer;
        private Chip8Timer soundTimer;
        private Chip8Keyboard keyboard;

        public Chip8CPU(Chip8Emulator emulator) {
            this.random = new Random();
            this.delayTimer = emulator.getDelayTimer();
            this.soundTimer = emulator.getSoundTimer();
            this.keyboard = emulator.getKeyboard();

            this.registers = new byte[REGISTER_COUNT];
            this.I = I_ADDRESS;
        }

        public void executeInstruction() {
            byte byte1 = Chip8Memory.memory[Chip8InstructionPointer.offset++];
            byte byte2 = Chip8Memory.memory[Chip8InstructionPointer.offset++];
            byte registerX = registers[Chip8Opcode.getX(byte1)];
            byte registerY = registers[Chip8Opcode.getY(byte2)];
            byte value;
            switch (Chip8Opcode.getCode(byte1))
            {
                case 0x00:
                    switch (Chip8Opcode.getNN(byte2)) {
                        case 0xE0:
                            Chip8Display.clear();
                            break;
                        case 0xEE:
                            byte returnByte1 = Chip8Memory.memory[--Chip8StackPointer.offset];
                            byte returnByte2 = Chip8Memory.memory[--Chip8StackPointer.offset];
                            Chip8InstructionPointer.offset = (ushort)((returnByte1 << 8) + returnByte2);
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + byte1.ToString("X2") + byte2.ToString("X2"));
                    }
                    break;
                case 0x01:
                    Chip8InstructionPointer.offset = Chip8Opcode.getNNN(byte1, byte2);
                    break;
                case 0x02:
                    Chip8Memory.memory[Chip8StackPointer.offset++] = (byte)(Chip8InstructionPointer.offset & 0x0F);
                    Chip8Memory.memory[Chip8StackPointer.offset++] = (byte)(Chip8InstructionPointer.offset >> 8);
                    Chip8InstructionPointer.offset = Chip8Opcode.getNNN(byte1, byte2);
                    break;
                case 0x03:
                    if (registerX == Chip8Opcode.getNN(byte2)) {
                        Chip8InstructionPointer.increment();
                    }
                    break;
                case 0x04:
                    if (registerX != Chip8Opcode.getNN(byte2)) {
                        Chip8InstructionPointer.increment();
                    }
                    break;
                case 0x05:
                    if (registerX == registerY) {
                        Chip8InstructionPointer.increment();
                    }
                    break;
                case 0x06:
                    registers[Chip8Opcode.getX(byte1)] = Chip8Opcode.getNN(byte2);
                    break;
                case 0x07:
                    value = (byte)(registerX + Chip8Opcode.getNN(byte2));
                    registers[Chip8Opcode.getX(byte1)] = value;
                    break;
                case 0x08:
                    switch (Chip8Opcode.getN(byte2))
                    {
                        case 0x00:
                            registers[Chip8Opcode.getX(byte1)] = registerY;
                            break;
                        case 0x01:
                            value = (byte)(registerX | registerY);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            break;
                        case 0x02:
                            value = (byte)(registerX & registerY);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            break;
                        case 0x03:
                            value = (byte)(registerX ^ registerY);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            break;
                        case 0x04:
                            // handle VF
                            value = (byte)(registerX + registerY);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            break;
                        case 0x05:
                            //handle VF
                            value = (byte)(registerX - registerY);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            break;
                        case 0x06:
                            //handle VF
                            value = (byte)(registerY >> 1);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            break;
                        case 0x07:
                            //handle VF
                            value = (byte)(registerY - registerX);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            break;
                        case 0x0E:
                            //handle VF 
                            value = (byte)(registerY << 1);
                            registers[Chip8Opcode.getX(byte1)] = value;
                            registers[Chip8Opcode.getY(byte2)] = value;
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + byte1.ToString("X2") + byte2.ToString("X2"));
                    }
                    break;
                case 0x09:
                    if (registerX != registerY) {
                        Chip8InstructionPointer.increment();
                    }
                    break;
                case 0x0A:
                    I = Chip8Opcode.getNNN(byte1, byte2);
                    break;
                case 0x0B:
                    ushort offset = (ushort)(registers[0] + Chip8Opcode.getNNN(byte1, byte2));
                    Chip8InstructionPointer.offset = offset;
                    break;
                case 0x0C:
                    value = (byte)(random.Next(256) & Chip8Opcode.getNN(byte2));
                    registers[Chip8Opcode.getX(byte1)] = value;
                    break;
                case 0x0D:
                    int x = registerX;
                    int y = registerY;
                    int n = Chip8Opcode.getN(byte2);
                    draw(x, y, n);
                    break;
                case 0x0E:
                    switch (Chip8Opcode.getNN(byte2))
                    {
                        case 0x9E:
                            if (keyboard.keyState[Chip8Opcode.getX(byte1)]) {
                                Chip8InstructionPointer.increment();
                            }
                            break;
                        case 0xA1:
                            if (!keyboard.keyState[Chip8Opcode.getX(byte1)]) {
                                Chip8InstructionPointer.increment();
                            }
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + byte1.ToString("X2") + byte2.ToString("X2"));
                    }
                    break;
                case 0x0F:
                    switch (Chip8Opcode.getNN(byte2))
                    {
                        case 0x07:
                            registers[Chip8Opcode.getX(byte1)] = (byte)delayTimer.get();
                            break;
                        case 0x0A:
                            // TODO: implment readkey
                            registers[Chip8Opcode.getX(byte1)] = 0; // should be read key
                            break;
                        case 0x15:
                            delayTimer.set(registerX);
                            break;
                        case 0x18:
                            soundTimer.set(registerX);
                            break;
                        case 0x1E:
                            I += registerX;
                            break;
                        case 0x29:
                            I = (byte)(Chip8Fontset.FONT_HEIGHT * registerX);
                            break;
                        case 0x33:
                            Chip8Memory.memory[I] = (byte)(registerX / 100);
                            Chip8Memory.memory[I + 1] = (byte)((registerX % 100) / 10);
                            Chip8Memory.memory[I + 2] = (byte)(registerX % 10);
                            break;
                        case 0x55:
                            for (int i = 0; i <= Chip8Opcode.getX(byte1); i++) {
                                Chip8Memory.memory[I++] = registers[i];
                            }
                            break;
                        case 0x65:
                            for (int i = 0; i <= Chip8Opcode.getX(byte1); i++) {
                                registers[i] = Chip8Memory.memory[I++];
                            }
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + byte1.ToString("X2") + byte2.ToString("X2"));
                    }
                    break;
                default:
                    throw new IllegalChip8OpcodeException("Illegal Opcode " + byte1.ToString("X2") + byte2.ToString("X2"));
            }
        }

        public void draw(int x, int y, int n) {
            registers[0x0F] = 0;
            for (int dY = 0; dY < n; dY++) {
                ushort location = (ushort)(I + dY);
                byte pixel = Chip8Memory.memory[location];
                for (int dX = 0; dX < 8; dX++) {
                    int offset = ((y + dY) * Chip8Display.SCREEN_WIDTH) + x + dX;
                    if ((pixel & (0x80 >> dX)) != 0) {
                        if (Chip8Display.isWhite(offset)) {
                            registers[0x0F] = 1;
                        }
                        bool isWhite = Chip8Display.isWhite(offset) ^ true;
                        Chip8Display.draw(offset, isWhite);
                    }
                }
            }
        }
    }
}
