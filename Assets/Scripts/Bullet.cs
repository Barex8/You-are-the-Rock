using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    private Transform cam;
    private CameraHandler camHandler;

    [SerializeField] private float minPushForce;
    [SerializeField] private float maxPushForce;
    private float actualpushForce;
    [SerializeField] private AnimationCurve chargeCurve;
    private float timeChargeCurve;
    private bool isCharging = false;
    private bool charged = false;

    [SerializeField] private float timeSlow;
    [SerializeField] private float antiGravityForce;

    private bool moved = false;   //Ha chocado y est� en pausa
    private bool hasStarted = false;  //Ha hecho el primer golpe

    private void Start()
        {
        rb = GetComponent<Rigidbody>();
        camHandler = CameraHandler.instance;
        cam = camHandler.mainCamera;
        Cursor.lockState = CursorLockMode.Locked;
        
        rb.useGravity = false;
        }

    void Update()
    {
        if (!hasStarted)
            {
            FirstPush();
            }
        else
            {
            ForceMouseHandler();
            }
        }

    private void OnCollisionEnter(Collision collision)
        {
        if (collision.transform.CompareTag("Enemy"))
            {
            Destroy(collision.gameObject);
            }

        transform.forward = collision.impulse;
        moved = false;
        StartCoroutine(SlowMov());
        }

    private IEnumerator SlowMov()
        {
        float elapsedTime = 0;
        rb.useGravity = false;
        Vector3 vel = rb.velocity;
        print(vel);
        //rb.velocity = Vector3.zero;

        while (elapsedTime < timeSlow)
            {
            elapsedTime += Time.deltaTime;
            
            rb.AddForce(Vector3.up * antiGravityForce,ForceMode.Acceleration);
            
            if (Input.GetMouseButtonUp(0)) break;
            yield return null;
            }
        if (!moved) rb.velocity = vel;
        rb.useGravity = true;
        }

    private void FirstPush()
        {
        if (Input.GetMouseButtonDown(0))
            {
            rb.AddForce(cam.forward * maxPushForce, ForceMode.Acceleration);
            moved = true;
            rb.useGravity = true;
            hasStarted = true;
            }
        }
    private void ForceMouseHandler()
        {
        if (!moved)
            {
            if (Input.GetMouseButtonDown(0))
                {
                isCharging = true;
                camHandler.Charging();
                }

            if ((Input.GetMouseButtonUp(0) && isCharging) || chargeCurve.Evaluate(timeChargeCurve) >= 1) charged = true;

            if (isCharging)
                {
                rb.velocity = Vector3.zero;

                timeChargeCurve += Time.deltaTime;
                actualpushForce = Mathf.Lerp(minPushForce, maxPushForce, chargeCurve.Evaluate(timeChargeCurve));
                }
            if (charged)
                {
                camHandler.EndCharging();
                rb.velocity = Vector3.zero;
                rb.AddForce(cam.forward * actualpushForce, ForceMode.Acceleration);
                moved = true;
                timeChargeCurve = 0;
                rb.useGravity = true;
                charged = false;
                isCharging = false;
                }
            }
        }

    }
