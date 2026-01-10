using UnityEngine;
using UnityEngine.UI;
using TMPro; // ต้องมี TextMeshPro

// ตรวจสอบว่ามี Slider ติดอยู่กับ GameObject นี้
[RequireComponent(typeof(Slider))]
public class SliderTextEffects : MonoBehaviour
{
    [Header("Target UI (ลากมาใส่)")]
    [Tooltip("TMP_Text ที่คุณต้องการให้เปลี่ยนสีและขนาด")]
    [SerializeField] private TMP_Text targetText;

    [Header("Color Effect")]
    [Tooltip("ตั้งค่าการไล่สีที่คุณต้องการสำหรับค่า 1.0 ถึง 2.0")]
    [SerializeField] private Gradient gradient;

    [Header("Font Size Effect")]
    [Tooltip("ขนาดฟอนต์เมื่อ Slider มีค่า 1.0")]
    [SerializeField] private float minFontSize = 24f;

    [Tooltip("ขนาดฟอนต์เมื่อ Slider มีค่า 2.0")]
    [SerializeField] private float maxFontSize = 48f;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("SliderTextEffects: ยังไม่ได้ลาก 'Target Text' มาใส่!");
            return;
        }

        // 1. ตั้งค่า "Listener"
        // สั่งให้ Slider เรียกฟังก์ชัน UpdateTextEffects "ทุกครั้ง" ที่ค่าเปลี่ยน
        slider.onValueChanged.AddListener(UpdateTextEffects);

        // 2. อัปเดตค่าเริ่มต้น (เผื่อค่าไม่ได้เริ่มที่ 1.0)
        UpdateTextEffects(slider.value);
    }

    /// <summary>
    /// ฟังก์ชันหลักที่ถูกเรียกโดย Slider
    /// </summary>
    /// <param name="value">ค่าปัจจุบันของ Slider (เช่น 1.0 ถึง 2.0)</param>
    public void UpdateTextEffects(float value)
    {
        // 1. "Normalize" ค่า
        // เราต้องแปลงค่า [1.0 ... 2.0] ให้อยู่ในช่วง [0.0 ... 1.0]
        // เพื่อให้ Gradient และ Lerp ใช้งานได้
        // (เราใช้ค่า min/max จากตัว Slider โดยตรงเผื่อคุณเปลี่ยนใจ)
        float t = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);

        // 2. เปลี่ยนสี
        // Gradient.Evaluate(t) จะคืนค่าสีตามตำแหน่ง 0-1
        if (gradient != null)
        {
            targetText.color = gradient.Evaluate(t);
        }

        // 3. เปลี่ยนขนาด
        // Mathf.Lerp(a, b, t) จะคืนค่าที่อยู่ระหว่าง a กับ b ตามตำแหน่ง 0-1
        float newSize = Mathf.Lerp(minFontSize, maxFontSize, t);
        targetText.fontSize = newSize;
        targetText.text = value.ToString("F2");
    }

    // (Optional) อย่าลืมลบ Listener ออกเมื่อ Object ถูกทำลาย
    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(UpdateTextEffects);
        }
    }
}