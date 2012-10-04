using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace HideSeek
{
	//Classe para armazenar as constantes do jogo.
	//Considere como sendo um arquivo de configuração para facilitar mudanças em variáveis globais do jogo.
	public static class Constantes
	{
		public const float velocidadeSeeker = 6.0f;
		
		//Nomes dos sprites.
		public const String seekerSprite = "Seeker";
		public const String wallSprite = "Wall";
		public const String floorSprite = "Floor";

		public static Rectangle BoundingBox (Vector2 posicao, Texture2D sprite)
		{
			return new Rectangle((int)posicao.X, (int) posicao.Y, sprite.Width, sprite.Height);
		}


	}


}

