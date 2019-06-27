using System;
using Comora;
using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto.Models
{
    public abstract class Sprite
    {
        protected Texture2D _texture;
        protected Texture2D boundingBox;
        public Rectangle targetRectangle;
        private Rectangle boundingBoxRect;
        public float spriteScale;

        //RenderTarget2D shadow;

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

        public Sprite(){}

        public void SetSpriteAsset(Asset asset, Vector2 startingLoc)
        {
            _texture = asset.Texture;
            spriteScale = asset.Scale;
            location = startingLoc;
            nRows = asset.Rows;
            nColumns = asset.Columns;
            currRowFrame = 0;
            currColumnFrame = 0;
            bbKey = asset.BBKey;
            regionKey = asset.RegionKey;
            moving = true;
            remove = false;

            int width = _texture.Width / nColumns;
            int height = _texture.Height / nRows;
            targetRectangle = new Rectangle(width * currColumnFrame, height * currRowFrame, width, height); // x and y here are cords of the texture

            if (bbKey != null)
            {
                boundingBoxRect = BoundingBoxTextures.DynamicBoundingBoxTextures[bbKey][currColumnFrame.ToString() + currRowFrame.ToString()];
                boundingBoxRect.X = ((int)location.X + ((int)(targetRectangle.Right * spriteScale) - (int)(targetRectangle.Left * spriteScale)) / 2) - ((boundingBoxRect.Right - boundingBoxRect.Left) / 2);
                boundingBoxRect.Y = ((int)location.Y + ((int)(targetRectangle.Bottom * spriteScale) - (int)(targetRectangle.Top * spriteScale)) / 2) - ((boundingBoxRect.Bottom - boundingBoxRect.Top) / 2);
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

        public void DrawShadow(SpriteBatch spriteBatch, Camera camera, Vector2 shadowPos)
        {
            Rectangle tRect = new Rectangle(targetRectangle.X, targetRectangle.Y, targetRectangle.Width, targetRectangle.Height);
            tRect.X = (_texture.Width / nColumns) * currColumnFrame;
            tRect.Y = (_texture.Height / nRows) * currRowFrame;

            tRect.Height = (_texture.Height / nRows) / 2;
            Vector2 origin = new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2);
            // adjust how shadow lays based on sprite
            if (this is IShip || this is IGroundObject)
            {
                tRect = targetRectangle;
            }

            // update bounding box (x and y are cords of the screen here) -- WONT UPDATE STATIC SPRITES
            SetBoundingBox();

            if (camera == null)
                spriteBatch.Begin();
            else
                spriteBatch.Begin(camera);

            spriteBatch.Draw(_texture, shadowPos, tRect, Color.Black, 0f,
                origin, spriteScale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public void DrawTile(SpriteBatch spriteBatch, Camera camera)
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

            spriteBatch.Begin(camera, SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_texture, location, targetRectangle, Color.White, 0f,
                new Vector2(width / 2, height / 2), spriteScale, SpriteEffects.None, 0f);
            spriteBatch.End();
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
                spriteBatch.Draw(boundingBox, boundingBoxRect.Location.ToVector2(), boundingBoxRect, Color.Orange, 0f,
                    Vector2.Zero, 1.0f, SpriteEffects.None, 0f); // scaling is already done in constructor
            // 
            spriteBatch.Draw(_texture, location, targetRectangle, Color.White, 0f,
                new Vector2(width / 2, height / 2), spriteScale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public void SetBoundingBox()
        {
            if (bbKey != null)
            {
                boundingBoxRect = BoundingBoxTextures.DynamicBoundingBoxTextures[bbKey][currColumnFrame.ToString() + currRowFrame.ToString()];
                // texture is drawn to orgin, so we must offset our bounding boxes by this manually
                var originXOffset = ((int)(targetRectangle.Right * spriteScale) - (int)(targetRectangle.Left * spriteScale)) / 2;
                var originYOffset = ((int)(targetRectangle.Bottom * spriteScale) - (int)(targetRectangle.Top * spriteScale)) / 2;
                if (!(this is IHandHeld))
                {
                    boundingBoxRect.X = (((int)location.X + ((int)(targetRectangle.Right * spriteScale) - (int)(targetRectangle.Left * spriteScale)) / 2) - ((boundingBoxRect.Right - boundingBoxRect.Left) / 2)) - originXOffset;
                    boundingBoxRect.Y = (((int)location.Y + ((int)(targetRectangle.Bottom * spriteScale) - (int)(targetRectangle.Top * spriteScale)) / 2) - ((boundingBoxRect.Bottom - boundingBoxRect.Top) / 2)) - originYOffset;
                }
                else
                {
                    boundingBoxRect.X = (int)location.X  - originXOffset + boundingBoxRect.X;
                    boundingBoxRect.Y = (int)location.Y - originYOffset + boundingBoxRect.Y;
                }
            }
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

        public Sprite GetBase()
        {
            return (Sprite)this;
        }

        public abstract void HandleCollision(Sprite collidedWith, Rectangle overlap);
    }
}
