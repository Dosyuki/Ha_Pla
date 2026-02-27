using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode] // เพื่อให้เห็นผลใน Editor ทันทีโดยไม่ต้องกด Play
[RequireComponent(typeof(LineRenderer))]
public class UIRopeBezier : MonoBehaviour
{
    [Header("ลาก UI จุด A (เริ่ม) มาใส่")]
    public RectTransform startPoint;
    [Header("ลาก UI จุด B (จบ) มาใส่")]
    public RectTransform endPoint;
    [Header("ลาก Empty Object ที่อยู่ตรงกลางมาใส่ (เพื่อดึงเส้นให้โค้ง)")]
    public RectTransform controlPoint; 

    [Range(10, 100)]
    public int segmentCount = 50; // ความละเอียดของเส้น (ยิ่งมากยิ่งโค้งเนียน)

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (startPoint == null || endPoint == null || controlPoint == null)
        {
            return;
        }

        lineRenderer.positionCount = segmentCount + 1;
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 point = CalculateQuadraticBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);
            points.Add(point);
        }

        lineRenderer.SetPositions(points.ToArray());
    }

    // ฟังก์ชันคำนวณจุดโค้งเบซิเยร์
    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // สูตร math สำหรับ Bezier p = (1-t)^2*p0 + 2*(1-t)*t*p1 + t^2*p2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0; // (1-t)^2 * p0
        p += 2 * u * t * p1; // 2 * (1-t) * t * p1
        p += tt * p2;        // t^2 * p2
        return p;
    }
}