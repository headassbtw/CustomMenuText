using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using DiColors;
using static DiColors.Config;
using UnityEngine;

namespace CustomMenuText.Harmony_Patches
{
    [HarmonyPatch(typeof(DiColors.Services.SignColorSwapper), "Initialize")]
    internal static class HookDiCMenuEdited
    {
        internal static void Postfix(DiColors.Services.SignColorSwapper __instance, Config.Menu ____config)
        {
            ____config.ColorPairs.TryGetValue("Beat", out Config.ColorPair beatPair);
            if (beatPair.Enabled)
                Plugin.diMainColor = beatPair.Color;
            else
                Plugin.diMainColor = Plugin.defaultMainColor;
            ____config.ColorPairs.TryGetValue("Saber", out Config.ColorPair saberPair);
            if (saberPair.Enabled)
                Plugin.diBottomColor = saberPair.Color;
            else
                Plugin.diBottomColor = Plugin.defaultBottomColor;
            Plugin.instance.reloadFile();
            Plugin.instance.TextInit();
        }
    }
}
