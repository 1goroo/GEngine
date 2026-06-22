using System;

namespace GEngine.Audio
{
    public interface IPlayableSound : IDisposable
    {
        public bool Looping { get; set; }
        public bool IsPlaying { get; set; }
        public void Play(int position = 0);
        public void Stop();
        public void Pause();
        public void Resume();
    }
}
