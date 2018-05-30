using System;
using Microsoft.Xna.Framework.Input;

namespace Chip8.Desktop.Emulator
{
    public class Chip8CPU
    {
        private const ushort INSTRUCTION_POINTER_ADDRESS = 0x200;
        private const ushort STACK_ADDRESS = 0xEA0;
        private const ushort I_ADDRESS = 0;
        private const int REGISTER_COUNT = 16;

        private byte[] registers;
        private ushort I;
        private ushort instructionPointer;
        private ushort stackPointer;
        private Random random;

        public Chip8CPU() {
            this.random = new Random();
            this.instructionPointer = INSTRUCTION_POINTER_ADDRESS;
            this.stackPointer = STACK_ADDRESS;
            this.registers = new byte[REGISTER_COUNT];
            this.I = I_ADDRESS;
        }

        public void executeInstruction() {
            Chip8Opcode opcode = readOpcodeFromInstructionPointer();
            executeOpcode(opcode);
        }

        private Chip8Opcode readOpcodeFromInstructionPointer() {
            byte firstByte = memory[instructionPointer++];
            byte lastByte = memory[instructionPointer++];
            return new Chip8Opcode(firstByte, lastByte);
        }

        private Chip8Opcode readOpcodeFromStackPointer() {
            byte returnB1 = memory[--stackPointer];
            byte returnB2 = memory[--stackPointer];
            return new Chip8Opcode(returnB1, returnB2);
        }

        private void executeOpcode(Chip8Opcode opcode) {
            byte registerX = (byte)getRegisterX(opcode);
            byte registerY = (byte)getRegisterY(opcode);
            byte value;
            switch (opcode.code)
            {
                case 0x00:
                    switch (opcode.NN) {
                        case 0xE0:
                            display.clear();
                            break;
                        case 0xEE:
                            Chip8Opcode returnOpcode = readOpcodeFromStackPointer();
                            instructionPointer = returnOpcode.instruction;
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.instruction);
                    }
                    break;
                case 0x01:
                    instructionPointer = opcode.NNN;
                    break;
                case 0x02:
                    memory[stackPointer++] = (byte)(instructionPointer & 0xFF);
                    memory[stackPointer++] = (byte)(instructionPointer >> 8);
                    instructionPointer = opcode.NNN;
                    break;
                case 0x03:
                    if (registerX == opcode.NN) {
                        incrementInstructionPointer();
                    }
                    break;
                case 0x04:
                    if (registerX != opcode.NN) {
                        incrementInstructionPointer();
                    }
                    break;
                case 0x05:
                    if (registerX == getRegisterY(opcode)) {
                        incrementInstructionPointer();
                    }
                    break;
                case 0x06:
                    setRegister(opcode, opcode.NN);
                    break;
                case 0x07:
                    value = (byte)(registerX + opcode.NN);
                    setRegister(opcode, value);
                    break;
                case 0x08:
                    switch (opcode.N)
                    {
                        case 0x00:
                            setRegister(opcode, registerY);
                            break;
                        case 0x01:
                            value = (byte)(registerX | registerY);
                            setRegister(opcode, value);
                            break;
                        case 0x02:
                            value = (byte)(registerX & registerY);
                            setRegister(opcode, value);
                            break;
                        case 0x03:
                            value = (byte)(registerX ^ registerY);
                            setRegister(opcode, value);
                            break;
                        case 0x04:
                            // handle VF
                            value = (byte)(registerX + registerY);
                            setRegister(opcode, value);
                            break;
                        case 0x05:
                            //handle VF
                            value = (byte)(registerX - registerY);
                            setRegister(opcode, value);
                            break;
                        case 0x06:
                            //handle VF
                            value = (byte)(registerY >> 1);
                            setRegister(opcode, value);
                            break;
                        case 0x07:
                            //handle VF
                            value = (byte)(registerY - registerX);
                            setRegister(opcode, value);
                            break;
                        case 0x0E:
                            //handle VF 
                            value = (byte)(registerY << 1);
                            setRegisterY(opcode, value);
                            setRegister(opcode, value);
                            break;
                        default:
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.instruction);
                    }
                    break;
                case 0x09:
                    if (registerX != registerY) {
                        incrementInstructionPointer();
                    }
                    break;
                case 0x0A:
                    I = opcode.NNN;
                    break;
                case 0x0B:
                    byte register = getRegister(0);
                    instructionPointer = (ushort)(register + opcode.NNN);
                    break;
                case 0x0C:
                    value = (byte)(random.Next(256) & opcode.NN);
                    setRegister(opcode, value);
                    break;
                case 0x0D:
                    display.draw(opcode);
                    break;
                case 0x0E:
                    switch (opcode.NN)
                    {
                        case 0x9E:
                            if (keyboard.keyState[opcode.x]) {
                                incrementInstructionPointer();
                            }
                            break;
                        case 0xA1:
                            if (!keyboard.keyState[opcode.x]) {
                                incrementInstructionPointer();
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
                            setRegister(opcode, (byte)delayTimer.get());
                            break;
                        case 0x0A:
                            // TODO: implment readkey
                            setRegister(opcode, 0); // should be read key
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
                            memory[I] = (byte)(registerX / 100);
                            memory[I + 1] = (byte)((registerX % 100) / 10);
                            memory[I + 2] = (byte)(registerX % 10);
                            break;
                        case 0x55:
                            for (int i = 0; i <= opcode.x; i++) {
                                memory[I++] = getRegister(i);
                            }
                            break;
                        case 0x65:
                            for (int i = 0; i <= opcode.x; i++) {
                                setRegister(i, memory[I++]);
                            }
                            break;
                        default:
                            string errorCode = (opcode.code << 4 + opcode.x).ToString("X2") + " " + (opcode.y << 4 + opcode.N).ToString("X2"); 
                            throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.NN.ToString("X2"));
                    }
                    break;
                default:
                    throw new IllegalChip8OpcodeException("Illegal Opcode " + opcode.instruction);
            }
        }

        private void incrementInstructionPointer() {
            this.instructionPointer += 2;
        }

        public byte getRegisterX(Chip8Opcode opcode) {
            return getRegister(opcode.x);
        } 

        public byte getRegisterY(Chip8Opcode opcode) {
            return getRegister(opcode.y);
        }

        public byte getRegister(int i) {
            return registers[i];
        }

        public ushort getRegisterI() {
            return I;
        }

        public void setRegister(Chip8Opcode opcode, byte data) {
            setRegister(opcode.x, data);
        }

        public void setRegisterY(Chip8Opcode opcode, byte data) {
            setRegister(opcode.y, data);
        }

        public void setRegister(int i, byte data) {
            registers[i] = data;
        }

        public void updateChip8Keyboard(KeyboardState state) {
            keyboard.setState(state);
        }
    }
}
