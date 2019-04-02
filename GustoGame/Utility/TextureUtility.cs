using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto.Utility
{
    public class TextureUtility
    {
        //convert texture to 2d array
        public static Color[,] TextureTo2DArray(Texture2D texture)
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
