using System;
using System.IO;

namespace Chip8.Desktop.Emulator
{
    public class Chip8Rom
    {
        private const int LOAD_ADDRESS = 0x200;
        private const string ROM_DIR = @"/Users/evancoulson/Downloads/games/";

        private string romName;
        private byte[] romData;

        public Chip8Rom(string romName) {
            this.romName = romName;
            this.romData = File.ReadAllBytes(ROM_DIR + romName);
        }

        public string getRomName() {
            return this.romName;
        }

        private byte[] getRomBytes() {
            return this.romData;
        }

        public void load() {
            Chip8Memory.writeBlock(getRomBytes(), LOAD_ADDRESS);
        }
    }
}
