using System;
using System.Collections.Generic;
using System.IO;
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
using TMPro;
using IPA.Utilities;
using System.Text.RegularExpressions;
/*
namespace BeatSaberMarkupLanguage.Components
{
public class CustomListTableData : MonoBehaviour, TableView.IDataSource
{
public enum ListStyle
{
List, Box, Simple
}

private ListStyle listStyle = ListStyle.List;

private LevelListTableCell songListTableCellInstance;
//private LevelPackTableCell levelPackTableCellInstance;
private SimpleTextTableCell simpleTextTableCellInstance;

public List<CustomCellInfo> data = new List<CustomCellInfo>();
public float cellSize = 8.5f;
public string reuseIdentifier = "BSMLListTableCell";
public TableView tableView;

public bool expandCell = false;

public ListStyle Style
{
get => listStyle;
set
{
//Sets the default cell size for certain styles
switch (value)
{
case ListStyle.List:
cellSize = 8.5f;
break;
case ListStyle.Simple:
cellSize = 8f;
break;
}

listStyle = value;
}
}

public LevelListTableCell GetTableCell()
{
LevelListTableCell tableCell = (LevelListTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
if (!tableCell)
{
if (songListTableCellInstance == null)
songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));

tableCell = Instantiate(songListTableCellInstance);

}

//tableCell.SetField("_beatmapCharacteristicImages", new Image[0]);
//tableCell.SetField("_notOwned", false);

tableCell.reuseIdentifier = reuseIdentifier;
return tableCell;
}



public SimpleTextTableCell GetSimpleTextTableCell()
{
SimpleTextTableCell tableCell = (SimpleTextTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
if (!tableCell)
{
if (simpleTextTableCellInstance == null)
simpleTextTableCellInstance = Resources.FindObjectsOfTypeAll<SimpleTextTableCell>().First(x => x.name == "SimpleTextTableCell");

tableCell = Instantiate(simpleTextTableCellInstance);
}

tableCell.reuseIdentifier = reuseIdentifier;
return tableCell;
}

public virtual TableCell CellForIdx(TableView tableView, int idx)
{
switch (listStyle)
{
case ListStyle.List:
LevelListTableCell tableCell = GetTableCell();

TextMeshProUGUI nameText = tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songNameText");
TextMeshProUGUI authorText = tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songAuthorText");
tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songBpmText").gameObject.SetActive(false);
tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songDurationText").gameObject.SetActive(false);
tableCell.GetField<Image, LevelListTableCell>("_favoritesBadgeImage").gameObject.SetActive(false);
tableCell.transform.Find("BpmIcon").gameObject.SetActive(false);
if (expandCell)
{
nameText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
authorText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
}

nameText.text = data[idx].text;
authorText.text = data[idx].subtext;
tableCell.GetField<Image, LevelListTableCell>("_coverImage").sprite = data[idx].icon == null ? Utilities.LoadSpriteFromTexture(Texture2D.blackTexture) : data[idx].icon;

return tableCell;
case ListStyle.Simple:
SimpleTextTableCell simpleCell = GetSimpleTextTableCell();
simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").richText = true;
simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").enableWordWrapping = true;
simpleCell.text = data[idx].text;

return simpleCell;
}

return null;
}

public float CellSize()
{
return cellSize;
}

public int NumberOfCells()
{
return data.Count();
}

public class CustomCellInfo
{
public string text;
public string subtext;
public string subsubtext;
public Sprite icon;

public CustomCellInfo(string text, string subtext = null, string subsubtext = null,  Sprite icon = null)
{
this.text = text;
this.subtext = subtext;
this.subsubtext = subsubtext;
this.icon = icon;
}
};
}


}
*/


namespace CustomMenuText.ViewControllers
{
    public class Cell
    {
        [UIValue("text-top")]
        public string TopText;
        [UIValue("text-bottom")]
        public string BottomText;
        /*[UIComponent("bg")]
        public Backgroundable background;
        [UIValue("bgColor")]
        public string bgColor;*/
        [UIAction("refresh-visuals")]
        public void Refresh(bool selected, bool highlighted)
        {

        }


