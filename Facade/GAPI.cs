using FontStashSharp;
using GEngine.Core;
using Microsoft.Xna.Framework;
using GEngine.Graphics;
using GEngine.Assets;
using System;
using GEngine.Audio;
namespace GAPI
{
    public static class Assets
    {
        public static class Fonts
        {
            public static FontSystem baseEngineFont = new ();
            static Fonts()
            {
                baseEngineFont = AssetsManager.Fonts.baseEngineFont;
            }
            public static void AddFonts(string name, string path) => AssetsManager.Fonts.AddFont(name, path);
            public static SpriteFontBase GetFont(string name, int fontSize) => AssetsManager.Fonts.userFonts[name].GetFont(fontSize);
        } 
        public static class Images
        {
            public static void AddImage(string name, ASCIIImage image, StorageSpace spase = StorageSpace.Cache)
                => AssetsManager.ASCIIImage.AddImage(name, image, spase);
            public static void AddImageFromFile(string name, SpriteFontBase spf, Color color, string path, StorageSpace spase = StorageSpace.Cache) 
                => AssetsManager.ASCIIImage.AddImageFromFile(name, spf, color, path, spase);
            public static ASCIIImage GetImage(string name, StorageSpace spase = StorageSpace.Cache) => AssetsManager.ASCIIImage.GetImage(name, spase);
        }
        public static class Audio
        {
            public static void AddAudioFromFile(string name, string path) => AssetsManager.Audio.AddAudioFromFile(name, path);
            public static AudioSource GetAudio(string name) => AssetsManager.Audio.GetAudio(name);
        }
    }
    public static class Graphic
    {
        private static SpriteFontBase baseSpf = AssetsManager.Fonts.baseEngineFont.GetFont(18);
        public static void WriteLine(string text, Color? color = null, SpriteFontBase spf = null)
            => Core.GameConsole.Instance.WriteLine(spf ?? baseSpf, text, color ?? Core.ConsoleGray);

        public static void WriteLineInformation(string text, Color? color = null, SpriteFontBase spf = null)
            => Core.InformationConsole.Instance.WriteLine(spf ?? baseSpf, text, color ?? Core.ConsoleGray);

        public static void ClearImage() => Core.ImageWindow.Instance.Clear();
        public static void SetImage(ASCIIImage image, float ignoreScale = 1, Vector2? ignoreVector = null) 
            => Core.ImageWindow.Instance.SetImage(image, ignoreScale, ignoreVector ?? Vector2.Zero);

        public static void ReadLine(Color? color = null, SpriteFontBase spf = null) 
            => Core.GameConsole.Instance.ReadLine(spf ?? baseSpf, color ?? Core.ConsoleGray);
        public static void ReadLine(Action<string> action, Color? color = null, SpriteFontBase spf = null) 
            => Core.GameConsole.Instance.ReadLine(spf ?? baseSpf, color ?? Core.ConsoleGray, action);

        public static bool HasRead() => Core.GameConsole.Instance.HasLastRead();
        public static string GetRead() => Core.GameConsole.Instance.GetLastRead();
    }
    public static class Palettes
    {
        public static class ConsoleColor
        {
            public static readonly Color Black = new Color(0, 0, 0);
            public static readonly Color DarkBlue = new Color(0, 0, 128);
            public static readonly Color DarkGreen = new Color(0, 128, 0);
            public static readonly Color DarkCyan = new Color(0, 128, 128);
            public static readonly Color DarkRed = new Color(128, 0, 0);
            public static readonly Color DarkMagenta = new Color(128, 0, 128);
            public static readonly Color DarkYellow = new Color(128, 128, 0);
            public static readonly Color Gray = new Color(192, 192, 192);
            public static readonly Color DarkGray = new Color(128, 128, 128);
            public static readonly Color Blue = new Color(0, 0, 255);
            public static readonly Color Green = new Color(0, 255, 0);
            public static readonly Color Cyan = new Color(0, 255, 255);
            public static readonly Color Red = new Color(255, 0, 0);
            public static readonly Color Magenta = new Color(255, 0, 255);
            public static readonly Color Yellow = new Color(255, 255, 0);
            public static readonly Color White = new Color(255, 255, 255);
        }
        public static class GameBoy
        {
            public static readonly Color Black = new Color(15, 56, 15);
            public static readonly Color DarkGray = new Color(48, 98, 48);
            public static readonly Color LightGray = new Color(139, 172, 15);
            public static readonly Color White = new Color(155, 188, 15);
        }
        public static class CGA
        {
            public static readonly Color Background = new Color(0, 0, 0);
            public static readonly Color Cyan = new Color(85, 255, 255);
            public static readonly Color Magenta = new Color(255, 85, 255);
            public static readonly Color White = new Color(255, 255, 255);
        }
        public static class Pico8
        {
            public static readonly Color Black = new Color(0, 0, 0);
            public static readonly Color DarkBlue = new Color(29, 43, 83);
            public static readonly Color DarkPurple = new Color(126, 37, 83);
            public static readonly Color DarkGreen = new Color(0, 135, 81);
            public static readonly Color Brown = new Color(171, 82, 54);
            public static readonly Color DarkGray = new Color(95, 87, 79);
            public static readonly Color LightGray = new Color(194, 195, 199);
            public static readonly Color White = new Color(255, 241, 232);
            public static readonly Color Red = new Color(255, 0, 77);
            public static readonly Color Orange = new Color(255, 163, 0);
            public static readonly Color Yellow = new Color(255, 236, 39);
            public static readonly Color Green = new Color(0, 228, 54);
            public static readonly Color Blue = new Color(41, 173, 255);
            public static readonly Color Indigo = new Color(131, 118, 156);
            public static readonly Color Pink = new Color(255, 119, 168);
            public static readonly Color Peach = new Color(255, 204, 170);
        }
        public static class C64
        {
            public static readonly Color Black = new Color(0, 0, 0);
            public static readonly Color White = new Color(255, 255, 255);
            public static readonly Color Red = new Color(136, 0, 0);
            public static readonly Color Cyan = new Color(170, 255, 238);
            public static readonly Color Violet = new Color(204, 68, 204);
            public static readonly Color Green = new Color(0, 204, 85);
            public static readonly Color Blue = new Color(0, 0, 170);
            public static readonly Color Yellow = new Color(238, 238, 119);
            public static readonly Color Orange = new Color(221, 136, 85);
            public static readonly Color Brown = new Color(102, 68, 0);
            public static readonly Color LightRed = new Color(255, 119, 119);
            public static readonly Color DarkGray = new Color(51, 51, 51);
            public static readonly Color MediumGray = new Color(119, 119, 119);
            public static readonly Color LightGreen = new Color(170, 255, 102);
            public static readonly Color LightBlue = new Color(0, 136, 255);
            public static readonly Color LightGray = new Color(187, 187, 187);
        }
    }
}
