namespace MonoGame1
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Button
    {
        private SpriteFont btnFont;
        private string btnTxt;
        private Vector2 btnPos;
        private Color btnColour;
        private Texture2D rectTexture;
        private Vector2 rectPos;

        private const int margin = 2;

        // See http://stackoverflow.com/questions/23305577/draw-rectangle-in-monogame
        // See http://stackoverflow.com/questions/32020022/c-sharp-monogame-change-tint-color-on-hover
        // 
        public Button(GraphicsDevice device, SpriteFont newFont, string newTxt, Vector2 newPos, Color newColour)
        {
            btnFont = newFont;
            btnPos = newPos;
            btnTxt = newTxt;
            btnColour = newColour;

            Vector2 newSize = newFont.MeasureString(newTxt);

            int w = (int)(newSize.X) + margin * 2;
            int h = (int)(newSize.Y) + margin * 2;

            Color[] data = new Color[w * h];
            rectTexture = new Texture2D(device, w, h);

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }

            rectTexture.SetData(data);

            rectPos = new Vector2(newPos.X - margin, newPos.Y - margin);

            //Vector2 position = new Vector2(newPos.X, newPos.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rectTexture, rectPos, Color.White);
            spriteBatch.DrawString(btnFont, btnTxt, btnPos, btnColour);
        }

        public void Draw(SpriteBatch spriteBatch, int X, int Y)
        {
            // Rectangle tmpRect = new Rectangle(new Point(X - margin, Y - margin), rectTexture.Bounds.Size);
            spriteBatch.Draw(rectTexture, new Vector2(X - margin, Y - margin), Color.White);

            spriteBatch.DrawString(btnFont, btnTxt, new Vector2(X, Y), btnColour);
        }
    }
}
