using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine;
using UnityEngine.UI;
using HMUI;
using System.Linq;
using BeatSaberMarkupLanguage.ViewControllers;

namespace CustomMenuText.ViewControllers
{
    class TextSelectorViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomMenuText.Views.TextSelector.TextSelector.bsml";
        [UIComponent("TextList")] public CustomListTableData textListData;

        [UIAction("textSelect")]
        public void Select(TableView _, int row)
        {
            int row2 = 0;
            if(row == 0)
            {
                row2 = row;
                Plugin.selection_type = 0;
                Configuration.PluginConfig.Instance.SelectionType = 0;
                Plugin.setText(Plugin.DEFAULT_TEXT);
                
            }
            if(row == 1)
            {
                Plugin.selection_type = 1;
                Configuration.PluginConfig.Instance.SelectionType = 1;
                Plugin.Instance.pickRandomEntry();
            }
            if(row > 1)
            {
                row2 = row - 2;
                Plugin.selection_type = 2;
                Plugin.choice = row2;
                Configuration.PluginConfig.Instance.SelectionType = 2;
                Configuration.PluginConfig.Instance.SelectedEntry = row2;
                Plugin.setText(Plugin.allEntries.ElementAt(row2));
            }

            
        }
        public CustomListTableData.CustomCellInfo random = new CustomListTableData.CustomCellInfo("Random");
        public CustomListTableData.CustomCellInfo defaultt = new CustomListTableData.CustomCellInfo("Default");

        [UIAction("refreshEntries")]
        public void ReloadEntries()
        {
            Plugin.instance.reloadFile();
            SetupList();
        }


        [UIAction("#post-parse")]
        public void PostParse()
        {
            SetupList();
        }
        public void SetupList()
        {
            string line1 = "";
            string line2 = "";
            textListData.data.Clear();
            textListData.data.Add(defaultt);
            textListData.data.Add(random);
            foreach(var TextEntry in Plugin.allEntries)
            {
                if(TextEntry.Length != 2)
                {
                    line1 = String.Join("\n", TextEntry);
                    line2 = "";
                }
                if(TextEntry.Length == 2)
                {
                    line1 = TextEntry[0];
                    line2 = TextEntry[1];
                }

                CustomListTableData.CustomCellInfo toAdd = new CustomListTableData.CustomCellInfo(line1, line2);
                textListData.data.Add(toAdd);

            }
            textListData.tableView.ReloadData();

            switch (Configuration.PluginConfig.Instance.SelectionType)
            {
                case 0:
                    textListData.tableView.SelectCellWithIdx(0);
                    break;
                case 1:
                    textListData.tableView.SelectCellWithIdx(1);
                    break;
                case 2:
                    textListData.tableView.SelectCellWithIdx(Configuration.PluginConfig.Instance.SelectedEntry + 2);
                    break;
            }
        }

        public void SelectCorrectCell(int selType, int choice)
        {
            switch (selType)
            {
                case 0:
                    textListData.tableView.SelectCellWithIdx(0);
                    break;
                case 1:
                    textListData.tableView.SelectCellWithIdx(1);
                    break;
                case 2:
                    textListData.tableView.SelectCellWithIdx(choice + 2);
                    break;
            }
        }
    }
}
