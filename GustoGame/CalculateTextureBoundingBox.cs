using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto
{
    public static class CalculateTextureBoundingBox
    {
        //Get smallest rectangle from Texture, cased on color
        public static Rectangle GetSmallestRectangleFromTexture(Texture2D Texture, float scale)
        {
            //Create our index of sprite frames
            Color[,] Colors = TextureTo2DArray(Texture);

            //determine the min/max bounds
            int x1 = 9999999, y1 = 9999999;
            int x2 = -999999, y2 = -999999;

            for (int a = 0; a < Texture.Width; a++)
            {
                for (int b = 0; b < Texture.Height; b++)
                {
                    //If we find a non transparent pixel, update bounds if required
                    if (Colors[a, b].A != 0)
                    {
                        if (x1 > a) x1 = a;
                        if (x2 < a) x2 = a;

                        if (y1 > b) y1 = b;
                        if (y2 < b) y2 = b;
                    }
                }
            }

            //We now have our smallest possible rectangle for this texture
            return new Rectangle(x1, y1, (int)((x2 - x1 + 1) * scale), (int)((y2 - y1 + 1) * scale)); // check for rounding errors here with scale

            //return new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
        }

        //convert texture to 2d array
        private static Color[,] TextureTo2DArray(Texture2D texture)
        {
            //Texture.GetData returns a 1D array
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            //convert the 1D array to 2D for easier processing
            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }
}
