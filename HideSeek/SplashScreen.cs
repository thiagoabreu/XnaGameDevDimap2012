using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HideSeek
{
    public class SplashScreen
    {
        TimeSpan tempo;
        Texture2D dev_logo;
        double total;

        public SplashScreen ()
        {
            total = 5.0f;
        }

        public void LoadContent(ContentManager Content)
        {
            //dev_logo = Content.Load<Texture2D>("logos/black_berets_logo_i.png");
            dev_logo = Content.Load<Texture2D>("logos/dimap.png");
        }

        public void Update(GameTime gameTime, ref EstadoDeJogo estado)
        {
            tempo = gameTime.TotalRealTime;
            if (tempo > TimeSpan.FromSeconds(total))
                estado = EstadoDeJogo.Menu;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            double segs = tempo.TotalSeconds;
            float alfa = (float)(Math.Sin (Math.PI * segs / total));

            Color cor = new Color (alfa, alfa, alfa);

            float x = (spriteBatch.GraphicsDevice.Viewport.Width - dev_logo.Width) / 2;
            float y = (spriteBatch.GraphicsDevice.Viewport.Height - dev_logo.Height) / 2;

            spriteBatch.Begin ();
            spriteBatch.Draw (dev_logo, new Vector2 (x, y), cor);
            spriteBatch.End ();
        }
    }
}

