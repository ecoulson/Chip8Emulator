using System;
using Microsoft.Xna.Framework.Input;

namespace Chip8.Desktop.Emulator
{
    public class Chip8Emulator
    {
        private Chip8Fontset fontset;
        private Chip8CPU cpu;
        private Chip8Rom rom;
        private Chip8Timer delayTimer;
        private Chip8Timer soundTimer;
        private Chip8Display display;
        private Chip8Keyboard keyboard;
        private Chip8Memory memory;
        private Chip8InstructionPointer instructionPointer;
        private Chip8StackPointer stackPointer;

        public Chip8Emulator(Chip8Rom rom) {
            this.fontset = new Chip8Fontset();
            this.keyboard = new Chip8Keyboard();
            this.memory = new Chip8Memory();
            this.delayTimer = new Chip8Timer();
            this.soundTimer = new Chip8Timer();
            this.instructionPointer = new Chip8InstructionPointer(memory);
            this.stackPointer = new Chip8StackPointer(memory);
            this.display = new Chip8Display(this);
            this.cpu = new Chip8CPU(this);

            memory.writeBlock(fontset);
            loadRom(rom);
        }

        private void loadRom(Chip8Rom newRom) {
            this.rom = newRom;
            memory.writeBlock(rom);
        }

        public void cycle() {
            delayTimer.decrement();

            soundTimer.decrement();
            if (soundTimer.isActive()) {
                // play beep sound
            }

            cpu.executeInstruction();
        }

        public void handleKeyboardState(KeyboardState state) {
            keyboard.setState(state);
        }

        public Chip8Memory getMemory() {
            return memory;
        }

        public Chip8InstructionPointer getInstructionPointer() {
            return instructionPointer;
        }

        public Chip8StackPointer getStackPointer() {
            return stackPointer;
        }

        public Chip8Display getDisplay() {
            return display;
        }

        public Chip8Timer getDelayTimer() {
            return delayTimer;
        }

        public Chip8Timer getSoundTimer() {
            return soundTimer;
        }

        public Chip8Keyboard getKeyboard() {
            return keyboard;
        }

        public Chip8CPU getCPU() {
            return cpu;
        }
    }
}
