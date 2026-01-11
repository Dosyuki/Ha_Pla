using UnityEngine;

public class UIAlert : Singleton<UIAlert>
{
    public GameObject alertPrefab;
    public Transform alertParent;

    public void FishWarning()
    {
        GameObject temp = Instantiate(alertPrefab,alertParent.transform.position + 
                                                  new Vector3(Random.Range(-50,50),Random.Range(-50,50)),
                                                    alertParent.transform.rotation,alertParent);
        Destroy(temp,0.8f);
        Debug.Log("Fish Warning");
    }
}
