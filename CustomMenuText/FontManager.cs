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

        public static void FirstTimeFontLoad()
        {
            List<Font> tempFontList = new List<Font>();

            #region inbuilt fonts
            
            Stream ntf = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomMenuText.Fonts.NeonTubes");
            AssetBundle neonBundle = AssetBundle.LoadFromStream(ntf);
            GameObject NeonTubesPrefab = neonBundle.LoadAsset<GameObject>("Text");
            NeonTubesPrefab.name = "NeonTubes";
            Font NeonTubes = new Font(NeonTubesPrefab, "Neon Tubes 2", true);
            ntf.Close();
            neonBundle.Unload(false);

            Stream bf = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomMenuText.Fonts.Beon");
            AssetBundle beonBundle = AssetBundle.LoadFromStream(bf);
            GameObject BeonPrefab = beonBundle.LoadAsset<GameObject>("Text");
            Font Beon = new Font(BeonPrefab, "Beon", true);
            bf.Close();
            AssetBundle.Destroy(beonBundle);
            beonBundle.Unload(false);
            #endregion

            Plugin.Log.Notice("Searching " + Plugin.FONT_PATH + " For Font Files");
            var fonts = Directory.GetFiles(Plugin.FONT_PATH);
            Plugin.Log.Notice(fonts.Length + " External Fonts");
            if (fonts.Length > 0)
            {
                Plugin.Log.Notice("First Font: " + fonts[0]);
                Plugin.Log.Notice("First Font: " + fonts[0].Substring(Plugin.FONT_PATH.Length));
                for (int i = 0; i < fonts.Length; i++)
                {
                    Plugin.Log.Notice("Adding font " + fonts[i] + " " + i);
                    AssetBundle fontBundle = AssetBundle.LoadFromFile(fonts[i]);
                    GameObject prefab = fontBundle.LoadAsset<GameObject>("Text");
                    prefab.name = fonts[i].Substring(Plugin.FONT_PATH.Length);
                    Font tempFont = new Font(prefab, fonts[i].Substring(Plugin.FONT_PATH.Length));
                    AssetBundle.Destroy(fontBundle);
                    fontBundle.Unload(false);

                }

            }
            FontList = tempFontList.ToArray();
            Plugin.Log.Info("Font loading complete, ");
            Plugin.Log.Info("Found " + FontList.Length + " Total Fonts.");
        }
    }
}
