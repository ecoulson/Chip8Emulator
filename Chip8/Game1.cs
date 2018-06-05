using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Chip8.Desktop.Emulator;

namespace Chip8.Desktop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Chip8Emulator emulator;
        Texture2D whitePixel;
        Texture2D blackPixel;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = Chip8Display.SCREEN_WIDTH * Chip8Display.SCREEN_SCALE;
            graphics.PreferredBackBufferHeight = Chip8Display.SCREEN_HEIGHT * Chip8Display.SCREEN_SCALE;
            graphics.ApplyChanges();

            createTextures();

            Chip8Rom rom = new Chip8Rom("PONG");
            this.emulator = new Chip8Emulator(rom);

            base.Initialize();
        }

        private void createTextures() {
            this.whitePixel = createPixelTexture(Color.White);
            this.blackPixel = createPixelTexture(Color.Black);
        }

         private Texture2D createPixelTexture(Color color) {
            Texture2D texture = new Texture2D(graphics.GraphicsDevice, Chip8Display.SCREEN_SCALE, Chip8Display.SCREEN_SCALE);
            Color[] colorData = new Color[Chip8Display.SCREEN_SCALE * Chip8Display.SCREEN_SCALE];
            for (int i = 0; i < colorData.Length; i++) {
                colorData[i] = color;
            }
            texture.SetData(colorData);
            return texture;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            KeyboardState state = Keyboard.GetState();
            emulator.handleKeyboardState(state);
            emulator.cycle();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            bool[] currentGFX = Chip8Display.gfx;
            for (int i = 0; i < currentGFX.Length; i++) {
                int x = i % Chip8Display.SCREEN_WIDTH;
                int y = i / Chip8Display.SCREEN_WIDTH;
                Vector2 pos = new Vector2(x * Chip8Display.SCREEN_SCALE, y * Chip8Display.SCREEN_SCALE);
                if (currentGFX[i]) {
                    spriteBatch.Draw(whitePixel, pos, Color.White);
                } else {
                    spriteBatch.Draw(blackPixel, pos, Color.Black);
                }
            }
            spriteBatch.End();
        }
    }
}
