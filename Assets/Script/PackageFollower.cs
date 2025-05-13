using UnityEngine;

public class PackageFollower : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        }
    }
}
