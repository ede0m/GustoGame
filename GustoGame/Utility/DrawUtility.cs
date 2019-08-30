using Comora;
using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
