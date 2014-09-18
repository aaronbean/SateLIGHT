using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace POC
{
    public struct player_data
    {
        public Vector2 pos;
        public bool active;
        public Color color;
    }

    public class smoke_particle
    {
        public Vector2 pos;
        public byte frames_left;
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SateLightLite : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int screen_width;
        int screen_height;

        Vector2 cross_hairs_position;

        /// <summary>
        /// Background texture and positioning for it
        /// </summary>
        Texture2D background;
        Texture2D smoke_cloud;
        SpriteFont default_font;

        /* List of all of the current smoke particles */
        List<smoke_particle> smoke_list;
        Random randomizer;
        public SateLightLite()
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

            base.Initialize();
            smoke_list = new List<smoke_particle>();
            randomizer = new Random();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = this.Content.Load<Texture2D>("heightmap");
            default_font = this.Content.Load<SpriteFont>("default_text");
            smoke_cloud = this.Content.Load<Texture2D>("smoke");

            screen_height = GraphicsDevice.PresentationParameters.BackBufferHeight;
            screen_width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            process_input();
            create_smoke();

            base.Update(gameTime);
        }

        private void process_input()
        {
            MouseState ms_state = Mouse.GetState();
            cross_hairs_position.X = ms_state.X;
            cross_hairs_position.Y = ms_state.Y;
            if (ms_state.LeftButton == ButtonState.Pressed)
            {
                create_smoke();
            }
            
        }

        private void create_smoke()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 cur_pos = cross_hairs_position;
                /* move the smoke randomly -5 to 5 pixels in both x,y directions */
                cur_pos.X += randomizer.Next(-5, 5);
                cur_pos.Y += randomizer.Next(-5, 5);
                smoke_particle new_part = new smoke_particle();
                new_part.pos = cur_pos;
                new_part.frames_left = (byte)(200 + randomizer.Next(55));
                smoke_list.Add(new_part);
            }
        }

        private void draw_smoke()
        {
            int i = 0;
            while (i < smoke_list.Count)
            {
                Color smoke_color = new Color(255, 255, 255, smoke_list[i].frames_left);
                spriteBatch.Draw(smoke_cloud, smoke_list[i].pos, null, smoke_color, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);
                if (smoke_list[i].frames_left == 0)
                {
                    smoke_list.RemoveAt(i);
                }
                else
                {
                    smoke_list[i].frames_left--;
                    i++;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // CornHoleFlowerBlue is gay
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            draw_background();
            draw_text();
            draw_smoke();
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void draw_background()
        {
            Rectangle entire_screen = new Rectangle(0, 0, screen_width, screen_height);
            if (entire_screen == null)
            {
                throw new Exception("Failed to create Rectangle");
            }

            spriteBatch.Draw(background, entire_screen, Color.White);
        }

        private void draw_text()
        {
            spriteBatch.DrawString(default_font, "This is a test", new Vector2(20, 20), Color.White);
        }
    }
}
