// ไฟล์ใหม่: CharacterMaterialDatabase.cs
using System.Collections.Generic;
using UnityEngine;

// ใช้ Singleton.cs ที่คุณมี
public class CharacterMaterialDatabase : Singleton<CharacterMaterialDatabase>
{
    // ลาก Material Assets ทั้งหมดมาใส่ใน Inspector
    [SerializeField] private List<Material> skinMaterials;
    [SerializeField] private List<Material> hairMaterials;
    [SerializeField] private List<Material> clothMaterials;

    private Dictionary<string, Material> materialLookup;

    protected void Awake() // ใช้วิธี Override Awake() จาก Singleton
    {
        BuildLookupDictionary();
    }

    private void BuildLookupDictionary()
    {
        materialLookup = new Dictionary<string, Material>();
        
        // เราสามารถยัดทุกอย่างลงใน Dictionary เดียวกันได้เลย
        // ตราบใดที่ "ชื่อ" Material (.name) ไม่ซ้ำกัน
        foreach (Material mat in skinMaterials)
        {
            if (mat != null && !materialLookup.ContainsKey(mat.name))
                materialLookup.Add(mat.name, mat);
        }
        foreach (Material mat in hairMaterials)
        {
            if (mat != null && !materialLookup.ContainsKey(mat.name))
                materialLookup.Add(mat.name, mat);
        }
        foreach (Material mat in clothMaterials)
        {
            if (mat != null && !materialLookup.ContainsKey(mat.name))
                materialLookup.Add(mat.name, mat);
        }
    }

    /// <summary>
    /// ดึง Material Asset จากชื่อที่ Save ไว้
    /// </summary>
    public Material GetMaterialByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        materialLookup.TryGetValue(name, out Material mat);
        if (mat == null)
        {
            Debug.LogWarning($"Material not found in database: {name}");
        }
        return mat;
    }
}