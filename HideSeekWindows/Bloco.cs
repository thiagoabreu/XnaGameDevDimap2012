using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

		public void LoadContent(ContentManager theContentManager, string nomeDoArquivo)	 {
			sprite = theContentManager.Load<Texture2D>(nomeDoArquivo);	
		}

		public void Draw(SpriteBatch theSpriteBatch) {
			//theSpriteBatch.Draw(sprite, posicao, Color.White);
			theSpriteBatch.Draw(sprite, posicao, Color.White);
		}

		public bool ehParede ()
		{
			return parede;
		}
	}
}

