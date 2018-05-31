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
        private Chip8Memory memory;
        private Chip8InstructionPointer instructionPointer;
        private Chip8StackPointer stackPointer;
        private Chip8Display display;
        private Chip8Timer delayTimer;
        private Chip8Timer soundTimer;
        private Chip8Keyboard keyboard;

        public Chip8CPU(Chip8Emulator emulator) {
            this.random = new Random();
            this.memory = emulator.getMemory();
            this.instructionPointer = emulator.getInstructionPointer();
            this.stackPointer = emulator.getStackPointer();
            this.delayTimer = emulator.getDelayTimer();
            this.soundTimer = emulator.getSoundTimer();
            this.keyboard = emulator.getKeyboard();
            this.display = emulator.getDisplay();

            this.registers = new byte[REGISTER_COUNT];
            this.I = I_ADDRESS;
        }

        public void executeInstruction() {
            Chip8Opcode opcode = instructionPointer.readOpCode();
            executeOpcode(opcode);
        }

        private void executeOpcode(Chip8Opcode opcode) {
            byte registerX = (byte)getRegister(opcode.x);
            byte registerY = (byte)getRegister(opcode.y);
            byte value;
            switch (opcode.code)
            {
                case 0x00:
                    switch (opcode.NN) {
                        case 0xE0:
                            display.clear();
                            break;
                        case 0xEE:
                            Chip8Opcode returnOpcode = stackPointer.readOpCode();
                            instructionPointer.setOffset(returnOpcode.instruction);
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.instruction);
                    }
                    break;
                case 0x01:
                    instructionPointer.setOffset(opcode.NNN);
                    break;
                case 0x02:
                    stackPointer.storeInstructionPointer(instructionPointer);
                    instructionPointer.setOffset(opcode.NNN);
                    break;
                case 0x03:
                    if (registerX == opcode.NN) {
                        instructionPointer.increment();
                    }
                    break;
                case 0x04:
                    if (registerX != opcode.NN) {
                        instructionPointer.increment();
                    }
                    break;
                case 0x05:
                    if (registerX == registerY) {
                        instructionPointer.increment();
                    }
                    break;
                case 0x06:
                    setRegister(opcode.x, opcode.NN);
                    break;
                case 0x07:
                    value = (byte)(registerX + opcode.NN);
                    setRegister(opcode.x, value);
                    break;
                case 0x08:
                    switch (opcode.N)
                    {
                        case 0x00:
                            setRegister(opcode.x, registerY);
                            break;
                        case 0x01:
                            value = (byte)(registerX | registerY);
                            setRegister(opcode.x, value);
                            break;
                        case 0x02:
                            value = (byte)(registerX & registerY);
                            setRegister(opcode.x, value);
                            break;
                        case 0x03:
                            value = (byte)(registerX ^ registerY);
                            setRegister(opcode.x, value);
                            break;
                        case 0x04:
                            // handle VF
                            value = (byte)(registerX + registerY);
                            setRegister(opcode.x, value);
                            break;
                        case 0x05:
                            //handle VF
                            value = (byte)(registerX - registerY);
                            setRegister(opcode.x, value);
                            break;
                        case 0x06:
                            //handle VF
                            value = (byte)(registerY >> 1);
                            setRegister(opcode.x, value);
                            break;
                        case 0x07:
                            //handle VF
                            value = (byte)(registerY - registerX);
                            setRegister(opcode.x, value);
                            break;
                        case 0x0E:
                            //handle VF 
                            value = (byte)(registerY << 1);
                            setRegister(opcode.y, value);
                            setRegister(opcode.x, value);
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.instruction);
                    }
                    break;
                case 0x09:
                    if (registerX != registerY) {
                        instructionPointer.increment();
                    }
                    break;
                case 0x0A:
                    I = opcode.NNN;
                    break;
                case 0x0B:
                    byte register = getRegister(0);
                    ushort offset = (ushort)(register + opcode.NNN);
                    instructionPointer.setOffset(offset);
                    break;
                case 0x0C:
                    value = (byte)(random.Next(256) & opcode.NN);
                    setRegister(opcode.x, value);
                    break;
                case 0x0D:
                    int x = getRegister(opcode.x);
                    int y = getRegister(opcode.y);
                    int n = opcode.N;
                    draw(x, y, n);
                    break;
                case 0x0E:
                    switch (opcode.NN)
                    {
                        case 0x9E:
                            if (keyboard.keyState[opcode.x]) {
                                instructionPointer.increment();
                            }
                            break;
                        case 0xA1:
                            if (!keyboard.keyState[opcode.x]) {
                                instructionPointer.increment();
                            }
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.instruction);
                    }
                    break;
                case 0x0F:
                    switch (opcode.NN)
                    {
                        case 0x07:
                            setRegister(opcode.x, (byte)delayTimer.get());
                            break;
                        case 0x0A:
                            // TODO: implment readkey
                            setRegister(opcode.x, 0); // should be read key
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
                            memory.write((byte)(registerX / 100), I);
                            memory.write((byte)((registerX % 100) / 10), I + 1);
                            memory.write((byte)(registerX % 10), I + 2);
                            break;
                        case 0x55:
                            for (int i = 0; i <= opcode.x; i++) {
                                memory.write(getRegister(i), I++);
                            }
                            break;
                        case 0x65:
                            for (int i = 0; i <= opcode.x; i++) {
                                setRegister(i, memory.read(i++));
                            }
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.NN.ToString("X2"));
                    }
                    break;
                default:
                    throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.instruction);
            }
        }

        private byte getRegister(int i) {
            return registers[i];
        }

        private void setRegister(int i, byte data) {
            registers[i] = data;
        }

        public void draw(int x, int y, int n) {
            setRegister(0x0F, 0);
            for (int dY = 0; dY < n; dY++) {
                ushort location = (ushort)(I + dY);
                byte pixel = memory.read(location);
                for (int dX = 0; dX < 8; dX++) {
                    int offset = ((y + dY) * Chip8Display.SCREEN_WIDTH) + x + dX;
                    if ((pixel & (0x80 >> dX)) != 0) {
                        if (display.isWhite(offset)) {
                            setRegister(0x0F, 1);
                        }
                        bool isWhite = display.isWhite(offset) ^ true;
                        display.draw(offset, isWhite);
                    }
                }
            }
        }
    }
}
