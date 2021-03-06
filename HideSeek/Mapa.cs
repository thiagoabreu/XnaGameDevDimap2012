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
        private String descricao;

        public String Descricao {
            get {
                return this.descricao;
            }
            set {
                this.descricao = value;
            }
        }

        public Mapa (Vector2 posicao_)
        {
			int tam = Constantes.tamLabirinto;
            blocos = new Bloco[tam][];

            for (int i = 0; i < tam; i++)
            {
                blocos [i] = new Bloco[tam];

                for (int j = 0; j < tam; j++)
                {
                    blocos [i] [j] = new Bloco ();
                }
            }
            posicao.X = posicao_.X;
            posicao.Y = posicao_.Y;
        }

        public void Initialize (String aDescricao = "")
        {
            descricao = aDescricao;
			if (descricao == "") 
                descricao = Constantes.mapa1;

            if (descricao.Length == Constantes.tamLabirinto * Constantes.tamLabirinto)
            {
                for (int i = 0; i < Constantes.tamLabirinto; i++)
                {
                    for (int j = 0; j < Constantes.tamLabirinto; j++)
                    {

                        int indice = j * Constantes.tamLabirinto + i;
                        bool parede = true;

                        if (descricao [indice] == 'P')
                        {
                            parede = true;
                        } else if (descricao [indice] == 'C')
                        {
                            parede = false;
                        }
                        blocos [i] [j].Initialize (parede, new Vector2 (posicao.X + i * 32, posicao.Y + j * 32));
                    }
                }
            } else
            {
                //Lançamento de exceçao
            }

        }
    
        public void LoadContent (ContentManager theContentManager)
        {
            for (int i = 0; i < Constantes.tamLabirinto; i++)
            {
                for (int j = 0; j < Constantes.tamLabirinto; j++)
                {
                    String arquivo = defineSprite (i, j);
                    blocos [i] [j].LoadContent (theContentManager, arquivo);
                }
            }
        }

        private String defineSprite (int i, int j)
        {
            if (blocos [i] [j].Parede ())
            {

                bool paredeEsq = false;
                bool paredeDir = false;
                bool paredeCima = false;
                bool paredeBaixo = false;

                //Usando curto circuito pra ele nao chegar a valores negativos nos indices.
                if (i > 0 && blocos [i - 1] [j].Parede ())
                {
                    paredeEsq = true;
                }
                if (j > 0 && blocos [i] [j - 1].Parede ())
                {
                    paredeCima = true;
                }
                if (i < Constantes.tamLabirinto - 1 && blocos [i + 1] [j].Parede ())
                {
                    paredeDir = true;
                }
                if (j < Constantes.tamLabirinto - 1 && blocos [i] [j + 1].Parede ())
                {
                    paredeBaixo = true;
                }
            
                if (paredeEsq && paredeDir && paredeCima && paredeBaixo)
                {
                    return "Wall_Cross";
                } else if (paredeEsq && paredeDir && !paredeCima && !paredeBaixo)
                {
                    return "Wall_Line";
                } else if (!paredeEsq && !paredeDir && paredeCima && paredeBaixo)
                {
                    return "Wall_Line2";
                } else if (paredeEsq && paredeDir && !paredeCima && paredeBaixo)
                {
                    return "Wall_T";
                } else if (paredeEsq && !paredeDir && paredeCima && paredeBaixo)
                {
                    return "Wall_T2";
                } else if (paredeEsq && paredeDir && paredeCima && !paredeBaixo)
                {
                    return "Wall_T3";
                } else if (!paredeEsq && paredeDir && paredeCima && paredeBaixo)
                {
                    return "Wall_T4";
                } else if (!paredeEsq && paredeDir && !paredeCima && !paredeBaixo)
                {
                    return "Wall_Corner";
                } else if (!paredeEsq && !paredeDir && !paredeCima && paredeBaixo)
                {
                    return "Wall_Corner2";
                } else if (paredeEsq && !paredeDir && !paredeCima && !paredeBaixo)
                {
                    return "Wall_Corner3";
                } else if (!paredeEsq && !paredeDir && paredeCima && !paredeBaixo)
                {
                    return "Wall_Corner4";
                } else if (!paredeEsq && paredeDir && paredeCima && !paredeBaixo)
                {
                    return "Wall_L";
                } else if (!paredeEsq && paredeDir && !paredeCima && paredeBaixo)
                {
                    return "Wall_L2";   
                } else if (paredeEsq && !paredeDir && !paredeCima && paredeBaixo)
                {
                    return "Wall_L3";
                } else if (paredeEsq && !paredeDir && paredeCima && !paredeBaixo)
                {
                    return "Wall_L4";
                } else
                { //if (!paredeEsq && !paredeDir && !paredeCima && !paredeBaixo)
                    return "Wall";
                }
            } else
            {
                return "Floor"; 
            }
        }

        public void Draw (SpriteBatch theSpriteBatch)
        {
            for (int i = 0; i < Constantes.tamLabirinto; i++)
            {
                for (int j = 0; j < Constantes.tamLabirinto; j++)
                {
                    blocos [i] [j].Draw (theSpriteBatch);
                }
            }
        }

        public Bloco GetBloco (int i, int j)
        {
            return blocos [i] [j];
        }

		public String GenerateMap() {
			System.Text.StringBuilder builderMap = new System.Text.StringBuilder();
			int tamanho = Constantes.tamLabirinto;
			Random rand = new Random();

			for (int i = 0; i < tamanho; i++)
				builderMap.Append("P");

			for (int i = 0; i < tamanho-2; i++) {
				builderMap.Append("PC");
				for (int j = 0; j < tamanho-4; j++)
					if (i == 0 || i == tamanho-3)
						builderMap.Append("C");
					else
						if (rand.Next() % 3 != 0)
							builderMap.Append("C");
						else
						builderMap.Append("P");
				builderMap.Append("CP");
			}

			for (int i = 0; i < tamanho; i++)
				builderMap.Append("P");

			return builderMap.ToString();
		}
    }
}

