using System;
namespace Chip8.Desktop.Emulator
{
    public class Chip8Timer
    {
        private byte value;

        public Chip8Timer() {
        }

        public bool isActive() {
            return value > 0;
        }

        public void decrement() {
            value--;
        }

        public byte get() {
            return value;
        }

        public void set(byte value) {
            this.value = value;
        }
    }
}
