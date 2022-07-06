using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalArrow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform car;
    public Transform Target
    {
        set => target = value;
    }

    public void Update()
    {
        this.transform.position = new Vector3(car.position.x, car.position.y + 3.5f, car.position.z);
        Vector3 targetPostition = new Vector3(target.position.x, this.transform.position.y, target.position.z);
        this.transform.LookAt(targetPostition);
    }
}
