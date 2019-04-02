using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class Asset
    {
        public Texture2D Texture { get; }
        public Texture2D BBTexture { get; }
        public int Columns { get; }
        public int Rows { get; }
        public float Scale { get; }
        public string BBKey { get; }

        public Asset(Texture2D texture, Texture2D bbTexture, int columns, int rows, float scale, string bbKey)
        {
            Texture = texture;
            BBTexture = bbTexture;
            Columns = columns;
            Rows = rows;
            Scale = scale;
            BBKey = bbKey;
        }

    }
}
