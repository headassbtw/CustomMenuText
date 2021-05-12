using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMenuText.CustomTypes;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace CustomMenuText
{
    class FontManager
    {
        public static Font[] FontList;

        public static Font loadBuiltInFont(string fontName, string displayName)
        {
            Stream ntf = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomMenuText.Fonts." + fontName);
            AssetBundle neonBundle = AssetBundle.LoadFromStream(ntf);
            GameObject NeonTubesPrefab = neonBundle.LoadAsset<GameObject>("Text");
            Font NeonTubes = new Font(NeonTubesPrefab, displayName, true);
            ntf.Close();
            neonBundle.Unload(false);
            return NeonTubes;
        }


        public static void FirstTimeFontLoad()
        {
            List<Font> tempFontList = new List<Font>();

            #region inbuilt fonts

            tempFontList.Add(loadBuiltInFont("NeonTubes", "Neon Tubes 2"));
            tempFontList.Add(loadBuiltInFont("Beon", "Beon"));
            tempFontList.Add(loadBuiltInFont("Teko", "Teko"));
            #endregion


            Plugin.Log.Notice("FontManager)  Searching " + Plugin.FONT_PATH + " For Font Files");
            var fonts = Directory.GetFiles(Plugin.FONT_PATH);
            Plugin.Log.Notice("FontManager)  " + fonts.Length + " External Fonts");
            if (fonts.Length > 0)
            {
                for (int i = 0; i < fonts.Length; i++)
                {
                    
                    AssetBundle fontBundle = AssetBundle.LoadFromFile(fonts[i]);
                    GameObject prefab = fontBundle.LoadAsset<GameObject>("Text");
                    prefab.name = fonts[i].Substring(Plugin.FONT_PATH.Length);
                    Font tempFont = new Font(prefab, fonts[i].Substring(Plugin.FONT_PATH.Length));
                    tempFontList.Add(tempFont);
                    Plugin.Log.Notice("Adding Font " + tempFont.name);
                    AssetBundle.Destroy(fontBundle);
                    fontBundle.Unload(false);
                    
                }

            }
            FontList = tempFontList.ToArray();
            Plugin.Log.Info("FontManager)  Font loading complete, ");
            Plugin.Log.Info("FontManager)  Found " + FontList.Length + " Total Fonts.");
        }
    }
}
