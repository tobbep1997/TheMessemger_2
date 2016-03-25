using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public float lookSensitivity = 1.5f;
	float yRotation, xRotation;
	float currentYRotation, currentXRotation;
	float yRotationVel, xRotationVel;
	public float lookSmoothDamp = 0.1f;

	public float cameraMoveSpeed;

	public GameObject standView, slideView;
	public GameObject player;

	ParkourMovement pm;
    PlayerSneek ps;

    private float offset = .1f;
    [SerializeField]
    private Transform Neck;
	void Start () {
		Cursor.visible = false;
		pm = transform.parent.parent.GetComponent<ParkourMovement> ();
        ps = transform.parent.parent.GetComponent<PlayerSneek>();
	}

	void FixedUpdate () {
		yRotation += Input.GetAxis ("Mouse X") * lookSensitivity;
		xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;

		xRotation = Mathf.Clamp (xRotation, -90, 60);

		currentXRotation = Mathf.SmoothDamp (currentXRotation, xRotation, ref xRotationVel, lookSmoothDamp);
		currentYRotation = Mathf.SmoothDamp (currentYRotation, yRotation, ref yRotationVel, lookSmoothDamp);

		transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        //Neck.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        
		player.transform.rotation = Quaternion.Euler (0, currentYRotation, 0);
	    
	}

	void Update() {
		//IDEA FOR SMOOTH MOVEMENT: make an if statement that checks if its at the others position. as an example if transform position is less
		//than standview then += a vector depending on how quick you want it to transition until it reaches.
		//same thing for the movement to the other camera view but inverted
	
		if (pm.playerColStanding.enabled)
		{
			if(transform.position.y + offset < standView.transform.position.y)
				transform.position += Vector3.up * Time.deltaTime * cameraMoveSpeed;
			else
				transform.position = standView.transform.position;
		}
		if (pm.playerColSliding.enabled)
		{
			if(transform.position.y > slideView.transform.position.y)
				transform.position -= Vector3.up * Time.deltaTime * cameraMoveSpeed;
			else
				transform.position = slideView.transform.position;
		}
	
	}
}
