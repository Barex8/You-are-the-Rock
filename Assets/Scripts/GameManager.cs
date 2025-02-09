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
    [SerializeField] private CinemachineFreeLook cFL;
    [SerializeField] private Volume blurVolume;

    private void Start()
        {
        Cursor.lockState = CursorLockMode.Locked;
        }

    private void Awake()
        {
        instance = this;
        }

    public void ChangeScene(string sceneName)
        {
        SceneManager.LoadScene(sceneName);
        }

    public void TargetHit()
        {
        Cursor.lockState = CursorLockMode.None;
        blurVolume.enabled = true;
        cFL.enabled = false;
        Bullet.instance.enabled = false;
        CameraHandler.instance.enabled = false;
        }

    }
