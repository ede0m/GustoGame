using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Bounding;
using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Gusto.Models.Menus;
using Gusto.Utility;
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

        public string gameName;
        public List<ISaveState> GameObjectSaveStates;
        public HashSet<Sprite> UpdateOrder;

        public PiratePlayer player; // only track the player because there are some special focuses around it.. obviously

        public GameState(ContentManager c, GraphicsDevice g)
        {
            _content = c;
            _graphics = g;
            UpdateOrder = new HashSet<Sprite>();

            player = new PiratePlayer(TeamType.Player, "GustoMap", new Vector2(0, -300), _content, _graphics); // This is a default location (for new game) if there is a load it will be overwritten
        }

        public void CreateNewGame()
        {

            //TEMPORARY NEED TO CREATE SOME SORT OF GAME SETUP / REGION SETUP
            List<Sprite> giannaLandTiles = BoundingBoxLocations.RegionMap["Gianna"].RegionLandTiles;
            Sprite GiannaRegionTile = giannaLandTiles[RandomEvents.rand.Next(giannaLandTiles.Count)];
            var screenCenter = new Vector2(_graphics.Viewport.Bounds.Width / 2, _graphics.Viewport.Bounds.Height / 2);

            BaseShip baseShip = new BaseShip(TeamType.Player, "GustoMap", new Vector2(-100, -500), _content, _graphics);
            BaseShip baseShipAI = new BaseShip(TeamType.A, "GustoMap", new Vector2(470, 0), _content, _graphics);
            BaseTribal baseTribal = new BaseTribal(TeamType.B, "Gianna", GiannaRegionTile.location, _content, _graphics);
            Tower tower = new BaseTower(TeamType.A, "GustoMap", new Vector2(200, 700), _content, _graphics);
            ClayFurnace furnace = new ClayFurnace(TeamType.Player, "GustoMap", new Vector2(180, 140), _content, _graphics);
            CraftingAnvil craftingAnvil = new CraftingAnvil(TeamType.Player, "GustoMap", new Vector2(120, 40), _content, _graphics);
            BaseBarrel barrelLand = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(-20, -160), _content, _graphics);
            BaseBarrel barrelOcean = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(380, -60), _content, _graphics);
            BaseChest chestLand = new BaseChest(TeamType.A, "GustoMap", new Vector2(100, -120), _content, _graphics);
            BaseChest chestOcean = new BaseChest(TeamType.A, "GustoMap", new Vector2(350, 0), _content, _graphics);

            Shovel shovel = new Shovel(TeamType.A, "GustoMap", new Vector2(200, -330), _content, _graphics);
            shovel.onGround = true;
            Pickaxe pickaxe = new Pickaxe(TeamType.Player, "GustoMap", new Vector2(130, -430), _content, _graphics);
            pickaxe.onGround = true;
            Pistol pistol = new Pistol(TeamType.A, "GustoMap", new Vector2(250, -300), _content, _graphics);
            pistol.amountStacked = 1;
            pistol.onGround = true;
            PistolShotItem pistolAmmo = new PistolShotItem(TeamType.A, "GustoMap", new Vector2(220, -300), _content, _graphics);
            pistolAmmo.amountStacked = 14;
            pistolAmmo.onGround = true;
            CannonBallItem cannonAmmo = new CannonBallItem(TeamType.A, "GustoMap", new Vector2(200, -300), _content, _graphics);
            cannonAmmo.amountStacked = 10;
            cannonAmmo.onGround = true;
            Lantern lantern = new Lantern(TeamType.A, "GustoMap", new Vector2(180, -300), _content, _graphics);
            lantern.onGround = true;
            BasePlank basePlank = new BasePlank(TeamType.A, "GustoMap", new Vector2(150, -300), _content, _graphics);
            basePlank.onGround = true;
            basePlank.amountStacked = 10;


            UpdateOrder.Add(baseShip);
            UpdateOrder.Add(baseShipAI);
            UpdateOrder.Add(player);
            UpdateOrder.Add(baseTribal);
            UpdateOrder.Add(tower);
            UpdateOrder.Add(lantern);
            UpdateOrder.Add(furnace);
            UpdateOrder.Add(craftingAnvil);
            UpdateOrder.Add(barrelLand);
            UpdateOrder.Add(barrelOcean);
            UpdateOrder.Add(chestLand);
            UpdateOrder.Add(chestOcean);
            UpdateOrder.Add(shovel);
            UpdateOrder.Add(pistol);
            UpdateOrder.Add(pickaxe);
            UpdateOrder.Add(pistolAmmo);
            UpdateOrder.Add(cannonAmmo);
            UpdateOrder.Add(basePlank);

            ready = true;
        }

        public HashSet<Sprite> Update (KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            List<Sprite> toRemove = new List<Sprite>();

            // camera follows player
            if (!player.onShip)
                camera.Position = player.location;
            else
                camera.Position = player.playerOnShip.location;

            foreach (Sprite sp in UpdateOrder)
            {
                // ICanUpdate is the update for main sprites. Any sub-sprites (items, weapons, sails, etc) that belong to the main sprite are updated within the sprite's personal update method. 
                ICanUpdate updateSp = (ICanUpdate)sp;
                updateSp.Update(kstate, gameTime, camera);
            }


            return UpdateOrder;
        }


    }
}
