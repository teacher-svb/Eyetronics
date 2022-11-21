using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add a rigid body to the capsule
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSWalker script to the capsule

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class LookAtMouse : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;

	public float maximumX = 120.0f;
	public float maximumY = 120.0f;
	
	public float scaleFactor = 1.0f;
	
	public float offsetX = 108.0f;
	public float offsetY = 25.0f;

	float rotationX = 0F;
	float rotationY = 0F;
	
	Quaternion originalRotation;

	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			// Read the mouse input axis
			rotationX = (-Input.mousePosition.x / Screen.width) * maximumX + offsetX;
			rotationY = (-Input.mousePosition.y / Screen.height) * maximumY + offsetY;
			
			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX / scaleFactor, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY / scaleFactor, Vector3.left);
			
			transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		}
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}