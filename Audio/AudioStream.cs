using Microsoft.Xna.Framework.Audio;
using NLayer;
using System;
using System.Runtime.InteropServices;

namespace GEngine.Audio
{
    public class AudioStream : IPlayableSound
    {
        MpegFile MPegFile;
        DynamicSoundEffectInstance SoundEffectInstance;
        public bool Looping { get; set; } = false;
        public bool IsPlaying { get; set; } = false;
        public AudioStream(string path)
        {
            MPegFile = new MpegFile(path);
            SoundEffectInstance = new DynamicSoundEffectInstance(MPegFile.SampleRate, AudioManager.AudioFormatToMonogame(MPegFile));
            SoundEffectInstance.BufferNeeded += GetBuffer;
            Core.Core.Instance.game.Exiting += D;
        }
        private void BlankRead() => MPegFile.Position = 1152;
        public void GetBuffer(object sender, EventArgs e)
        {
            byte[] buffer = new byte[GetBufferSize(MPegFile)];
            float[] bf = new float[MPegFile.SampleRate * MPegFile.Channels / 10];
            int bytesRead = MPegFile.ReadSamples(bf, 0, bf.Length);
            if (bytesRead <= 0) return;
            short[] schBuffer = new short[bf.Length];
            buffer = GetByteArray(bf, schBuffer, bytesRead).ToArray();
            if (bytesRead < bf.Length)
            {
                Span<byte> b = buffer;
                Span<byte> a = b.Slice(0, bytesRead);
                SoundEffectInstance.SubmitBuffer(a.ToArray());
                if (!Looping) SoundEffectInstance.Stop();
                else MPegFile.Position = 0;
            }
            else
                SoundEffectInstance.SubmitBuffer(buffer);
        }
        Span<byte> GetByteArray(float[] SamplesBuffer, short[] schBuffer, int reads)
        {
            for (int i = 0; i < reads; i++)
                schBuffer[i] = (short)(Normalize(SamplesBuffer[i])*32767);
            return MemoryMarshal.Cast<short, byte>(schBuffer.AsSpan(0,reads));
            float Normalize(float n)
            {
                if (n > 1f) n = 1f;
                else if (n < -1f) n = -1f;
                return n;
            }
        }
        static int GetBufferSize(MpegFile stream) => stream.SampleRate * stream.Channels * 2 / 10;

        public void Play(int position = 0)
        {
            IsPlaying = true;
            if (position == 0)
            {
                MPegFile.Position = 0;
                BlankRead();
            }
            else MPegFile.Position = position;
            GetBuffer(default, default);
            GetBuffer(default, default);
            SoundEffectInstance.Play();
        }
        public void Stop()
        {
            IsPlaying = false;
            SoundEffectInstance.Stop(true);
            MPegFile.Position = 0;
        }
        public void Pause()
        {
            IsPlaying = false;
            SoundEffectInstance.Stop();
        }
        public void Resume()
        {
            IsPlaying = true;
            SoundEffectInstance.Play();
        }
        void D(object sender, EventArgs e) => Dispose();
        public void Dispose()
        {
            MPegFile.Dispose();
            SoundEffectInstance.Dispose();
            SoundEffectInstance.BufferNeeded -= GetBuffer;
            Core.Core.Instance.game.Exiting -= D;
        }
    }
}
