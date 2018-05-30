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

        private bool[] gfx;

        public Texture2D whitePixel;
        public Texture2D blackPixel;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Chip8Display() {
            this.gfx = new bool[SCREEN_SIZE];
        }

        public void initializeGraphics(GraphicsDeviceManager graphics, SpriteBatch spriteBatch) {
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.whitePixel = createPixelTexture(Color.White);
            this.blackPixel = createPixelTexture(Color.Black);
        }

        private Texture2D createPixelTexture(Color color) {
            Texture2D texture = new Texture2D(graphics.GraphicsDevice, SCREEN_SCALE, SCREEN_SCALE);
            Color[] colorData = new Color[SCREEN_SCALE * SCREEN_SCALE];
            for (int i = 0; i < colorData.Length; i++) {
                colorData[i] = color;
            }
            texture.SetData(colorData);
            return texture;
        }

        public void clear() {
            for (int i = 0; i < SCREEN_SIZE; i++) {
                gfx[i] = false;
            }
        }

        public void draw(Chip8Opcode opcode) {
            cpu.setRegister(0x0F, 0);
            spriteBatch.Begin();
            for (int dY = 0; dY < opcode.N; dY++) {
                ushort location = (ushort)(cpu.getRegisterI() + dY);
                byte pixel = cpu.read(location);
                for (int dX = 0; dX < 8; dX++) {
                    int offset = ((opcode.y + dY) * SCREEN_WIDTH) + opcode.x + dX;
                    if ((pixel & (0x80 >> dX)) != 0) {
                        if (gfx[offset]) {
                            cpu.setRegister(0x0F, 1);
                        }
                        gfx[offset] ^= true;
                        drawRect(opcode.x + dX, opcode.y + dY, gfx[offset]);
                    }
                }
            }
            spriteBatch.End();
        }

        private void drawRect(int x, int y, bool isWhite) {
            if (isWhite) {
                spriteBatch.Draw(whitePixel, new Vector2(x, y), Color.White);
            } else {
                spriteBatch.Draw(blackPixel, new Vector2(x, y), Color.Black);
            }
        }
    }
}
