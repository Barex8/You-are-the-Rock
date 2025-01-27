using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler instance;
    public Transform mainCamera;

    [SerializeField] private GameObject particleCharging;
    private GameObject particle;
    private void Awake()
        {
        instance = this;
        }

    private void Start()
        {
        mainCamera = Camera.main.transform;
        }

    public void Charging()
        {
        if (particle == null)
            {
            particle = Instantiate(particleCharging, transform.position+transform.forward*6.5f,Quaternion.Euler(transform.eulerAngles.x,-transform.eulerAngles.y,transform.eulerAngles.z),transform);
            particle.transform.localEulerAngles =new Vector3(0, 180, 0);
            }
        }

    public void EndCharging()
        {
        particle.transform.parent = null;
        Destroy(particle);
        }
}
