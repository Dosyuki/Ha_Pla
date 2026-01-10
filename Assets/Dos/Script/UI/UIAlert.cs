using UnityEngine;

public class UIAlert : Singleton<UIAlert>
{
    public GameObject alertPrefab;
    public Transform alertParent;

    public void FishWarning()
    {
        GameObject temp = Instantiate(alertPrefab,alertParent);
        Destroy(temp,1.5f);
    }
}
