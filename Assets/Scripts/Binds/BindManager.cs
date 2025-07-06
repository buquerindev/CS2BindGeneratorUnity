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
    private readonly string jsonURL = "https://buquerindev.github.io/CS2BindGeneratorUnity/binds.json";

    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject bindSeparatorPrefab;
    [SerializeField] private GameObject bindPanelPrefab;
    [SerializeField] private GameObject togglePanelPrefab;

    [SerializeField] private Transform controlsPanelContainer;
    [SerializeField] private Transform hiddenPanelContainer;
    [SerializeField] private Transform buyPanelContainer;
    [SerializeField] private Transform togglesPanelContainer;
    private Transform currentContainer;

    [SerializeField] private Button controlsButton;
    [SerializeField] private Button hiddenButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button togglesButton;
    [SerializeField] private Button exportBindsButton;

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
        currentContainer = controlsPanelContainer;
        scrollRect.content = currentContainer as RectTransform;
        InitializeContainerDictionary();

        JSONLoader.LoadFile(jsonURL, OnJSONReceived);

        controlsButton.onClick.AddListener(() => SwitchContainer(controlsPanelContainer));
        hiddenButton.onClick.AddListener(() => SwitchContainer(hiddenPanelContainer));
        buyButton.onClick.AddListener(() => SwitchContainer(buyPanelContainer));
        togglesButton.onClick.AddListener(() => SwitchContainer(togglesPanelContainer));
        exportBindsButton.onClick.AddListener(ExportBinds);
    }

    private void OnJSONReceived(string jsonText)
    {
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
                    if(category.Key != "TOGGLES")
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

    private void SwitchContainer(Transform newContainer)
    {
        currentContainer.gameObject.SetActive(false);
        newContainer.gameObject.SetActive(true);

        scrollRect.content = newContainer as RectTransform;
        currentContainer = newContainer;
    }

    private void InitializeContainerDictionary()
    {
        categoryContainers = new Dictionary<string, Transform>
            {
                { "controls", controlsPanelContainer },
                { "hidden", hiddenPanelContainer },
                { "buy", buyPanelContainer },
                { "toggles", togglesPanelContainer }
            };

    }

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
                    Debug.Log($"El primer scancode de {bind.name} es {bind.scancode}");
                    writer.WriteLine($"{bind.name}|{bind.localKey}|{bind.americanKey}|{bind.scancode}" + (bind.values != null ? $"|{bind.values}" : ""));
                }   
                if (bind.secondScancode != null)
                {
                    Debug.Log($"El segundo scancode de {bind.name} es {bind.secondScancode}");
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

        // Also save the binds
        SaveBinds();

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
                return $"bind {scancode} {actions}".Length;
            })
            .Max();
        }

        maxLeftWidthUnique = uniqueBinds
            .Select(bind => $"bind {bind.scancode} {bind.name}".Length)
            .Max();

        int maxLeftWidth = Math.Max(maxLeftWidthMulti, maxLeftWidthUnique) + 4;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string line in csLogoASCII)
                writer.WriteLine($"echo \"{line}\"");

            writer.WriteLine();
            writer.WriteLine("//========== YOU SHOULD NOT CHANGE THESE ==========");
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
                            string left = $"bind \"{bind.scancode}\" \"{bind.name}\"".PadRight(maxLeftWidth + 4);
                            writer.WriteLine(left + $"// {bind.localKey} ({bind.americanKey}) -> {bind.ingameName}");
                        } else
                        {
                            Debug.Log($"{bind.name} su second scancode es = {bind.secondScancode}");
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
                    string actions = string.Join("; ", pair.Value.Select(b => b.name));
                    Bind example = pair.Value[0]; // para mostrar info auxiliar

                    string left = $"bind \"{pair.Key}\" \"{actions}\"".PadRight(maxLeftWidth + 4);
                    writer.WriteLine(left + $"// {example.localKey} ({example.americanKey}) -> MULTI: {string.Join(", ", pair.Value.Select(b => b.ingameName))}");
                }
            }
        }
    }
}
