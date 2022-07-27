using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : HoldableItem
{
    [Space(10), Header("Weapon")]
    [SerializeField] private TextAsset recoilFile;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform pivot;
    [SerializeField] private float cadence;
    [SerializeField] private int magazineSize;
    [SerializeField] private int magazineContent;
    [SerializeField] private float zOffsetTarget;

    private float nextShot;
    private float zOffset;
    private float currentZOffset;
    private Vector3 pivotOrigin;
    private int recoilStep;
    private bool canShot = true;

    private List<Vector2> recoils = new List<Vector2>();

    public override void Awake()
    {
        base.Awake();

        LoadRecoils();
        
        //Define start rotation
        pivot.localRotation = Quaternion.Euler(30, 0, 0);
    }

    public override void Update()
    {
        base.Update();

        HandleReload();

        HandleShot();
        HandleRecoilAnim();
    }

    public void SetCanShot(bool val)
    {
        canShot = val;
    }

    void LoadRecoils()
    {
        string input = recoilFile.text;

        string[] steps = input.Split("\n");

        foreach (var v in steps)
        {
            string[] vals = v.Split(", ");

            recoils.Add(new Vector2(float.Parse(vals[0]), float.Parse(vals[1])));
        }
    }

    void ValidateReload()
    {
        magazineContent = magazineSize;
    }

    void HandleReload()
    {
        if (!InputManager.Instance.Input.PlayerGround.Reload.triggered)
            return;

        if (magazineContent == magazineSize)
            return;

        //TODO: Start an animation who call ValidateReload()
    }

    void HandleRecoilAnim()
    {
        zOffset = Mathf.Lerp(zOffset, 0, Time.deltaTime * 25f);
        currentZOffset = Mathf.Lerp(currentZOffset, zOffset, Time.deltaTime * 30f);

        root.localPosition = new Vector3(root.localPosition.x, root.localPosition.y, currentZOffset);
    }

    void HandleShot()
    {
        if (nextShot > 0)
            nextShot -= Time.deltaTime;

        if (InputManager.Instance.Input.PlayerGround.Fire.ReadValue<float>() < .3f || magazineContent < 1)
        {
            recoilStep = 0;

            return;
        }

        if (nextShot <= 0)
        {
            nextShot = cadence;
            zOffset = zOffsetTarget;

            Fire();

            recoilStep++;
        }
    }

    void Fire()
    {
        magazineContent--;

        GameObject go = Instantiate(bullet);
        Transform camTransform = owner.GetCamera().transform;

        go.transform.position = camTransform.position + camTransform.forward;
        go.transform.rotation = camTransform.rotation;

        go.GetComponent<Bullet>().Shot(camTransform.forward);

        StopAllCoroutines();
        StartCoroutine(Recoil());
    }

    public override void UpdateAnimationStates()
    {
        animator.SetBool("IsSprinting", owner.IsSprinting && owner.IsMoving);
        animator.SetBool("IsAiming", owner.IsAiming);
    }

    public override Vector3 ComputeMovingOffsets()
    {
        xRotationOffset = InputManager.Instance.Input.PlayerGround.Movement.ReadValue<Vector2>().x + InputManager.Instance.Input.PlayerGround.Look.ReadValue<Vector2>().x * mouseSensibility * owner.Sensibility.x;
        yRotationOffset = owner.IsAiming ? 0 : InputManager.Instance.Input.PlayerGround.Movement.ReadValue<Vector2>().y + InputManager.Instance.Input.PlayerGround.Look.ReadValue<Vector2>().y * mouseSensibility * owner.Sensibility.y;

        xRotationOffset = Mathf.Clamp(xRotationOffset, xRotationLimits.x, xRotationLimits.y);
        yRotationOffset = Mathf.Clamp(yRotationOffset, yRotationLimits.x, yRotationLimits.y);

        if (owner.IsSprinting && owner.IsMoving)
        {
            xOffset = Mathf.Cos(Time.time * sprintSpeedX) * sprintForce;
            yOffset = Mathf.Cos(Time.time * sprintSpeedY) * sprintForce;

            return new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }
        
        if (owner.IsCrouching && owner.IsMoving)
        {
            xOffset = Mathf.Cos(Time.time * crouchSpeedX) * (owner.IsAiming ? crouchForce / 4 : crouchForce);
            yOffset = Mathf.Cos(Time.time * crouchSpeedY) * (owner.IsAiming ? crouchForce / 4 : crouchForce);

            return new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }
        
        if (owner.IsWalking)
        {
            xOffset = Mathf.Cos(Time.time * walkSpeedX) * (owner.IsAiming ? walkForce / 2.5f : walkForce);
            yOffset = Mathf.Cos(Time.time * walkSpeedY) * (owner.IsAiming ? walkForce / 2.5f : walkForce);

            return new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }

        return origin;
    }

    //Lifetime is used to animate gun equip (Animator will override sway if used itself)
    public override void ApplyMovingOffsets(Vector3 destination)
    {
        root.localPosition = Vector3.Lerp(root.localPosition, destination, Time.deltaTime * (owner.IsAiming ? 10f : 5f));
        root.localRotation = Quaternion.Lerp(root.localRotation, Quaternion.Euler(yRotationOffset * yRotationForce, 0, -xRotationOffset * xRotationForce), Time.deltaTime * (owner.IsAiming ? 10f : 5f));
    }

    IEnumerator Recoil()
    {
        float t = 0;

        Vector2 target = new Vector2(recoils[recoilStep].x * 80f, recoils[recoilStep].y * 10f);

        while (t < cadence)
        {
            t += Time.deltaTime;

            owner.ForceMoveLook(target * (Mathf.Cos(t / cadence) * Time.deltaTime));

            yield return null;
        }
    }
}
