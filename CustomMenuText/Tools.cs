using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Data;
using System.IO;
using CustomMenuText.CustomTypes;
using System.Reflection;
using IPA.Utilities;
using System.Text.RegularExpressions;
using System.Diagnostics;

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

        public static GameObject FindLogo(logo l)
        {
            switch (l)
            {
                case logo.bat:
                    return GameObject.Find("Logo/BatLogo");
                case logo.a:
                    return GameObject.Find("Logo/EFlickering/LogoE");
                case logo.saber:
                    return GameObject.Find("Logo/SaberLogo");
            }
            if (l == null)
                return null;
            else
                return null;
        }

        public static void ReplaceTexture(logo logoType, Sprite texture)
        {
            FindLogo(logoType).GetComponent<SpriteRenderer>().sprite = texture;
            
        }
        public static Sprite GetTexture(logo logoType)
        {
            Plugin.Log.Info("Finding Logo");
            return (Sprite)FindLogo(logoType).GetComponentInChildren<SpriteRenderer>().sprite;
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
            string customTag1 = "<diColor1>";
            string customTag2 = "<diColor2>";
            string maincol = Tools.ColorToHex(Plugin.diMainColor);
            string botcol = Tools.ColorToHex(Plugin.diBottomColor);
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
                    else
                    {
                        string _line = line.Replace(customTag1, maincol);
                        _line = _line.Replace(customTag2, botcol);
                        currentEntry.Add(_line);
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
                    FileUtils.WriteDefaultConfig();
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
        public static Sprite LoadPNG(string filePath)
        {

            Texture2D tex = null;
            byte[] fileData;
            Sprite spr = null;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadRawTextureData(fileData); //..this will auto-resize the texture dimensions.


                spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            return spr;
        }

        public static LogoImages LoadImagesFromChunk(string chunkPath)
        {
            Sprite s;
            Sprite e;
            Sprite b;
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
        public static void WriteDefaultConfig()
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomMenuText.DefaultTextFile.txt"))
            {
                using (var file = new FileStream(Path.Combine(UnityGame.UserDataPath, "CustomMenuText.txt"), FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
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

    public struct OldFont
    {
        public GameObject prefab;
        public string name;
        public bool builtin;

        public OldFont(GameObject prefab, string name, bool builtIn = false)
        {
            this.prefab = prefab;
            this.name = name;
            this.builtin = builtIn;
        }
    }

    public struct LogoImages
    {
        public Sprite BatLogo;
        public Sprite SaberLogo;
        public Sprite ELogo;
        public bool E;
        public string name;

        public LogoImages(Sprite BatLogo, Sprite SaberLogo, string name, Sprite ELogo = null, bool E = false)
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

                Sprite c = Sprite.Create(Tools.ClearTexture2D(256, 256), new Rect(0.0f, 0.0f, 256.0f, 256.0f), new Vector2(0.5f, 0.5f));

                this.ELogo = c;
            }


        }
    }
}
