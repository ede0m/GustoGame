using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Gusto.Utility
{
    public class DrawUtility
    {

        public static void DrawPlayer(SpriteBatch spriteBatchView, Camera camera, PiratePlayer pirate)
        {

            // wont draw if pirate not hurt
            pirate.DrawHealthBar(spriteBatchView, camera);

            if (pirate.inCombat && pirate.currRowFrame == 3) // draw sword before pirate when moving up
                pirate.inHand.Draw(spriteBatchView, camera);
            if (pirate.nearShip)
                pirate.DrawEnterShip(spriteBatchView, camera);
            else if (pirate.onShip)
                pirate.DrawOnShip(spriteBatchView, camera);


            if (pirate.swimming && !pirate.onShip)
                pirate.DrawSwimming(spriteBatchView, camera);
            else if (!pirate.onShip)
                pirate.Draw(spriteBatchView, camera);

            if (pirate.canBury)
                pirate.DrawCanBury(spriteBatchView, camera);

            if (pirate.inCombat && pirate.currRowFrame != 3)
                pirate.inHand.Draw(spriteBatchView, camera);

            foreach (var shot in pirate.inHand.Shots)
                shot.Draw(spriteBatchView, camera);
        }

        public static void DrawSpotLighting(SpriteBatch sb, Camera cam, RenderTarget2D lightsTarget, List<Sprite> drawOrder)
        {
            // draw lights (render target should already be set to lightsTarget for lights)
            foreach (var obj in drawOrder)
            {
                if (obj is IPlayer) // check the player's handheld
                {
                    PlayerPirate p = (PlayerPirate)obj;
                    Light l = p.inHand.GetEmittingLight();
                    if (l != null && l.lit)
                        l.Draw(sb, cam);
                }
                else if (obj is ILight)
                {
                    ILight l = (ILight)obj;
                    Light lt = l.GetEmittingLight();
                    if (l != null && lt != null && lt.lit)
                        lt.Draw(sb, cam);
                }
            }
        }

    }
}
