using System;
using System.IO;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Chip8Emulator2
{
    class Emulator
    {
        const int SCALE = 16;
        const string FILE_NAME = "BRIX";
        const ushort LOAD_ADDR = 0x200;
        const ushort DISPLAY_SIZE = 256;
        const ushort STACK_SIZE = 96;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        private static Texture2D blackTexture;
        private static Texture2D whiteTexture;

        private static DateTime delayLast = DateTime.UtcNow;
        private static DateTime soundLast = DateTime.UtcNow;

        private static byte[] memory = new byte[4096];
        private static byte[] V = new byte[16];
        private static byte[] fileBytes;
        private static ushort I = 0;
        private static ushort ip = LOAD_ADDR;
        private static ushort sp = (ushort)(memory.Length - DISPLAY_SIZE - STACK_SIZE);
        private static byte delay_timer = 0;
        private static byte sound_timer = 0;
        public static bool[] gfx = new bool[64 * 32];
        private static Dictionary<Keys, int> keys = new Dictionary<Keys, int>() {
            { Keys.D1, 1 },
            { Keys.D2, 2 },
            { Keys.D3, 3 },
            { Keys.D4, 12},
            { Keys.Q, 4 },
            { Keys.W, 5 },
            { Keys.E, 6 },
            { Keys.R, 13 },
            { Keys.A, 7 },
            { Keys.S, 8 },
            { Keys.D, 9 },
            { Keys.F, 14 },
            { Keys.Z, 10 },
            { Keys.X, 0 },
            { Keys.C, 11 },
            { Keys.V, 15 }
        };

        private static byte[] chip8Fontset = new byte[] {
          0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
          0x20, 0x60, 0x20, 0x20, 0x70, // 1
          0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
          0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
          0x90, 0x90, 0xF0, 0x10, 0x10, // 4
          0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
          0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
          0xF0, 0x10, 0x20, 0x40, 0x40, // 7
          0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
          0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
          0xF0, 0x90, 0xF0, 0x90, 0x90, // A
          0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
          0xF0, 0x80, 0x80, 0x80, 0xF0, // C
          0xE0, 0x90, 0x90, 0x90, 0xE0, // D
          0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
          0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        public static void run() {
            Random random = new Random();
            byte b1 = memory[ip++];
            byte b2 = memory[ip++];
            byte nib = (byte)(b1 >> 4);
            byte x = (byte)(b1 & 0x0F);
            byte y = (byte)(b2 >> 4);
            ushort NNN = (ushort)((x << 8) + b2);
            byte lastNib = (byte)(b2 & 0x0F);

            if (delay_timer > 0)
            {
                DateTime now = DateTime.UtcNow;
                if (now - delayLast > TimeSpan.FromSeconds(1 / 60.0))
                {
                    delay_timer--;
                    delayLast = now;
                }
            }

            if (sound_timer > 0)
            {
                DateTime now = DateTime.UtcNow;
                if (now - soundLast > TimeSpan.FromSeconds(1 / 60.0))
                {
                    sound_timer--;
                    soundLast = now;
                }
            }
            else
            {
                Console.Beep();
            }

            switch (nib) {
                    case 0x00:
                        switch(b2) {
                            case 0xE0:
                                gfx = new bool[64 * 32];
                                break;
                            case 0xEE:
                                byte returnB1 = memory[--sp];
                                byte returnB2 = memory[--sp];
                                ip = (ushort)((returnB1 << 8) + returnB2);
                                break;
                            default:
                                throw new Exception();
                        }
                        break;
                    case 0x01:
                        ip = NNN;
                        break;
                    case 0x02:
                        memory[sp++] = (byte)(ip & 0xFF);
                        memory[sp++] = (byte)(ip >> 8);
                        ip = NNN;
                        break;
                    case 0x03:
                        if (V[x] == b2) {
                            ip += 2;
                        }
                        break;
                    case 0x04:
                        if (V[x] != b2) {
                            ip += 2;
                        }
                        break;
                    case 0x05:
                        if (V[x] == V[y]) {
                            ip += 2;
                        }
                        break;
                    case 0x06:
                        V[x] = b2;
                        break;
                    case 0x07:
                        V[x] += b2;
                        break;
                    case 0x08:
                        switch(lastNib) {
                            case 0x00:
                                V[x] = V[y];
                                break;
                            case 0x01:
                                V[x] = (byte)(V[x] | V[y]);
                                break;
                            case 0x02:
                                V[x] = (byte)(V[x] & V[y]);
                                break;
                            case 0x03:
                                V[x] = (byte)(V[x] ^ V[y]);
                                break;
                            case 0x04:
                                // handle VF
                                V[x] += V[y];
                                break;
                            case 0x05:
                                //handle VF
                                V[x] -= V[y];
                                break;
                            case 0x06:
                                //handle VF
                                byte lsb = (byte)(V[y] & 0xFF);
                                V[0x0F] = lsb;
                                V[x] = (byte)(V[y] >> 1);
                                break;
                            case 0x07:
                                //handle VF
                                V[x] = (byte)(V[y] - V[x]);
                                break;
                            case 0x0E:
                                //handle VF 
                                byte msb = (byte)(V[y] >> 7);
                                V[0x0f] = msb;
                                V[x] = V[y] = (byte)(V[y] << 1);
                                break;
                            default:
                                throw new Exception();
                        }
                        break;
                    case 0x09:
                        if (V[x] != V[y]) {
                            ip += 2;
                        }
                        break;
                    case 0x0A:
                        I = NNN;
                        break;
                    case 0x0B:
                        ip = (ushort)(V[0] + NNN);
                        break;
                    case 0x0C:
                        V[x] = (byte)(random.Next(256) & b2);
                        break;
                    case 0x0D:
                        draw(V[x], V[y], lastNib);
                        break;
                    case 0x0E:
                        switch(b2) {
                            case 0x9E:
                                if (getKeyPressed() == V[x]) {
                                    ip += 2;
                                }
                                break;
                            case 0xA1:
                                if (getKeyPressed() != V[x]) {
                                    ip += 2;
                                }
                                break;
                            default:
                                throw new Exception();
                        }
                        break;
                    case 0x0F:
                        switch(b2) {
                            case 0x07:
                                V[x] = delay_timer;
                                break;
                            case 0x0A:
                                V[x] = (byte)readKey();
                                break;
                            case 0x15:
                                delay_timer = V[x];
                                break;
                            case 0x18:
                                sound_timer = V[x];
                                break;
                            case 0x1E:
                                I += V[x];
                                break;
                            case 0x29:
                                I = (byte)(5 * V[x]);
                                break;
                            case 0x33:
                                memory[I] = (byte)(V[x] / 100);
                                memory[I + 1] = (byte)((V[x] % 100) / 10);
                                memory[I + 2] = (byte)(V[x] % 10);
                                break;
                            case 0x55:
                                for (int i = 0; i <= x; i++) {
                                    memory[I++] = V[i];
                                }
                                break;
                            case 0x65:
                                for (int i = 0; i <= x; i++) {
                                    V[i] = memory[I++];
                                }
                                break;
                            default:
                                throw new Exception();
                        }
                        break;
                    default:
                        throw new Exception();
                }
        }

        static int getKeyPressed() {
            KeyboardState state = Keyboard.GetState();
            foreach (KeyValuePair<Keys, int> pair in keys) {
                if (state.IsKeyDown(pair.Key)) {
                    return pair.Value;
                }
            }
            return -1;
        }

        static int readKey() {
            while (true) {
                int i = getKeyPressed();
                if (i != -1) {
                    return i;
                }
            }
        }

        public static void loadFile() {
            fileBytes = File.ReadAllBytes(@"/Users/evancoulson/Downloads/games/" + FILE_NAME);
            int reservedSize = memory.Length - LOAD_ADDR - DISPLAY_SIZE - STACK_SIZE;
            if (fileBytes.Length > reservedSize) {
                throw new Exception("file size greater than reserved memory");
            }
            Buffer.BlockCopy(fileBytes, 0, memory, LOAD_ADDR, fileBytes.Length);
        }

        static void disassemble(int fileSize) {
            for (int i = LOAD_ADDR; i < fileSize + LOAD_ADDR; i += 2) {
                Console.WriteLine(i.ToString("X4") + " " + memory[i].ToString("X2") + " " + memory[i + 1].ToString("X2"));
            }
        }

        private static void draw(byte x, byte y, byte n) {
            V[0x0F] = 0;
            for (int dY = 0; dY < n; dY++) {
                byte pixel = memory[I + dY];
                for (int dX = 0; dX < 8; dX++) {
                    int offset = ((y + dY) * 64) + x + dX;
                    if ((pixel & (0x80 >> dX)) != 0) {
                        if (gfx[offset]) {
                            V[0x0F] = 1;
                        }
                        gfx[offset] ^= true;
                    }
                }
            }
        }

        public static void setGraphicsDevice(GraphicsDeviceManager graphics) {
            Emulator.graphics = graphics;
            whiteTexture = createTexture(Color.White);
            blackTexture = createTexture(Color.Black);
        }

        private static Texture2D createTexture(Color color) {
            Texture2D texture = new Texture2D(graphics.GraphicsDevice, SCALE, SCALE);
            Color[] colors = new Color[SCALE * SCALE];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = color;
            }
            texture.SetData(colors);
            return texture;
        }

        public static void drawRect(int x, int y, bool color) {
            Rectangle coords = new Rectangle(x * SCALE, y * SCALE, SCALE, SCALE);
            if (color) {
                spriteBatch.Draw(whiteTexture, coords, Color.White);
            } else {
                spriteBatch.Draw(blackTexture, coords, Color.Black);
            }
        }
    }
}
