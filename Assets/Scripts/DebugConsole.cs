using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour {

    public bool showConsole = false;
    public GameObject windowPanel;
    public TMP_Text consoleText;
    public TMP_InputField consoleInput;
    public GameObject autocompleteMenu;
    public GameObject autocompleteCommandPrefab;

    public static DebugCommand<bool> CLEAR_FOG;
    public static DebugCommand CLEAR;
    public static DebugCommand<int> TEST_WITH_NUM;
    public static DebugCommand<bool> FAST_PROD;
    public static DebugCommand HELP;

    public List<object> commandList;
    public Trie commandTrie;

    private List<DebugCommandBase> results;
    private bool isAutocompleteShowing = false;

    public void Awake() {
        CLEAR_FOG = new DebugCommand<bool>("clear_fog", "Clears the fog of war for the current player.", "clear_fog <bool>", false, (x) => {
            GameManager.Instance.GetCurrentPlayer().RevealAllTiles(x);
            if (x) {
                PrintSuccess($"Fog of War for Player {GameManager.Instance.currentPlayerIndex} cleared.");
            } else {
                PrintSuccess($"Re-enabled Fog of War for Player {GameManager.Instance.currentPlayerIndex}.");
            }
        });
        CLEAR = new DebugCommand("clear", "Clears the console.", "clear", false, () => {
            ClearConsole();
        });
        TEST_WITH_NUM = new DebugCommand<int>("test_with_num", "Test the debug console with num.", "test_with_num <num>", false, (x) => {
            PrintSuccess($"Test Num: {x}");
        });
        FAST_PROD = new DebugCommand<bool>("fast_prod", "Changes all unit TTCs to 1 day.", "fast_prod <bool>", false, (x) => {
            GameManager.Instance.fastProd = x;
            if (x) {
                PrintSuccess("Set all unit TTCs to 1.");
            } else {
                PrintSuccess("Reset all unit TTCs.");
            }
        });
        HELP = new DebugCommand("help", "Shows list of available commands.", "help", false, () => {
            PrintHelp();
        });

        commandList = new List<object> {
            CLEAR_FOG,
            CLEAR,
            TEST_WITH_NUM,
            FAST_PROD,
            HELP,
        };

        List<DebugCommandBase> debugCommandList = new List<DebugCommandBase>();
        foreach (var command in commandList) {
            debugCommandList.Add(command as DebugCommandBase);
        }
        commandTrie = new Trie(debugCommandList);
    }

    public void Start() {
        consoleInput.onSubmit.AddListener(HandleInput);
        consoleInput.onSelect.AddListener((string x) => Pause());
        consoleInput.onDeselect.AddListener((string x) => Resume());
        consoleInput.onValueChanged.AddListener(UpdateAutocomplete);
        consoleText.text = "";
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            if (showConsole) {
                showConsole = false;
                Close();
            } else {
                showConsole = true;
                Open();
            }
            consoleInput.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (isAutocompleteShowing) {
                if (results.Count > 0) {
                    consoleInput.text = results[0].commandId;
                    consoleInput.MoveToEndOfLine(false, false);
                }
            }
        }
    }

    public void UpdateAutocomplete(string value) {
        if (value == "") {
            foreach (Transform child in autocompleteMenu.transform) {
                GameObject.Destroy(child.gameObject);
            }
            autocompleteMenu.SetActive(false);
            isAutocompleteShowing = false;
            return;
        }

        results = commandTrie.FindKeysByPrefix(value);
        if (results.Count > 0) {
            isAutocompleteShowing = true;
            autocompleteMenu.SetActive(true);
            foreach (Transform child in autocompleteMenu.transform) {
                GameObject.Destroy(child.gameObject);
            }
            for (int i = 0; i < results.Count; i++) {
                GameObject instantiatedAutoCompleteCommand = GameObject.Instantiate(autocompleteCommandPrefab, Vector3.zero, Quaternion.identity);
                instantiatedAutoCompleteCommand.transform.SetParent(autocompleteMenu.transform);
                instantiatedAutoCompleteCommand.transform.localScale = Vector3.one;
                instantiatedAutoCompleteCommand.GetComponentInChildren<TMP_Text>().text = results[i].commmandFormat;
            }
        } else {
            foreach (Transform child in autocompleteMenu.transform) {
                GameObject.Destroy(child.gameObject);
            }
            autocompleteMenu.SetActive(false);
            isAutocompleteShowing = false;
        }
    }

    public void ClearConsole() {
        consoleText.text = "";
    }

    public void PrintHelp() {
        for (int i = 0; i < commandList.Count; i++) {
            DebugCommandBase command = commandList[i] as DebugCommandBase;
            PrintString($"<color=green>{command.commmandFormat}</color> - {command.commandDescription}");
        }
    }

    public void PrintString(string value) {
        consoleText.text += $"\n{value}";
    }

    public void PrintError(string value) {
        consoleText.text += $"\n<color=red>Error: {value}</color>";
    }

    public void PrintSuccess(string value) {
        consoleText.text += $"\n<color=green>{value}</color>";
    }

    public void Pause() {
        GameManager.Instance.Pause();
    }

    public void Resume() {
        GameManager.Instance.Resume();
    }

    public void Open() {
        windowPanel.SetActive(true);
        showConsole = true;
        consoleInput.Select();
        consoleInput.ActivateInputField();
        Pause();
    }

    public void Close() {
        Resume();
        windowPanel.SetActive(false);
        showConsole = false;
    }

    public void HandleInput(string input) {
        if (input == "") { return; }

        string[] properties = input.Split(' ');

        bool commandExecuted = false;
        for (int i = 0; i < commandList.Count; i++) {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (properties[0] == (commandList[i] as DebugCommandBase).commandId) {
                if (commandList[i] is DebugCommand command) {
                    PrintString(consoleInput.text);
                    command.Invoke();
                    if (command.printCommandCompleteMsg) {
                        PrintSuccess($"Executed Command: {consoleInput.text}");
                    }
                    Debug.Log($"<b>Debug Console:</b> Executed command '{consoleInput.text}'");
                    commandExecuted = true;
                } else if (properties.Length > 1) {
                    if (commandList[i] is DebugCommand<int> commandInt) {
                        int inputInt;
                        if (int.TryParse(properties[1], out inputInt)) {
                            PrintString(consoleInput.text);
                            commandInt.Invoke(inputInt);
                            if (commandInt.printCommandCompleteMsg) {
                                PrintSuccess($"Executed Command: {consoleInput.text}");
                            }
                            Debug.Log($"<b>Debug Console:</b> Executed command '{consoleInput.text}'");
                            commandExecuted = true;
                        } else {
                            PrintError($"Failed to parse int '{properties[1]}'!");
                            Debug.Log($"<b>Debug Console:</b> Failed to parse int '{properties[1]}'");
                        }
                    } else if (commandList[i] is DebugCommand<bool> commandBool) {
                        bool inputBool;
                        if (bool.TryParse(properties[1], out inputBool)) {
                            PrintString(consoleInput.text);
                            commandBool.Invoke(inputBool);
                            if (commandBool.printCommandCompleteMsg) {
                                PrintSuccess($"Exectued Command: {consoleInput.text}");
                            }
                            Debug.Log($"<b>Debug Console:</b> Executed command '{consoleInput.text}'");
                            commandExecuted = true;
                        } else {
                            PrintError($"Failed to parse bool '{properties[1]}'!");
                            Debug.Log($"<b>Debug Console:</b> Failed to parse bool '{properties[1]}'");
                        }
                    }
                } else {
                    PrintError("Missing parameters!");
                    Debug.Log("<b>Debug Console:</b> Missing parameters!");
                }
            }
        }

        if (!commandExecuted) {
            PrintError($"Command '{consoleInput.text}' failed to execute or was not found!");
            Debug.Log($"<b>Debug Console:</b> Failed to execute command '{consoleInput.text}'");
        }
        consoleInput.text = "";
        consoleInput.Select();
        consoleInput.ActivateInputField();
        Pause();
    }
}

