using Gusto.Models;
using Gusto.Bounding;
using Gusto.GameMap;
using Gusto.Mappings;
using Gusto.Models.Types;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Gusto.Utility
{
    public class AIUtility
    {
        public static byte[,] OceanPathWeights; // for A* pathing
        public static byte[,] LandPathWeights; // for A* pathing
        public static byte[,] AllPathWeights; // for A* pathing

        //public static byte[,] Weight;

        public static List<TilePiece> Pathfind(Point start, Point end, PathType pathType)
        {
            // nodes that have already been analyzed and have a path from the start to them
            var closedSet = new List<Point>();
            // nodes that have been identified as a neighbor of an analyzed node, but have 
            // yet to be fully analyzed
            var openSet = new List<Point> { start };
            // a dictionary identifying the optimal origin point to each node. this is used 
            // to back-track from the end to find the optimal path
            var cameFrom = new Dictionary<Point, Point>();
            // a dictionary indicating how far each analyzed node is from the start
            var currentDistance = new Dictionary<Point, int>();
            // a dictionary indicating how far it is expected to reach the end, if the path 
            // travels through the specified node. 
            var predictedDistance = new Dictionary<Point, float>();

            // initialize the start node as having a distance of 0, and an estmated distance 
            // of y-distance + x-distance, which is the optimal path in a square grid that 
            // doesn't allow for diagonal movement
            currentDistance.Add(start, 0);
            predictedDistance.Add(
                start,
                0 + +Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y)
            );

            // if there are any unanalyzed nodes, process them
            while (openSet.Count > 0)
            {
                // get the node with the lowest estimated cost to finish
                var current = (
                    from p in openSet orderby predictedDistance[p] ascending select p
                ).First();

                // if it is the finish, return the path
                if (current.X == end.X && current.Y == end.Y)
                {
                    // generate the found path
                    List<Point> pathPoints = ReconstructPath(cameFrom, end);
                    // map to tile pieces
                    List<TilePiece> tiles = new List<TilePiece>();
                    foreach (Point p in pathPoints)
                    {
                        tiles.Add(GameMapTiles.map[(p.X * GameMapTiles.cols) + p.Y]);
                    }
                    return tiles;
                }

                // move current node from open to closed
                openSet.Remove(current);
                closedSet.Add(current);

                // process each valid node around the current node
                foreach (var neighbor in GetNeighborNodes(current, pathType))
                {
                    var tempCurrentDistance = currentDistance[current] + 1;

                    // if we already know a faster way to this neighbor, use that route and 
                    // ignore this one
                    if (closedSet.Contains(neighbor)
                        && tempCurrentDistance >= currentDistance[neighbor])
                    {
                        continue;
                    }

                    // if we don't know a route to this neighbor, or if this is faster, 
                    // store this route
                    if (!closedSet.Contains(neighbor)
                        || tempCurrentDistance < currentDistance[neighbor])
                    {
                        if (cameFrom.Keys.Contains(neighbor))
                        {
                            cameFrom[neighbor] = current;
                        }
                        else
                        {
                            cameFrom.Add(neighbor, current);
                        }

                        currentDistance[neighbor] = tempCurrentDistance;
                        predictedDistance[neighbor] =
                            currentDistance[neighbor]
                            + Math.Abs(neighbor.X - end.X)
                            + Math.Abs(neighbor.Y - end.Y);

                        // if this is a new node, add it to processing
                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            // unable to figure out a path, abort.
            throw new Exception(
                string.Format(
                    "unable to find a path between {0},{1} and {2},{3}",
                    start.X, start.Y,
                    end.X, end.Y
                )
            );
        }

        private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            if (!cameFrom.Keys.Contains(current))
            {
                return new List<Point> { current };
            }

            var path = ReconstructPath(cameFrom, cameFrom[current]);
            path.Add(current);
            return path;
        }


        private static IEnumerable<Point> GetNeighborNodes(Point node, PathType pathType)
        {
            var nodes = new List<Point>();
            byte[,] Weights = null;
            switch (pathType)
            {
                case PathType.Ocean:
                    Weights = OceanPathWeights;
                    break;
                case PathType.Land:
                    Weights = LandPathWeights;
                    break;
                case PathType.AllOutdoor:
                    Weights = AllPathWeights;
                    break;
            }

            if (node.Y > 0)
            {
                // up
                if (Weights[node.X, node.Y - 1] > 0)
                {
                    nodes.Add(new Point(node.X, node.Y - 1));
                }
            }

            // right
            if (node.X < GameMapTiles.rows - 1)
            {
                if (Weights[node.X + 1, node.Y] > 0)
                {
                    nodes.Add(new Point(node.X + 1, node.Y));
                }
            }

            if (node.Y < GameMapTiles.cols - 1)
            {
                // down
                if (Weights[node.X, node.Y + 1] > 0)
                {
                    nodes.Add(new Point(node.X, node.Y + 1));
                }
            }

            if (node.X > 0)
            {
                // left
                if (Weights[node.X - 1, node.Y] > 0)
                {
                    nodes.Add(new Point(node.X - 1, node.Y));
                }
            }

            return nodes;
        }


        // Returns attack postion of target in world
        public static Vector2? ChooseTargetVector(TeamType teamType, float range, Rectangle bb, Guid interiorId)
        {
            foreach (var otherTeam in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
            {
                if (AttackMapping.AttackMappings[teamType][otherTeam])
                {
                    Vector2? shotCords = null;
                    if (BoundingBoxLocations.BoundingBoxLocationMap[otherTeam].Any())
                    {
                        float minVMag = float.MaxValue;
                        
                        foreach (var target in BoundingBoxLocations.BoundingBoxLocationMap[otherTeam])
                        {
                            float vmag = PhysicsUtility.VectorMagnitude(target.targetLoc.X, bb.X, target.targetLoc.Y, bb.Y);
                            if (vmag < minVMag && interiorId == target.interiorId)
                            {
                                minVMag = vmag;
                                shotCords = target.targetLoc;
                            }

                        }
                        if (minVMag <= range)
                            return shotCords;
                    }
                    else
                        return null;
                }
            }
            return null;
        }


        // Returns tile point of target (pass the pathType on which you desire to find a target - i.e. ships don't build paths to land tiles) and the distance between target and this bb
        public static Tuple<Point?, float> ChooseTargetPoint(TeamType teamType, float range, Rectangle bb, Guid interiorId, PathType pathType)
        {
            foreach (var otherTeam in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
            {
                if (AttackMapping.AttackMappings[teamType][otherTeam])
                {
                    Point? targetTilePoint = null;
                    if (BoundingBoxLocations.BoundingBoxLocationMap[otherTeam].Any())
                    {
                        float minVMag = float.MaxValue;

                        foreach (var target in BoundingBoxLocations.BoundingBoxLocationMap[otherTeam])
                        {
                            if (target.pathType != pathType)
                                continue;

                            float vmag = PhysicsUtility.VectorMagnitude(target.targetLoc.X, bb.X, target.targetLoc.Y, bb.Y);
                            if (vmag < minVMag && interiorId == target.interiorId)
                            {
                                minVMag = vmag;
                                targetTilePoint = target.mapCordPoint;
                            }

                        }
                        if (minVMag <= range)
                            return new Tuple<Point?, float>(targetTilePoint, minVMag);
                    }
                    else
                        return null;
                }
            }
            return null;
        }

        public static int SetAIShipDirection(Vector2 target, Vector2 location)
        {
            int currRowFrame = 0;
            float slope = (target.Y - location.Y) / (target.X - location.X);

            if (slope > 0)
            {
                if (slope < 2.5 && slope > 0.4)
                {
                    if ((target.X - location.X) > 0)
                        currRowFrame = 5; // upper right
                    else
                        currRowFrame = 1; // lower left
                }
                else if (slope < 0.4 && slope > 0)
                {
                    if ((target.X - location.X) > 0)
                        currRowFrame = 6; // right
                    else
                        currRowFrame = 2; // left
                }
                else if (slope > 2.5)
                {
                    if ((target.Y - location.Y) > 0)
                        currRowFrame = 4; // down
                    else
                        currRowFrame = 0; // up
                }
            }
            else
            {
                if (slope > -2.5 && slope < -0.4)
                {
                    if ((target.X - location.X) > 0)
                        currRowFrame = 7; // lower right
                    else
                        currRowFrame = 3; // upper left
                }
                else if (slope > -0.4 && slope < 0)
                {
                    if ((target.X - location.X) > 0)
                        currRowFrame = 6; // right
                    else
                        currRowFrame = 2; // left
                }
                else if (slope < -2.5)
                {
                    if ((target.Y - location.Y) > 0)
                        currRowFrame = 4; // down
                    else
                        currRowFrame = 0; // up
                }
            }
            return currRowFrame;
        }

        public static Tuple<int, int> SetAIGroundMovement(Vector2 target, Vector2 location)
        {
            int currRowFrame = 0;
            int currDirectionalFrame = 0;
            float slope = (target.Y - location.Y) / (target.X - location.X);

            if (slope > 0)
            {
                if (slope < 2.5 && slope > 0.4)
                {
                    if ((target.X - location.X) > 0)
                    {
                        if (target.Y > location.Y)
                        {
                            currRowFrame = 0; // lower right
                            currDirectionalFrame = 5;
                        }
                        else
                        {
                            currRowFrame = 3; // upper right
                            currDirectionalFrame = 7;
                        }
                    }
                    else
                    {
                        if (target.Y < location.Y)
                        {
                            currRowFrame = 3; // upper left
                            currDirectionalFrame = 1;
                        }
                        else
                        {
                            currRowFrame = 0; // lower left
                            currDirectionalFrame = 3;
                        }
                    }
                }
                else if (slope < 0.4 && slope > 0)
                {
                    if ((target.X - location.X) > 0)
                    {
                        currRowFrame = 1; // right
                        currDirectionalFrame = 6;
                    }
                    else
                    {
                        currRowFrame = 2; // left
                        currDirectionalFrame = 2;
                    }
                }
                else if (slope > 2.5)
                {
                    if ((target.Y - location.Y) > 0)
                    {
                        currRowFrame = 0; // down
                        currDirectionalFrame = 4;
                    }
                    else
                    {
                        currRowFrame = 3; // up
                        currDirectionalFrame = 0;
                    }
                }
            }
            else
            {
                if (slope > -2.5 && slope < -0.4)
                {
                    if ((target.X - location.X) > 0)
                    {
                        if (target.Y < location.Y)
                        {
                            currRowFrame = 3; // upper right
                            currDirectionalFrame = 7;
                        }
                        else
                        {
                            currRowFrame = 0; // lower right
                            currDirectionalFrame = 5;
                        }
                    }
                    else
                    {
                        if (target.Y > location.Y)
                        {
                            currRowFrame = 0; // lower left
                            currDirectionalFrame = 3;
                        }
                        else
                        {
                            currRowFrame = 3; // upper left
                            currDirectionalFrame = 1;
                        }
                    }
                }
                else if (slope > -0.4 && slope < 0)
                {
                    if ((target.X - location.X) > 0)
                    {
                        currRowFrame = 1; // right
                        currDirectionalFrame = 6;
                    }
                    else
                    {
                        currRowFrame = 2; // left
                        currDirectionalFrame = 2;
                    }
                }
                else if (slope < -2.5)
                {
                    if ((target.Y - location.Y) > 0)
                    {
                        currRowFrame = 0; // down
                        currDirectionalFrame = 4;
                    }
                    else
                    {
                        currRowFrame = 3; // up
                        currDirectionalFrame = 0;
                    }
                }
            }
            return new Tuple<int, int>(currRowFrame, currDirectionalFrame);
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
