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
        public const float velocidadeSeeker = 4.0f;
        public const int tamLabirinto = 15;

        
        //Nomes dos sprites.
        public const String seekerSprite = "Seeker.png";
        public const String wallSprite = "Wall.png";
        public const String floorSprite = "Floor.png";
        public const String mapa1 = "PPPPPPPPPPPPPPP" +
                                    "PCCCCCCCCCCCCCP" +
                                    "PCPPPCPPPCPPPCP" +
                                    "PCPCCCCPCCCCPCP" +
                                    "PCPCPPCCCPPCPCP" +
                                    "PCCCPCCPCCPCCCP" +
                                    "PCPCCCPPPCCCPCP" +
                                    "PCPPCPPPPPCPPCP" +
                                    "PCPCCCPPPCCCPCP" +
                                    "PCCCPCCPCCPCCCP" +
                                    "PCPCPPCCCPPCPCP" +
                                    "PCPCCCCPCCCCPCP" +
                                    "PCPPPCPPPCPPPCP" +
                                    "PCCCCCCCCCCCCCP" +
                                    "PPPPPPPPPPPPPPP";
        public const String mapa2 = "PPPPPPPPPPPPPPP" +
                                    "PCCCCCCCCCCCCCP" +
                                    "PCPPCCPPPCPPPCP" +
                                    "PCPCCCCPCCCCPCP" +
                                    "PCPCPPCCCPPCPCP" +
                                    "PCCCPCCPCCPCCCP" +
                                    "PCPCCCPPPCCCPCP" +
                                    "PCPPCPPPPPCPPCP" +
                                    "PCPCCCPPPCCCPCP" +
                                    "PCCCPCCPCCPCCCP" +
                                    "PCPCPPCCCPPCPCP" +
                                    "PCPCCCCPCCCCPCP" +
                                    "PCPPPCPPPCPPPCP" +
                                    "PCCCCCCCCCCCCCP" +
                                    "PPPPPPPPPPPPPPP";

        public static Rectangle BoundingBox (Vector2 posicao)
        {
            return new Rectangle ((int)posicao.X, (int)posicao.Y, 32,32);
        }

        public static bool collision (Rectangle r1, Rectangle r2)
        {
            return !(r1.X + r1.Width < r2.X || r2.X + r2.Width < r1.X || r1.Y + r1.Height < r2.Y || r2.Y + r2.Height < r1.Y);
        }
    }
}
