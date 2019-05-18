﻿using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Mappings;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Utility
{
    public class AIUtility
    {
        // Returns attack postion of target as tuple
        public static Tuple<int, int> ChooseTarget(TeamType teamType, float range, Rectangle bb)
        {
            foreach (var otherTeam in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
            {
                if (AttackMapping.AttackMappings[teamType][otherTeam])
                {
                    Tuple<int, int> shotCords;
                    if (BoundingBoxLocations.BoundingBoxLocationMap[otherTeam].Any())
                    {
                        shotCords = BoundingBoxLocations.BoundingBoxLocationMap[otherTeam][0]; // TODO REMOVE HARDCODED random target (pick team member with lowest health)
                        float vmag = PhysicsUtility.VectorMagnitude(shotCords.Item1, bb.X, shotCords.Item2, bb.Y);
                        if (vmag <= range)
                            return shotCords;
                    }
                    else
                        return null;
                }
            }
            return null;
        }

        public static int SetAIShipDirection(Tuple<int, int> target, Vector2 location)
        {
            int currRowFrame = 0;
            float slope = (target.Item2 - location.Y) / (target.Item1 - location.X);

            if (slope > 0)
            {
                if (slope < 2.5 && slope > 0.4)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 5; // upper right
                    else
                        currRowFrame = 1; // lower left
                }
                else if (slope < 0.4 && slope > 0)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 6; // right
                    else
                        currRowFrame = 2; // left
                }
                else if (slope > 2.5)
                {
                    if ((target.Item2 - location.Y) > 0)
                        currRowFrame = 4; // down
                    else
                        currRowFrame = 0; // up
                }
            }
            else
            {
                if (slope > -2.5 && slope < -0.4)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 7; // lower right
                    else
                        currRowFrame = 3; // upper left
                }
                else if (slope > -0.4 && slope < 0)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 6; // right
                    else
                        currRowFrame = 2; // left
                }
                else if (slope < -2.5)
                {
                    if ((target.Item2 - location.Y) > 0)
                        currRowFrame = 4; // down
                    else
                        currRowFrame = 0; // up
                }
            }
            return currRowFrame;
        }

        public static bool LineIntersectsRect(Vector2 p1, Vector2 p2, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Vector2(r.X, r.Y), new Vector2(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Vector2(r.X + r.Width, r.Y), new Vector2(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Vector2(r.X + r.Width, r.Y + r.Height), new Vector2(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Vector2(r.X, r.Y + r.Height), new Vector2(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        private static bool LineIntersectsLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
    }
}
