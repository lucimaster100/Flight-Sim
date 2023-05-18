using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCamera : MonoBehaviour
{
    [SerializeField]
    new Camera camera;
    [SerializeField]
    Vector3 cameraOffset;
    [SerializeField]
    Vector2 lookAngle;
    [SerializeField]
    float movementScale;
    [SerializeField]
    float lookAlpha;
    [SerializeField]
    float movementAlpha;
    [SerializeField]
    Vector3 deathOffset;
    [SerializeField]
    float deathSensitivity;

    Transform cameraTransform;
    Plane plane;
    Transform planeTransform;
    Vector2 lookInput;

    Vector2 lookAverage;
    Vector3 movementAverage;

    void Awake()
    {
        cameraTransform = camera.GetComponent<Transform>();
    }

    public void SetPlane(Plane plane)
    {
        this.plane = plane;

        if (plane == null)
        {
            planeTransform = null;
        }
        else
        {
            planeTransform = plane.GetComponent<Transform>();
        }

        cameraTransform.SetParent(planeTransform);
    }

    public void SetInput(Vector2 input)
    {
        lookInput = input;
    }

    void LateUpdate()
    {
        if (plane == null) return;

        var cameraOffset = this.cameraOffset;
  
        var lookAngle = Vector2.Scale(lookInput, this.lookAngle);
        lookAverage = (lookAverage * (1 - lookAlpha)) + (lookAngle * lookAlpha);

        var angularVelocity = plane.LocalAngularVelocity;
        angularVelocity.z = -angularVelocity.z;

        movementAverage = (movementAverage * (1 - movementAlpha)) + (angularVelocity * movementAlpha);
        
        var rotation = Quaternion.Euler(-lookAverage.y, lookAverage.x, 0);

        var offsetRotation = Quaternion.Euler(new Vector3(movementAverage.x, movementAverage.y) * -movementScale);
        var offset = rotation * offsetRotation * cameraOffset;

        cameraTransform.localPosition = offset;
        cameraTransform.localRotation = rotation * Quaternion.Inverse(offsetRotation) * Quaternion.Euler(0, 0, movementAverage.z * movementScale);
    }
}
