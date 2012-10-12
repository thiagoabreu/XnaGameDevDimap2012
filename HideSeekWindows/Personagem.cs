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
        private Vector2 posicaoAlvo;
        private bool emMovimento;
        private Texture2D sprite;
        private Rectangle animationRec;
        private int dir;
        private int frameRate;
        private int tempoDecorrido;
        private int frameCount;
        private int frameAtual;

        public Personagem ()
        {
            tempoDecorrido = 0;
            frameAtual = 0;
            frameRate = 100;

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
            #region Movimentacao do Sprite
            // Desliza o sprite em direçao a posicao alvo
            if (posicaoAlvo.X > posicao.X)
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

            #region Animaçao do Sprite
            tempoDecorrido += (int)theGameTime.ElapsedGameTime.TotalMilliseconds;
            if (tempoDecorrido > frameRate)
            {
                frameAtual = frameAtual < frameCount ? frameAtual + 1 : 0;
                tempoDecorrido = 0;
            }
            animationRec.Y = 32 * frameAtual;
            animationRec.X = 32 * dir;
            #endregion

            theSpriteBatch.Draw(sprite, posicao, animationRec, Color.White);
        }

        public void Update (KeyboardState currentState, Mapa mapa)
        {
            if (!emMovimento)
            {
                Vector2 proxPosicao = new Vector2 (this.getPosicao().X, this.getPosicao().Y);
                Rectangle thisBox;

                if (currentState.IsKeyDown(Keys.Down))
                {
                    dir = 3;
                    proxPosicao.Y += 32.0f;
                    thisBox = Constantes.BoundingBox(proxPosicao);
                               
                    for (int i = 0; i < Constantes.tamLabirinto; i++)
                    {
                        for (int j = 0; j < Constantes.tamLabirinto; j++)
                        {
                            Bloco bloco = mapa.GetBloco(i, j);
                            Rectangle blocoBox = Constantes.BoundingBox(bloco.getPosicao());
                        
                            if (bloco.Parede() && blocoBox.Intersects(thisBox))
                            {
                                proxPosicao.Y = blocoBox.Y - thisBox.Height;
                            }
                        }
                    }
                }

                if (currentState.IsKeyDown(Keys.Up))
                {
                    dir = 1;
                    proxPosicao.Y -= 32.0f;
                    thisBox = Constantes.BoundingBox(proxPosicao);

                    for (int i = 0; i < Constantes.tamLabirinto; i++)
                    {
                        for (int j = 0; j < Constantes.tamLabirinto; j++)
                        {
                            Bloco bloco = mapa.GetBloco(i, j);
                            Rectangle blocoBox = Constantes.BoundingBox(bloco.getPosicao());
                        
                            if (bloco.Parede() && blocoBox.Intersects(thisBox))
                            {
                                proxPosicao.Y = blocoBox.Y + blocoBox.Height;
                            }
                        }
                    }
                } 

                if (currentState.IsKeyDown(Keys.Left))
                {
                    dir = 2;
                    proxPosicao.X -= 32.0f;
                    thisBox = Constantes.BoundingBox(proxPosicao);

                    for (int i = 0; i < Constantes.tamLabirinto; i++)
                    {
                        for (int j = 0; j < Constantes.tamLabirinto; j++)
                        {
                            Bloco bloco = mapa.GetBloco(i, j);
                            Rectangle blocoBox = Constantes.BoundingBox(bloco.getPosicao());
                        
                            if (bloco.Parede() && blocoBox.Intersects(thisBox))
                            {
                                proxPosicao.X = blocoBox.X + blocoBox.Width;
                            }
                        
                        }
                    
                    }
                } 

                if (currentState.IsKeyDown(Keys.Right))
                {
                    dir = 0;
                    proxPosicao.X += 32.0f;
                    thisBox = Constantes.BoundingBox(proxPosicao);

                    for (int i = 0; i < Constantes.tamLabirinto; i++)
                    {
                        for (int j = 0; j < Constantes.tamLabirinto; j++)
                        {
                            Bloco bloco = mapa.GetBloco(i, j);
                            Rectangle blocoBox = Constantes.BoundingBox(bloco.getPosicao());
                        
                            if (bloco.Parede() && blocoBox.Intersects(thisBox))
                            {
                                proxPosicao.X = blocoBox.X - thisBox.Width;
                            }
                        }
                    }

                }

                // Ao final, atualiza a posiçao alvo.
                posicaoAlvo = proxPosicao;
            }
        }
    }   
}

