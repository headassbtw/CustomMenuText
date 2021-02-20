using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;

namespace CustomMenuText.Views
{
    internal class UICreator
    {
        public static MenuTextFlowCoordinator MenuTextFlowCoordinator;
        public static bool Created;

        public static void CreateMenu()
        {
            if (!Created)
            {
                MenuButton menuButton = new MenuButton("Menu Text", "Manage custom menu text", ShowFlow);
                MenuButtons.instance.RegisterButton(menuButton);
                Created = true;
            }
        }


        public static void ShowFlow()
        {
            if (MenuTextFlowCoordinator == null)
                MenuTextFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<MenuTextFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(MenuTextFlowCoordinator);
        }
    }
}
