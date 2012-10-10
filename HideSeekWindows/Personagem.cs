using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HideSeek 
	{

	public class Personagem 
	{
		private Vector2 posicao;
		private Texture2D sprite;
		private bool active;
		private int dir;
		private int cor;
		private Color[] cores;

		public Personagem () 
		{
		}

		public Vector2 getPosicao ()
		{
			return posicao;
		}

		//Retorna o sprite do personagem. O tamanho do sprite é utilizado nos métodos de colisão.
		public Texture2D getSprite ()
		{
			return sprite;
		}

		public void Initialize (Vector2 posicaoInicial)
		{
			posicao = posicaoInicial;
		}
		public void LoadContent(ContentManager theContentManager)	 {
			sprite = theContentManager.Load<Texture2D>(Constantes.seekerSprite);	
		}

		public void Draw(SpriteBatch theSpriteBatch) {
			theSpriteBatch.Draw(sprite, posicao, Color.White);
		}

		public void Update (Vector2 novaPosicao)
		{
			posicao = novaPosicao;
		}
	}	
}

