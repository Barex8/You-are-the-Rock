using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Transition Scenes")]
    [SerializeField] public CinemachineFreeLook cFL;
    [SerializeField] private Volume blurVolume;

    [SerializeField] private bool lockCursor;

    private void Start()
        {
        if(lockCursor)Cursor.lockState = CursorLockMode.Locked;
        }

    private void Awake()
        {
        instance = this;
        }

    public void ChangeScene(string sceneName)
        {
        SceneManager.LoadScene(sceneName);
        }

    public void ResetScene()
        {
        Bullet.instance.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        blurVolume.enabled = false;
        cFL.enabled = true;
        Bullet.instance.enabled = true;
        Bullet.instance.transform.position = new Vector3(0, 5.37f, 0);
        CameraHandler.instance.enabled = true;
        }

    public void TargetHit()
        {
        Cursor.lockState = CursorLockMode.None;
        blurVolume.enabled = true;
        cFL.enabled = false;
        Bullet.instance.enabled = false;
        CameraHandler.instance.enabled = false;
        }

    public void UnbindCamera()
        {
        cFL.enabled = false;
        Bullet.instance.enabled = false;
        CameraHandler.instance.enabled = false;
        }

    public void ResetCameraView()
        {
        cFL.m_YAxis.Value = .5f;
        cFL.m_XAxis.Value = 0;
        }

    //Menú
    public void ContinueBt()
        {
        SceneManager.LoadScene("Level-1 1");
        }

    }
