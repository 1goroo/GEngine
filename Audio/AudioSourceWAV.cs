using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace GEngine.Audio
{
    public class AudioSourceWAV : AudioSource
    {
        WAVFile file;
        byte[] buffer;
        public AudioSourceWAV(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "Wav path == null");
            file = new WAVFile(path);
            SoundEffectInstance = new DynamicSoundEffectInstance(file.SampleRate, AudioManager.AudioFormatToMonogame(file.Channels));

            SoundEffectInstance.BufferNeeded += GetBuffer;
            Core.Core.Instance.game.Exiting += D;

            buffer = new byte[GetBufferSize(file)];
        }
        static int GetBufferSize(WAVFile file) => file.SampleRate * file.Channels * 2 / 10;
        private void BlankRead() => file.stream.Seek(GetAudioStartPosition(), SeekOrigin.Begin);
        public void GetBuffer(object sender, EventArgs e)
        {
            int bytesRead = file.stream.Read(buffer, 0, buffer.Length);
            if (bytesRead <= 0) 
            {
                if (!Looping) SoundEffectInstance.Stop();
                else { file.Position = 0; BlankRead(); }
                return;
            }
            if (bytesRead < buffer.Length)
            {
                Span<byte> b = buffer;
                Span<byte> a = b.Slice(0, bytesRead);
                SoundEffectInstance.SubmitBuffer(a.ToArray());
                if (!Looping) SoundEffectInstance.Stop();
                else file.Position = 0;
            }
            else
                SoundEffectInstance.SubmitBuffer(buffer);
        }
        private long GetAudioStartPosition()
        {
            file.stream.Seek(12, SeekOrigin.Begin);
            while (file.Position < file.stream.Length)
            {
                string chuncID = new string(file.reader.ReadChars(4));
                int chuncLenght = file.reader.ReadInt32();
                if (chuncID == "data") return file.Position;
                file.stream.Seek(chuncLenght, SeekOrigin.Current);
            }
            return 0;
        }
        public override void Play(int position = 0)
        {
            IsPlaying = true;
            if (position == 0)
            {
                file.Position = 0;
                BlankRead();
            }
            else file.Position = position;
            GetBuffer(default, default);
            GetBuffer(default, default);
            SoundEffectInstance.Play();
        }
        public override void Stop()
        {
            IsPlaying = false;
            SoundEffectInstance.Stop(true);
            file.Position = 0;
        }
        void D(object sender, EventArgs e) => Dispose();
        public override void Dispose()
        {
            file.Dispose();
            SoundEffectInstance.Dispose();
        }
        private class WAVFile
        {
            public FileStream stream;
            public BinaryReader reader;

            public int SampleRate;
            public int Channels;
            public long Position { get => stream.Position; set => stream.Position = value; }
            public WAVFile(string path)
            {
                stream = new FileStream(path, FileMode.Open);
                reader = new BinaryReader(stream);

                stream.Seek(0, SeekOrigin.Begin);
                if (new string(reader.ReadChars(4)) != "RIFF") throw new InvalidDataException("WAV is not RIFF");
                stream.Seek(22, SeekOrigin.Begin);
                Channels = reader.ReadUInt16();
                stream.Seek(24, SeekOrigin.Begin);
                SampleRate = reader.ReadUInt16();
                stream.Position = 0;
            }
            public void Dispose()
            {
                stream.Dispose();
                reader.Dispose();
            }
        }
    }
}
