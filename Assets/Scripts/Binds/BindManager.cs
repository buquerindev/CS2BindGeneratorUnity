using Newtonsoft.Json.Linq;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BindManager : MonoBehaviour
{
    [SerializeField] private JSONLoader JSONLoader;
    private BindList bindList = new();
    private readonly string jsonURL = "https://buquerindev.github.io/CS2BindGeneratorUnity/appdata/binds.json";

    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject bindSeparatorPrefab;
    [SerializeField] private GameObject bindPanelPrefab;
    [SerializeField] private GameObject togglePanelPrefab;

    [SerializeField] private Transform controlsPanelContainer;
    [SerializeField] private Transform hiddenPanelContainer;
    [SerializeField] private Transform buyPanelContainer;
    [SerializeField] private Transform togglesPanelContainer;
    [SerializeField] private Transform sayPanelContainer;
    private Transform currentContainer;

    [SerializeField] private Button controlsButton;
    [SerializeField] private Button hiddenButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button togglesButton;
    [SerializeField] private Button sayButton;
    [SerializeField] private Button exportBindsButton;

    private bool addUnbindall = false;

    private List<Button> menusButtons;

    private Color buttonDefaultColor;
    private Color buttonSelectedColor;

    private ColorBlock selectedCB;
    private ColorBlock cb;

    private List<BindPanel> bindPanels = new();
    private List<TogglePanel> togglePanels = new();

    [SerializeField] private GameObject ANSIKeyboard;
    [SerializeField] private GameObject ISOKeyboard;

    private string[] csLogoASCII = new string[]
    {
        @"                                                   $$$$$                                          ",
        @"                                                  X$$$$$$                                         ",
        @"                                                 .:$$$$$............                              ",
        @"                                               X$$$$$$$$$$$$$$$$$$$.                              ",
        @"                                              X$$$$$$$$$$$$$$                                     ",
        @"                                            .;$$$$$$$$$$$$$$$                                     ",
        @"             .$$$$$$$$$$$$$$$$$$$$$$$X      $$$$$$$$$$X$$$           $$$$$$$$$$$$$$$$$$$$$$$$$:   ",
        @"            $$$$$$$$$$$$$$$$$$$$$$$$$      $$$$$$$$$$$             $$$$$$$$$$$$$$$$$$$$$$$$$$$    ",
        @"           $$$$$$$$$$$$$$$$$$$$$$$$X        $$$$$$$$$             $$$$$$$$$$$$$$$$$$$$$$$$$$;     ",
        @"          $$$$$$$                          $$$$$$$$x             $$$$$$$                          ",
        @"         $$$$$$$                          $$$$$$$$$$            X$$$$$$                           ",
        @"        x$$$$$$.                         $$$$$$$$$$$$           $$$$$$$$$$$$$$$$$$$$$$$$          ",
        @"        x$$$$$$.                         $$$$$$$$$$$$           $$$$$$$$$$$$$$$$$$$$$$$$          ",
        @"       $$$$$$$                          X$$$$x   $$$$$$           ;$$$$$$$$$$$$$$$$$$$$$$         ",
        @"      $$$$$$$                          $$$$$$      $$$$+                         .$$$$$$          ",
        @"     $$$$$$$                          $$$$$$      $$$$$                          $$$$$$;          ",
        @"     $$$$$$$$$$$$$$$$$$$$$$$$$       $$$$$:      $$$$$      $$$$$$$$$$$$$$$$$$$$$$$$$$X           ",
        @"     ;$$$$$$$$$$$$$$$$$$$$$$$       $$$$$        $$$$.     $$$$$$$$$$$$$$$$$$$$$$$$$$.            ",
        @"       +$$$$$$$$$$$$$$$$$$$$       X$$$.        +$$$x     $$$$$$$$$$$$$$$$$$$$$$$$.               ",
        @"                                   $$$          $$$+                                              ",
        @"                                  $$$$         :$$$$$$                                            ",
        @"                ____  _           __  ______                           __                         ",
        @"               / __ )(_)___  ____/ / / ____/__  ____  ___  _________ _/ /_____  _____             ",
        @"              / __  / / __ \/ __  / / / __/ _ \/ __ \/ _ \/ ___/ __ `/ __/ __ \/ ___/             ",
        @"             / /_/ / / / / / /_/ / / /_/ /  __/ / / /  __/ /  / /_/ / /_/ /_/ / /                 ",
        @"            /_____/_/_/ /_/\__,_/  \____/\___/_/ /_/\___/_/   \__,_/\__/\____/_/                  ",
        @"                                                                                                  ",
        @"                                            Created by:                                           ",
        @"                                           @buquerindev                                           ",
        @"              . -------------------------------------------------------------------.              ",
        @"              | [Esc] [F1][F2][F3][F4][F5][F6][F7][F8][F9][F0][F10][F11][F12] o o o|              ",
        @"              |                                                                    |              ",
        @"              | [`][1][2][3][4][5][6][7][8][9][0][-][=][_<_] [I][H][U] [N][/][*][-]|              ",
        @"              | [|-][Q][W][E][R][T][Y][U][I][O][P][{][}] | | [D][E][D] [7][8][9]|+||              ",
        @"              | [CAP][A][S][D][F][G][H][J][K][L][;]['][#]|_|           [4][5][6]|_||              ",
        @"              | [^][\][Z][X][C][V][B][N][M][,][.][/] [__^__]    [^]    [1][2][3]| ||              ",
        @"              | [c]   [a][________________________][a]   [c] [<][V][>] [ 0  ][.]|_||              ",
        @"              `--------------------------------------------------------------------'              "
    };

    string[] rodrex = new string[] {
    "                                                                                                                                           ",
    "                                                                                                                                           ",
    "                                                                                                                                           ",
    "                                                                     %%@@@@@@@@@#                                                          ",
    "                                                                 #@@@@@@@@@@@@@@@@@@@%                                                     ",
    "                                                              %@@@@@@@@@@@@@@@@@@@@@@@@%#                                                  ",
    "                                                            @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#                                                ",
    "                                                          #@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@                                               ",
    "                                                         #%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@                                              ",
    "                                                        #%***####*##%%@@@%@@@@@@@@@@@@@@@@@@@@                                             ",
    "                                                        %+**+++***+**###%%%%%%@@@@@@@@@@@@@@@@                                             ",
    "                                                       #++++++++++++**#####%%%%@@@@@@@@@@@@@@@%                                            ",
    "                                                       #+++=========++**#####%%@@@@@@%%%%@@@@@@                                            ",
    "                                                       *+===----=====+**#####%%%%@@@%%%%%%@@@@@                                            ",
    "                                                       ++===----=====++**###%%%%%%%%%%%%%%%@@@@                                            ",
    "                                                       +===========++++**####%%%%%%%%%%%%%%@@@@                                            ",
    "                                                       *======---==+=+=+**###%%%@@@@%%%%%%%%@@@                                            ",
    "                                                       +===+=--=======++*#%%@@@@@@@@@@%%%%%@@@@                                            ",
    "                                                        =#*##%###*++==+#%%%%%#**#%%%%%%%%%%@@@@@%#                                         ",
    "                                                      +#=+====*%%%+=-+#%%@@@@*=@@@%@%%##%#%@@%%#%%                                         ",
    "                                                       ====%+-#@%%*+=*#%@@@@%##%%%%%@%%###%@%#%%%%                                         ",
    "                                                       =====--=+##++=*%%%@@@@@%%%%%%%%%%#%%@%@@%%%                                         ",
    "                                                       -==-==++++===+#%%%%%@@%%%%%%%%%%%##%@%%@@@#                                         ",
    "                                                       ===--=======++#%%%%%%%%%%%%%%%%####%@%@@##*                                         ",
    "                                                        *=-------===+#%%%%%@@%%%%%%%#####%@@%%%%#                                          ",
    "                                                        +=+------=--+*%####%@%%%%#%%####%%@@%%##                                           ",
    "                                                        ===---==-=*=+*#%%@@%%%%##%%###%%%@@@%%                                             ",
    "                                                         *==-==--==*++#%@@@@@@%%#%%%%%%%@@@@%%                                             ",
    "                                                         #*==-+#**+*##*##%%%%%@@%%%%%%%%@@@@%%                                             ",
    "                                                         #**==#@#++++++**#%%%@@##%%%%%%@@@@@%#                                             ",
    "                                                          #++=+#=====+++*##%%%%#%%%@%%%@@@@%%#                                             ",
    "                                                          **+=*=--==+**#*##%%%%%%%%%@@@@@@@%%#                                             ",
    "                                                           %*++*+---==+***###%%@@%%@@@@@@@%%%#                                             ",
    "                                                            ##**+=-==+*+####%@@@@@%%@@%@%%%%%%                                             ",
    "                                                             *###***+*#*#%%@@@@@@@@@@@@%%%%%%%%                                            ",
    "                                                               ##%###*##%%%@@@@@@%@@%%%%%%%%%@@%*#**++=                                    ",
    "                                                                +%%%%%#%%%%@@@@@%#%%%%%%%%%@@@@**+++===                                    ",
    "                                                                ====*%%%%%%%####%%%%%%%%%@@@++++++++++=                                    ",
    "                                                                .==---==++**####****####%=-=====++++++++                                   ",
    "                                                              .:-=-=----=+*****++++****:---=========++=*+                                  ",
    "                                                             .:-==--=---+*****===+++:::------------*#**+--=+                               ",
    "                                                           :.::=+*--==-=+*+++=====.::-----:----:+***=-==::::-++                            ",
    "                                                          ..::-=+#=--===++=+===-.::::::::::::++*++*==:-:::::::-++=                         ",
    "                                                         .::..=-=#+=----=====::::-::::::::=+=+++=--=-:::::::-:----=+=                      ",
    "                                                       :..:.-..-=+%*=--=::-:::::::::...:--======-::=::::::::-:::---==++=                   ",
    "                                                ::-:..:....:::.:..=%*===:---:::::-:.::::--=------:::..:::::::-------=++++=                 ",
    "                                            :...:....::.............*#*.:::::---:.....::::::-:::--:..:::::--:::::---==++++++               ",
    "                                         .:....:::..::...............-.:::---...:...::..:::::::--:.:.::--::::::::---==++++++++=            ",
    "                                      :..:....:::..::................:::--.....:...:.....:::::-::.:::-::::::::::::-=+++++++++++=           ",
    "                                    :::.::...:::..::...............::::..........::........:::::..:-:::::::::::-=*+====+++++++++=          ",
    "                                  :..:..:...:::..::...............::::..........::...:...:::::...-:::::::::::-*+#---===++++*++++==         ",
    "                                 ....:.....::....:...............:..:..........:--:-+=:.:::::..:-:::..:::::=*--:@@@+++++++*++++++==        ",
    "                                ::...:...:.:....:.............................:.-==--:.:::..:..:.-......:-+:::--@@@@@=+++*@@@++++===       ",
    "                                .:........::............*......................-:++:.::::..:.:.::......:=:::----@@@@@@@@@@@@@*+++===       ",
    "                              :..:...:....%:..:=#.=#+.#+*...................-:=+-+:-.::..::.:.:......:=::---::::--=@@@@@@@@@*++++===       ",
    "                             ....:.......-%.%-@@%*%==:-@......:.............--=#+=..:...::.:.:.:...:=::--::::-==-=+#@@@@@@*++++++===       ",
    "                             .....:......:...................::............*%::*#......:-..:..:...:::-::::--:@@=++++*@=+#++*++++====       ",
    "                            .................................:............#%#%##......:-..:..:...:::..:::::-%@@@=====***+***+++=====       ",
    "                            ........:.......................::............#%%#*......:-:..:.::...:......:==-@@@@@==+*+*#@****+=+=====      ",
    "                           ...........:.....................::.............:........:-:...-.-........:---::-%@@@@@+%@@@@@***+=++=====      ",
    "                          ..........:......................:::.....................:-::..:-::.....::::::::---*@@@@@@@@@@*+++++++=+=+=      ",
    "                          .:....:.....:....................:::.....................:::...:=:..........:::::--=@@@@@@@@@==++++++=++*==      ",
    "                        ::..-.....:........................:-......................::....:=..........::--===-==@@@@+--++**++**+=+=*==      ",
    "                       ::....:.....::..:..................:::......................::....:.......:::---:-----====-:+++*******+==**+==      ",
    "                      .:..........:..:....................:::......................::..........:--:::::::------:=++********+++=++*=+=      ",
    "                     ...........-...:.:...................:::......................::.......:::::::::::::::::=+***********+++=+**=+=       ",
    "                   ..............:::.::...................:::......................:.....::..::::::::::::-++************+++++=++=++=       ",
    "                  -====:.........::::.:...................:::......................:........::::::::::=++***++**+++****+++++=+++++=        ",
    "                 =+=========....::.::::...................:::.............:.......::.....:::::::::-=++=======+==++*****+++==++++++         ",
    "                ++============+.::::::-..................:::..................::.::............::---------=-+++********++==+++*++=         ",
    "               +=============+++++::::--.................:::...........:.....:::::............:::::::---====++*******++++=+****++          ",
    "              +++===========++++++++*:-=.................:::..........:::::::::::..--==--:...:::::::----=-=++++****+++++==***+++           ",
    "             ++++==========++++++*****##::...............::..........::--::::....==========++++++**------=+++++++*+++++==***+++            ",
    "            *+++**++++=+++++++********##%::..............:-........:---:......::==========+++++***###%%*=++++***+++++==****++=             ",
    "           +++*+*#######*#####*#########-:::...........::-===++++++++-....:--:.:==========+++++***###%%%%%@@*++++*+====#****+=             ",
    "          ++++*+++**+**********#########***-:----=+++++=+=+=+++++++++++++=++=============+++++***###%%%%%@%%%@%*+++*+=##***++              ",
    "         =+++++=+++++*+*+****************+*++++++==========--==++++++++++*+++++========+++++****###%%%%@@@@%%%%%@+==+###***+               ",
    "         *+++=+===+====++=+++++**+****++++++++======+========-===+++++++*++++++========+++++**###%%%%@@@@%@@%%%%%@*=###***+                ",
    "        +++++======+========+++=++++=+++===========-======+=-=-=====+++++++++++==+===++++++**###%%%@@@@@@%%%%%%%%%@###***+                 ",
    "        ++++++++==================================-====-======-==+=====++++*++++++==++++++**###%%%%@@@@@@%%%%%%%%###****+                  ",
    "        +*++++++++=============-=====-=================---=======+*#***++++=+++++++++++++***##%%%%@@%@@@%%%%%%%#*******+                   ",
    "       ++++++++++++++==+=+=========+====+++=++++++++++***********+++++++==+++++++++++++***###%%%%@@@@@%@%%%%%%####*****                    ",
    "       +++=++++++++++++++++++++++++++++*+*++*++++++***###%%%#++++=+=============++**+++**###%%%%@@@@@@%%%%%%%#####***+=                    ",
    "       +++=+++++++++++++*+++*++**********++++++++*###%%%%%*++=+=================++***+**##%%%%@@@@@@%%%%%%%%+*##****+-                     ",
    "       *+++++++++*********#*#********+++++++++*##%%%@@%##*+++===============++=+++++++*##%%%%%@@@@@%%%%%%%#*+++++++++                      ",
    "       +**+++****+*+***********+++++++++++*##%%%@@@%#*#***++++====+==+=====+========+*##%%%%@@@@@@%%%%%%%#***+++++++                       ",
    "        ++*********************+***+*+**#%%%@@@@%#**+****+++++++=+=================+*###%%@@@@@@@%%%%%%%###******+++                       ",
    "        +***#####**********###**+***##%%%%@@@@##**++++***+++++++*+++++++++==-=-===+*##%%%@@@@@@@%%%%%%%*%%##****+++                        ",
    "         =**##%##############*****##%###@@@@%##**+*++*++++**+++**++*+++====-=====+*##%%@@@@@@@%%%%%%#*###%%#***+++=                        ",
    "                   *#%%%%###########*%%@@%%###********++**++++++*+++++++=======++**##%%@@@@@@@%%%%%%%=+#####***+++                         ",
    "                        *#####*+  +######%%%#####********************++++++=+++****#%@@@@@@@@%%%%%@%%++*#####**++                          ",
    "                                     *****###########*####**####******++++++*****##%%%%@@@@@@@%%@@%%%*+*#####**++                          ",
    "                                     ++++=----*****########%##%#####*************###%%%@@@@@@@%@@@%%%*+**####*++=                          ",
    "                                    ====---:::::-+***###############*#######**#***##%%@@@@@@@%@@@@%%%*+**###**++                           ",
    "                                   :----:::::....:=:-=**#########%#############**###%@@@@%%##%@@@%%%%*+**###**++                           ",
    "                                   :::-:::......:.....::--=+***##%#%%%%%%%%%%%%###%%@@@%@%##*#%@@%%%%*****#***+                            ",
    "                                   ::::::.........::---::::::--==+%*###%###%%%%%###%@@@@@%##*#%@@%%%%********++                            ",
    "                                  ::::::.......:----:.....::::.:=#-=**+++****######%%%%%%%##**%@@%%%%*#+*****+-                            ",
    "                                  ..:.......::----:......:::..:**:-++---==-==+***###%########*#%@%%%%*#+****++                             ",
    "                                  ..:.....::---::......::::..=#*:-==:::--::--=+***####**#####**%@%%##*#+****+-                             ",
    "                                 :..:....:--::.......::::..-+#+::=-:::::::::-=++**#***++***##**#%%%####*+***+                              ",
    "                                 :.::::::-::.......:::::.:=##=::--:::::::.::--=+**#**++=+*******%%%####*+++++                              "
};

    string[] nerd = new string[] {
    "                     :::::::::::::::                     ",
    "                :::::::::::::::::::::::::                ",
    "             :::::::::::::::::::::::::::::::             ",
    "          :::::::::::::::::::::::::::::::::::::          ",
    "        :::::::::::::::::::::::::::::::::::::::::        ",
    "       :::::::::::::::::::::::::::::::::::::::::::       ",
    "     :::::::::::::::::::::::::::::::::::::::::::::::     ",
    "    ::::*%%%%%%%%%%%%=:::::::::::::-%%%%%%%%%%%%#::::    ",
    "  %%%%%%%%%%%%%%%%%%%%%%%-::::::%%%%%%%%%%%%%%%%%%%%%%%  ",
    "  %%%%%%:::::::::+#:::%%%%%%%%%%%%%:::#*:::::::::%%%%%%  ",
    " ::::%%%::::::::#####::%%%%%-%%%%%::#####+:::::::%%%-::: ",
    " ::::%%%:::::::######+:%%%=::::%%%:-######:::::::%%%:::: ",
    ":::::%%%%::::::######*:%%%:::::%%%:-######::::::-%%%:::::",
    "::::::%%%::::::+#####:*%%%:::::%%%%:######::::::%%%+:::::",
    "::::::%%%%:::::::###:#%%%:::::::%%%%:*##:::::::%%%%::::::",
    ":::::::%%%%%%%#+*%%%%%%%:::::::::%%%%%%%*+#%%%%%%%:::::::",
    ":::::::::#%%%%%%%%%%%#:::::::::::::+%%%%%%%%%%%%:::::::::",
    ":::::::::::::::::::::::::::::::::::::::::::::::::::::::::",
    ":::::::::::::::::::::::::::::::::::::::::::::::::::::::::",
    " ::::::::::::::::::::::::::::::::::::::::::::::::::::::: ",
    " ::::::::::::+#:::::::::::::::::::::::::::#+:::::::::::: ",
    "  ::::::::::::####+:::::::::::::::::::+####::::::::::::  ",
    "   :::::::::::::########:::::::::########:::::::::::::   ",
    "    ::::::::::::::####  .#######.  ####::::::::::::::    ",
    "     ::::::::::::::::#      #      #::::::::::::::::     ",
    "       :::::::::::::::      #      :::::::::::::::       ",
    "        ::::::::::::::::    #    ::::::::::::::::        ",
    "          :::::::::::::::::::::::::::::::::::::          ",
    "             :::::::::::::::::::::::::::::::             ",
    "                :::::::::::::::::::::::::                ",
    "                     :::::::::::::::                      "
};


    private Dictionary<string, Transform> categoryContainers;

    private void Start()
    {
        ColorUtility.TryParseHtmlString("#00000064", out buttonDefaultColor);
        ColorUtility.TryParseHtmlString("#000000C8", out buttonSelectedColor);

        menusButtons = new List<Button> {
            controlsButton,
            hiddenButton,
            buyButton,
            togglesButton,
            sayButton
        };

        currentContainer = controlsPanelContainer;
        scrollRect.content = currentContainer as RectTransform;
        InitializeContainerDictionary();

        JSONLoader.LoadFile(jsonURL, (json) =>
        {
            Debug.Log("JSON recibido con �xito");
            OnJSONReceived(json);
        });

        controlsButton.onClick.AddListener(() => SwitchContainer(controlsPanelContainer, controlsButton));
        hiddenButton.onClick.AddListener(() => SwitchContainer(hiddenPanelContainer, hiddenButton));
        buyButton.onClick.AddListener(() => SwitchContainer(buyPanelContainer, buyButton));
        togglesButton.onClick.AddListener(() => SwitchContainer(togglesPanelContainer, togglesButton));
        sayButton.onClick.AddListener(() => SwitchContainer(sayPanelContainer, sayButton));
        exportBindsButton.onClick.AddListener(ExportBinds);
    }

    private void OnJSONReceived(string jsonText)
    {
        Debug.Log("Comenzando a instanciar los paneles");
        JObject root = JObject.Parse(jsonText);
        JArray binds = (JArray)root["binds"];

        foreach (JObject JObject in binds)
        {
            Bind bind = new();
            bind.name = JObject.Value<string>("name");
            bind.ingameName = JObject.Value<string>("ingameName");
            bind.description = JObject.Value<string>("description");
            bind.category = JObject.Value<string>("category");
            bind.subcategory = JObject.Value<string>("subcategory");
            bind.scancode = null;
            bind.secondScancode = null;
            bindList.Add(bind);
        }
        InitializeBindPanels();
    }

    private void InitializeBindPanels()
    {
        var groupedBinds = bindList.binds
            .GroupBy(bind => bind.category)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(bind => bind.subcategory)
                    .ToDictionary(subgroup => subgroup.Key, subgroup => subgroup.ToList())
            );

        foreach (var category in groupedBinds)
        {
            Transform targetTransform = categoryContainers.ContainsKey(category.Key.ToLower())
                ? categoryContainers[category.Key.ToLower()]
                : null;
            var subcategories = category.Value;
            foreach (var subcategory in subcategories)
            {
                var binds = subcategory.Value;
                Separator commandSeparator = Instantiate(bindSeparatorPrefab, targetTransform).GetComponent<Separator>();
                commandSeparator.SetName($"{category.Key.ToUpper()} - {subcategory.Key}");
                foreach (var bind in binds)
                {
                    if((category.Key != "TOGGLES") && (category.Key != "SAY"))
                    {
                        BindPanel bindPanel = Instantiate(bindPanelPrefab, targetTransform).GetComponent<BindPanel>();
                        bindPanel.SetBind(bind);
                        bindPanels.Add(bindPanel);
                    } else
                    {
                        TogglePanel togglePanel = Instantiate(togglePanelPrefab, targetTransform).GetComponent<TogglePanel>();
                        togglePanel.SetBind(bind);
                        togglePanels.Add(togglePanel);
                    }
                }
            }
        }

        string filePath = Path.Combine(Application.persistentDataPath, "binds.txt");
        Debug.Log(filePath);
        if (!File.Exists(filePath))
            return;

        string[] lines = File.ReadAllLines(filePath);

        foreach(BindPanel bindPanel in bindPanels)
        {
            bindPanel.LoadBind(lines);
        }

        foreach(TogglePanel togglePanel in togglePanels)
        {
            togglePanel.LoadBind(lines);
        }
    }

    // Switches the current container to a new one and updates the button colors
    private void SwitchContainer(Transform newContainer, Button button)
    {
        foreach (Button btn in menusButtons)
        {
            cb = btn.colors;

            cb.normalColor = buttonDefaultColor;
            cb.highlightedColor = buttonDefaultColor;
            cb.pressedColor = buttonDefaultColor;
            cb.selectedColor = buttonDefaultColor;
            cb.disabledColor = buttonDefaultColor;

            btn.colors = cb;
        }

        // Ahora aplicar los colores "activos" al bot�n seleccionado
        selectedCB = button.colors;

        selectedCB.normalColor = buttonSelectedColor;
        selectedCB.highlightedColor = buttonSelectedColor;
        selectedCB.pressedColor = buttonSelectedColor;
        selectedCB.selectedColor = buttonSelectedColor;
        selectedCB.disabledColor = buttonSelectedColor;

        button.colors = selectedCB;

        currentContainer.gameObject.SetActive(false);
        newContainer.gameObject.SetActive(true);

        scrollRect.content = newContainer as RectTransform;
        currentContainer = newContainer;
    }

    // Initializes the dictionary that maps category names to their respective containers
    private void InitializeContainerDictionary()
    {
        categoryContainers = new Dictionary<string, Transform>
            {
                { "controls", controlsPanelContainer },
                { "hidden", hiddenPanelContainer },
                { "buy", buyPanelContainer },
                { "toggles", togglesPanelContainer },
                { "say", sayPanelContainer }
            };

    }

    // Save the binds into txt file in the persistent data path (appdata)
    private void SaveBinds()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "binds.txt");

        Debug.Log("Created file: " + filePath);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Bind bind in bindList.binds)
            {
                if(bind.scancode != null)
                {
                    writer.WriteLine($"{bind.name}|{bind.localKey}|{bind.americanKey}|{bind.scancode}" + (bind.values != null ? $"|{bind.values}" : ""));
                }   
                if (bind.secondScancode != null)
                {
                    writer.WriteLine($"{bind.name}|{bind.secondLocalKey}|{bind.secondAmericanKey}|{bind.secondScancode}");
                }  
            }
        }
    }

    private string SelectFolder()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);
        if (paths.Length > 0)
        {
            string folderPath = paths[0];
            Debug.Log("Selected folder: " + folderPath);
            return folderPath;
        }
        return null;
    }

    private void ExportBinds()
    {
        // Also save the binds
        SaveBinds();

        string filePath = SelectFolder();
        filePath = Path.Combine(filePath, "binds.cfg");

        // Create an scancode Dictionary:

        var bindsByScancode = new Dictionary<string, List<Bind>>();

        foreach (var bind in bindList.binds)
        {
            if (string.IsNullOrEmpty(bind.scancode))
                continue;

            if (!bindsByScancode.ContainsKey(bind.scancode))
                bindsByScancode[bind.scancode] = new List<Bind>();

            bindsByScancode[bind.scancode].Add(bind);
        }

        // Divide binds between unique scancodes and shared scancodes
        // For example bind "MWHEELUP" "+jump" and "SPACE" "+jump" would be unique binds
        var uniqueBinds = new List<Bind>();
        var multiBinds = new Dictionary<string, List<Bind>>();

        foreach (var pair in bindsByScancode)
        {
            if (pair.Value.Count == 1)
                uniqueBinds.Add(pair.Value[0]);
            else
                multiBinds[pair.Key] = pair.Value;
        }

        var groupedUniqueBinds = uniqueBinds
            .GroupBy(bind => bind.category)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(bind => bind.subcategory)
                    .ToDictionary(subgroup => subgroup.Key, subgroup => subgroup.ToList())
            );

        // Get the biggest command to align the comments
        int maxLeftWidthMulti = 0;
        int maxLeftWidthUnique = 0;

        if(multiBinds.Count > 0)
        {
            maxLeftWidthMulti = multiBinds
            .Select(pair =>
            {
                string scancode = pair.Key;
                string actions = string.Join("; ", pair.Value.Select(b => b.name));
                return $"bind \"{scancode}\" \"{actions}\"".Length;
            })
            .Max();
        }

        maxLeftWidthUnique = uniqueBinds
            .Select(bind =>
            {
                // Checks if it's a toggle command because it adds "toggle " at the beginning
                bool usesToggle = bind.category == "TOGGLES";
                string command = usesToggle ? "toggle " : "";
                
                if(usesToggle)
                    bind.name = "toggle " + bind.name;

                string values = bind.values?.Trim();
                string space = string.IsNullOrWhiteSpace(values) ? "" : " ";
                string line = $"bind \"{bind.scancode}\" \"{command}{bind.name}{space}{values}\"";
                Debug.Log($"'{line}' ({line.Length})");
                return line.Length;
            })
            .Max();

        Debug.Log($"maxunique: {maxLeftWidthUnique}, maxMulti: {maxLeftWidthMulti}");

        int maxLeftWidth = Math.Max(maxLeftWidthMulti, maxLeftWidthUnique) + 4;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string line in csLogoASCII)
                writer.WriteLine($"echo \"{line}\"");

            writer.WriteLine();
            writer.WriteLine("//========== YOU SHOULD NOT CHANGE THESE ==========");
            if(MenuManager.Instance.addUnbindall) writer.WriteLine("unbindall");
            writer.WriteLine("bind \"MOUSE1\" \"+attack\"");
            writer.WriteLine("bind \"MOUSE_X\" \"yaw\"");
            writer.WriteLine("bind \"MOUSE_Y\" \"pitch\"");

            // Export unique binds ordenados
            foreach (var categoryPair in groupedUniqueBinds)
            {
                var subcategories = categoryPair.Value;
                foreach (var subcategory in subcategories)
                {
                    writer.WriteLine();
                    writer.WriteLine($"//========== {categoryPair.Key.ToUpper()} - {subcategory.Key} ==========");

                    foreach (var bind in subcategory.Value)
                    {
                        if (string.IsNullOrEmpty(bind.secondScancode))
                        {
                            if(string.IsNullOrEmpty(bind.values)) // If it's a command without values (not say,say_team,toggle...)
                            {
                                string left = $"bind \"{bind.scancode}\" \"{bind.name}\"".PadRight(maxLeftWidth + 4);
                                writer.WriteLine(left + $"// {bind.localKey} ({bind.americanKey}) -> {bind.ingameName}");
                            } else if (bind.name.StartsWith("say_team")) // If the bind is a say_team command
                            {
                                string left = $"bind \"{bind.scancode}\" \"say_team {bind.values}\"".PadRight(maxLeftWidth + 4);
                                writer.WriteLine(left + $"// {bind.localKey} ({bind.americanKey}) -> {bind.ingameName}");
                            } else if (bind.name.StartsWith("say")) // If the bind is a say command
                            {
                                string left = $"bind \"{bind.scancode}\" \"say {bind.values}\"".PadRight(maxLeftWidth + 4);
                                writer.WriteLine(left + $"// {bind.localKey} ({bind.americanKey}) -> {bind.ingameName}");
                            } else // If the bind is just a toggle command with values
                            {
                                string left = $"bind \"{bind.scancode}\" \"{bind.name} {bind.values}\"".PadRight(maxLeftWidth + 4);
                                writer.WriteLine(left + $"// {bind.localKey} ({bind.americanKey}) -> {bind.ingameName}");
                            }
                        } else
                        {
                            string left = $"bind \"{bind.scancode}\" \"{bind.name}\"".PadRight(maxLeftWidth + 4);
                            writer.WriteLine(left + $"// {bind.localKey} ({bind.americanKey}) -> {bind.ingameName}");
                            string extraLeft = $"bind \"{bind.secondScancode}\" \"{bind.name}\"".PadRight(maxLeftWidth + 4);
                            writer.WriteLine(extraLeft + $"// {bind.localKey} ({bind.americanKey}) -> {bind.ingameName}");
                        }
                    }
                }
            }

            // Export multi-binds
            if (multiBinds.Count > 0)
            {
                writer.WriteLine();
                writer.WriteLine("//========== MULTI-BINDS ==========");

                foreach (var pair in multiBinds)
                {
                    string actions = string.Join("; ", pair.Value.Select(b =>
                    {
                        if (string.IsNullOrEmpty(b.values))
                            return b.name;
                        else if (b.name.StartsWith("say_team"))
                        {
                            string editedName = "say_team";
                            return editedName + " " + b.values;
                        } else if (b.name.StartsWith("say"))
                        {
                            string editedName = "say";
                            return editedName + " " + b.values;
                        } else 
                        {
                            return $"{b.name} {b.values}";
                        }
                    }
                    ));
                    
                    Bind example = pair.Value[0]; // para mostrar info auxiliar

                    string left = $"bind \"{pair.Key}\" \"{actions}\"".PadRight(maxLeftWidth + 4);
                    writer.WriteLine(left + $"// {example.localKey} ({example.americanKey}) -> MULTI: {string.Join(", ", pair.Value.Select(b => b.ingameName))}");
                }
            }
        }
    }
}
