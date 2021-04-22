using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Data;
using System.IO;
using CustomMenuText.CustomTypes;

namespace CustomMenuText
{
    public static class Tools
    {
        public static string ColorToHex(Color color)
        {
            float r = color.r;
            float g = color.g;
            float b = color.b;

            int ir = (int)(r * 255);
            int ig = (int)(g * 255);
            int ib = (int)(b * 255);

            string hr = string.Format("{0:x}", ir);
            if (hr.Length == 1) hr = "0" + hr;
            string hg = string.Format("{0:x}", ig);
            if (hg.Length == 1) hg = "0" + hg;
            string hb = string.Format("{0:x}", ib);
            if (hb.Length == 1) hb = "0" + hb;

            return "<#" + hr + hg + hb + ">";
        }

        public static GameObject FindLogo(CustomTypes.logo l)
        {
            switch (l)
            {
                case CustomTypes.logo.bat:
                    return GameObject.Find("Logo/BatLogo");
                case CustomTypes.logo.a:
                    return GameObject.Find("Logo/EFlickering/LogoE");
                case CustomTypes.logo.saber:
                    return GameObject.Find("Logo/SaberLogo");
            }
            if (l == null)
                return null;
            else
                return null;
        }

        public static void ReplaceTexture(CustomTypes.logo logoType, Texture2D texture)
        {
            FindLogo(logoType).GetComponent<SpriteRenderer>().material.SetTexture("_MainTex", texture);
            
        }
        public static Texture2D GetTexture(CustomTypes.logo logoType)
        {
            Plugin.Log.Info("Finding Logo");
            return (Texture2D)FindLogo(logoType).GetComponentInChildren<SpriteRenderer>().sprite.texture;
        }

        

        public static Texture2D ClearTexture2D(int width, int height)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.clear);
            tex.Resize(width, height);
            return tex;
        }


        public static void ReplaceLogos(CustomTypes.LogoImages imgs)
        {
            ReplaceTexture(logo.bat, imgs.BatLogo);
            ReplaceTexture(logo.a, imgs.ELogo);
            ReplaceTexture(logo.saber, imgs.SaberLogo);
        }
    }

    public static class FileUtils
    {
        public static List<string[]> readFromFile(string relPath)
        {
            List<string[]> entriesInFile = new List<string[]>();

            // Look for the custom text file
            string gameDirectory = Environment.CurrentDirectory;
            gameDirectory = gameDirectory.Replace('\\', '/');
            if (File.Exists(gameDirectory + relPath))
            {
                var linesInFile = File.ReadLines(gameDirectory + relPath);

                // Strip comments (all lines beginning with #)
                linesInFile = linesInFile.Where(s => s == "" || s[0] != '#');
                // Collect entries, splitting on empty lines
                List<string> currentEntry = new List<string>();
                foreach (string line in linesInFile)
                {
                    if (line == "")
                    {
                        entriesInFile.Add(currentEntry.ToArray());
                        currentEntry.Clear();
                    }
                    else if (line.Contains("<diColor1>"))
                    {
                        currentEntry.Add(line.Replace("<diColor1>", Tools.ColorToHex(Plugin.diMainColor)));
                    }
                    else if (line.Contains("<diColor2>"))
                    {
                        currentEntry.Add(line.Replace("<diColor2>", Tools.ColorToHex(Plugin.diBottomColor)));
                    }
                    else
                    {
                        currentEntry.Add(line);
                    }
                }
                if (currentEntry.Count != 0)
                {
                    // in case the last entry doesn't end in a newline
                    entriesInFile.Add(currentEntry.ToArray());
                }
            }
            else
            {
                // No custom text file found!
                // Create the file and populate it with the default config
                try
                {
                    using (FileStream fs = File.Create(gameDirectory + relPath))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(Plugin.DEFAULT_CONFIG
                            // normalize newlines to CRLF
                            .Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n"));
                        fs.Write(info, 0, info.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[CustomMenuText] No custom text file found, and an error was encountered trying to generate a default one!");
                    Console.WriteLine("[CustomMenuText] Error:");
                    Console.WriteLine(ex);
                    Console.WriteLine("[CustomMenuText] To use this plugin, manually create the file " + relPath + " in your Beat Saber install directory.");
                    return entriesInFile;
                }
                // File was successfully created; load from it with a recursive call.
                return readFromFile(relPath);
            }

            return entriesInFile;
        }

        public static List<String> GetFileChunks(string path)
        {
            return Directory.GetDirectories(path).ToList();
        }
        public static Texture2D LoadPNG(string filePath)
        {

            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadRawTextureData(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }

        public static LogoImages LoadImagesFromChunk(string chunkPath)
        {
            Texture2D s;
            Texture2D e;
            Texture2D b;
            CustomTypes.LogoImages li = new CustomTypes.LogoImages();

            if(File.Exists(chunkPath + "\\LogoSaber.png"))
                li.SaberLogo = LoadPNG(chunkPath + "\\LogoSaber.png");

            if (File.Exists(chunkPath + "\\LogoE.png"))
                li.ELogo = LoadPNG(chunkPath + "\\LogoE.png");
            else if (!File.Exists(chunkPath + "\\LogoE.png"))
                li.ELogo = null;


            if (File.Exists(chunkPath + "\\LogoBat.png"))
                li.BatLogo = LoadPNG(chunkPath + "\\LogoBat.png");


            return li;
        }

    }
}


namespace CustomMenuText.CustomTypes
{
    public enum logo
    {
        bat,
        a,
        saber
    }

    public struct Font
    {
        public GameObject prefab;
        public string name;
        public bool builtin;

        public Font(GameObject prefab, string name, bool builtIn = false)
        {
            this.prefab = prefab;
            this.name = name;
            this.builtin = builtIn;
        }
    }


    public struct LogoImages
    {
        public Texture2D BatLogo;
        public Texture2D SaberLogo;
        public Texture2D ELogo;
        public bool E;
        public string name;

        public LogoImages(Texture2D BatLogo, Texture2D SaberLogo, string name, Texture2D ELogo = null, bool E = false)
        {
            this.BatLogo = BatLogo;
            this.SaberLogo = SaberLogo;
            this.name = name;
            if (ELogo)
            {
                this.E = true;
                this.ELogo = ELogo;
            }
            else
            {
                this.E = false;
                this.ELogo = Tools.ClearTexture2D(256, 256);
            }


        }
    }
}
