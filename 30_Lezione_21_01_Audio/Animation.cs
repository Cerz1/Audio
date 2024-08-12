using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace _30_Lezione_21_01_Audio
{
    class Animation
    {
        protected int numFrames;
        protected float frameDuration;
        protected bool isPlaying;
        protected int currentFrame;
        protected float elapsedTime;

        protected int frameWidth;
        protected int frameHeight;

        public bool Loop;

        public Animation(int frameW, int frameH, float fps, int frames, bool loop = true)
        {
            this.frameWidth = frameW;
            this.frameHeight = frameH;

            this.frameDuration = 1 / fps;

            this.numFrames = frames;

            this.Loop = loop;
        }

        public virtual void Update(float deltaTime, ref Vector2 offset)
        {
            if(isPlaying)
            {
                elapsedTime += deltaTime;

                if(elapsedTime >= frameDuration)
                {
                    currentFrame++;
                    elapsedTime = 0.0f;

                    if(currentFrame >= numFrames)
                    {
                        if(Loop)
                        {
                            currentFrame = 0;
                        }
                        else
                        {
                            OnAnimationEnd();
                            return;
                        }
                    }

                    offset.X = frameWidth * currentFrame;
                }
            }
        }

        protected virtual void OnAnimationEnd()
        {
            isPlaying = false;
        }

        public virtual void Play()
        {
            isPlaying = true;
        }

        protected virtual void Pause()
        {
            isPlaying = false;
        }

        public virtual void Stop()
        {
            isPlaying = false;
            currentFrame = 0;
            elapsedTime = 0.0f;
        }
    }
}
