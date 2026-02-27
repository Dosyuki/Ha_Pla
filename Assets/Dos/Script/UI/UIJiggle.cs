using UnityEngine;

public class UIJiggle : MonoBehaviour
{
    public float shakePower = 2f; 
    public float shakeAngle = 5f; 
    
    private bool isShaking = false;
    private Quaternion originalRot;

    void Start()
    {
        // จำค่าหมุนเริ่มต้นไว้ (เผื่อ Object มีการหมุนไว้ก่อนหน้า)
        originalRot = transform.localRotation;
    }

    public void StartShake()
    {
        isShaking = true;
    }

    public void StopShake()
    {
        if (isShaking)
        {
            isShaking = false;
            // เมื่อหยุดสั่น ให้รีเซ็ตกลับไปที่ตำแหน่ง 0 ของ Parent (Handle) และท่าหมุนเดิม
            transform.localPosition = Vector3.zero; 
            transform.localRotation = originalRot;
        }
    }

    void Update()
    {
        if (isShaking)
        {
            // สุ่มตำแหน่ง "บวกเพิ่ม" จากจุดศูนย์กลางของ Handle (Vector3.zero) 
            // วิธีนี้จะทำให้มันขยับตาม Handle ไปได้ตลอดเวลา
            transform.localPosition = new Vector3(
                Random.Range(-shakePower, shakePower), 
                Random.Range(-shakePower, shakePower), 
                0);
            
            // สุ่มหมุน +- 5 องศา
            float randomZ = Random.Range(-shakeAngle, shakeAngle);
            transform.localRotation = originalRot * Quaternion.Euler(0, 0, randomZ);
        }
    }
}