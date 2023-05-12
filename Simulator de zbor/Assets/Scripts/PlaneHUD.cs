using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneHUD : MonoBehaviour
{
    [SerializeField]
    Text airspeed;
    [SerializeField]
    Text altitude;
    [SerializeField]
    Transform targetBox;
    [SerializeField]
    Transform hudCenter;
    [SerializeField]
    Transform velocityMarker;
    [SerializeField]
    Transform targetArrow;
    [SerializeField]
    Text targetRange;
    [SerializeField]
    float targetArrowThreshold;
    [SerializeField]
    Text scoreDisplay;
    [SerializeField]
    Checkpoint checkpoint;

    Plane plane;
    Transform planeTransform;
    new Camera camera;
    Transform cameraTransform;

    GameObject hudCenterGO;
    GameObject velocityMarkerGO;
    GameObject targetBoxGO;
    GameObject targetArrowGO;

    const float metersToKnots = 1.94384f;

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
    }
    public void SetCamera(Camera camera)
    {
        this.camera = camera;

        if (camera == null)
        {
            cameraTransform = null;
        }
        else
        {
            cameraTransform = camera.GetComponent<Transform>();
        }
    }
    void Start()
    {
        hudCenterGO = hudCenter.gameObject;
        velocityMarkerGO = velocityMarker.gameObject;
        targetBoxGO = targetBox.gameObject;
        targetArrowGO = targetArrow.gameObject;
    }

    void UpdateAirspeed()
    {
        var speed = plane.LocalVelocity.z * metersToKnots;
        airspeed.text = string.Format("{0:0}", speed);
    }
    void UpdateAltitude()
    {
        var altitude = plane.Rigidbody.position.y ;
        if (altitude <= 1000)
        {
            this.altitude.text = string.Format("{0:0} m", altitude);
        }
        else
        {
            this.altitude.text = string.Format("{0:0.0} km", altitude / 1000f);
        }
    }

    Vector3 TransformToHUDSpace(Vector3 worldSpace)
    {
        var screenSpace = camera.WorldToScreenPoint(worldSpace);
        return screenSpace - new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
    }

    void UpdateHUDCenter()
    {
        var rotation = cameraTransform.localEulerAngles;
        var hudPos = TransformToHUDSpace(cameraTransform.position + planeTransform.forward);

        if (hudPos.z > 0)
        {
            hudCenterGO.SetActive(true);
            hudCenter.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
            hudCenter.localEulerAngles = new Vector3(0, 0, -rotation.z);
        }
        else
        {
            hudCenterGO.SetActive(false);
        }
    }
    void UpdateVelocityMarker()
    {
        var velocity = planeTransform.forward;

        if (plane.LocalVelocity.sqrMagnitude > 1)
        {
            velocity = plane.Rigidbody.velocity;
        }

        var hudPos = TransformToHUDSpace(cameraTransform.position + velocity);

        if (hudPos.z > 0)
        {
            velocityMarkerGO.SetActive(true);
            velocityMarker.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
        }
        else
        {
            velocityMarkerGO.SetActive(false);
        }
    }
    void UpdateTargetArrow() 
    {
        var targetDistance = Vector3.Distance(plane.Rigidbody.position, checkpoint.transform.position);
        var targetPos = TransformToHUDSpace(checkpoint.transform.position);

        if (targetPos.z > 0)
        {
            targetBoxGO.SetActive(true);
            targetBox.localPosition = new Vector3(targetPos.x, targetPos.y, 0);
        }
        else
        {
            targetBoxGO.SetActive(false);
        }
        targetRange.text = string.Format("{0:0 m}", targetDistance);

        var targetDir = (checkpoint.transform.position - plane.Rigidbody.position).normalized;
        var targetAngle = Vector3.Angle(cameraTransform.forward, targetDir);

        if (targetAngle > targetArrowThreshold)
        {
            targetArrowGO.SetActive(true);
            //add 180 degrees if target is behind camera
            float flip = targetPos.z > 0 ? 0 : 180;
            targetArrow.localEulerAngles = new Vector3(0, 0, flip + Vector2.SignedAngle(Vector2.up, new Vector2(targetPos.x, targetPos.y)));
        }
        else
        {
            targetArrowGO.SetActive(false);
        }
    }

    void UpdateScore()
    {
        scoreDisplay.text = string.Format("Score: {0:0}", checkpoint.getScore());
    }

    void LateUpdate()
    {
        UpdateAirspeed();
        UpdateAltitude();
        UpdateHUDCenter();
        UpdateVelocityMarker();
        UpdateTargetArrow();
        UpdateScore();
    }
}
