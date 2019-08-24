using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Menus
{
    public class OpenGameMenu
    {
        public bool showMenu;

        Dictionary<string, Rectangle> MenuButtonLocations;
        GraphicsDevice _graphics;
        ContentManager _content;
        SpriteFont font;
        Texture2D cursor;
        Vector2 cursotLoc;
        public string selected = null;

        // todo: maybe a nice texture to use?

        public OpenGameMenu(ContentManager content, GraphicsDevice graphics)
        {
            _graphics = graphics;
            _content = content;
            showMenu = true;

            MenuButtonLocations = new Dictionary<string, Rectangle>();
            MenuButtonLocations.Add("new", new Rectangle(GameOptions.PrefferedBackBufferWidth / 2 - 170, GameOptions.PrefferedBackBufferHeight / 2 - 170, 400, 200));
            MenuButtonLocations.Add("load", new Rectangle(GameOptions.PrefferedBackBufferWidth / 2 - 170, GameOptions.PrefferedBackBufferHeight / 2 + 100, 400, 200));

            font = _content.Load<SpriteFont>("helperFont");
            cursor = _content.Load<Texture2D>("pointer");

        }

        public void DrawInventory(SpriteBatch sb)
        {
            _graphics.Clear(Color.Firebrick);

            Color color = Color.Gainsboro;
            Color selectColor = Color.Gold;
            Color newColor = color;
            Color loadColor = color;
            if (selected == "new")
                newColor = selectColor;
            else if (selected == "load")
                loadColor = selectColor;

            Texture2D newGameButton = new Texture2D(_graphics, 400, 200);
            Color[] cdataNew = new Color[400 * 200];
            for (int i = 0; i < cdataNew.Length; ++i) cdataNew[i] = newColor;
            newGameButton.SetData(cdataNew);

            Texture2D loadGameButton = new Texture2D(_graphics, 400, 200);
            Color[] cdataLoad = new Color[400 * 200];
            for (int i = 0; i < cdataLoad.Length; ++i) cdataLoad[i] = loadColor;
            loadGameButton.SetData(cdataLoad);

            sb.Begin();
            sb.Draw(newGameButton, new Vector2(GameOptions.PrefferedBackBufferWidth / 2 - 170, GameOptions.PrefferedBackBufferHeight / 2 - 200), Color.White);
            sb.DrawString(font, "NEW GAME", new Vector2(GameOptions.PrefferedBackBufferWidth / 2 - 140, GameOptions.PrefferedBackBufferHeight / 2 - 170), Color.Black);
            sb.Draw(loadGameButton, new Vector2(GameOptions.PrefferedBackBufferWidth / 2 - 170, GameOptions.PrefferedBackBufferHeight / 2 + 100), Color.White);
            sb.DrawString(font, "LOAD GAME", new Vector2(GameOptions.PrefferedBackBufferWidth / 2 - 140, GameOptions.PrefferedBackBufferHeight / 2 + 130), Color.Black);
            sb.Draw(cursor, cursotLoc, Color.Black);
            sb.End();

        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            // if hovering button
            cursotLoc = Mouse.GetState().Position.ToVector2();
            Rectangle cursorRect = new Rectangle((int)cursotLoc.X, (int)cursotLoc.Y, 20, 20);
            selected = null;
            foreach (var button in MenuButtonLocations)
            {
                if (button.Value.Intersects(cursorRect))
                    selected = button.Key;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                showMenu = false;
            }
            
        }
    }
}
