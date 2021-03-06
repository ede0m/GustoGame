﻿using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.GameMap;
using Gusto.Bounding;
using Gusto.Models.Animated;
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

namespace Gusto.Models
{
    public class Interior
    {

        GraphicsDevice _graphics;
        ContentManager _content;

        public Guid interiorId;
        public string interiorTypeKey;
        public Sprite interiorForObj;

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

        public HashSet<Sprite> interiorGroundObjects; // objects that are encoded into the interior design
        public HashSet<Sprite> interiorObjects; // anything placed or drop in this interior (Similar to ItemUtility.ItemsToUpdate except that is for world view) The state of the interiror
        public HashSet<Sprite> interiorObjectsToAdd; // anything that needs to be added to this interior (can't just add in the sprite's update because it modifies collection while lookping through)

        // the following four are used by the calling draw method to do menus
        public Storage invStorage;
        public ICraftingObject craftObj;
        public bool showStorageMenu;
        public bool showCraftingMenu;

        public Vector2 startDrawPoint;

        public Interior(string itk, Sprite interiorFor, ContentManager content, GraphicsDevice graphics)
        {
            /*interiorWasLoaded = loadState;
            if (!interiorWasLoaded)
                interiorId = Guid.NewGuid();*/

            _graphics = graphics;
            _content = content;

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
                    List<Sprite> groundObjects = null;

                    // ground object
                    if ((string)tileDetails["sittingObject"] != "null")
                    {
                        groundObjects = GetGroundObjects((string)tileDetails["sittingObject"], drawPoint, content, graphics);

                        foreach (var groundObject in groundObjects)
                        {
                            groundObject.SetTileDesignRow(RandomEvents.rand.Next(0, groundObject.nRows));
                            //interiorGroundObjects.Add(groundObject);
                            interiorObjects.Add(groundObject);
                        }
                    }

                    // set interiorPiece piece
                    switch (tileDetails["terrainPiece"].ToString())
                    {
                        case "sd1":
                            tile = new ShipDeckTile(index, null, groundObjects, drawPoint, "GameGusto", content, graphics, "shipDeckTile");
                            break;
                        case "sd1w":
                            tile = new ShipDeckTileWall(index, null, groundObjects, drawPoint, "GameGusto", content, graphics, "shipDeckTileWall");
                            break;
                        case "si1":
                            tile = new ShipInteriorTile(index, null, groundObjects, drawPoint, "GameGusto", content, graphics, "shipInteriorTile");
                            break;
                        case "si1w":
                            tile = new ShipInteriorTileWall(index, null, groundObjects, drawPoint, "GameGusto", content, graphics, "shipInteriorTileWall");
                            break;
                        case "d1":
                            tile = new DirtTile(index, null, groundObjects, drawPoint, "GameGusto", content, graphics, "dirtTile");
                            break;
                        case "cvs1w":
                            tile = new CanvasTileWall(index, null, groundObjects, drawPoint, "GameGusto", content, graphics, "canvasTileWall");
                            break;
                    }

                    if (tile != null)
                    {
                        //tile.inInteriorId = interiorId;
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
                obj.inInteriorId = interiorId;

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
            {
                remove.inInteriorId = Guid.Empty;
                interiorObjects.Remove(remove);
            }
        }


        public void Draw(SpriteBatch sb, Camera cam, RenderTarget2D interiorScene)
        {

            // Draw the tileset
            Vector2 minCorner = new Vector2(cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));

            // setup drawing for interior on backbuffer
            _graphics.SetRenderTarget(interiorScene);
            _graphics.Clear(Color.Black);

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
                    tile.inInteriorId = interiorId; // TODO: need to move setting of InteriorId to constructor, but this screws up serialization
                    interiorTiles.Add(tile);

                    if (tile.groundObjects != null)
                    {
                        foreach (var groundObject in tile.groundObjects)
                            groundObject.location = loc;
                    }

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

            // reset these menu trackers
            showCraftingMenu = false;
            showStorageMenu = false;
            craftObj = null;
            invStorage = null;

            List<Sprite> drawOrder = interiorObjects.ToList();
            drawOrder.Sort((a, b) => a.GetBoundingBox().Bottom.CompareTo(b.GetBoundingBox().Bottom));
            // Draw any interior objs
            foreach (var obj in drawOrder)
            {
                // npcs always have random loc set when entering interior, other objects are randomly set initially, unless coming from a save
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

                if (obj is ICraftingObject)
                {
                    ICraftingObject tcraftObj = (ICraftingObject)obj;
                    tcraftObj.DrawCanCraft(sb, cam);
                    if (tcraftObj.GetShowMenu())
                    {
                        showCraftingMenu = true;
                        craftObj = tcraftObj;
                    }
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
                }
            }

            tilesSet = true;
            showingInterior = true;

        }

        private List<Sprite> GetGroundObjects(string key, Vector2 loc, ContentManager content, GraphicsDevice graphics)
        {
            List<Sprite> groundObjs = new List<Sprite>();
            switch (key)
            {
                case "fire1":
                    groundObjs.Add(new CampFire(TeamType.GroundObject, "GustoGame", loc, content, graphics));
                    break;
            }
            return groundObjs;
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
