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
using HarmonyLib;
using IPA.Loader;

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
        internal static Harmony harmony;

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
            harmony = new Harmony("com.headassbtw.custommenutext");
            Log.Info("CustomMenuText initialized.");
        }

        public static Color defaultMainColor = Color.red;
        public static Color defaultBottomColor = new Color(0, 0.659f, 1);
        public static Color diMainColor = Color.red;
        public static Color diBottomColor = new Color(0, 0.659f, 1);
        public static Color MainColor = Color.red;
        public static Color BottomColor = new Color(0, 0.659f, 1);

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
            selection_type = Configuration.PluginConfig.Instance.SelectionType;
            choice = Configuration.PluginConfig.Instance.SelectedTextEntry;
        }
        #endregion

        
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
        // Logo Top Pos : 0.63, 18.61, 26.1
        // Logo Bottom Pos : 0.63, 14, 26.1
        public static Vector3 DefTopPos = new Vector3(0.63f, 18.61f, 26.1f);
        public static Vector3 DefBotPos = new Vector3(0.63f, 14f, 26.1f);


        public static bool initalFunctionsFinished = false;

        // caches entries loaded from the file so we don't need to do IO every time the menu loads
        public static List<string[]> allEntries = null;

        public string Name => "Custom Menu Text";
        public string Version => "3.4.0";

        // Store the text objects so when we leave the menu and come back, we aren't creating a bunch of them
        public static TextMeshPro mainText;
        public static TextMeshPro bottomText; // BOTTOM TEXT

        public System.Random random;

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchAll("com.headassbtw.custommenutext");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            if (!Directory.Exists(Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Cache")))
                Directory.CreateDirectory(Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Cache"));
            IMG_PATH = Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Images") + "\\";
            FONT_PATH = Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Fonts") + "\\";
            InitializeImageFolder();
            
            choice = Configuration.PluginConfig.Instance.SelectedTextEntry;
            try
            {
                PluginManager.GetPlugin("DiColors");
                try
                {
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                }catch(Exception e) { Plugin.Log.Critical("Harmony Patching Failed:"); Plugin.Log.Critical(e.ToString()); }

            }
            catch (Exception)
            {
                Log.Critical("DiColors is not installed, or was not loaded at the current time, disabling DiColors specific features.");
                Configuration.PluginConfig.Instance.UsingDiColors = false;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            }
            
            //new GameObject("CustomMenuTextController").AddComponent<CustomMenuTextController>();
            instance = this;
            
            FontManager.FirstTimeFontLoad();
            
            BeatSaberMarkupLanguage.Settings.BSMLSettings.instance.AddSettingsMenu("Menu Text", "CustomMenuText.Configuration.settings.bsml", Configuration.CustomMenuTextSettingsUI.instance);
            Views.UICreator.CreateMenu();
            reloadFile();
        }

        void InitializeImageFolder()
        {
            if (!Directory.Exists(Path.Combine(UnityGame.UserDataPath, "CustomMenuText") + "\\"))
                Directory.CreateDirectory(Path.Combine(UnityGame.UserDataPath, "CustomMenuText") + "\\");
            if (!Directory.Exists(FONT_PATH))
                Directory.CreateDirectory(FONT_PATH);
            //if (!Directory.Exists(IMG_PATH))
            //        Directory.CreateDirectory(IMG_PATH);
        }


        public void YeetUpTheText()
        {
            if (Configuration.PluginConfig.Instance.UsingDiColors)
            {
                MainColor = diMainColor;
                BottomColor = diBottomColor;
            }
            else
            {
                MainColor = defaultMainColor;
                BottomColor = defaultBottomColor;
            }


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
        
        public void ApplyFont()
        {
            Plugin.replaceLogo();
            YeetUpTheText();
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name.Contains("Menu")) // Only run in menu scene
            {
                TextInit();
            }
        }

        public void TextInit()
        {
            if (Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText")?.transform.SetParent(defaultLogo.transform.parent.transform, true);
            if (Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText-Bot")?.transform.SetParent(defaultLogo.transform.parent.transform, true);
            if (!Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText")?.transform.SetParent(null, true);
            if (!Configuration.PluginConfig.Instance.OnlyInMainMenu) GameObject.Find("CustomMenuText-Bot")?.transform.SetParent(null, true);
            if (mainText != null) mainText.color = MainColor;
            if (bottomText != null) bottomText.color = BottomColor;
            defaultLogo = FindUnityObjectsHelper.GetAllGameObjectsInLoadedScenes().Where(go => go.name == "Logo").FirstOrDefault();

            if (allEntries == null)
            {
                reloadFile();
            }
            if (allEntries.Count == 0)
            {
                Log.Notice("File found, but it contained no entries! Leaving original logo intact.");
            }
            else
            {
                YeetUpTheText();
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

            

            defaultLogo = FindUnityObjectsHelper.GetAllGameObjectsInLoadedScenes().Where(go => go.name == "Logo").FirstOrDefault();

            // Logo Top Pos : 0.63, 18.61, 26.1
            // Logo Bottom Pos : 0, 14, 26.1
            if(!textPrefab)
                textPrefab = FontManager.loadPrefab("NeonTubes");
            if (mainText == null) mainText = GameObject.Find("CustomMenuText")?.GetComponent<TextMeshPro>();
            if (mainText == null)
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
            mainText.rectTransform.position = DefTopPos;

            mainText.color = MainColor;

            mainText.text = "BEAT";
            mainText.font = FontManager.Fonts[Configuration.PluginConfig.Instance.Font];

            if (bottomText == null) bottomText = GameObject.Find("CustomMenuText-Bot")?.GetComponent<TextMeshPro>();
            if (bottomText == null)
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
            bottomText.rectTransform.position = DefBotPos;
            bottomText.color = BottomColor;
            bottomText.text = "SABER";
            bottomText.font = FontManager.Fonts[Configuration.PluginConfig.Instance.Font];
            

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
            if(!mainText)
                replaceLogo();
            CURRENT_TEXT = lines;
            if (lines.Length == 2)
            {
                mainText.color = MainColor;
                mainText.transform.position = DefTopPos;
                mainText.text = lines[0];
                bottomText.color = BottomColor;
                bottomText.transform.position = DefBotPos;
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
            if(allEntries != null)
                allEntries.Clear();
            allEntries = FileUtils.readFromFile(FILE_PATH);
            Configuration.PluginConfig.Instance.SelectionType = selection_type;
            Configuration.PluginConfig.Instance.SelectedTextEntry = choice;
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Configuration.PluginConfig.Instance.SelectionType = selection_type;
            Configuration.PluginConfig.Instance.SelectedTextEntry = choice;
            try
            {
                SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            }
            catch (Exception) { }

            string cachePath = Path.Combine(UnityGame.UserDataPath, "CustomMenuText", "Cache");
            if (Directory.Exists(cachePath))
            {
                foreach (var st in Directory.GetFiles(cachePath))
                {
                    File.Delete(st);
                }
                Directory.Delete(cachePath);
            }
                
        }

        


        

    }
}
