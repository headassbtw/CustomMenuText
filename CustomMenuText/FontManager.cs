using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMenuText.CustomTypes;
using UnityEngine;
using System.IO;
using System.Reflection;
using IPA.Utilities;
using TMPro;

namespace CustomMenuText
{
    class FontManager
    {
        public static GameObject Prefab;
        public static TMP_FontAsset Font;
        public static List<TMP_FontAsset> Fonts;

        public static GameObject loadPrefab(string fontName)
        {
            Stream ntf = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomMenuText.Fonts." + fontName);
            AssetBundle neonBundle = AssetBundle.LoadFromStream(ntf);
            GameObject NeonTubesPrefab = neonBundle.LoadAsset<GameObject>("Text");
            ntf.Close();
            neonBundle.Unload(false);
            return NeonTubesPrefab;
        }

        public static Font embeddedFont(string fileName)
        {
            string FontPath = Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Cache", fileName);
            Plugin.Log.Info($"Cache path is: {FontPath}");
            using (Stream ntf = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("CustomMenuText.Fonts." + fileName))
            {
                if (File.Exists(FontPath))
                {
                    File.Delete(FontPath);
                }
                using (FileStream fileStream = new FileStream(FontPath, FileMode.CreateNew))
                {
                    for (int i = 0; i < ntf.Length; i++)
                        fileStream.WriteByte((byte)ntf.ReadByte());
                    fileStream.Close();
                }
                ntf.Close();
            }

            Font tf = new Font(FontPath);
            return tf;
        }

        public static TMP_FontAsset LoadFromTTF(string path)
        {
            var fnt = new Font(path);
            Font = TMP_FontAsset.CreateFontAsset(fnt);
            return Font;
        }
        
        
        public static void FirstTimeFontLoad()
        {
            List<TMP_FontAsset> fonts = new List<TMP_FontAsset>();
            Prefab = loadPrefab("NeonTubes");
            #region inbuilt fonts
            fonts.Add(TMP_FontAsset.CreateFontAsset(embeddedFont("NeonTubes2.otf")));
            fonts.Add(TMP_FontAsset.CreateFontAsset(embeddedFont("beon.ttf")));
            fonts.Add(TMP_FontAsset.CreateFontAsset(embeddedFont("Teko.ttf")));
            #endregion

            string[] files = Directory.GetFiles(Path.Combine(UnityGame.UserDataPath,"CustomMenuText","Fonts"));
            List<string> TTFs = new List<string>();
            List<TMP_FontAsset> TTFFiles = new List<TMP_FontAsset>();
            foreach (var file in files)
            {
                if (file.EndsWith(".ttf") || file.EndsWith(".otf"))
                {
                    TTFs.Add(file);
                }
            }
            foreach (var ttf in TTFs)
            {
                fonts.Add(LoadFromTTF(ttf));
            }
            Fonts = fonts;

            Plugin.Log.Info("FontManager)  Font loading complete, ");
        }
    }
}
