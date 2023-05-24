using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    [SerializeField]
    GameObject afterburnerGO;
    [SerializeField]
    float afterburnerThreshold;
    [SerializeField]
    float afterburnerMinSize;
    [SerializeField]
    float afterburnerMaxSize;
    [SerializeField]
    float maxAileronDeflection;
    [SerializeField]
    float maxElevatorDeflection;
    [SerializeField]
    float maxRudderDeflection;
    [SerializeField]
    float airbrakeDeflection;
    [SerializeField]
    float deflectionSpeed;
    [SerializeField]
    Transform rightAileron;
    [SerializeField]
    Transform leftAileron;
    [SerializeField]
    Transform elevators;
    [SerializeField]
    Transform rudder;
    [SerializeField]
    Transform airbrakeTop;
    [SerializeField]
    Transform airbrakeBottom;

    Plane plane;
    Dictionary<Transform, Quaternion> neutralPoses;
    Vector3 deflection;
    float airbrakePosition;
    void Start()
    {
        plane = GetComponent<Plane>();

        neutralPoses = new Dictionary<Transform, Quaternion>();

        AddNeutralPose(leftAileron);
        AddNeutralPose(rightAileron);
        AddNeutralPose(rudder);
        AddNeutralPose(elevators);
        AddNeutralPose(airbrakeTop);
        AddNeutralPose(airbrakeBottom);
    }

    void AddNeutralPose(Transform transform)
    {
        neutralPoses.Add(transform, transform.localRotation);
    }

    void UpdateControlSurfaces(float dt)
    {
        var input = plane.EffectiveInput;

        deflection.x = Utilities.IncrementalMove(deflection.x, input.x, deflectionSpeed, dt, -1, 1);
        deflection.y = Utilities.IncrementalMove(deflection.y, input.y, deflectionSpeed, dt, -1, 1);
        deflection.z = Utilities.IncrementalMove(deflection.z, input.z, deflectionSpeed, dt, -1, 1);

        rightAileron.localRotation = neutralPoses[rightAileron] * Quaternion.Euler(deflection.z * maxAileronDeflection, 0, 0);
        leftAileron.localRotation = neutralPoses[leftAileron] * Quaternion.Euler(-deflection.z * maxAileronDeflection, 0, 0);
        elevators.localRotation = neutralPoses[elevators] * Quaternion.Euler(deflection.x * maxElevatorDeflection, 0, 0);
        rudder.localRotation = neutralPoses[rudder] * Quaternion.Euler(0, -deflection.y * maxRudderDeflection, 0);

    }
    void UpdateAirbrakes(float dt)
    {
        var target = plane.AirbrakeDeployed ? 1 : 0;

        airbrakePosition = Utilities.IncrementalMove(airbrakePosition, target, deflectionSpeed, dt);

        airbrakeTop.localRotation = neutralPoses[airbrakeTop]* Quaternion.Euler(airbrakePosition * airbrakeDeflection, 0, 0);
        airbrakeBottom.localRotation= neutralPoses[airbrakeTop] * Quaternion.Euler(-airbrakePosition * airbrakeDeflection, 0, 0);
    }

    void UpdateAfterburner()
    {
        float throttle = plane.throttle;
        float afterburnerT = Mathf.Clamp01(Mathf.InverseLerp(afterburnerThreshold, 1, throttle));
        float size = Mathf.Lerp(afterburnerMinSize, afterburnerMaxSize, afterburnerT);

        if (throttle >= afterburnerThreshold)
        {
                afterburnerGO.SetActive(true);
                afterburnerGO.transform.localScale = new Vector3(size, size, size);
        }
        else
        {
                afterburnerGO.SetActive(false);
        }
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;

        UpdateAfterburner();
        UpdateControlSurfaces(dt);
        UpdateAirbrakes(dt);
    }
}
