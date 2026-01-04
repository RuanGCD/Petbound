using UnityEngine;

public class VFXAutoDestroy : MonoBehaviour
{
    public float lifeTime = 1.0f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
