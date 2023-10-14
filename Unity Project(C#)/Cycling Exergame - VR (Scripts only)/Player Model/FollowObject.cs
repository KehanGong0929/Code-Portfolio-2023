using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool includeRotation = false;

    // Start is called before the first frame update
    void Update()
    {
        transform.position = target.position + offset;
        transform.eulerAngles = new Vector3(0, 0, target.eulerAngles.z);
    }
}
