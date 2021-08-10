using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

    public bool showSettings = true;
    public bool showPlayerSettings = true;
    public bool showGameInfo = true;
    public bool showUIData = true;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        GameManager gameManager = (GameManager)target;

        SerializedProperty tileGrid_prop = serializedObject.FindProperty("tileGrid");
        SerializedProperty cameraController_prop = serializedObject.FindProperty("cameraController");
        SerializedProperty gameInfo_prop = serializedObject.FindProperty("gameInfo");
        SerializedProperty unitInfo_prop = serializedObject.FindProperty("unitInfo");
        SerializedProperty fogOfWarRenderer_prop = serializedObject.FindProperty("fogOfWarRenderer");

        SerializedProperty numberOfPlayers_prop = serializedObject.FindProperty("numberOfPlayers");
        SerializedProperty playerPrefab_prop = serializedObject.FindProperty("playerPrefab");
        SerializedProperty playerParent_prop = serializedObject.FindProperty("playerParent");

        SerializedProperty day_prop = serializedObject.FindProperty("day");
        SerializedProperty currentPlayerIndex_prop = serializedObject.FindProperty("currentPlayerIndex");
        SerializedProperty playerList_prop = serializedObject.FindProperty("playerList");
        SerializedProperty dayCompleted_prop = serializedObject.FindProperty("dayCompleted");

        SerializedProperty currentCity_prop = serializedObject.FindProperty("currentCity");
        SerializedProperty currentUnit_prop = serializedObject.FindProperty("currentUnit");
        SerializedProperty mouseOverTile_prop = serializedObject.FindProperty("mouseOverTile");

        if (gameManager.gameInfo == null) {
            EditorGUILayout.HelpBox("Game Info is not set! 'Game Mode' and 'Number of Players' will not show.", MessageType.Warning);
        }

        if (gameManager.tileGrid == null) {
            EditorGUILayout.HelpBox("Tile Grid is not set! 'Number of Players' will not show.", MessageType.Warning);
        }

        showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSettings, "Settings");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (showSettings) {
            if (gameManager.gameInfo != null) {
                if (EditorApplication.isPlayingOrWillChangePlaymode) {
                    EditorGUI.BeginDisabledGroup(true);
                    gameManager.gameInfo.gameMode = (GameMode)EditorGUILayout.EnumPopup("Game Mode", gameManager.gameInfo.gameMode);
                    EditorGUI.EndDisabledGroup();
                } else {
                    gameManager.gameInfo.gameMode = (GameMode)EditorGUILayout.EnumPopup("Game Mode", gameManager.gameInfo.gameMode);
                }
            }
            EditorGUILayout.PropertyField(tileGrid_prop);
            EditorGUILayout.PropertyField(cameraController_prop);
            EditorGUILayout.PropertyField(gameInfo_prop);
            EditorGUILayout.PropertyField(unitInfo_prop);
            EditorGUILayout.PropertyField(fogOfWarRenderer_prop);
        }
        EditorGUILayout.Space(10);

        showPlayerSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showPlayerSettings, "Player Settings");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (showPlayerSettings) {
            if (gameManager.tileGrid != null && gameManager.gameInfo != null) {
                if (EditorApplication.isPlayingOrWillChangePlaymode) {
                    EditorGUI.BeginDisabledGroup(true);
                    gameManager.gameInfo.numberOfPlayers = EditorGUILayout.IntSlider("Number of Players", gameManager.gameInfo.numberOfPlayers, 0, gameManager.tileGrid.numberOfCostalCities);
                    EditorGUI.EndDisabledGroup();
                } else {
                    gameManager.gameInfo.numberOfPlayers = EditorGUILayout.IntSlider("Number of Players", gameManager.gameInfo.numberOfPlayers, 0, gameManager.tileGrid.numberOfCostalCities);
                }
            }
            EditorGUILayout.PropertyField(playerPrefab_prop);
            EditorGUILayout.PropertyField(playerParent_prop);
        }
        EditorGUILayout.Space(10);

        showGameInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showGameInfo, "Game Info");
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUI.BeginDisabledGroup(true);
        if (showGameInfo) {
            EditorGUILayout.PropertyField(day_prop);
            EditorGUILayout.PropertyField(currentPlayerIndex_prop);
            EditorGUILayout.PropertyField(playerList_prop);
            EditorGUILayout.PropertyField(dayCompleted_prop);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);

        if (EditorApplication.isPlayingOrWillChangePlaymode) {
            showUIData = EditorGUILayout.BeginFoldoutHeaderGroup(showUIData, "UIData");
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.BeginDisabledGroup(true);
            if (showUIData) {
                EditorGUILayout.PropertyField(currentCity_prop);
                EditorGUILayout.PropertyField(currentUnit_prop);
                EditorGUILayout.PropertyField(mouseOverTile_prop);
            }
            EditorGUI.EndDisabledGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }
}