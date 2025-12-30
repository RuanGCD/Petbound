using UnityEngine;
using System.Collections;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 2000f;

    Vector3 targetPos;

    public void Init(Vector3 start, Vector3 target)
{
    transform.position = start;
    targetPos = target;

    Vector3 dir = (target - start).normalized;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle);

    StartCoroutine(Move());
}

    IEnumerator Move()
    {
        while (Vector3.Distance(transform.position, targetPos) > 5f)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);
    }
}
