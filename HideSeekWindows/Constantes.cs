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
		public const float velocidadeSeeker = 6.0f;
		public const int tamLabirinto = 15;

		
		//Nomes dos sprites.
		public const String seekerSprite = "Seeker";
		public const String wallSprite = "Wall";
		public const String floorSprite = "Floor";

		public static Rectangle BoundingBox (Vector2 posicao, Texture2D sprite)
		{
			return new Rectangle((int)posicao.X, (int) posicao.Y, sprite.Width, sprite.Height);
		}

		public static bool collision (Rectangle r1, Rectangle r2){
			return !(r1.X + r1.Width < r2.X || r2.X + r2.Width < r1.X || r1.Y + r1.Height < r2.Y || r2.Y + r2.Height < r1.Y);

		}


	}


}

