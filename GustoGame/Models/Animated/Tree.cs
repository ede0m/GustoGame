using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gusto.Models.Animated
{
    public class Tree : Sprite, ICanUpdate, IGroundObject
    {

        int timeSinceLastFrame;

        TeamType team;
        ContentManager _content;
        GraphicsDevice _graphics;

        bool objectHit;
        bool animate;
        bool animateLeft;
        bool animateRight;
        bool animateUp; // TODO
        bool animateDown;

        Vector2 startingLoc;

        public Tree (TeamType t, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            team = t;
            _content = content;
            _graphics = graphics;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith is IHandHeld && !(collidedWith is IRanged))
            {
                objectHit = true;
                animateLeft = animateRight = false;

                if (collidedWith.GetBoundingBox().Left < GetBoundingBox().Left)  //left side collision
                    animateRight = true;
                else if (collidedWith.GetBoundingBox().Right > GetBoundingBox().Right) // right side collision
                    animateLeft = true;

                // one or the other
                animateLeft = !animateRight;

            }
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {

            // hitting a tree
            if (objectHit && !animate)
            {
                startingLoc = location;
                animate = true;
            }

            if (animate) { 
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > 100)
                {
                    if (currColumnFrame < nColumns - 1)
                        currColumnFrame++;
                    else
                    {
                        currColumnFrame = 0;
                        animate = false;
                    }

                    // movement animation
                    if (currColumnFrame == 1)
                    {
                        if (animateRight)
                            location.X += 2;
                        else
                            location.X -= 2;

                    }
                    else
                        location = startingLoc;
                        

                    timeSinceLastFrame = 0;
                }
            }
            objectHit = false;
        }
    }
}
