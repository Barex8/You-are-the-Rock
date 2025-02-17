using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static Bullet instance;

    Rigidbody rb;
    private Transform cam;
    private CameraHandler camHandler;

    [SerializeField] private float minPushForce;
    [SerializeField] private float maxPushForce;
    [HideInInspector] public float actualpushForce;
    [SerializeField] private AnimationCurve chargeCurve;
    private float timeChargeCurve;
    private bool isCharging = false;
    private bool charged = false;

    [SerializeField] private float timeSlow;
    [SerializeField] private float antiGravityForce;

    private bool moved = false;   //Ha chocado y está en pausa
    //private bool hasStarted = true;  //Ha hecho el primer golpe, de momento no lo voy a usar, pero puede que cambie de opinión
    private bool targetHitted = false;

    [SerializeField] private AudioClip waterAudioClip;
    [SerializeField] private Transform waterParticles;

    [Header("Gizmo")]
    [SerializeField] private float maxDistance;

    private void Awake()
        {
        instance = this;
        }

    private void Start()
        {
        rb = GetComponent<Rigidbody>();
        camHandler = CameraHandler.instance;
        cam = camHandler.mainCamera;
        
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        }

    private void OnEnable()
        {
        if (rb != null)
            {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            moved = false;
            }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    void Update()
    {
    ForceMouseHandler();

    }

    private void OnCollisionEnter(Collision collision)
        {
        if (collision.transform.CompareTag("Enemy"))
            {
            collision.transform.GetComponent<Target>().GetHit();
            targetHitted = true;
            enabled = false;
            }
        else
            {
            transform.forward = collision.impulse;
            moved = false;
            StartCoroutine(SlowMov());
            }
        SoundManager.instance.PlaySound(collision.collider.sharedMaterial, true,false);

        }

    private void OnTriggerEnter(Collider other)
        {
        if (other.transform.CompareTag("Water") && !targetHitted)
            {
            //La camara se queda quieta unos segundos
            GameManager.instance.UnbindCamera();
            Instantiate(waterParticles, transform.position, Quaternion.Euler(-90, 0, 0));
            //Se espera y se reinicia
            StartCoroutine(ResetScene());

            SoundManager.instance.PlaySound(waterAudioClip, true);
            }
        }

    private IEnumerator ResetScene()
        {
        yield return new WaitForSeconds(3f);
        GameManager.instance.ResetScene();
        GameManager.instance.ResetCameraView();
        }

    private IEnumerator SlowMov()
        {
        yield return new WaitForSeconds(.1f);
        float elapsedTime = 0;
        rb.useGravity = false;
        Vector3 vel = rb.velocity;
        rb.velocity = vel / 10;

        //Espera un tiempo a "camara lenta", pero si pulsas click para volve a lanzarte se corta el bucle
        while (elapsedTime < timeSlow)
            {
            elapsedTime += Time.deltaTime;            
            if (Input.GetMouseButtonUp(0)) break;
            yield return null;
            }
        if (!moved) rb.velocity = vel;
        rb.useGravity = true;
        }

    /*private void FirstPush()
        {
        if (Input.GetMouseButtonDown(0))
            {
            rb.AddForce(cam.forward * maxPushForce, ForceMode.Impulse);
            moved = true;
            rb.useGravity = true;
            //hasStarted = true;
            }
        }       */

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
                //rb.velocity = Vector3.zero;
                rb.useGravity = true;
                rb.AddForce(cam.forward * actualpushForce, ForceMode.Impulse);
                moved = true;
                timeChargeCurve = 0;
                charged = false;
                isCharging = false;

                camHandler.EndCharging();
                }
            }
        }
    private void OnDrawGizmos()
        {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,maxDistance);
        }

    }
