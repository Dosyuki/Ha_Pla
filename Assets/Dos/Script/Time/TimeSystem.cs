using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystem : Singleton<TimeSystem>
{
    [SerializeField] private Light sun;
    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField] private float sunRotationSpeed;
    [SerializeField] private float sleepTimeMultipier;
    private float startSpeed;

    [Header("LightingPreset")]
    [SerializeField] private Gradient skyColor;
    [SerializeField] private Gradient equatorColor;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private Gradient fogColor;
    [SerializeField] private Image dayLightUI;

    // --- เพิ่มตัวแปรเก็บค่ามุม Y, Z เริ่มต้น ---
    private float initialSunY;
    private float initialSunZ;

    private void Start()
    {
        startSpeed = sunRotationSpeed;

        // --- จำค่ามุมเริ่มต้นไว้ (เก็บเป็นองศา) ---
        if (sun != null)
        {
            initialSunY = sun.transform.eulerAngles.y;
            initialSunZ = sun.transform.eulerAngles.z;
        }
    }

    private void Update()
    {
        timeOfDay += (24f / sunRotationSpeed) * Time.deltaTime;
        
        if (timeOfDay >= 24)
            timeOfDay %= 24;
            
        UpdateSunRotation();
        UpdateLighting();
    }

    private void OnValidate()
    {
        // ซิงค์สี Gradient หัว-ท้าย เพื่อป้องกันการกระโดดของสี
        SyncGradientColors(skyColor);
        SyncGradientColors(equatorColor);
        SyncGradientColors(sunColor);
        SyncGradientColors(fogColor);

        UpdateSunRotation();
        UpdateLighting();
    }

    // ฟังก์ชันช่วยทำให้สีหัวท้ายเท่ากันเป๊ะๆ
    private void SyncGradientColors(Gradient gradient)
    {
        if (gradient == null) return;
        GradientColorKey[] colorKeys = gradient.colorKeys;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

        if (colorKeys.Length > 0 && alphaKeys.Length > 0)
        {
            Color startColor = gradient.Evaluate(0f);
            // ก๊อปปี้สีจาก 0% ไปใส่ที่ 100%
            colorKeys[colorKeys.Length - 1].color = startColor;
            alphaKeys[alphaKeys.Length - 1].alpha = startColor.a;
            gradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    private void UpdateSunRotation()
    {
        if (sun == null) return;

        // คำนวณมุม X ตามเวลา
        float sunRotationX = Mathf.Lerp(-90, 270, timeOfDay / 24);
        
        // --- แก้ไขตรงนี้: ใช้ค่า Y, Z เดิมที่จำไว้ แทนการอ่านค่าผิดๆ ---
        // ใช้ eulerAngles เพื่อความแน่นอน
        if (Application.isPlaying)
        {
             sun.transform.rotation = Quaternion.Euler(sunRotationX, initialSunY, initialSunZ);
        }
        else
        {
             // ใน Editor Mode (OnValidate) เราอาจจะยังไม่มี initial values
             // ให้อ่านจาก eulerAngles ปัจจุบันแทน (แต่ต้องเป็น eulerAngles นะ ห้ามใช้ rotation.y)
             sun.transform.rotation = Quaternion.Euler(sunRotationX, sun.transform.eulerAngles.y, sun.transform.eulerAngles.z);
        }

        if (dayLightUI != null)
        {
            float uiRotation = Mathf.Lerp(-90, 270, timeOfDay / 24f);
            dayLightUI.transform.rotation = Quaternion.Euler(0, 0, uiRotation);
        }
    }

    private void UpdateLighting()
    {
        float timeFraction = (timeOfDay % 24) / 24;
        
        if(equatorColor != null) RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        if(skyColor != null) RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
        if(sunColor != null && sun != null) sun.color = sunColor.Evaluate(timeFraction);
        if(fogColor != null) RenderSettings.fogColor = fogColor.Evaluate(timeFraction);
    }

    // ... (ส่วนที่เหลือ: GetDynamicCondition, Sleep, Save/Load เหมือนเดิม) ...
    public float[] GetDynamicCondition()
    {
        float[] condition = new float[2];
        if ((timeOfDay >= 0 && timeOfDay < 6) || (timeOfDay >= 20 && timeOfDay < 24))
        {
            condition[0] = 1.1f;
            condition[1] = 0.9f;
            return condition;
        }
        condition[0] = 1f;
        condition[1] = 1f;
        return condition;
    }

    public void Sleep()
    {
        if(startSpeed == sunRotationSpeed)
            sunRotationSpeed /= sleepTimeMultipier;
        else
            sunRotationSpeed = startSpeed;
    }
    
    public WorldData GetSaveData()
    {
        return new WorldData { timeOfDay = this.timeOfDay };
    }

    public void LoadData(WorldData data)
    {
        this.timeOfDay = data.timeOfDay;
        UpdateSunRotation();
        UpdateLighting();
    }
}