using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HideSeek
{
    public class Personagem
    {
        Vector2 posicaoAlvo;
        Vector2 posicao;
        bool emMovimento;
        Texture2D sprite;
        Rectangle animationRec;
        Mapa mapa;
        int dir;
        int frameRate;
        int tempoDecorrido;
        int frameCount;
        int frameAtual;

        public Mapa Mapa {
            get {
                return this.mapa;
            }
            set {
                this.mapa = value;
            }
        }

        public Vector2 PosicaoAlvo {
            get {
                return this.posicaoAlvo;
            }
            set {
                posicaoAlvo = value;
            }
        }

        public Vector2 Posicao {
            get {
                return this.posicao;
            }
            set {
                this.posicao = value;
            }
        }



        public Personagem ()
        {
            tempoDecorrido = 0;
            frameAtual = 0;
            frameRate = 200;

            dir = 0;

            animationRec = new Rectangle (0, 0, 32, 32);

            emMovimento = false;

        }

        public Vector2 getPosicao ()
        {
            return posicao;
        }

        public Texture2D getSprite ()
        {
            return sprite;
        }

        public void Initialize (Vector2 posicaoInicial)
        {
            posicao = posicaoInicial;
            posicaoAlvo = posicao;

        }

        public void LoadContent (ContentManager theContentManager)
        {
            sprite = theContentManager.Load<Texture2D>(Constantes.seekerSprite);    
            frameCount = (sprite.Height / 32) - 1;
        }

        public void Draw (SpriteBatch theSpriteBatch, GameTime theGameTime)
        {
            #region Animaçao do Sprite
            tempoDecorrido += (int)theGameTime.ElapsedGameTime.TotalMilliseconds;
            if (emMovimento) {
                if (tempoDecorrido > frameRate) {
                    frameAtual = frameAtual < frameCount ? frameAtual + 1 : 0;
                    tempoDecorrido = 0;
                }
                animationRec.Y = 32 * frameAtual;
                animationRec.X = 32 * dir;
            }
            #endregion

            theSpriteBatch.Draw (sprite, posicao, animationRec, Color.White);
        }

        public void Update (KeyboardState currentState)
        {
            if (!emMovimento && currentState.GetPressedKeys ().Length > 0) {
                Vector2 proxPosicao = new Vector2 (this.getPosicao ().X, this.getPosicao ().Y);
                Rectangle thisBox;

                if (currentState.IsKeyDown (Keys.Down)) {
                    dir = 3;
                    proxPosicao.Y += 32.0f;
                } else if (currentState.IsKeyDown (Keys.Up)) {
                    dir = 1;
                    proxPosicao.Y -= 32.0f;
                } else if (currentState.IsKeyDown (Keys.Left)) {
                    dir = 2;
                    proxPosicao.X -= 32.0f;
                } else if (currentState.IsKeyDown (Keys.Right)) {
                    dir = 0;
                    proxPosicao.X += 32.0f;
                }
                thisBox = new Rectangle ((int)proxPosicao.X, (int)proxPosicao.Y, 32, 32);

                // Verifica colisoes
                
                Bloco bloco = mapa.GetBloco (((int)proxPosicao.X / 32), ((int)proxPosicao.Y / 32));
                Rectangle blocoBox = Constantes.BoundingBox (bloco.getPosicao ());
                bool colidiu = (bloco.Parede () && blocoBox.Intersects (thisBox));
                

                // Ao final, se nao houver colisoes, atualiza a posiçao alvo.
                if (!colidiu)
                    posicaoAlvo = proxPosicao;
            } else {
                #region Movimentacao do Sprite
                // Desliza o sprite em direçao a posicao alvo
                if (posicaoAlvo.X > posicao.X) {
                    posicao.X += Constantes.velocidadeSeeker;
                if (posicaoAlvo.X < posicao.X)
                    posicao.X -= Constantes.velocidadeSeeker;
                if (posicaoAlvo.Y > posicao.Y)
                    posicao.Y += Constantes.velocidadeSeeker;
                if (posicaoAlvo.Y < posicao.Y)
                    posicao.Y -= Constantes.velocidadeSeeker;

                // Verifica se chegou na posiçao.
                emMovimento = (posicao.Y != posicaoAlvo.Y || posicao.X != posicaoAlvo.X);
                #endregion
            }
        }
    }   
}

