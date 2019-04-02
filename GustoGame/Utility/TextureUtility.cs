using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        public static Tuple<int, int> GetSailMountCords(Texture2D texture, int rows, int cols, string key)
        {
            Color[,] Colors = TextureUtility.TextureTo2DArray(texture);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // target frame on the texture
                    int width = texture.Width / cols;
                    int height = texture.Height / rows;
                    int X = width * c;
                    int Y = height * r;

                    for (int a = X; a < X + width; a++)
                    {
                        for (int b = Y; b < Y + height; b++)
                        {
                            //If we find a the red sail mount color
                                if (Colors[a, b].R == 255 && Colors[a, b].G == 45 && Colors[a, b].B == 0 && Colors[a, b].A == 255)
                            {
                                // TODO: previous version didn't work..
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
