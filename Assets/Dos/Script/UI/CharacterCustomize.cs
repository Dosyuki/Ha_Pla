// แก้ไขไฟล์: CharacterCustomize.cs
using UnityEngine;

// ใช้ Singleton.cs ที่คุณมี
public class CharacterCustomize : Singleton<CharacterCustomize>
{
    [SerializeField] private GameObject character;
    [SerializeField] private Renderer characterRenderer; // ลาก Renderer มาใส่
    [SerializeField] private Renderer captureCharacter;

    // Index ของ Material (ตั้งค่าใน Inspector)
    [SerializeField] private int hairMaterialIndex = 0;
    [SerializeField] private int skinMaterialIndex = 4;
    [SerializeField] private int clothMaterialIndex = 1;

    // Material เริ่มต้น (ลากใส่ Inspector)
    [SerializeField] private Material defaultHair;
    [SerializeField] private Material defaultSkin;
    [SerializeField] private Material defaultCloth;

    // ตัวแปรเก็บ Material ที่เลือกในปัจจุบัน
    private Material currentHair;
    private Material currentSkin;
    private Material currentCloth;

    private void Start()
    {
        // หากไม่ได้ลากใส่ใน Inspector
        if (characterRenderer == null)
        {
            characterRenderer = character.GetComponentInChildren<MeshRenderer>();
        }
        
        // โหลด Material เริ่มต้น (ถ้ายังไม่มีการ Load Game)
        ApplyMaterials(defaultHair, defaultSkin, defaultCloth);
    }

    /// <summary>
    /// ใช้สำหรับตั้งค่า Material ทั้งตอนเริ่มและตอน Load
    /// </summary>
    private void ApplyMaterials(Material hair, Material skin, Material cloth)
    {
        // 1. ดึง Array ปัจจุบัน
        Material[] currentMaterials = characterRenderer.materials;

        // 2. อัปเดต Array ด้วย Material ใหม่
        if (hair != null)
        {
            currentMaterials[hairMaterialIndex] = hair;
            currentHair = hair;
        }
        if (skin != null)
        {
            currentMaterials[skinMaterialIndex] = skin;
            currentSkin = skin;
        }
        if (cloth != null)
        {
            currentMaterials[clothMaterialIndex] = cloth;
            currentCloth = cloth;
        }

        // 3. ยัด Array กลับเข้าไป
        characterRenderer.materials = currentMaterials;
        captureCharacter.materials = currentMaterials;
    }
    
    // --- ฟังก์ชันที่ UI Buttons เรียกใช้ ---

    public void SelectSkin(Material newSkinMaterial)
    {
        ApplyMaterials(null, newSkinMaterial, null); // ส่ง null ถ้าไม่ต้องการเปลี่ยน
    }

    public void SelectHair(Material newHairMaterial)
    {
        ApplyMaterials(newHairMaterial, null, null);
    }

    public void SelectCloth(Material newClothMaterial)
    {
        ApplyMaterials(null, null, newClothMaterial);
    }

    // --- ฟังก์ชันสำหรับ Save/Load ---

    public CharacterCustomizeData GetSaveData()
    {
        return new CharacterCustomizeData
        {
            // เรา Save "ชื่อ" ของ Asset (เช่น "Hair_Brown")
            hairMaterialName = (currentHair != null) ? currentHair.name : string.Empty,
            skinMaterialName = (currentSkin != null) ? currentSkin.name : string.Empty,
            clothMaterialName = (currentCloth != null) ? currentCloth.name : string.Empty
        };
    }

    public void LoadData(CharacterCustomizeData data)
    {
        // 1. ค้นหา Material จาก "ชื่อ" โดยใช้ Database
        Material hair = CharacterMaterialDatabase.Instance.GetMaterialByName(data.hairMaterialName);
        Material skin = CharacterMaterialDatabase.Instance.GetMaterialByName(data.skinMaterialName);
        Material cloth = CharacterMaterialDatabase.Instance.GetMaterialByName(data.clothMaterialName);

        // 2. ถ้าหาไม่เจอ ให้ใช้ค่าเริ่มต้นแทน
        if (hair == null) hair = defaultHair;
        if (skin == null) skin = defaultSkin;
        if (cloth == null) cloth = defaultCloth;

        // 3. สวมใส่ Material ที่ Load มา
        ApplyMaterials(hair, skin, cloth);
        
        Debug.Log("Character customization loaded.");
    }
}