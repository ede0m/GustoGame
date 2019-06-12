﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Gusto.Models;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Models.Animated;

namespace Gusto.AnimatedSprite
{
    public class PiratePlayer : PlayerPirate
    {
        public PiratePlayer(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastTurnFrame = 0;
            timeSinceLastWalkFrame = 0;
            millisecondsPerTurnFrame = 500; // turn speed
            millisecondsPerWalkFrame = 100; // turn speed
            millisecondsCombatSwing = 75;

            fullHealth = 40;
            health = fullHealth;

            //MapModelMovementVectorValues();
            Texture2D texturePlayerPirate = content.Load<Texture2D>("Pirate1-combat");
            Texture2D texturePlayerPirateBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                texturePlayerPirateBB = new Texture2D(graphics, texturePlayerPirate.Width, texturePlayerPirate.Height);
            Asset playerPirateAsset = new Asset(texturePlayerPirate, texturePlayerPirateBB, 11, 4, 1.0f, "playerPirate", region);
            // Temporary??
            inHand = new BaseSword(team, region, location, content, graphics);
            SetSpriteAsset(playerPirateAsset, location);
        }
    }

}