        public Cell(string topText, string bottomText = null)
        {
            this.TopText = topText;
            this.BottomText = bottomText;
        }
    }

    
    class TextSelectorViewController : BSMLResourceViewController
    {
        public int LastSelectedCell = 0;
        public Cell LastKnownCell;

        public override string ResourceName => "CustomMenuText.Views.TextSelector.TextSelector.bsml";
        
        [UIComponent("TextList")] public CustomCellListTableData textListData;
        [UIComponent("FontList")] public CustomListTableData fontListData = new CustomListTableData();
        //[UIComponent("ImgList")] public CustomListTableData imgListData = new CustomListTableData();
        [UIValue("contents")] public List<object> CellList = new List<object>();
        [UIAction("textSelect")]
        public void textSelect(TableView _, Cell cell)
        {
            int row2 = 0;
            int row = CellList.IndexOf(cell);
            if(row == 0)
            {
                row2 = row;
                Plugin.selection_type = 0;
                Configuration.PluginConfig.Instance.SelectionType = 0;
                Plugin.instance.YeetUpTheText();

                
            }
            if(row == 1)
            {
                Plugin.selection_type = 1;
                Configuration.PluginConfig.Instance.SelectionType = 1;
                Plugin.Instance.pickRandomEntry();
                Plugin.instance.YeetUpTheText();
            }
            if(row > 1)
            {
                row2 = row - 2;
                Plugin.selection_type = 2;
                Plugin.choice = row2;
                Configuration.PluginConfig.Instance.SelectionType = 2;
                Configuration.PluginConfig.Instance.SelectedTextEntry = row2;
                Plugin.instance.YeetUpTheText();
            }
            
        }
        public Cell random = new Cell("Random");
        public Cell defaultt = new Cell("Default");
        [UIAction("fontSelect")]
        public void fontSelect(TableView _, int row)
        {
            //int row = fontListData.data.IndexOf(cell);
            Configuration.PluginConfig.Instance.Font = row;
            Plugin.mainText.font = FontManager.Fonts[row];
            Plugin.bottomText.font = FontManager.Fonts[row];
        }
        /*[UIAction("imgSelect")]
        public void imgSelect(TableView _, int row)
        {
            Tools.ReplaceLogos(ImageManager.ImageChunks[row]);
        }*/

        [UIAction("refreshTextEntries")]
        public void ReloadTextEntries()
        {
            Plugin.instance.reloadFile();
            SetupTextList();
        }
        [UIAction("refreshFontEntries")]
        public void ReloadFontEntries()
        {
            FontManager.FirstTimeFontLoad();
            SetupFontList();
        }
        /*[UIAction("refreshImageEntries")]
        public void ReloadImageEntries()
        {
            SetupImageList();
        }*/


