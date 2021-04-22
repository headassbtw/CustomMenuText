using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using TMPro;
using System.IO;
using System.Text;
using IPA.Utilities;
using System.Reflection;
using System.Threading.Tasks;



namespace CustomMenuText
{
    [Plugin(RuntimeOptions.SingleStartInit)]


    

    public class Plugin
    {
        public static int selection_type = 0;
        public static int choice = 0;

        public static GameObject defaultLogo = new GameObject();

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("CustomMenuText initialized.");
        }

        public static Color defaultMainColor = Color.red;
        public static Color defaultBottomColor = new Color(0, 0.659f, 1);
        public static Color diMainColor = Color.red;
        public static Color diBottomColor = new Color(0, 0.659f, 1);
        public static Color MainColor = Color.red;
        public static Color BottomColor = new Color(0, 0.659f, 1);

        /*public Config diConfig;
        public Dictionary<string, DiColors.Config.ColorPair> colorPairs;*/
        #region BSIPA Config
        //Uncomment to use BSIPA's config
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            /*if (Configuration.PluginConfig.Instance.UsingDiColors)
            {
                getDi();
            }
            refreshDi();*/

           

            Log.Debug("Config loaded");
            selection_type = Configuration.PluginConfig.Instance.SelectionType;
            choice = Configuration.PluginConfig.Instance.SelectedTextEntry;
        }
        #endregion

        /*public void getDi()
        {
            diConfig = Config.GetConfigFor("DiColors");
        }

        public void refreshDi()
        {
            if (Configuration.PluginConfig.Instance.UsingDiColors)
            {
                try
                {
                    colorPairs.TryGetValue("Beat", out DiColors.Config.ColorPair beatPair);
                    colorPairs.TryGetValue("Saber", out DiColors.Config.ColorPair saberPair);
                    diMainColor = beatPair.Color;
                    diBottomColor = saberPair.Color;
                    MainColor = diMainColor;
                    BottomColor = diBottomColor;
                }
                catch
                {
                    getDi();
                    refreshDi();
                }
                
                
            }
            
        }*/
        

        public static Plugin instance;

        // path to the file to load text from
        private const string FILE_PATH = "/UserData/CustomMenuText.txt";

        public static string IMG_PATH = null;
        public static string FONT_PATH = null;
        // prefab to instantiate when creating the TextMeshPros
        public static GameObject textPrefab;
        // used if we can't load any custom entries
        public static readonly string[] DEFAULT_TEXT = { "BEAT", "SABER" };
        public static readonly string[] EMPTY = { "", "" };
        public static string[] CURRENT_TEXT = { "FUCKIN", "BEES" };

        public static bool initalFunctionsFinished = false;
        public const string DEFAULT_CONFIG =
@"# Custom Menu Text v3.2.2
# by Arti, heavily modified by headassbtw
# Special Thanks: Kyle1413, Alphie
#
# Use # for comments!
# Separate entries with empty lines; a random one will be picked each time the menu loads.
# Entries with a number of lines other than 2 won't be colored by default.
# Color them yourself with formatting!
<#FF0000>B<#0080FF>S

# Finally allowed again!
MEAT
SABER

# You can override the colors even when the text is 2 lines, plus do a lot of other stuff!
# (contributed by @Rolo)
<size=+5><#ffffff>SBU<#ffff00>BBY
            <size=5><#1E5142>eef freef.

# Some more random messages:
BEAT
SAMER

1337 
SABER

YEET
SABER

BEET
SABER

<size=+5>OWO
   <size=5>what's this?

BAT
SAVER

SATE
BIEBER

BEAR
BEATS

<#FF0000>BEAR <#0080FF>BEATS
<#DDDDDD>BATTLESTAR GALACTICA

BEE
MOVIE

MEME

BEAM
TASER

ENVOY OF
NEZPHERE

BEER
TASTER

ABBA
TREES

EAT
ASS

BERATE
ABS

FLYING
CARS

BEATMANIA
IIDX

# requested by Reaxt
<#8A0707>HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK
HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK HECK

<size=+125><#FF0000>HECK

HECK
OFF

Having problems?
Ask in <#7289DA>#pc-help

READ
BOOKS

# wrong colors
<#0080FF>BEAT
<#FF0000>SABER

<#FF0000>HARDER
<#0080FF>BETTER
<#FF0000>FASTER
<#0080FF>SABER

DON'T
PANIC

STAN
AUROS

<line-height=75%><#cf7100>ARTI
<#FF0000><size=+4><</size>3
<#0080FF>JADE

