using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{

    public Transform car;
    private void LateUpdate()
    {
        Vector3 newPosition = car.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, car.eulerAngles.y, 0f);
    }
}
