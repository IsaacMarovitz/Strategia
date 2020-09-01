using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using TMPro.EditorUtilities;

/*public class UGUITextToTextMeshPro : Editor {
    [MenuItem("Tools/Convert To Text Mesh Pro", false, 4000)]
    static void DoIt() {
        foreach (var gameObject in Selection.gameObjects) {
            Stuff(gameObject.GetComponent<Text>());
        }
    }

    static void Stuff(Text uiText) {
        //Text uiText = Selection.activeGameObject.GetComponent<Text>();
        if (uiText == null) {
            EditorUtility.DisplayDialog(
                "ERROR!", "You must select a Unity UI Text Object to convert.", "OK", "");
            return;
        }

        MenuCommand command = new MenuCommand(uiText);

        // If you have an error due to this line then go to TMPro_CreateObjectMenu.cs
        // and make this method public.
        TMPro_CreateObjectMenu.CreateTextMeshProGuiObjectPerform(command);

        TextMeshProUGUI tmp = Selection.activeGameObject.GetComponent<TextMeshProUGUI>();

        if (tmp == null) {
            EditorUtility.DisplayDialog(
                "ERROR!",
                "Something went wrong! Text Mesh Pro did not select the newly created object.",
                "OK",
                "");
            return;
        }

        tmp.fontStyle = GetTmpFontStyle(uiText.fontStyle);

        tmp.fontSize = uiText.fontSize;
        tmp.fontSizeMin = uiText.resizeTextMinSize;
        tmp.fontSizeMax = uiText.resizeTextMaxSize;
        tmp.enableAutoSizing = uiText.resizeTextForBestFit;
        tmp.alignment = GetTmpAlignment(uiText.alignment);
        tmp.text = uiText.text;
        tmp.color = uiText.color;

        tmp.transform.SetParent(uiText.transform.parent);
        tmp.name = uiText.name;

        tmp.rectTransform.anchoredPosition3D = uiText.rectTransform.anchoredPosition3D;
        tmp.rectTransform.anchorMax = uiText.rectTransform.anchorMax;
        tmp.rectTransform.anchorMin = uiText.rectTransform.anchorMin;
        tmp.rectTransform.localPosition = uiText.rectTransform.localPosition;
        tmp.rectTransform.localRotation = uiText.rectTransform.localRotation;
        tmp.rectTransform.localScale = uiText.rectTransform.localScale;
        tmp.rectTransform.pivot = uiText.rectTransform.pivot;
        tmp.rectTransform.sizeDelta = uiText.rectTransform.sizeDelta;

        tmp.transform.SetSiblingIndex(uiText.transform.GetSiblingIndex());

        // Copy all other components
        Component[] components = uiText.GetComponents<Component>();
        int componentsCopied = 0;
        for (int i = 0; i < components.Length; i++) {
            var thisType = components[i].GetType();
            if (thisType == typeof(Text) ||
                thisType == typeof(RectTransform) ||
                thisType == typeof(Transform) ||
                thisType == typeof(CanvasRenderer))
                continue;

            UnityEditorInternal.ComponentUtility.CopyComponent(components[i]);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(tmp.gameObject);

            componentsCopied++;
        }
        GameObject.DestroyImmediate(uiText.gameObject);
        /*if (componentsCopied == 0)
            Undo.DestroyObjectImmediate((Object)uiText.gameObject);
        else {
            EditorUtility.DisplayDialog(
                "uGUI to TextMesh Pro",
                string.Format(
                    "{0} components copied. Please check for accuracy as some references may not transfer properly.",
                    componentsCopied),
                "OK",
                "");
            uiText.name += " OLD";
            uiText.gameObject.SetActive(false);
        }
    }


    private static FontStyles GetTmpFontStyle(FontStyle uGuiFontStyle) {
        FontStyles tmp = FontStyles.Normal;
        switch (uGuiFontStyle) {
            case FontStyle.Normal:
            default:
                tmp = FontStyles.Normal;
                break;
            case FontStyle.Bold:
                tmp = FontStyles.Bold;
                break;
            case FontStyle.Italic:
                tmp = FontStyles.Italic;
                break;
            case FontStyle.BoldAndItalic:
                tmp = FontStyles.Bold | FontStyles.Italic;
                break;
        }

        return tmp;
    }


    public static TextAlignmentOptions GetTmpAlignment(TextAnchor uGuiAlignment) {
        TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft;

        switch (uGuiAlignment) {
            default:
            case TextAnchor.UpperLeft:
                alignment = TextAlignmentOptions.TopLeft;
                break;
            case TextAnchor.UpperCenter:
                alignment = TextAlignmentOptions.Top;
                break;
            case TextAnchor.UpperRight:
                alignment = TextAlignmentOptions.TopRight;
                break;
            case TextAnchor.MiddleLeft:
                alignment = TextAlignmentOptions.MidlineLeft;
                break;
            case TextAnchor.MiddleCenter:
                alignment = TextAlignmentOptions.Midline;
                break;
            case TextAnchor.MiddleRight:
                alignment = TextAlignmentOptions.MidlineRight;
                break;
            case TextAnchor.LowerLeft:
                alignment = TextAlignmentOptions.BottomLeft;
                break;
            case TextAnchor.LowerCenter:
                alignment = TextAlignmentOptions.Bottom;
                break;
            case TextAnchor.LowerRight:
                alignment = TextAlignmentOptions.BottomRight;
                break;
        }

        return alignment;
    }
}*/