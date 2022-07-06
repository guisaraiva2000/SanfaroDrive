using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInside : MonoBehaviour
{
	public Transform MinimapCam;
	public float MinimapSize;
	Vector3 TempV3;

    void Update()
	{
		TempV3 = transform.parent.transform.position;
		TempV3.y = transform.position.y;
		transform.position = TempV3;
		transform.rotation = Quaternion.Euler(90f, MinimapCam.eulerAngles.y, 0f);
	}

	void LateUpdate()
	{
		Vector3 centerPosition = MinimapCam.transform.localPosition;

		centerPosition.y -= 0.5f;

		float Distance = Vector3.Distance(transform.position, centerPosition);

		if (Distance > MinimapSize)
		{
			Vector3 fromOriginToObject = transform.position - centerPosition;
			fromOriginToObject *= MinimapSize / Distance;
			transform.position = centerPosition + fromOriginToObject;
			transform.localScale = new Vector3(0.7f,0.7f,0.7f);
		} else
        {
			transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
		}
	}
}
