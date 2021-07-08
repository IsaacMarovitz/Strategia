using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class DebugConsole : MonoBehaviour {

    public bool showConsole = false;
    public GameObject windowPanel;
    public TMP_Text consoleText;
    public TMP_InputField consoleInput;
    public GameObject autocompleteMenu;
    public GameObject autocompleteCommandPrefab;

    public DebugCommandAttribute[] commandList;
    public MethodInfo[] methods;
    public Trie commandTrie;

    private List<DebugCommandAttribute> results;
    private bool isAutocompleteShowing = false;
    public List<string> commandHistory = new List<string>();
    public int currentCommandHistoryIndex = -1;

    public void Awake() {        
        methods = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttributes(typeof(DebugCommandAttribute), false).Length > 0)
            .ToArray();

        commandList = new DebugCommandAttribute[methods.Length];
        
        for (int i = 0; i < methods.Length; i++) {
            commandList[i] = (DebugCommandAttribute)System.Attribute.GetCustomAttribute(methods[i], typeof(DebugCommandAttribute));
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
        for (int i = 0; i < commandList.Length; i++) {
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
        currentCommandHistoryIndex = -1;
        PrintString(input);
        commandHistory.Insert(0, input);

        bool commandExecuted = false;
        for (int i = 0; i < commandList.Length; i++) {
            if (properties[0] == commandList[i].commandId) {
                object[] parameters = new object[] {properties.Skip(1).ToArray(), this};
                commandExecuted = (bool)methods[i].Invoke(null, parameters);
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
        public DebugCommandAttribute debugCommand;

        public TrieNode() {
            children = new Dictionary<char, TrieNode>();
        }
    };

    static TrieNode root;

    public Trie(DebugCommandAttribute[] commands) {
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
