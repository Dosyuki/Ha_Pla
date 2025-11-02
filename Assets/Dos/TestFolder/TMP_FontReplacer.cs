using UnityEngine;
using UnityEditor;
using TMPro;

public class TMP_FontReplacer : EditorWindow
{
    private TMP_FontAsset oldFont;
    private TMP_FontAsset newFont;

    [MenuItem("Tools/Gemini Tools/TMP Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<TMP_FontReplacer>("TMP Font Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Replace TMP Fonts", EditorStyles.boldLabel);
        GUILayout.Space(10);

        oldFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font เก่า (ที่จะค้นหา)", oldFont, typeof(TMP_FontAsset), false);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font ใหม่ (ที่จะแทนที่)", newFont, typeof(TMP_FontAsset), false);

        GUILayout.Space(20);

        if (GUILayout.Button("แทนที่ทั้งหมด (ใน Scene และ Prefab)", GUILayout.Height(40)))
        {
            if (oldFont == null || newFont == null)
            {
                Debug.LogError("กรุณาใส่ Font Asset ทั้งเก่าและใหม่");
                return;
            }
            ReplaceFonts();
        }
    }

    private void ReplaceFonts()
    {
        int count = 0;

        // 1. ค้นหา "ทุก Prefab" ในโปรเจกต์
        string[] allPrefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in allPrefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            // ค้นหา TMP_Text ทุกตัวที่อยู่ใน Prefab (รวมถึงลูกๆ)
            TMP_Text[] textsInPrefab = prefab.GetComponentsInChildren<TMP_Text>(true); 
            
            foreach (TMP_Text text in textsInPrefab)
            {
                if (text.font == oldFont)
                {
                    text.font = newFont;
                    EditorUtility.SetDirty(prefab); // บันทึกการเปลี่ยนแปลง
                    count++;
                }
            }
        }

        // 2. ค้นหา "ทุก Object ใน Scene ที่เปิดอยู่"
        TMP_Text[] textsInScene = FindObjectsOfType<TMP_Text>(true);
        foreach (TMP_Text text in textsInScene)
        {
            if (text.font == oldFont)
            {
                text.font = newFont;
                EditorUtility.SetDirty(text); // บันทึกการเปลี่ยนแปลง
                count++;
            }
        }
        
        // 3. บันทึก Asset ที่เปลี่ยน
        AssetDatabase.SaveAssets();
        Debug.Log($"แทนที่ฟอนต์ '{oldFont.name}' ด้วย '{newFont.name}' ทั้งหมด {count} ตำแหน่ง สำเร็จ!");
    }
}