using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models;
using Gusto.Models.Interfaces;
using Gusto.Models.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto
{
    public class GameState
    {

        public bool ready; 

        ContentManager _content;
        GraphicsDevice _graphics;

        public string name;
        public List<ISaveState> GameObjectSaveStates;
        public HashSet<Sprite> UpdateOrder;

        public GameState(ContentManager c, GraphicsDevice g)
        {
            _content = c;
            _graphics = g;
            UpdateOrder = new HashSet<Sprite>();
        }

        public void CreateNewGame()
        {
            BaseShip baseShip = new BaseShip(TeamType.Player, "GustoMap", new Vector2(-100, -500), _content, _graphics);
            BaseShip baseShipAI = new BaseShip(TeamType.A, "GustoMap", new Vector2(470, 0), _content, _graphics);

            UpdateOrder.Add(baseShip);
            UpdateOrder.Add(baseShipAI);

            ready = true;
        }

        public HashSet<Sprite> Update (KeyboardState kstate, GameTime gameTime, Camera camera)
        {

            foreach (Sprite sp in UpdateOrder)
            {
                //if (sp.remove)
                //    toRemove.Add(sp);
                
                // ICanUpdate is the update for main sprites. Any sub-sprites (items, weapons, sails, etc) that belong to the main sprite are updated within the sprite's personal update method. 
                ICanUpdate updateSp = (ICanUpdate)sp;
                updateSp.Update(kstate, gameTime, camera);
            }

            return UpdateOrder;
        }


    }
}
