using System;
using System.Collections.Generic;
using Comora;
using Gusto.Bounds;
using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto.Models
{
    public abstract class Sprite
    {
        private GraphicsDevice _graphics; 

        protected Texture2D _texture;
        protected Texture2D boundingBox;
        protected Texture2D shadowText;
        public Rectangle targetRectangle;
        private Rectangle boundingBoxRect;
        private Polygon boundingPolygon;
        public float spriteScale;
        public float transparency;
        public float rotation;

        public Vector2 location;
        public int nRows { get; set; }
        public int nColumns { get; set; }
        public int currRowFrame;
        public int currColumnFrame;

        public string bbKey { get; set; }
        public string regionKey;
        public bool moving { get; set; }
        public bool colliding { get; set; }
        public bool remove;

        public Point mapCordPoint { get; set; }
        public Guid inInteriorId { get; set; }

        public Sprite(GraphicsDevice graphics)
        {
            _graphics = graphics;
            transparency = 1;
            inInteriorId = Guid.Empty;
        }

        public void SetSpriteAsset(Asset asset, Vector2 startingLoc)
        {
            _texture = asset.Texture;
            spriteScale = asset.Scale;
            transparency = 1;
            rotation = 0;
            location = startingLoc;
            nRows = asset.Rows;
            nColumns = asset.Columns;
            currRowFrame = 0;
            currColumnFrame = 0;
            bbKey = asset.BBKey;
            regionKey = asset.RegionKey;
            moving = true;
            remove = false;
            inInteriorId = Guid.Empty;

            int width = _texture.Width / nColumns;
            int height = _texture.Height / nRows;
            targetRectangle = new Rectangle(width * currColumnFrame, height * currRowFrame, width, height); // x and y here are cords of the texture

            if (bbKey != null)
            {
                boundingBoxRect = BoundingBoxTextures.DynamicBoundingBoxTextures[bbKey][currColumnFrame.ToString() + currRowFrame.ToString()];
                boundingBoxRect.X = ((int)location.X + ((int)(targetRectangle.Right * spriteScale) - (int)(targetRectangle.Left * spriteScale)) / 2) - ((boundingBoxRect.Right - boundingBoxRect.Left) / 2);
                boundingBoxRect.Y = ((int)location.Y + ((int)(targetRectangle.Bottom * spriteScale) - (int)(targetRectangle.Top * spriteScale)) / 2) - ((boundingBoxRect.Bottom - boundingBoxRect.Top) / 2);

                // polygon
                if (BoundingBoxTextures.DynamicBoundingPolygons.ContainsKey(bbKey))
                {
                    boundingPolygon = new Polygon();
                    boundingPolygon.Verts = BoundingBoxTextures.DynamicBoundingPolygons[bbKey][currColumnFrame.ToString() + currRowFrame.ToString()].Verts;
                    boundingPolygon.UpperLeftPoint = Vector2.Zero;
                }
            }

            // For DEBUG highlighting bounding box
            if (asset.BBTexture != null)
            {
                // generate a bounding box around the sprite at the current location
                int bbW = asset.BBTexture.Width;
                int bbH = asset.BBTexture.Height;
                Color[] data = new Color[bbW * bbH];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.Orange;
                asset.BBTexture.SetData(data);
                boundingBox = asset.BBTexture;
            }
        }

        public void SetTileDesignColumn(int c)
        {
            currColumnFrame = c;
        }

        public void SetTileDesignRow(int r)
        {
            currRowFrame = r;
        }

        // handles cycling the frames at sprite sheet end and beginning
        public Tuple<int, int> BoundFrames(int currRowFrame, int currColFrame)
        {
            if (currRowFrame < 0)
                currRowFrame = nRows - 1;
            else if (currRowFrame == nRows)
                currRowFrame = 0;

            if (currColumnFrame == nColumns)
                currColumnFrame = nColumns - 1;
            else if (currColumnFrame < 0)
                currColumnFrame = 0;

            return new Tuple<int, int>(currRowFrame, currColumnFrame);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            int width = _texture.Width / nColumns;
            int height = _texture.Height / nRows;

            // update target index frame on sprite sheet (x and Y are cords of the sprite)
            targetRectangle.X = width * currColumnFrame;
            targetRectangle.Y = height * currRowFrame;
            targetRectangle.Width = width;
            targetRectangle.Height = height;


            // update bounding box (x and y are cords of the screen here) -- WONT UPDATE STATIC SPRITES
            SetBoundingBox();

            if (camera == null)
                spriteBatch.Begin();
            else
                spriteBatch.Begin(camera);
            
            // TEST STUFF for trying to draw bounding box around sprite
            if (boundingBox != null)
            {
                if (boundingPolygon != null)
                    DrawPolygonBB(spriteBatch);
                else
                    spriteBatch.Draw(boundingBox, boundingBoxRect.Location.ToVector2(), boundingBoxRect, Color.Orange, 0f,
                        Vector2.Zero, 1.0f, SpriteEffects.None, 0f); // scaling is already done in constructor
            }

            Vector2 origin = new Vector2(width / 2, height / 2);
            if (this is ITilePiece)
            {
                origin = Vector2.Zero;
            }
            // normal drawing call
            spriteBatch.Draw(_texture, location, targetRectangle, Color.White * transparency, rotation,
                origin, spriteScale, SpriteEffects.None, 0f);

            spriteBatch.End();
        }

        // SETS Location of bounding box using width and height from preprocessed bb size. 
        public void SetBoundingBox()
        {
            if (bbKey != null)
            {

                boundingBoxRect = BoundingBoxTextures.DynamicBoundingBoxTextures[bbKey][currColumnFrame.ToString() + currRowFrame.ToString()];
                // texture is drawn to orgin, so we must offset our bounding boxes by this manually
                var originXOffset = ((int)(targetRectangle.Right * spriteScale) - (int)(targetRectangle.Left * spriteScale)) / 2;
                var originYOffset = ((int)(targetRectangle.Bottom * spriteScale) - (int)(targetRectangle.Top * spriteScale)) / 2;
                if (this is ITilePiece)
                {
                    boundingBoxRect.X = (int)location.X;
                    boundingBoxRect.Y = (int)location.Y;
                    boundingBoxRect.Width = _texture.Width / nColumns;
                    boundingBoxRect.Height = _texture.Height / nRows;
                }
                else if (!(this is IHandHeld))
                {
                    boundingBoxRect.X = (((int)location.X + ((int)(targetRectangle.Right * spriteScale) - (int)(targetRectangle.Left * spriteScale)) / 2) - ((boundingBoxRect.Right - boundingBoxRect.Left) / 2)) - originXOffset;
                    boundingBoxRect.Y = (((int)location.Y + ((int)(targetRectangle.Bottom * spriteScale) - (int)(targetRectangle.Top * spriteScale)) / 2) - ((boundingBoxRect.Bottom - boundingBoxRect.Top) / 2)) - originYOffset;
                }
                else
                {
                    boundingBoxRect.X = (int)location.X  - originXOffset + boundingBoxRect.X;
                    boundingBoxRect.Y = (int)location.Y - originYOffset + boundingBoxRect.Y;
                }

                // THIS DOESN'T WORK FOR NOW :( - non axis aligned collision
                /*if (rotation != 0)
                {
                    Vector2 bbRectVect = boundingBoxRect.Location.ToVector2();
                    var m = Matrix.CreateRotationZ((rotation * (float)(180.0 / Math.PI)));
                    var translateTo = Matrix.CreateTranslation(bbRectVect.X, bbRectVect.Y, 0);
                    var translateBack = Matrix.CreateTranslation(-bbRectVect.X, -bbRectVect.Y, 0);
                    var combined = translateTo * m * translateBack;
                    Vector2 rotatedBBLoc = Vector2.Transform(boundingBoxRect.Location.ToVector2(), combined);
                    boundingBoxRect.X = (int)rotatedBBLoc.X;
                    boundingBoxRect.Y = (int)rotatedBBLoc.Y;
                }*/

                // polygon collision
                if (boundingPolygon != null)
                {
                    boundingPolygon.Verts = BoundingBoxTextures.DynamicBoundingPolygons[bbKey][currColumnFrame.ToString() + currRowFrame.ToString()].Verts; // TODO - Bug! we nee a copy here, not the static ref
                    boundingPolygon.UpperLeftPoint = new Vector2(location.X - originXOffset, location.Y - originYOffset);
                }
            }
        }

        public void DrawShadow(SpriteBatch spriteBatch, Camera camera, float sunAngleX, float shadowTransparency)
        {
            int width = _texture.Width / nColumns;
            int height = _texture.Height / nRows;

            sunAngleX = -1 * sunAngleX;

            targetRectangle.X = width * currColumnFrame;
            targetRectangle.Y = height * currRowFrame;
            targetRectangle.Width = width;
            targetRectangle.Height = height;

            if (!(this is IShip)) // don't rotate ship shadow as much - doesn't look right
                sunAngleX = sunAngleX * 1.75f;

            Matrix slant = Matrix.CreateTranslation(-location.X, -location.Y + ((GetBoundingBox().Height/2)), 0f) *
                Matrix.CreateRotationX(MathHelper.ToRadians(-1 * sunAngleX)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(40)) *
                Matrix.CreateScale(1.3f, 1.0f, 0) * /*some x stretch and y compress*/
                Matrix.CreateTranslation(location.X, location.Y + ((GetBoundingBox().Height/2)), 0f);

            Vector2 rotateAtBottomOrigin = new Vector2((width / 2), (height / 2) + (GetBoundingBox().Height / spriteScale)); // divide by spritescale here to reverse the scale before sending through draw

            Rectangle tRect = targetRectangle;

            if (this is IWalks)
            {
                // for cutting the shadow when swimming
                IWalks walker = (IWalks)this;
                if (walker.GetSwimming())
                {
                    tRect = new Rectangle(targetRectangle.X, targetRectangle.Y, targetRectangle.Width, targetRectangle.Height);
                    tRect.X = (_texture.Width / nColumns) * currColumnFrame;
                    tRect.Y = (_texture.Height / nRows) * currRowFrame;

                    // cut the bottom half of the targetRectangle off to hide the "under water" portion of the body
                    tRect.Height = (int)((_texture.Height / nRows) / 2.5);
                }
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, slant * camera.ViewportOffset.InvertAbsolute);
            spriteBatch.Draw(_texture, location, tRect, Color.Black * shadowTransparency, 0, rotateAtBottomOrigin, spriteScale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private void DrawPolygonBB(SpriteBatch sb)
        {
            foreach (var line in boundingPolygon.VertsInWorld(boundingPolygon.Verts, boundingPolygon.UpperLeftPoint))
            {
                Texture2D lineText = new Texture2D(_graphics, 1, 1, false, SurfaceFormat.Color);
                lineText.SetData<Color>(new Color[] { Color.White });// fill the texture with white

                DrawLine(sb, //draw line
                    line.Start, //start of line
                    line.End, //end of line
                    lineText
                );
            }
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Texture2D t)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);

            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

        public float GetYPosition()
        {
            return location.Y;
        }

        public float GetXPosition()
        {
            return location.X;
        }

        public double GetHeight()
        {
            return _texture.Height;
        }

        public Vector2 GetTextureCenter()
        {
            return _texture.Bounds.Center.ToVector2();
        }

        public Texture2D GetTexture()
        {
            return _texture;
        }

        public double GetWidth()
        {
            return _texture.Width;
        }

        public Rectangle GetBoundingBox()
        {
            return boundingBoxRect;
        }

        public Polygon GetBoundingPolygon()
        {
            return boundingPolygon;
        }

        public Sprite GetBase()
        {
            return (Sprite)this;
        }

        public abstract void HandleCollision(Sprite collidedWith, Rectangle overlap);
    }
}
