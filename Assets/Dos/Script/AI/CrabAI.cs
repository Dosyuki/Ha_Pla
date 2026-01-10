using UnityEngine;
using UnityEngine.AI;
using System.Collections; // ต้องใช้สำหรับ IEnumerator

[RequireComponent(typeof(NavMeshAgent))]
public class CrabAI : MonoBehaviour
{
    [Header("Crab Movement")]
    [Tooltip("ระยะก้าว 'ขั้นต่ำ' ที่ปูจะเดินไปด้านข้าง")]
    public float minWanderDistance = 5.0f;
    [Tooltip("ระยะก้าว 'สูงสุด' ที่ปูจะเดินไปด้านข้าง")]
    public float maxWanderDistance = 10.0f;

    [Header("Crab Turning")]
    [Tooltip("ปูจะเดินข้างนานแค่ไหน ก่อนจะหันหน้าใหม่ (วินาที)")]
    public float wanderDuration = 5.0f;
    [Tooltip("ความเร็วในการหันของปู (ยิ่งมากยิ่งหันเร็ว)")]
    public float turnSpeed = 2.0f;
    
    private NavMeshAgent agent;
    private float timeSinceLastTurn = 0f;
    private bool isTurning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // --- นี่คือหัวใจสำคัญ ---
        // สั่งให้ Agent "ห้าม" อัปเดตการหมุน (Rotation)
        // เราจะหมุนมันเอง!
        agent.updateRotation = false;
        // ------------------------

        // เริ่มต้นด้วยการเดินไปด้านข้าง
        FindNewSideStep();
    }

    void Update()
    {
        if (isTurning)
        {
            // ถ้าอยู่ในสถานะ "กำลังหัน" ก็ไม่ต้องทำอะไร
            // (เราปล่อยให้ Coroutine 'ReorientCrab' ทำงานไป)
            return;
        }

        // --- สถานะ "กำลังเดินข้าง" ---

        // นับเวลา
        timeSinceLastTurn += Time.deltaTime;

        // 1. ตรวจสอบว่าถึงเวลา "หัน" ใหม่หรือยัง
        if (timeSinceLastTurn > wanderDuration)
        {
            StartCoroutine(ReorientCrab());
        }
        // 2. ตรวจสอบว่าเดินถึงจุดหมาย (ด้านข้าง) หรือยัง
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            FindNewSideStep();
        }
    }

    /// <summary>
    /// หาทิศทาง "ก้าวข้าง" (ซ้าย หรือ ขวา)
    /// </summary>
    void FindNewSideStep()
    {
        // สุ่มว่าจะไปซ้ายหรือขวา (จากทิศที่ปู "หันหน้า" อยู่)
        Vector3 sideDirection = (Random.value > 0.5f) ? transform.right : -transform.right;
        
        // สุ่มระยะทางที่จะก้าว
        float distance = Random.Range(minWanderDistance, maxWanderDistance);

        // คำนวณจุดหมายปลายทาง
        Vector3 destination = transform.position + (sideDirection * distance);

        // หาจุดที่ใกล้ที่สุดบน NavMesh
        Vector3 validDestination = GetValidPointOnNavMesh(destination, maxWanderDistance);

        // สั่งให้ Agent "ไถล" ไปยังจุดนั้น (มันจะไม่หันหน้า เพราะเราตั้ง agent.updateRotation = false)
        agent.SetDestination(validDestination);
    }

    /// <summary>
    /// Coroutine สำหรับ "หันหน้า" ปู (Reorient)
    /// </summary>
    IEnumerator ReorientCrab()
    {
        isTurning = true;
        timeSinceLastTurn = 0f; // รีเซ็ตเวลา
        
        // หยุด Agent ชั่วคราวขณะหัน
        agent.isStopped = true; 

        // สุ่มทิศทางใหม่ (ไม่สนใจแกน Y)
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(randomDirection.normalized);

        // ค่อยๆ หัน (Slerp) ไปยังทิศทางใหม่
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            yield return null; // รอเฟรมถัดไป
        }

        // หันเสร็จแล้ว
        transform.rotation = targetRotation; // ล็อคค่าให้ตรง
        isTurning = false;
        agent.isStopped = false;

        // เมื่อหันเสร็จ ก็ให้เริ่ม "เดินข้าง" ทันที
        FindNewSideStep();
    }


    /// <summary>
    /// หาตำแหน่งที่ถูกต้องบน NavMesh (เหมือนเดิม)
    /// </summary>
    private Vector3 GetValidPointOnNavMesh(Vector3 origin, float radius)
    {
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(origin, out navHit, radius, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        // ถ้าหาไม่เจอจริงๆ ก็ให้กลับไปที่เดิม
        return transform.position;
    }
}