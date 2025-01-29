using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler instance;
    public Transform mainCamera;

    private Bullet bullet;
    [SerializeField] GameObject bulletPrediction;

    [SerializeField] private GameObject particleCharging;
    private GameObject particle;

    private ProjectileThrow projectileThrow;
    private void Awake()
        {
        instance = this;
        }

    private void Start()
        {
        mainCamera = Camera.main.transform;
        bullet = Bullet.instance;
        projectileThrow = transform.GetChild(0).GetComponent<ProjectileThrow>();
        projectileThrow.enabled = false;
        }
    private void Update()
        {
        if(particle != null)bullet.transform.rotation = transform.rotation;
        }

    public void Charging()
        {
        if (particle == null)
            {
            projectileThrow.enabled = true;
            particle = Instantiate(particleCharging, transform.position+transform.forward*6.5f,Quaternion.Euler(transform.eulerAngles.x,-transform.eulerAngles.y,transform.eulerAngles.z),transform);
            particle.transform.localEulerAngles =new Vector3(0, 180, 0);
            }
        }

    public void EndCharging()
        {
        particle.transform.parent = null;
        Destroy(particle);
        projectileThrow.enabled = false;
        }
}
