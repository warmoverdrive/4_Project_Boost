using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    Camera mainCam;

	private void Start()
	{
		mainCam = Camera.main;
	}

	private void Update()
	{
		//transform.LookAt(-mainCam.transform.position);
		transform.LookAt(-mainCam.transform.position, Vector3.up);
	}
}
