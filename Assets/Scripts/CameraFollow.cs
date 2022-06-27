using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public GameObject target;
	public bool follow = false;
	private Vector3 delta;

	// Start is called before the first frame update
	void Start()
	{
	}

	public void StartFollow()
	{
		delta = target.transform.position - transform.position;
		Debug.Log("UpdateDelta");
		follow = true;
	}

	public void StopFollow()
	{
		follow = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (!follow) return;
		var distX = (target.transform.position - delta).x - transform.position.x;
		var distY = (target.transform.position - delta).y - transform.position.y;
		var posX = transform.position.x;
		var posY = transform.position.y;
		if(distX > 2f)
			posX = transform.position.x + (distX-2f);
		if(distX<-1f)
			posX = transform.position.x + (distX+1f);
		if(distY > 2f)
			posY = transform.position.y + (distY-2f);
		if(distY <-1f)
			posY = transform.position.y + (distY+1f);
		transform.position = new Vector3(posX, posY, transform.position.z);

	}
}
