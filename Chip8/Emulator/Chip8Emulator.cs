using System;
using Microsoft.Xna.Framework.Input;

namespace Chip8.Desktop.Emulator
{
    public class Chip8Emulator
    {
        private Chip8CPU cpu;
        private Chip8Rom rom;
        private Chip8Timer delayTimer;
        private Chip8Timer soundTimer;
        private Chip8Keyboard keyboard;

        public Chip8Emulator(Chip8Rom rom) {
            this.keyboard = new Chip8Keyboard();
            this.delayTimer = new Chip8Timer();
            this.soundTimer = new Chip8Timer();
            this.cpu = new Chip8CPU(this);

            Chip8Fontset.loadFontset();
            rom.load();
        }

        private void loadRom(Chip8Rom newRom) {
            this.rom = newRom;
            rom.load();
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

        public Chip8Timer getDelayTimer() {
            return delayTimer;
        }

        public Chip8Timer getSoundTimer() {
            return soundTimer;
        }

        public Chip8Keyboard getKeyboard() {
            return keyboard;
        }
    }
}
