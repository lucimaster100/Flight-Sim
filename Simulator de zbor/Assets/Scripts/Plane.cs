using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {
    [SerializeField]
    float maxThrust;
    [SerializeField]
    float throttleSpeed;
    [SerializeField]
    float gLimit;
    [SerializeField]
    float gLimitPitch;

    [Header("Lift")]
    [SerializeField]
    float liftPower;
    [SerializeField]
    AnimationCurve liftAOACurve;
    [SerializeField]
    float inducedDrag;
    [SerializeField]
    AnimationCurve inducedDragCurve;
    [SerializeField]
    float rudderPower;
    [SerializeField]
    AnimationCurve rudderAOACurve;
    [SerializeField]
    AnimationCurve rudderInducedDragCurve;

    [Header("Steering")]
    [SerializeField]
    Vector3 turnSpeed;
    [SerializeField]
    Vector3 turnAcceleration;
    [SerializeField]
    AnimationCurve steeringCurve;

    [Header("Drag")]
    [SerializeField]
    AnimationCurve dragForward;
    [SerializeField]
    AnimationCurve dragBack;
    [SerializeField]
    AnimationCurve dragLeft;
    [SerializeField]
    AnimationCurve dragRight;
    [SerializeField]
    AnimationCurve dragTop;
    [SerializeField]
    AnimationCurve dragBottom;
    [SerializeField]
    Vector3 angularDrag;
    [SerializeField]
    float airbrakeDrag;
    
    [Header("Misc")]
    [SerializeField]
    List<GameObject> graphics;
    [SerializeField]
    GameObject crashEffect;
    [SerializeField]
    GameObject gameOverScreen;
    

    float throttleInput;
    Vector3 controlInput;

    Vector3 lastVelocity;

    public bool Dead { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public float throttle { get; private set; }
    public Vector3 EffectiveInput { get; private set; }
    public Vector3 velocity { get; private set; }
    public Vector3 localVelocity { get; private set; }
    public Vector3 LocalGForce { get; private set; }
    public Vector3 localAngularVelocity { get; private set; }
    public float angleOfAttack { get; private set; }
    public float angleOfAttackYaw { get; private set; }
    public bool AirbrakeDeployed { get; private set; }

    

    void Start() {
        Rigidbody = GetComponent<Rigidbody>();
        throttle = 0.5f;
    }

    public void SetThrottleInput(float input) {
        if (Dead) return;
        throttleInput = input;
    }

    public void SetControlInput(Vector3 input) {
        if (Dead) return;
        controlInput = input;
    }

    void Die() {
        throttleInput = 0;
        throttle = 0;
        Dead = true;

        foreach (var go in graphics) {
            go.SetActive(false);
        }
        
    }

    void UpdateThrottle(float dt) {
        float target = 0;
        if (throttleInput > 0) target = 1;

        throttle = Utilities.IncrementalMove(throttle, target, throttleSpeed * Mathf.Abs(throttleInput), dt);

        AirbrakeDeployed = throttle == 0 && throttleInput == -1;
        
    }

    void CalculateAngleOfAttack() {
        if (localVelocity.sqrMagnitude < 0.1f) {
            angleOfAttack = 0;
            angleOfAttackYaw = 0;
            return;
        }

        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);
        angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z);
    }

    void CalculateGForce(float dt) {
        var invRotation = Quaternion.Inverse(Rigidbody.rotation);
        var acceleration = (velocity - lastVelocity) / dt;
        LocalGForce = invRotation * acceleration;
        lastVelocity = velocity;
    }

    void CalculateState(float dt) {
        var invRotation = Quaternion.Inverse(Rigidbody.rotation);
        velocity = Rigidbody.velocity;
        localVelocity = invRotation * velocity;
        localAngularVelocity = invRotation * Rigidbody.angularVelocity;

        CalculateAngleOfAttack();
    }

    void UpdateThrust() {
        Rigidbody.AddRelativeForce(throttle * maxThrust * Vector3.forward);
    }

    void UpdateDrag() {
        var localVelocity = this.localVelocity;
        var localVelocitySq = localVelocity.sqrMagnitude;

        float airbrakeDrag = AirbrakeDeployed ? this.airbrakeDrag : 0;

        var coefficient = Utilities.ModifiedScale(
            localVelocity.normalized,
            dragRight.Evaluate(Mathf.Abs(localVelocity.x)), dragLeft.Evaluate(Mathf.Abs(localVelocity.x)),
            dragTop.Evaluate(Mathf.Abs(localVelocity.y)), dragBottom.Evaluate(Mathf.Abs(localVelocity.y)),
            dragForward.Evaluate(Mathf.Abs(localVelocity.z)) + airbrakeDrag,
            dragBack.Evaluate(Mathf.Abs(localVelocity.z))
        );

        var drag = coefficient.magnitude * localVelocitySq * -localVelocity.normalized;

        Rigidbody.AddRelativeForce(drag);
    }

    Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve, AnimationCurve inducedDragCurve) {
        var liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);
        var liftVelocitySq = liftVelocity.sqrMagnitude;

        var liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        var liftForce = liftVelocitySq * liftCoefficient * liftPower;

        var liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        var lift = liftDirection * liftForce;

        var dragForce = liftCoefficient * liftCoefficient;
        var dragDirection = -liftVelocity.normalized;
        var inducedDrag = dragDirection * liftVelocitySq * dragForce * this.inducedDrag * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z));

        return lift + inducedDrag;
    }

    void UpdateLift() {
        if (localVelocity.sqrMagnitude < 1f) return;


        var liftForce = CalculateLift(
            angleOfAttack , Vector3.right,
            liftPower,
            liftAOACurve,
            inducedDragCurve
        );

        var yawForce = CalculateLift(angleOfAttackYaw, Vector3.up, rudderPower, rudderAOACurve, rudderInducedDragCurve);

        Rigidbody.AddRelativeForce(liftForce);
        Rigidbody.AddRelativeForce(yawForce);
    }

    void UpdateAngularDrag() {
        var angularVelocity = localAngularVelocity;
        var drag = angularVelocity.sqrMagnitude * -angularVelocity.normalized;
        Rigidbody.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);
    }

    Vector3 CalculateGForce(Vector3 angularVelocity, Vector3 velocity) {
        return Vector3.Cross(angularVelocity, velocity);
    }

    Vector3 CalculateGForceLimit(Vector3 input) {
        return Utilities.ModifiedScale(input,
            gLimit, gLimitPitch,
            gLimit, gLimit,
            gLimit, gLimit
        ) * 9.81f;
    }

    float CalculateGLimiter(Vector3 controlInput, Vector3 maxAngularVelocity) {
        if (controlInput.magnitude < 0.01f) {
            return 1;
        }

        var maxInput = controlInput.normalized;

        var limit = CalculateGForceLimit(maxInput);
        var maxGForce = CalculateGForce(Vector3.Scale(maxInput, maxAngularVelocity), localVelocity);

        if (maxGForce.magnitude > limit.magnitude) {
            return limit.magnitude / maxGForce.magnitude;
        }

        return 1;
    }

    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration) {
        var error = targetVelocity - angularVelocity;
        var accel = acceleration * dt;
        return Mathf.Clamp(error, -accel, accel);
    }

    void UpdateSteering(float dt) {
        var speed = Mathf.Max(0, localVelocity.z);
        var steeringPower = steeringCurve.Evaluate(speed);

        var gForceScaling = CalculateGLimiter(controlInput, turnSpeed * Mathf.Deg2Rad * steeringPower);

        var targetAngularVelocity = Vector3.Scale(controlInput, turnSpeed * steeringPower * gForceScaling);
        var angularVelocity = localAngularVelocity * Mathf.Rad2Deg;

        var correction = new Vector3(
            CalculateSteering(dt, angularVelocity.x, targetAngularVelocity.x, turnAcceleration.x * steeringPower),
            CalculateSteering(dt, angularVelocity.y, targetAngularVelocity.y, turnAcceleration.y * steeringPower),
            CalculateSteering(dt, angularVelocity.z, targetAngularVelocity.z, turnAcceleration.z * steeringPower)
        );

        Rigidbody.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);

        var correctionInput = new Vector3(
            Mathf.Clamp((targetAngularVelocity.x - angularVelocity.x) / turnAcceleration.x, -1, 1),
            Mathf.Clamp((targetAngularVelocity.y - angularVelocity.y) / turnAcceleration.y, -1, 1),
            Mathf.Clamp((targetAngularVelocity.z - angularVelocity.z) / turnAcceleration.z, -1, 1)
        );

        var scaledInput = (correctionInput + controlInput) * gForceScaling;

        EffectiveInput = new Vector3(
            Mathf.Clamp(scaledInput.x, -1, 1),
            Mathf.Clamp(scaledInput.y, -1, 1),
            Mathf.Clamp(scaledInput.z, -1, 1)
        );
    }

    void FixedUpdate() {
        float dt = Time.fixedDeltaTime;

        CalculateState(dt);
        CalculateGForce(dt);

        UpdateThrottle(dt);

        UpdateThrust();
        UpdateLift();

        if (!Dead) {
            UpdateSteering(dt);
        }

        UpdateDrag();
        UpdateAngularDrag();

        CalculateState(dt);
    }

    void OnCollisionEnter(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            var contact = collision.contacts[i];

            Die();

            Rigidbody.isKinematic = true;
            Rigidbody.position = contact.point;
            Rigidbody.rotation = Quaternion.Euler(0, Rigidbody.rotation.eulerAngles.y, 0);

            crashEffect.SetActive(true);
            gameOverScreen.SetActive(true);
        }
    }
}
