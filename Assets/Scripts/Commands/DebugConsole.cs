using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public class DebugConsole : MonoBehaviour {

    public bool showConsole = false;
    public DragWindow debugWindowPanel;
    public TMP_Text consoleText;
    public TMP_InputField consoleInput;
    public GameObject autocompleteMenu;
    public GameObject autocompleteCommandPrefab;

    public List<DebugCommandAttribute> commandList = new List<DebugCommandAttribute>();
    public List<MethodInfo> methods = new List<MethodInfo>();
    public Trie commandTrie;

    private List<DebugCommandAttribute> results;
    private bool isAutocompleteShowing = false;
    private List<string> commandHistory = new List<string>();
    private int currentCommandHistoryIndex = -1;

    public enum DebugCommandCode { CommandNotFound, MissingParameters, ParameterOutOfRange, ParameterFailedParse, Success };

    public void Awake() {
        MethodInfo[] methodInfos = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .SelectMany(t => t.GetMethods())
            .ToArray();

        foreach (MethodInfo method in methodInfos) {
            if (method.CustomAttributes.ToArray().Length > 0) {
                DebugCommandAttribute attribute = method.GetCustomAttribute<DebugCommandAttribute>();
                if (attribute != null) {
                    methods.Add(method);
                    commandList.Add(attribute);
                }
            }
        }

        commandTrie = new Trie(commandList);
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
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (showConsole && consoleInput.isActiveAndEnabled && commandHistory.Count > 0) {
                currentCommandHistoryIndex++;
                if (currentCommandHistoryIndex > commandHistory.Count - 1) {
                    currentCommandHistoryIndex = commandHistory.Count - 1;
                } else if (currentCommandHistoryIndex < 0) {
                    currentCommandHistoryIndex = 0;
                }
                consoleInput.text = commandHistory[currentCommandHistoryIndex];
                consoleInput.MoveToEndOfLine(false, false);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (showConsole && consoleInput.isActiveAndEnabled && commandHistory.Count > 0) {
                currentCommandHistoryIndex--;
                if (currentCommandHistoryIndex < 0) {
                    consoleInput.text = "";
                    currentCommandHistoryIndex = -1;
                } else {
                    consoleInput.text = commandHistory[currentCommandHistoryIndex];
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
                instantiatedAutoCompleteCommand.GetComponentInChildren<TMP_Text>().text = results[i].commandFormat;
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
            PrintString($"<color=green>{commandList[i].commandFormat}</color> - {commandList[i].commandDescription}");
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
        showConsole = true;
        debugWindowPanel.Open(() => {
            consoleInput.Select();
            consoleInput.ActivateInputField();
            Pause();
        });
    }

    public void Close() {
        showConsole = false;
        debugWindowPanel.Close(() => {
            Resume();
        });
    }

    public void HandleInput(string input) {
        if (input == "") { return; }

        string[] properties = input.Split(' ');
        currentCommandHistoryIndex = -1;
        PrintString(input);
        commandHistory.Insert(0, input);

        DebugCommandCode commandCode = DebugCommandCode.CommandNotFound;
        for (int i = 0; i < commandList.Count; i++) {
            if (properties[0] == commandList[i].commandId) {
                object[] parameters = new object[] { properties.Skip(1).ToArray(), this };
                commandCode = (DebugCommandCode)methods[i].Invoke(null, parameters);
            }
        }

        switch (commandCode) {
            case DebugCommandCode.CommandNotFound:
                PrintError($"Command '{consoleInput.text}' was not found!");
                Debug.Log($"<b>Debug Console:</b> Failed to execute command: '{consoleInput.text}' Command not found");
                break;
            case DebugCommandCode.MissingParameters:
                PrintError($"Missing arguments!");
                Debug.Log($"<b>Debug Console:</b> Failed to execute command: '{consoleInput.text}' Command missing arguments");
                break;
            case DebugCommandCode.ParameterFailedParse:
                PrintError($"Invalid arguments! Refer to 'help' command for proper usage.");
                Debug.Log($"<b>Debug Console:</b> Failed to execute command: '{consoleInput.text}' Invalid arguments");
                break;
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
        public DebugCommandAttribute debugCommand;

        public TrieNode() {
            children = new Dictionary<char, TrieNode>();
        }
    };

    static TrieNode root;

    public Trie(List<DebugCommandAttribute> commands) {
        root = new TrieNode();
        foreach (var command in commands) {
            Insert(command);
        }
    }

    static void Insert(DebugCommandAttribute command) {
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

    static List<DebugCommandAttribute> Collect(TrieNode node, string prefix) {
        if (node == null) { return new List<DebugCommandAttribute>(); }

        List<DebugCommandAttribute> results = new List<DebugCommandAttribute>();
        if (node.isEndOfWord) {
            results.Add(node.debugCommand);
        }
        foreach (var child in node.children) {
            string newPrefix = prefix + child.Key.ToString();
            results.AddRange(Collect(child.Value, newPrefix));
        }

        return results;
    }

    public List<DebugCommandAttribute> FindKeysByPrefix(string prefix) {
        List<DebugCommandAttribute> results = new List<DebugCommandAttribute>();
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
