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
		//Variaveis numericas
		public const float velocidadeSeeker = 4.2f;
		public const int tamLabirinto = 6;

		
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