public class Trie {
    class TrieNode {
        public Dictionary<char, TrieNode> children;
        public bool isEndOfWord;
        public DebugCommandBase debugCommand;

        public TrieNode() {
            children = new Dictionary<char, TrieNode>();
        }
    };

    static TrieNode root;

    public Trie(List<DebugCommandBase> commands) {
        root = new TrieNode();
        foreach (var command in commands) {
            Insert(command);
        }
    }

    static void Insert(DebugCommandBase command) {
        TrieNode pCrawl = root;
        string word = command.commandId;

        for (int i = 0; i < word.Length; i++) {
            char key = word[i];
            if (!pCrawl.children.ContainsKey(key)) {
                pCrawl.children.Add(key, new TrieNode());
            }
            pCrawl = pCrawl.children[key];
        }

        pCrawl.isEndOfWord = true;
        pCrawl.debugCommand = command;
    }

    static TrieNode GetNode(string word) {
        TrieNode pCrawl = root;

        for (int i = 0; i < word.Length; i++) {
            char key = word[i];
            if (!pCrawl.children.ContainsKey(key)) {
                return null;
            }

            pCrawl = pCrawl.children[key];
        }

        return pCrawl;
    }

    static List<DebugCommandBase> Collect(TrieNode node, string prefix) {
        if (node == null) { return new List<DebugCommandBase>(); }

        List<DebugCommandBase> results = new List<DebugCommandBase>();
        if (node.isEndOfWord) {
            results.Add(node.debugCommand);
        }
        foreach (var child in node.children) {
            string newPrefix = prefix + child.Key.ToString();
            results.AddRange(Collect(child.Value, newPrefix));
        }

        return results;
    }

    public List<DebugCommandBase> FindKeysByPrefix(string prefix) {
        List<DebugCommandBase> results = new List<DebugCommandBase>();
        TrieNode node = GetNode(prefix);
        results = Collect(node, prefix);
        foreach (var command in results) {
            if (command.commandId == prefix) {
                results.Remove(command);
                break;
            }
        }
        return results;
    }
}