using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.GameMap;
using Gusto.Bounding;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated
{
    public class Interior
    {

        public Guid interiorId;
        public string interiorTypeKey;
        private int width;
        private int height;
        private int cols;
        private int rows;

        private JObject _interiorMapData;
        private List<TilePiece> interiorMap;

        public HashSet<TilePiece> interiorTiles;
        bool tilesSet;
        public bool showingInterior;
        public bool interiorWasLoaded; // was the interior loaded from a file? used to preserve obj location on load instead of random loc

        public Vector2 speed; // needed for moving interiors like ships

        public HashSet<Sprite> interiorObjects; // anything placed or drop in this interior (Similar to ItemUtility.ItemsToUpdate except that is for world view) The state of the interiror
        public HashSet<Sprite> interiorObjectsToAdd; // anything that needs to be added to this interior (can't just add in the sprite's update because it modifies collection while lookping through)

        // the following three are used by the calling draw method to do menus
        public Storage invStorage;
        public bool showStorageMenu;
        public bool showCraftingMenu;

        public Vector2 startDrawPoint;

        public Sprite interiorForObj;

        public Interior(string itk, Sprite interiorFor, ContentManager content, GraphicsDevice graphics)
        {
            interiorTypeKey = itk;
            interiorMap = new List<TilePiece>();
            interiorTiles = new HashSet<TilePiece>();

            interiorObjects = new HashSet<Sprite>();
            interiorObjectsToAdd = new HashSet<Sprite>();
            interiorForObj = interiorFor;
            // load the interiorMap tileset
            _interiorMapData = JObject.Parse(File.ReadAllText(@"C:\Users\GMON\source\repos\GustoGame\GustoGame\Content\" + interiorTypeKey + "Interior.json"));

            // set the interior map
            int multX = (int)_interiorMapData["multX"];
            int multY = (int)_interiorMapData["multY"];

            width = multX * GameOptions.PrefferedBackBufferWidth;
            height = multY * GameOptions.PrefferedBackBufferHeight;
            cols = width / (GameOptions.tileWidth * 2);
            rows = height / (GameOptions.tileHeight * 2);

            if (interiorForObj != null)
                startDrawPoint = new Vector2(interiorForObj.location.X - (width / 2), interiorForObj.location.Y - (height / 2));
            else
                startDrawPoint = Vector2.Zero;

            Vector2 drawPoint = startDrawPoint;

            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    TilePiece tile = null;
                    JObject tileDetails = _interiorMapData["data"][index.ToString()].Value<JObject>();

                    //Vector2 loc = drawPoint;

                    // set interiorPiece piece
                    switch (tileDetails["terrainPiece"].ToString())
                    {
                        case "sd1":
                            tile = new ShipDeckTile(index, null, drawPoint, "GameGusto", content, graphics, "shipDeckTile");
                            break;
                        case "sd1w":
                            tile = new ShipDeckTileWall(index, null, drawPoint, "GameGusto", content, graphics, "shipDeckTileWall");
                            break;
                        case "si1":
                            tile = new ShipInteriorTile(index, null, drawPoint, "GameGusto", content, graphics, "shipInteriorTile");
                            break;
                        case "si1w":
                            tile = new ShipInteriorTileWall(index, null, drawPoint, "GameGusto", content, graphics, "shipInteriorTileWall");
                            break;
                    }

                    if (tile != null)
                    {
                        tile.SetTileDesignRow(RandomEvents.rand.Next(0, tile.nRows));
                        interiorTiles.Add(tile);
                    }

                    interiorMap.Add(tile);
                    index++;
                }
            }

            // Go through again to set all columns to correct frame for walls
            int index2 = 0;
            foreach (var tile in interiorMap)
            {
                if (tile == null)
                {
                    index2++;
                    continue;
                }

                if (tile.wallPiece)
                {
                    // left
                    if (interiorMap[index2 + 1] != null && interiorMap[index2 - 1] == null && !interiorMap[index2 + 1].wallPiece)
                        tile.currColumnFrame = 3; // left wall
                    // right
                    if (interiorMap[index2 - 1] != null && interiorMap[index2 + 1] == null && !interiorMap[index2 - 1].wallPiece)
                        tile.currColumnFrame = 1; // right wall
                    // bottom wall
                    if (interiorMap[index2 - cols] != null && interiorMap[index2 + cols] == null && !interiorMap[index2 - cols].wallPiece) // check above
                        tile.currColumnFrame = 2; // bottom wall
                    // top wall (looks the best)
                    if (interiorMap[index2 + cols] != null && interiorMap[index2 - cols] == null && !interiorMap[index2 + cols].wallPiece) // check below
                        tile.currColumnFrame = 0;
                }

                index2++;
            }

        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            HashSet<Sprite> toRemove = new HashSet<Sprite>();

            // add any dropped objects to the interior
            foreach (var dropped in interiorObjectsToAdd)
                interiorObjects.Add(dropped);
            interiorObjectsToAdd.Clear();

            foreach (var obj in interiorObjects)
            {

                if (obj.remove)
                    toRemove.Add(obj);

                // match speed of interior (for ships)
                obj.location.X += speed.X;
                obj.location.Y += speed.Y;

                if (obj is ICanUpdate && !(obj is IPlayer)) // let gamestate update the player
                {
                    ICanUpdate updateSp = (ICanUpdate)obj;
                    updateSp.Update(kstate, gameTime, camera);
                }

            }

            foreach (var remove in toRemove)
                interiorObjects.Remove(remove);
        }


        public void Draw(SpriteBatch sb, Camera cam)
        {

            // Draw the tileset
            Vector2 minCorner = new Vector2(cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));

            startDrawPoint = new Vector2(interiorForObj.location.X - (width / 2), interiorForObj.location.Y - (height / 2));
            Vector2 drawPoint = startDrawPoint;
            interiorTiles.Clear();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    TilePiece tile = interiorMap[((cols) * i) + j];
                    if (tile == null)
                    {
                        drawPoint.X += GameOptions.tileWidth * 2;
                        continue;
                    }
                    tile.location = drawPoint;
                    var loc = tile.location;
                    tile.location += speed;
                    interiorTiles.Add(tile);

                    // draw if in viewporit
                    if ((loc.X >= minCorner.X && loc.X <= maxCorner.X) && (loc.Y >= minCorner.Y && loc.Y <= maxCorner.Y))
                    {
                        tile.Draw(sb, cam);
                    }
                    drawPoint.X += GameOptions.tileWidth * 2;
                }
                drawPoint.Y += GameOptions.tileHeight * 2;
                drawPoint.X = startDrawPoint.X;
            }

            List<Sprite> drawOrder = interiorObjects.ToList();
            drawOrder.Sort((a, b) => a.GetYPosition().CompareTo(b.GetYPosition()));
            // Draw any items
            foreach (var obj in drawOrder)
            {
                if ((!tilesSet && !(obj is IPlayer) && !interiorWasLoaded) || (obj is INPC && !showingInterior))
                    obj.location = RandomInteriorTile().location;

                // special draw for player
                if (obj is IPlayer)
                {
                    DrawUtility.DrawPlayer(sb, cam, (PiratePlayer)obj);
                    continue;
                }

                obj.Draw(sb, cam);

                if (obj is IVulnerable)
                {
                    IVulnerable v = (IVulnerable)obj;
                    v.DrawHealthBar(sb, cam);
                }

                if (obj is IInventoryItem)
                {
                    InventoryItem item = (InventoryItem)obj;
                    if (!item.inInventory)
                        item.DrawPickUp(sb, cam);
                }

                if (obj is IPlaceable)
                {
                    IPlaceable placeObj = (IPlaceable)obj;
                    placeObj.DrawCanPickUp(sb, cam);
                }

                if (obj is IStorage)
                {
                    Storage storage = (Storage)obj;
                    storage.DrawOpenStorage(sb, cam);
                    if (storage.storageOpen)
                    {
                        showStorageMenu = true;
                        invStorage = storage;
                    }
                    else
                    {
                        showStorageMenu = false;
                        invStorage = null;
                    }
                }

                if (obj.GetType().BaseType == typeof(Gusto.Models.Animated.CraftingObject))
                {
                    CraftingObject craft = (CraftingObject)obj;
                    craft.DrawCanCraft(sb, cam);
                    if (craft.drawCraftingMenu)
                        showCraftingMenu = true;
                    else
                        showCraftingMenu = false;
                }
            }

            tilesSet = true;
            showingInterior = true;

        }

        public TilePiece RandomInteriorTile()
        {
            // don't include wall pieces
            TilePiece t = interiorTiles.ElementAt(RandomEvents.rand.Next(interiorTiles.Count));
            while (t.wallPiece == true)
            {
                t = interiorTiles.ElementAt(RandomEvents.rand.Next(interiorTiles.Count));
            }
            return t;
        }

        public Guid GetInteriorForId()
        {
            if (interiorForObj is IHasInterior)
            {
                IHasInterior obj = (IHasInterior)interiorForObj;
                return obj.GetInteriorForId();
            }
            else
                return Guid.Empty;
        }

    }
}
