using UnityEngine;

public class RepeatBG : MonoBehaviour
{
    public float Speed;
    public Material BGmaterial;
    void Update()
    {
        BGmaterial.mainTextureOffset+= new Vector2(Speed,Speed) * Time.deltaTime;
    }
}
