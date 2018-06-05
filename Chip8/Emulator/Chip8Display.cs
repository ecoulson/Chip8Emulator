using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Chip8.Desktop.Emulator
{
    public class Chip8Display
    {
        public const int SCREEN_WIDTH = 64;
        public const int SCREEN_HEIGHT = 32;
        public const int SCREEN_SIZE = SCREEN_WIDTH * SCREEN_HEIGHT;
        public const int SCREEN_SCALE = 16;
        public static bool[] gfx = new bool[SCREEN_SIZE];

        public static void clear() {
            for (int i = 0; i < SCREEN_SIZE; i++) {
                gfx[i] = false;
            }
        }

        public static void draw(int offset, bool isWhite) {
            gfx[offset] = isWhite;
        }

        public static bool isWhite(int offset) {
            return gfx[offset];
        }
    }
}
