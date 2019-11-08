using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GustoGame.Utility
{
    public class QuadRenderer
    {
        VertexPositionTexture[] verts = null;
        short[] ib = null;
        GraphicsDevice graphics;

        public QuadRenderer(GraphicsDevice graphics)
        {
            this.graphics = graphics;
            LoadContent();
        }

        public void LoadContent()
        {
            verts = new VertexPositionTexture[]
            {
            new VertexPositionTexture(
                new Vector3(0,0,0),
                new Vector2(0, 0)),
            new VertexPositionTexture(
                new Vector3(0,0,0),
                new Vector2(1,0)),
            new VertexPositionTexture(
                new Vector3(0,0,0),
                new Vector2(0,1)),
            new VertexPositionTexture(
                new Vector3(0,0,0),
                new Vector2(1,1))
            };

            // The order of the vertices affects the normal of the face, which can cause the primitive to be culled if facing away, (order of triangles affects something but not the normals)
            ib = new short[] { 3, 2, 1, 0, 1, 2 };
        }

        /// <summary>
        /// call with v1 as Vector2.One * -1 and v2 as Vector2.One
        /// </summary>
        /// <param name="v1">Vector2.One * -1</param>
        /// <param name="v2">Vector2.One</param>
        public void Render(Vector2 v1, Vector2 v2)
        {
            //(-1, 1)
            verts[0].Position.X = v1.X;
            verts[0].Position.Y = v2.Y;

            // (1, 1)
            verts[1].Position.X = v2.X;
            verts[1].Position.Y = v2.Y;

            // (-1, -1)
            verts[2].Position.X = v1.X;
            verts[2].Position.Y = v1.Y;

            // (1, -1)
            verts[3].Position.X = v2.X;
            verts[3].Position.Y = v1.Y;

            graphics.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    }
}