<i>slontey";

        // caches entries loaded from the file so we don't need to do IO every time the menu loads
        public static List<string[]> allEntries = null;

        public string Name => "Custom Menu Text";
        public string Version => "3.2.2";

        // Store the text objects so when we leave the menu and come back, we aren't creating a bunch of them
        public static TextMeshPro mainText;
        public static TextMeshPro bottomText; // BOTTOM TEXT

        public System.Random random;


        [OnStart]
        public void OnApplicationStart()
        {
            IMG_PATH = Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Images") + "\\";
            FONT_PATH = Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Fonts") + "\\";
            InitializeImageFolder();
            ImageManager.ImgInit();
            
            new GameObject("CustomMenuTextController").AddComponent<CustomMenuTextController>();
            instance = this;
            
            FontManager.FirstTimeFontLoad();
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            //SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            BeatSaberMarkupLanguage.Settings.BSMLSettings.instance.AddSettingsMenu("Menu Text", "CustomMenuText.Configuration.settings.bsml", CustomMenuText.Configuration.CustomMenuTextSettingsUI.instance);
            Views.UICreator.CreateMenu();
            
        }

        void InitializeImageFolder()
        {
            if (!Directory.Exists(Path.Combine(UnityGame.UserDataPath, "CustomMenuText") + "\\"))
                Directory.CreateDirectory(Path.Combine(UnityGame.UserDataPath, "CustomMenuText") + "\\");
            if (!Directory.Exists(FONT_PATH))
                Directory.CreateDirectory(FONT_PATH);
            if (!Directory.Exists(IMG_PATH))
                    Directory.CreateDirectory(IMG_PATH);
        }


        public void YeetUpTheText()
        {
            switch (Configuration.PluginConfig.Instance.SelectionType)
            {
                case 0:
                    //default
                    setText(EMPTY);
                    defaultLogo.SetActive(true);

                    break;
                case 1:
                    //random
                    defaultLogo.SetActive(false);
                    pickRandomEntry();
                    break;
                case 2:
                    //pre-chosen
                    defaultLogo.SetActive(false);
                    setText(allEntries[choice]);
                    break;
            }
        }
        
        public static void ApplyFont()
        {
            GameObject.Destroy(textPrefab);
            Plugin.replaceLogo();
            setText(CURRENT_TEXT);
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText")?.transform.SetParent(defaultLogo.transform.parent.transform, true);
            if (Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText-Bot")?.transform.SetParent(defaultLogo.transform.parent.transform, true);
            if (!Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText")?.transform.SetParent(null, true);
            if (!Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText-Bot")?.transform.SetParent(null, true);
            //refreshDi();
            if (mainText != null) mainText.color = MainColor;
            if (bottomText != null) bottomText.color = BottomColor;
            defaultLogo = FindUnityObjectsHelper.GetAllGameObjectsInLoadedScenes().Where(go => go.name == "Logo").FirstOrDefault();



            

            //Log.Notice("Changed to scene " + arg1.name);
            if (arg1.name.Contains("Menu")) // Only run in menu scene
            {
                if (allEntries == null)
                {
                    reloadFile();
                }
                if (allEntries.Count == 0)
                {
                    Console.WriteLine("[CustomMenuText] File found, but it contained no entries! Leaving original logo intact.");
                }
                else
                {
                    YeetUpTheText();
                    
                }
            }
        }

        /// <summary>
        /// Chooses a random entry from the current config and sets the menu text to that entry.
        /// Warning: Only call this function from the main menu scene!
        /// </summary>
        public void pickRandomEntry()
        {
            // Choose an entry randomly

            // Unity's random seems to give biased results
            // int entryPicked = UnityEngine.Random.Range(0, entriesInFile.Count);
            // using System.Random instead
            if (random == null) random = new System.Random();
            int entryPicked = random.Next(allEntries.Count);

            // Set the text
            setText(allEntries[entryPicked]);
        }

        

        


        
        /// <summary>
        /// Replaces the logo in the main menu (which is an image and not text
        /// as of game version 0.12.0) with an editable TextMeshPro-based
        /// version. Performs only the necessary steps (if the logo has already
        /// been replaced, restores the text's position and color to default
        /// instead).
        /// Warning: Only call this function from the main menu scene!
        /// 
        /// Code generously donated by Kyle1413; edited some by Arti
        /// </summary>
        /// 
        public static void replaceLogo()
        {
            // Since 0.13.0, we have to create our TextMeshPros differently! You can't change the font at runtime, so we load a prefab with the right font from an AssetBundle. This has the side effect of allowing for custom fonts, an oft-requested feature.
            if(textPrefab == null)
            {
                textPrefab = FontManager.FontList[Configuration.PluginConfig.Instance.Font].prefab;
            } 

            defaultLogo = FindUnityObjectsHelper.GetAllGameObjectsInLoadedScenes().Where(go => go.name == "Logo").FirstOrDefault();

            // Logo Top Pos : 0.63, 18.61, 26.1
            // Logo Bottom Pos : 0, 14, 26.1
            /*if (mainText != null)
            {
                GameObject.Destroy(GameObject.Find("CustomMenuText"));
                GameObject.Destroy(mainText);
                mainText = null;
            }
            if (bottomText != null)
            {
                GameObject.Destroy(GameObject.Find("CustomMenuText-Bot"));
                GameObject.Destroy(bottomText);
                bottomText = null;
            }
            */
            //if (mainText == null) mainText = GameObject.Find("CustomMenuText")?.GetComponent<TextMeshPro>();
            //if (mainText == null)
            {
                GameObject textObj = GameObject.Instantiate(textPrefab);
                textObj.name = "CustomMenuText";
                textObj.SetActive(false);
                mainText = textObj.GetComponent<TextMeshPro>();
                mainText.alignment = TextAlignmentOptions.Center;
                mainText.fontSize = 12;
                mainText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
                mainText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
                mainText.richText = true;
                textObj.transform.localScale *= 3.7f;
                mainText.overflowMode = TextOverflowModes.Overflow;
                mainText.enableWordWrapping = false;
                textObj.SetActive(true);
                if (Configuration.PluginConfig.Instance.OnlyInMainMenu) textObj.transform.SetParent(defaultLogo.transform.parent.transform, true);
            }
            mainText.rectTransform.position = new Vector3(0f, 18.61f, 26.1f);

            mainText.color = MainColor;

            mainText.text = "BEAT";

            //if (bottomText == null) bottomText = GameObject.Find("CustomMenuText-Bot")?.GetComponent<TextMeshPro>();
            //if (bottomText == null)
            {
                GameObject textObj2 = GameObject.Instantiate(textPrefab);
                textObj2.name = "CustomMenuText-Bot";
                textObj2.SetActive(false);
                bottomText = textObj2.GetComponent<TextMeshPro>();
                bottomText.alignment = TextAlignmentOptions.Center;
                bottomText.fontSize = 12;
                bottomText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
                bottomText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
                bottomText.richText = true;
                textObj2.transform.localScale *= 3.7f;
                bottomText.overflowMode = TextOverflowModes.Overflow;
                bottomText.enableWordWrapping = false;
                textObj2.SetActive(true);
                if(Configuration.PluginConfig.Instance.OnlyInMainMenu) textObj2.transform.SetParent(defaultLogo.transform.parent.transform, true);

            }
            bottomText.rectTransform.position = new Vector3(0f, 14f, 26.1f);
            bottomText.color = BottomColor;
            bottomText.text = "SABER";

            

            // Destroy Default Logo

            //if (defaultLogo != null) defaultLogo.SetActive(false);
        }
        /// <summary>
        /// Sets the text in the main menu (which normally reads BEAT SABER) to
        /// the text of your choice. TextMeshPro formatting can be used here.
        /// Additionally:
        /// - If the text is exactly 2 lines long, the first line will be
        ///   displayed in red, and the second will be displayed in blue.
        /// Warning: Only call this function from the main menu scene!
        /// </summary>
        /// <param name="lines">
        /// The text to display, separated by lines (from top to bottom).
        /// </param>
        public static void setText(string[] lines)
        {
            // Set up the replacement logo
            replaceLogo();
            CURRENT_TEXT = lines;
            if (lines.Length == 2)
            {
                mainText.text = lines[0];
                bottomText.text = lines[1];
            }
            else
            {
                // Hide the bottom line entirely; we're just going to use the main one
                bottomText.text = "";

                // Center the text vertically (halfway between the original positions)
                Vector3 newPos = mainText.transform.position;
                newPos.y = (newPos.y + bottomText.transform.position.y) / 2;
                mainText.transform.position = newPos;

                // Set text color to white by default (users can change it with formatting anyway)
                mainText.color = Color.white;

                // Set the text
                mainText.text = String.Join("\n", lines);
            }
        }

        public void reloadFile()
        {
            allEntries = FileUtils.readFromFile(FILE_PATH);
            Configuration.PluginConfig.Instance.SelectionType = selection_type;
            Configuration.PluginConfig.Instance.SelectedTextEntry = choice;
        }

        /// <summary>
        /// Saves the current value of <see cref="allEntries"/> to the default config location.
        /// Warning: effectively strips comments from the file!
        /// </summary>
        public void writeFile()
        {
            // join entries by two newlines and lines by one
            string contents = String.Join("\n\n", allEntries.Select(e => String.Join("\n", e)));
            string gameDirectory = Environment.CurrentDirectory;
            gameDirectory = gameDirectory.Replace('\\', '/');
            var path = gameDirectory + FILE_PATH;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(contents
                        // normalize newlines to CRLF
                        .Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n"));
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CustomMenuText] Failed to save config!");
                Console.WriteLine("[CustomMenuText] Error:");
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Overwrites the current config with the default and loads it.
        /// </summary>
        public void restoreDefaultConfig()
        {
            string gameDirectory = Environment.CurrentDirectory;
            gameDirectory = gameDirectory.Replace('\\', '/');
            var path = gameDirectory + FILE_PATH;
            try
            {
                if (File.Exists(path)) File.Delete(path);
                reloadFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CustomMenuText] Failed to save config!");
                Console.WriteLine("[CustomMenuText] Error:");
                Console.WriteLine(ex);
            }
        }



        [OnExit]
        public void OnApplicationQuit()
        {
            Configuration.PluginConfig.Instance.SelectionType = selection_type;
            Configuration.PluginConfig.Instance.SelectedTextEntry = choice;
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            //SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }



        

    }
}
