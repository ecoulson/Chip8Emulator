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

        public Chip8Emulator(Chip8Rom rom) {
            this.fontset = new Chip8Fontset();
            this.cpu = new Chip8CPU();
            this.keyboard = new Chip8Keyboard();
            this.display = new Chip8Display();
            this.memory = new Chip8Memory();
            this.delayTimer = new Chip8Timer();
            this.soundTimer = new Chip8Timer();

            memory.write(fontset);
            loadRom(rom);
        }

        private void loadRom(Chip8Rom rom) {
            this.rom = rom;
            memory.write(rom);
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
    }
}
