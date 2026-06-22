using Microsoft.Xna.Framework.Audio;
using NLayer;
using System.Threading.Tasks;

namespace GEngine.Audio
{
    public static class AudioManager
    {
        internal static int DefaultChannelsCount = 1;
        internal static AudioChannels AudioFormatToMonogame(MpegFile stream)
        {
            return stream.Channels switch
            {
                1 => AudioChannels.Mono,
                2 => AudioChannels.Stereo,
                _ => AudioChannels.Mono
            };
        }
        public static async Task PlayOneShot(string Path)
        {
            var audio = new AudioSourceMP3(Path);
            audio.Play();
            while (audio.IsPlaying) await Task.Yield(); 
            audio.Stop();
            audio.Dispose();
        }
    };
}
