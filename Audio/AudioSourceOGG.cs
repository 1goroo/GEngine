using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System;
using System.Runtime.InteropServices;

namespace GEngine.Audio
{
    public class AudioSourceOGG : AudioSource
    {
        VorbisReader vorbis;
        byte[] buffer;
        float[] samplesBuffer;
        short[] schBuffer;
        public AudioSourceOGG(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "OGG path == null");
            vorbis = new VorbisReader(path);
            SoundEffectInstance = new DynamicSoundEffectInstance(vorbis.SampleRate, AudioManager.AudioFormatToMonogame(vorbis));
            SoundEffectInstance.BufferNeeded += GetBuffer;
            Core.Core.Instance.game.Exiting += D;

            buffer = new byte[GetBufferSize(vorbis)];
            samplesBuffer = new float[vorbis.SampleRate * vorbis.Channels / 10];
            schBuffer = new short[samplesBuffer.Length];
        }
        public void GetBuffer(object sender, EventArgs e)
        {
            int bytesRead = vorbis.ReadSamples(samplesBuffer, 0, samplesBuffer.Length);
            if (bytesRead <= 0)
            {
                if (!Looping) SoundEffectInstance.Stop();
                else vorbis.SamplePosition = 0;
                return;
            }
            buffer = GetByteArray(samplesBuffer, schBuffer, bytesRead).ToArray();
            if (bytesRead < samplesBuffer.Length)
            {
                Span<byte> b = buffer;
                Span<byte> a = b.Slice(0, bytesRead*2);
                SoundEffectInstance.SubmitBuffer(a.ToArray());
            }
            else
                SoundEffectInstance.SubmitBuffer(buffer);
        }
        static Span<byte> GetByteArray(float[] SamplesBuffer, short[] schBuffer, int reads)
        {
            for (int i = 0; i < reads; i++)
                schBuffer[i] = (short)(Math.Clamp(SamplesBuffer[i], -1f, 1f) * 32767);
            return MemoryMarshal.Cast<short, byte>(schBuffer.AsSpan(0, reads));
        }
        static int GetBufferSize(VorbisReader stream) => stream.SampleRate * stream.Channels * 2 / 10;

        public override void Play(int position = 0)
        {
            IsPlaying = true;
            if (position != 0)
            {
                vorbis.SamplePosition = position;
            }
            GetBuffer(default, default);
            GetBuffer(default, default);
            SoundEffectInstance.Play();
        }
        public override void Stop()
        {
            IsPlaying = false;
            SoundEffectInstance.Stop(true);
            vorbis.SamplePosition = 0;
        }
        void D(object sender, EventArgs e) => Dispose();
        public override void Dispose()
        {
            vorbis.Dispose();
            SoundEffectInstance.Dispose();
            SoundEffectInstance.BufferNeeded -= GetBuffer;
            Core.Core.Instance.game.Exiting -= D;

            GC.SuppressFinalize(this);
        }
    }
}
