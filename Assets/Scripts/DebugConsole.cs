using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour {

    public bool showConsole = false;
    public GameObject windowPanel;
    public TMP_Text consoleText;
    public TMP_InputField consoleInput;

    public static DebugCommand CLEAR_FOG;
    public static DebugCommand CLEAR;
    public static DebugCommand<int> TEST_WITH_NUM;
    public static DebugCommand HELP;

    public List<object> commandList;
    public Trie commandTrie;

    public void Awake() {
        CLEAR_FOG = new DebugCommand("clear_fog", "Clears the fog of war for the current player.", "clear_fog", true, () => {
            GameManager.Instance.GetCurrentPlayer().RevealAllTiles();
        });
        CLEAR = new DebugCommand("clear", "Clears the console.", "clear", false, () => {
            ClearConsole();
        });
        TEST_WITH_NUM = new DebugCommand<int>("test_with_num", "Test the debug console with num.", "test_with_num <num>", false, (x) => {
            PrintString($"Test Num: {x}");
        });
        HELP = new DebugCommand("help", "Shows list of available commands.", "help", false, () => {
            PrintHelp();
        });

        commandList = new List<object> {
            CLEAR_FOG,
            CLEAR,
            TEST_WITH_NUM,
            HELP
        };

        commandTrie = new Trie(new List<string> {
            CLEAR_FOG.commandId,
            CLEAR.commandId,
            TEST_WITH_NUM.commandId,
            HELP.commandId
        });
    }

    public void Start() {
        consoleInput.onEndEdit.AddListener(HandleInput);
        consoleInput.onSelect.AddListener(Pause);
        consoleInput.onDeselect.AddListener(Resume);
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

        List<string> results = commandTrie.FindKeysByPrefix(consoleInput.text);
    }

    public void ClearConsole(){
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

    public void Pause(string value) {
        GameManager.Instance.Pause();
    }

    public void Resume(string value) {
        GameManager.Instance.Resume();
    }

    public void Open() {
        windowPanel.SetActive(true);
        showConsole = true;
    }

    public void Close() {
        Resume("");
        windowPanel.SetActive(false);
        showConsole = false;
    }

    public void HandleInput(string input) {
        string[] properties = input.Split(' ');

        bool commandExecuted = false;
        for (int i = 0; i < commandList.Count; i++) {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (properties[0] == (commandList[i] as DebugCommandBase).commandId) {
                if (commandList[i] is DebugCommand command) {
                    command.Invoke();
                    if (command.printCommandCompleteMsg) {
                        PrintString($"Executed Command: {consoleInput.text}");
                    }
                    Debug.Log($"<b>Debug Console:</b> Executed command '{consoleInput.text}'");
                    commandExecuted = true;
                } else if (commandList[i] is DebugCommand<int> commandInt) {
                    commandInt.Invoke(int.Parse(properties[1]));
                    if (commandInt.printCommandCompleteMsg) {
                        PrintString($"Executed Command: {consoleInput.text}");
                    }
                    Debug.Log($"<b>Debug Console:</b> Executed command '{consoleInput.text}'");
                    commandExecuted = true;
                }
            }
        }

        if (!commandExecuted) {
            PrintString($"<color=red>Error: Command '{consoleInput.text}' not found!</color>");
            Debug.Log($"<b>Debug Console:</b> Failed to execute command '{consoleInput.text}'");
        }
        consoleInput.text = "";
    }
}

public class Trie {
    class TrieNode {
        public Dictionary<char, TrieNode> children;
        public string value;
        public bool isEndOfWord;

        public TrieNode(string value) {
            this.value = value;
            children = new Dictionary<char, TrieNode>();
        }
    };

    static TrieNode root;

    public Trie(List<string> words) {
        root = new TrieNode("");
        foreach (var word in words) {
            Insert(word);
        }
    }

    static void Insert(string word) {
        TrieNode pCrawl = root;
        string value = "";

        for (int i = 0; i < word.Length; i++) {
            char key = word[i];
            value += key.ToString();
            if (!pCrawl.children.ContainsKey(key)) {
                pCrawl.children.Add(key, new TrieNode(value));
            } 
            pCrawl = pCrawl.children[key];
        }

        pCrawl.isEndOfWord = true;
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

    static List<string> Collect(TrieNode node, string prefix) {
        if (node == null) { return new List<string>(); }

        List<string> results = new List<string>();
        if (node.isEndOfWord) {
            results.Add(node.value);
        }
        foreach (var child in node.children) {
            string newPrefix = prefix + child.Key.ToString();
            results.AddRange(Collect(child.Value, newPrefix));
        }

        return results;
    }
    
    public List<string> FindKeysByPrefix(string prefix) {
        List<string> results = new List<string>();
        TrieNode node = GetNode(prefix);
        results = Collect(node, prefix);
        return results;
    }
}