using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using HMUI;
using CustomMenuText.ViewControllers;

namespace CustomMenuText.Views
{
    internal class MenuTextFlowCoordinator : FlowCoordinator
    {
        private TextSelectorViewController _textSelectorViewController;

        public void Awake()
        {
            if (!_textSelectorViewController)
                _textSelectorViewController = BeatSaberUI.CreateViewController<TextSelectorViewController>();
        }


        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    SetTitle("Menu Text");
                    showBackButton = true;
                    ProvideInitialViewControllers(_textSelectorViewController);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Error(e);
            }
        }
        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
