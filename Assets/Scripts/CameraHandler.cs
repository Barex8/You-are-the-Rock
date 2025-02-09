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

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    [SerializeField] private LayerMask playerMask;
    private GameObject objTransparent;

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

        mpb = new MaterialPropertyBlock();   //to set Transparent objects at the front of the object
        }
    private void Update()
        {
        if(particle != null)bullet.transform.rotation = transform.rotation;
        CheckObjectInFront();
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

    private void CheckObjectInFront()
        {
        Vector3 direction = (bullet.transform.position-transform.position);
        if (Physics.Raycast(transform.position,direction,out RaycastHit hit))
            {
            if (hit.transform.gameObject.layer != playerMask)
                {
                if(objTransparent != hit.transform.gameObject)
                    {
                    SetObjectTransparency(hit.transform.gameObject, 0);
                    if(objTransparent)SetObjectTransparency(objTransparent, 1);

                    objTransparent = hit.transform.gameObject;
                    }
                }
            else
                {
                if(objTransparent)SetObjectTransparency(objTransparent, 1);
                objTransparent = null;
                }
            }
        Debug.DrawRay(transform.position, direction,Color.red);
        }
    private void SetObjectTransparency(GameObject gO,float alpha)
        {
        rend = gO.GetComponent<Renderer>();
        rend.GetPropertyBlock(mpb);
        mpb.SetFloat("_Alpha", alpha);
        rend.SetPropertyBlock(mpb);
        }
}
