using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;

namespace CustomMenuText.Configuration
{
    internal class CustomMenuTextSettingsUI : PersistentSingleton<CustomMenuTextSettingsUI>
    {
        [UIValue("diColors")]
        public bool DiColors
        {
            get => PluginConfig.Instance.UsingDiColors;
            set => PluginConfig.Instance.UsingDiColors = value;
        }
        [UIValue("onlyMain")]
        public bool OnlyMainMenu
        {
            get => PluginConfig.Instance.OnlyInMainMenu;
            set => PluginConfig.Instance.OnlyInMainMenu = value;
        }

        
    }
}
