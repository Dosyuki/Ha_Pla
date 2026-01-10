// ไฟล์ใหม่: CharacterMaterialDatabase.cs
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaterialDatabase : Singleton<CharacterMaterialDatabase>
{
    [SerializeField] private List<Material> skinMaterials;
    [SerializeField] private List<Material> hairMaterials;
    [SerializeField] private List<Material> clothMaterials;

    private Dictionary<string, Material> materialLookup;

    protected void Awake() 
    {
        BuildLookupDictionary();
    }

    private void BuildLookupDictionary()
    {
        materialLookup = new Dictionary<string, Material>();
        
     
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