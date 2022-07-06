using UnityEngine;

public class RotatingObject: MonoBehaviour
{

    [SerializeField] private int rotateSpeed;

    void Start()
    {
        rotateSpeed = 2;
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
