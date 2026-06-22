using Microsoft.Xna.Framework.Audio;
using System;

namespace GEngine.Audio
{
    public abstract class AudioSource : IDisposable
    {
        // public fields
        public bool Looping { get; set; }
        public bool IsPlaying { get; protected set; }

        // protected fields
        protected DynamicSoundEffectInstance SoundEffectInstance;

        // public methods
        public abstract void Play(int position = 0);
        public abstract void Stop();
        public virtual void Pause() 
        {
            IsPlaying = false;
            SoundEffectInstance.Stop();
        }
        public virtual void Resume()
        {
            IsPlaying = true;
            SoundEffectInstance.Play();
        }
        public abstract void Dispose();
    }
}
