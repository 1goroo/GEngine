using FontStashSharp;
using GEngine.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using GEngine.Core;
using Microsoft.Xna.Framework;

namespace GEngine.Assets
{
    public static class AssetsManager
    {
        public static class FontsManager
        {
            public static Dictionary<string, FontSystem> userFonts = new Dictionary<string, FontSystem>();
            public static FontSystem baseEngineFont = new FontSystem();
            static FontsManager()
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
        public static class ASCIIImageManager
        {
            static Dictionary<string, ASCIIImage> userASCIIImage = new Dictionary<string, ASCIIImage>();
            static string[] interrobangText = {
                    " --- ",
                    "| ? |",
                    " --- "
                };
            static ASCIIImage interrobangASCIIImage = new ASCIIImage(interrobangText, FontsManager.baseEngineFont.GetFont(18), Color.Wheat);
            public static void AddImage(string name, ASCIIImage image)
            {
                if (userASCIIImage.ContainsKey(name))
                {
                    Console.WriteLine($"[Exeption] GE0100: An element with the key {name} already exists (AddImage).");
                    return;
                }
                userASCIIImage.Add(name, image);
            }
            public static void AddImageFromFile(string name, SpriteFontBase spf, Color color, string path)
            {
                if (userASCIIImage.ContainsKey(name))
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
                ASCIIImage image = new ASCIIImage(File.ReadAllLines(contentPath), spf, color);
                userASCIIImage.Add(name, image);
            }
            public static ASCIIImage GetImage(string name) => userASCIIImage.ContainsKey(name) ? userASCIIImage[name] : interrobangASCIIImage;
        }
        public static class AudioManager
        {
            public static Dictionary<string, SoundEffect> userSoundEffects = new Dictionary<string, SoundEffect>();
            public static Dictionary<string, Song> userSongs = new Dictionary<string, Song>();
            public static void AddAudioElementFromMGCB<T>(string name, string MGCBPath)
            {
                T asset = Core.Core.Instance.contentManager.Load<T>(MGCBPath);
                if (asset == null) return;
                if (asset is SoundEffect)
                    userSoundEffects.Add(name, asset as SoundEffect);
                else if (asset is Song)
                    userSongs.Add(name, asset as Song);
            }
            public static T GetAudioElement<T>(string name)
            {
                if (typeof(T) == typeof(SoundEffect))
                {
                    if (userSoundEffects.TryGetValue(name, out var soundEffect))
                        return (T)(object)soundEffect;
                }
                else if (typeof(T) == typeof(Song) && userSongs.ContainsKey(name))
                {
                    if (userSongs.TryGetValue(name, out var song))
                        return (T)(object)song;
                }
                return default;
            }
        }
    }
}