        [UIAction("#post-parse")]
        public void PostParse()
        {
            SetupTextList();
            SetupFontList();
            //SetupImageList();
        }
        //public static string StripTMPTags(this string source) => source.Replace(@"<size", "<\u200B").Replace(@">", "\u200B>");
        public void SetupTextList()
        {
            string line1 = "";
            string line2 = "";
            CellList.Clear();
            CellList.Add(defaultt);
            CellList.Add(random);

            foreach(var TextEntry in Plugin.allEntries)
            {
                try
                {
                    if (TextEntry.Length > 4)
                    {
                        line1 = TextEntry[0];
                        line1 = line1.Substring(0, 10);
                        line2 = TextEntry[1];
                        line2 = line2.Substring(0, 7) + "...";
                    }

                    if (TextEntry.Length != 2)
                    {
                        string regex = "(<size[^a-b]{2,}?>)";
                        string top = "";
                        string bottom = "";
                        for (int i = 0; i < TextEntry.Length; i++)
                            if (i < TextEntry.Length / 2 && i != 0) top += " " + TextEntry[i];
                            else if (i < TextEntry.Length / 2) top += TextEntry[i];
                            else bottom += " " + TextEntry[i];

                        line1 = Regex.Replace(top, regex, "");
                        line2 = Regex.Replace(bottom, regex, "");

                        if (Regex.Replace(line1, "(<[^x]{2,}?>)", "").Length > 20) line1 = line1.Substring(0, 20);
                        if (Regex.Replace(line2, "(<[^x]{2,}?>)", "").Length > 20)
                            line2 = line2.Substring(0, 17) + "...";
                        //line1 = String.Join("\n", TextEntry);
                        //line2 = "";
                    }

                    if (TextEntry.Length == 2)
                    {
                        string regex = "(<size[^a-b]{2,}?>)";



                        line1 = Regex.Replace(TextEntry[0], regex, "");
                        line2 = Regex.Replace(TextEntry[1], regex, "");
                    }

                    if (TextEntry.Length == 0)
                    {
                        line1 = "[Empty]";
                        line2 = " ";
                    }

                    if (line1 != "" || line2 != "") line1 = Tools.ColorToHex(Plugin.MainColor) + line1;
                    if (line1 != "" || line2 != "") line2 = Tools.ColorToHex(Plugin.BottomColor) + line2;
                }
                catch (Exception e)
                {
                    line1 = "Failed to";
                    line2 = "load preview";
                }

                Cell toAdd = new Cell(line1, line2);
                CellList.Add(toAdd);

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
                    textListData.tableView.SelectCellWithIdx(Configuration.PluginConfig.Instance.SelectedTextEntry + 2);
                    break;
            }
        }
        public void SetupFontList()
        {
            //this should prevent selecting a font that doesn't exist
            if (Configuration.PluginConfig.Instance.Font > FontManager.Fonts.Count)
                Configuration.PluginConfig.Instance.Font = FontManager.Fonts.Count;
            fontListData.data.Clear();
            foreach (var font in FontManager.Fonts)
            {
                CustomListTableData.CustomCellInfo fontCell;
                try
                {
                    string name = Path.GetFileNameWithoutExtension(font.sourceFontFile.name);
                    Plugin.Log.Notice($"adding font\"{name}\" to table");
                    
                    if (name.ToLower().Equals("neontubes2") || name.ToLower().Equals("beon") || name.ToLower().Equals("teko"))
                    {
                        fontCell = new CustomListTableData.CustomCellInfo(name, "Built-In");
                        fontListData.data.Add(fontCell);
                    }
                    else
                    {
                        fontCell = new CustomListTableData.CustomCellInfo(name);
                        fontListData.data.Add(fontCell);
                    }
                }
                catch (Exception e)
                {
                    fontCell = new CustomListTableData.CustomCellInfo("Broken Font", "please remove it");
                    fontListData.data.Add(fontCell);
                    Plugin.Log.Critical("Exception while adding font:");
                    Console.WriteLine(e.ToString());
                }
                
            }
            fontListData.tableView.ReloadData();
            
            try{fontListData.tableView.SelectCellWithIdx(Configuration.PluginConfig.Instance.Font);}
            catch(IndexOutOfRangeException){Plugin.Log.Critical("Tried to select a font beyond the bounds of the list");}
            Plugin.mainText.font = FontManager.Fonts[Configuration.PluginConfig.Instance.Font];
            Plugin.bottomText.font = FontManager.Fonts[Configuration.PluginConfig.Instance.Font];
        }
        /*public void SetupImageList()
        {
            imgListData.data.Clear();
            foreach (var imageChunk in ImageManager.ImageChunks)
            {
                string yes;
                if (!imageChunk.E)
                    yes = "with Flickering E";
                else
                    yes = "";
                var imgCell = new CustomListTableData.CustomCellInfo(imageChunk.name, yes);
                imgListData.data.Add(imgCell);
            }
            imgListData.tableView.ReloadData();
        }*/
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
