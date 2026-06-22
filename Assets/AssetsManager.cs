using FontStashSharp;
using GEngine.Audio;
using GEngine.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace GEngine.Assets
{
    public static class AssetsManager
    {
        internal static void CleanCache()
        {
            ASCIIImage.CleanCache();
        }
        public static class Fonts
        {
            public static Dictionary<string, FontSystem> userFonts = new Dictionary<string, FontSystem>();
            public static FontSystem baseEngineFont = new FontSystem();
            static Fonts()
            {
                var assembly = typeof(Core.Core).Assembly;
                using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Content.Fonts.CascadiaCode-Light.otf"))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        baseEngineFont.AddFont(ms.ToArray());
                    }
                }
            }
            public static void AddFont(string name, string path)
            {
                userFonts.Add(name, new FontSystem());
                userFonts[name].AddFont(File.ReadAllBytes(path));
            }
        }
        public static class ASCIIImage
        {
            static Dictionary<string, Graphics.ASCIIImage> userASCIIImage = new Dictionary<string, Graphics.ASCIIImage>();
            static Dictionary<string, Graphics.ASCIIImage> cacheASCIIImage = new Dictionary<string, Graphics.ASCIIImage>();
            static string[] interrobangText = {
                    " --- ",
                    "| ? |",
                    " --- "
                };
            static Graphics.ASCIIImage interrobangASCIIImage = new Graphics.ASCIIImage(interrobangText, Fonts.baseEngineFont.GetFont(18), Color.Wheat);
            public static void AddImage(string name, Graphics.ASCIIImage image, StorageSpace spase = StorageSpace.Cache)
            {
                if ((userASCIIImage.ContainsKey(name) && spase == StorageSpace.Game)||(cacheASCIIImage.ContainsKey(name) && spase == StorageSpace.Cache))
                {
                    Console.WriteLine($"[Exeption] GE0100: An element with the key {name} already exists (AddImage).");
                    return;
                }
                if (spase == StorageSpace.Game)
                    userASCIIImage.Add(name, image);
                else if (spase == StorageSpace.Cache)
                    cacheASCIIImage.Add(name, image);
            }
            public static void AddImageFromFile(string name, SpriteFontBase spf, Color color, string path, StorageSpace spase = StorageSpace.Cache)
            {
                if ((userASCIIImage.ContainsKey(name) && spase == StorageSpace.Game) || (cacheASCIIImage.ContainsKey(name) && spase == StorageSpace.Cache))
                {
                    Console.WriteLine($"[Exeption] GE0100: An element with the key {name} already exists (AddImageFromFile).");
                    return;
                }
                string pathDomain = AppDomain.CurrentDomain.BaseDirectory;
                string contentPath = Path.Combine(pathDomain, "Content", path);
                if (!File.Exists(contentPath))
                {
                    Console.WriteLine("[Exeption] GE0101: Content path not exist (AddImageFromFile).");
                    return;
                }
                Graphics.ASCIIImage image = new Graphics.ASCIIImage(File.ReadAllLines(contentPath), spf, color);
                if (spase == StorageSpace.Game)
                    userASCIIImage.Add(name, image);
                else if (spase == StorageSpace.Cache)
                    cacheASCIIImage.Add(name, image);
            }
            public static Graphics.ASCIIImage GetImage(string name, StorageSpace spase = StorageSpace.Cache)
            {
                Graphics.ASCIIImage returnImage = interrobangASCIIImage;
                if (spase == StorageSpace.Game)
                    returnImage = userASCIIImage.ContainsKey(name) ? userASCIIImage[name] : interrobangASCIIImage;
                else if (spase == StorageSpace.Cache)
                    returnImage = cacheASCIIImage.ContainsKey(name) ? cacheASCIIImage[name] : interrobangASCIIImage;
                return returnImage;
            }
            internal static void CleanCache() { cacheASCIIImage.Clear(); }
        }
        public static class Audio
        {
            //public static Dictionary<string, SoundEffect> userSoundEffects = new Dictionary<string, SoundEffect>();
            //public static Dictionary<string, Song> userSongs = new Dictionary<string, Song>();
            //public static void AddAudioElementFromMGCB<T>(string name, string MGCBPath)
            //{
            //    T asset = Core.Core.Instance.contentManager.Load<T>(MGCBPath);
            //    if (asset == null) return;
            //    if (asset is SoundEffect)
            //        userSoundEffects.Add(name, asset as SoundEffect);
            //    else if (asset is Song)
            //        userSongs.Add(name, asset as Song);
            //}
            //public static T GetAudioElement<T>(string name)
            //{
            //    if (typeof(T) == typeof(SoundEffect))
            //    {
            //        if (userSoundEffects.TryGetValue(name, out var soundEffect))
            //            return (T)(object)soundEffect;
            //    }
            //    else if (typeof(T) == typeof(Song) && userSongs.ContainsKey(name))
            //    {
            //        if (userSongs.TryGetValue(name, out var song))
            //            return (T)(object)song;
            //    }
            //    return default;
            //}
            static Dictionary<string, AudioStream> userAudio = new Dictionary<string, AudioStream>();
            public static void AddAudioFromFile(string name, string Path) => userAudio.Add(name, new AudioStream(Path));
            public static AudioStream GetAudio(string name) => userAudio[name];
        }
    }
}
