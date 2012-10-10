using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HideSeek
{
	public class Bloco
	{

		private Vector2 posicao;
		private Texture2D sprite;
		private bool parede;

		public Bloco ()
		{
		}

		public Vector2 getPosicao ()
		{
			return posicao;
		}

		public Texture2D getSprite()
		{
			return sprite;
		}

		public void Initialize (bool parede_, Vector2 posicaoInicial)
		{
			parede = parede_;
			posicao = posicaoInicial;
		}

		public void LoadContent (ContentManager theContentManager)
		{
			if (parede) {
				sprite = theContentManager.Load<Texture2D> (Constantes.wallSprite);	
			} else {
				sprite = theContentManager.Load<Texture2D> (Constantes.floorSprite);	
			}
		}

		public void Draw(SpriteBatch theSpriteBatch) {
			//theSpriteBatch.Draw(sprite, posicao, Color.White);
			theSpriteBatch.Draw(sprite, posicao, Color.White);
		}

		//Retorna true se o bloco for uma parede, false caso contrario.
		public bool Parede ()
		{
			return parede;
		}
	}
}


