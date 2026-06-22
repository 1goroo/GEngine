using Microsoft.Xna.Framework.Audio;
using NLayer;
using System;
using System.Runtime.InteropServices;

namespace GEngine.Audio
{
    public class AudioSourceMP3 : AudioSource
    {
        MpegFile MPegFile;
        public AudioSourceMP3(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "MP3 path == null");
            MPegFile = new MpegFile(path);
            SoundEffectInstance = new DynamicSoundEffectInstance(MPegFile.SampleRate, AudioManager.AudioFormatToMonogame(MPegFile));
            SoundEffectInstance.BufferNeeded += GetBuffer;
            Core.Core.Instance.game.Exiting += D;
        }
        private void BlankRead() => MPegFile.Position = 1152;
        public void GetBuffer(object sender, EventArgs e)
        {
            byte[] buffer = new byte[GetBufferSize(MPegFile)];
            float[] samplesBuffer = new float[MPegFile.SampleRate * MPegFile.Channels / 10];
            int bytesRead = MPegFile.ReadSamples(samplesBuffer, 0, samplesBuffer.Length);
            if (bytesRead <= 0) return;
            short[] schBuffer = new short[samplesBuffer.Length];
            buffer = GetByteArray(samplesBuffer, schBuffer, bytesRead).ToArray();
            if (bytesRead < samplesBuffer.Length)
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
        static Span<byte> GetByteArray(float[] SamplesBuffer, short[] schBuffer, int reads)
        {
            for (int i = 0; i < reads; i++)
                schBuffer[i] = (short)(Math.Clamp(SamplesBuffer[i], -1f, 1f) * 32767);
            return MemoryMarshal.Cast<short, byte>(schBuffer.AsSpan(0,reads));
        }
        static int GetBufferSize(MpegFile stream) => stream.SampleRate * stream.Channels * 2 / 10;

        public override void Play(int position = 0)
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
        public override void Stop()
        {
            IsPlaying = false;
            SoundEffectInstance.Stop(true);
            MPegFile.Position = 0;
        }
        void D(object sender, EventArgs e) => Dispose();
        public override void Dispose()
        {
            MPegFile.Dispose();
            SoundEffectInstance.Dispose();
            SoundEffectInstance.BufferNeeded -= GetBuffer;
            Core.Core.Instance.game.Exiting -= D;

            GC.SuppressFinalize(this);
        }
    }
}
