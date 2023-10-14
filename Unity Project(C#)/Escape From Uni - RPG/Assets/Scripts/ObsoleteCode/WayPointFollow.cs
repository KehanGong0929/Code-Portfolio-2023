using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointFollow : MonoBehaviour
{

    [SerializeField] GameObject[] wayPoints;
    int currentWaypointIndex = 0;
    [SerializeField] float speed = 1.0f;
    private float _rotationFactorPerFramef = 15.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, wayPoints[currentWaypointIndex].transform.position)< 0.1f)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex >= wayPoints.Length)
            {
                currentWaypointIndex =0 ;
            }
        }
       // HandleRotation();
        transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentWaypointIndex].transform.position, speed*Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x =wayPoints[currentWaypointIndex].transform.position.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = wayPoints[currentWaypointIndex].transform.position.z;
        Quaternion currentRotation = transform.rotation;
        if (IsMoving())
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFramef * Time.deltaTime);
        }
    }

    bool IsMoving()
    {
        return transform.position.x != 0 || transform.position.z != 0;
    }


}
