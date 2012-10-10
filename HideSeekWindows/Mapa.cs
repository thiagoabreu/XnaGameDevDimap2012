using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HideSeek
{
	public class Mapa
	{

		private Bloco[][] blocos;
		private Vector2 posicao; //posicao do bloco superior esquerdo.


		public Mapa (Vector2 posicao_)
		{
			blocos = new Bloco[15][];
			for (int i = 0; i < 15; i++) {
				blocos [i] = new Bloco[15];
			}
			for (int i = 0; i < 15; i++) {
				for (int j = 0; j < 15; j++) {
					blocos[i][j] = new Bloco();
				}
			}
			posicao.X = posicao_.X;
			posicao.Y = posicao_.Y;
		}

		public void Initialize(String descricao){
			if (descricao.Length == Constantes.tamLabirinto * Constantes.tamLabirinto) {
				for (int i = 0; i < Constantes.tamLabirinto; i++){
					for (int j = 0; j < Constantes.tamLabirinto; j++){

						int indice = i*Constantes.tamLabirinto + j;
						bool parede = true;

						if (descricao[indice] == 'P'){
							parede = true;
						} else if (descricao[indice] == 'C'){
							parede = false;
						}
						blocos[i][j].Initialize(parede, new Vector2 (posicao.X + i * 32, posicao.Y + j *32));
					}
				}
			} else {
				//Lançamento de exceçao
			}

		}
	
		public void LoadContent (ContentManager theContentManager)
		{
			for (int i = 0; i < Constantes.tamLabirinto; i++) {
				for (int j = 0; j < Constantes.tamLabirinto; j++) {
					blocos[i][j].LoadContent(theContentManager);
				}
			}
		}

		public void Draw (SpriteBatch theSpriteBatch)
		{
			for (int i = 0; i < Constantes.tamLabirinto; i++) {
				for (int j = 0; j < Constantes.tamLabirinto; j++) {
					blocos[i][j].Draw(theSpriteBatch);
				}
			}
		}

		public Bloco GetBloco(int i, int j){
			return blocos[i][j];
		}
	}
}

