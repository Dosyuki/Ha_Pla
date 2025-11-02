using UnityEngine;
using UnityEngine.UI;

// สั่งให้สคริปต์นี้ต้องอยู่บน GameObject เดียวกับ Slider
[RequireComponent(typeof(Slider))]
public class SliderGradientFill : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("ลาก 'Fill' (ที่เป็น Image) ของ Slider มาใส่")]
    [SerializeField] private Image fillImage; 

    [Header("Gradient")]
    [Tooltip("ตั้งค่าการไล่สีที่คุณต้องการใน Inspector")]
    [SerializeField] private Gradient gradient;

    private Slider slider;

    private void Awake()
    {
        // 1. ดึง Slider ที่อยู่บน GameObject เดียวกัน
        slider = GetComponent<Slider>();

        // 2. ตั้งค่า "Listener" ให้ Slider
        // สั่งให้ Slider เรียกฟังก์ชัน UpdateColor() "ทุกครั้ง" ที่ค่าของมันเปลี่ยน
        slider.onValueChanged.AddListener(UpdateColor);
    }

    private void Start()
    {
        // 3. ตั้งค่าสีเริ่มต้น (เผื่อค่าไม่ได้เริ่มที่ 0)
        UpdateColor(slider.value);
    }

    /// <summary>
    /// ฟังก์ชันนี้จะถูกเรียกโดยอัตโนมัติเมื่อ Slider เปลี่ยนค่า
    /// </summary>
    /// <param name="value">ค่าปัจจุบันของ Slider (ระหว่าง 0 ถึง 1)</param>
    private void UpdateColor(float value)
    {
        if (fillImage == null || gradient == null)
            return;

        // 4. (หัวใจสำคัญ)
        // สั่งให้ Gradient "ประเมิน (Evaluate)" ว่าที่ค่า value นี้ (0.0 - 1.0)
        // ควรจะเป็นสีอะไร แล้วเอาสีนั้นไปใส่ใน Fill Image
        fillImage.color = gradient.Evaluate(value - 1);
    }

    // (Optional) ถ้าคุณมีการเปิด/ปิด Listener
    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(UpdateColor);
        }
    }
}