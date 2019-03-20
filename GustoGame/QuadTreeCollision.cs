using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto
{
    public class QuadTreeCollision
    {
        private int MAX_OBJECTS = 10;
        private int MAX_LEVELS = 5;

        private int level;
        private List<Sprite> objects;
        private Rectangle bounds;
        private QuadTreeCollision[] nodes;

        /*
        * Constructor
        */
        public QuadTreeCollision(int pLevel, Rectangle pBounds)
        {
            level = pLevel;
            objects = new List<Sprite>();
            bounds = pBounds;
            nodes = new QuadTreeCollision[4];
        }

        /*
        * Clears the quadtree
        */
        public void Clear()
        {
            objects.Clear();

            for (int i = 0; i < nodes.Count(); i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }
        
        /*
        * Splits the node into 4 subnodes
        */
        private void Split()
        {
            int subWidth = (int)(bounds.Width / 2);
            int subHeight = (int)(bounds.Height / 2);
            int x = (int)bounds.X;
            int y = (int)bounds.Y;

            nodes[0] = new QuadTreeCollision(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new QuadTreeCollision(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new QuadTreeCollision(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTreeCollision(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        /*
         * Determine which node the object belongs to. -1 means
         * object cannot completely fit within a child node and is part
         * of the parent node
         */
        private int GetIndex(Sprite sprite)
        {
            int index = -1;
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            float Ypos = sprite.GetYPosition();
            float Xpos = sprite.GetXPosition();

            // Object can completely fit within the top quadrants
            bool topQuadrant = (Ypos < horizontalMidpoint && Ypos + sprite.GetHeight() < horizontalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (Ypos > horizontalMidpoint);

            // Object can completely fit within the left quadrants
            if (Xpos < verticalMidpoint && Xpos + sprite.GetWidth() < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            // Object can completely fit within the right quadrants
            else if (Xpos > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }
            return index;
        }

        /*
         * Insert the object into the quadtree. If the node
         * exceeds the capacity, it will split and add all
         * objects to their corresponding nodes.
         */
        public void Insert(Sprite sprite)
        {
            if (nodes[0] != null)
            {
                int index = GetIndex(sprite);

                if (index != -1)
                {
                    nodes[index].Insert(sprite);

                    return;
                }
            }

            objects.Add(sprite);

            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(objects[i]);
                    if (index != -1)
                    {
                        Sprite toRemove = objects[i];
                        nodes[index].Insert(toRemove);
                        objects.Remove(toRemove);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /*
         * Return all objects that could collide with the given object (recursive)
         */
        public List<Sprite> Retrieve(List<Sprite> returnObjects, Sprite sprite)
        {
            int index = GetIndex(sprite);
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].Retrieve(returnObjects, sprite);
            }

            returnObjects.AddRange(objects);

            return returnObjects;
        }

    }
}
