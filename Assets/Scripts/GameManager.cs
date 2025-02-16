using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using DialogueEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Vector3 initialPos;

    [Header("Transition Scenes")]
    [SerializeField] public CinemachineFreeLook cFL;
    [SerializeField] private Volume blurVolume;

    [SerializeField] private bool lockCursor;

    [SerializeField] private NPCConversation startConversation;

    [Header("Settings Stuff")]
    [SerializeField] private TMP_Dropdown ddResolution;
    Resolution[] resolutions;
    [SerializeField] private Toggle toggleFullscreen;

    [SerializeField] private Button continueBt;


    private void Start()
        {
        if(lockCursor)Cursor.lockState = CursorLockMode.Locked;
        if (startConversation)
            {
            ConversationManager.Instance.StartConversation(startConversation);
            UnbindCamera();
            }
        if(Bullet.instance != null)initialPos = Bullet.instance.transform.position;

        }

    private void Awake()
        {
        instance = this;
        if (ddResolution != null) ManagerResolution();
        if(continueBt != null && PlayerPrefs.GetString("Level") == "") continueBt.interactable = false;

        Screen.fullScreen = PlayerPrefs.GetInt("fullscreen") == 1 ? true : false;
        }

    public void ChangeScene(string sceneName)
        {
        SceneManager.LoadScene(sceneName);
        PlayerPrefs.SetString("Level", sceneName);
        }

    public void ResetScene()
        {
        Bullet.instance.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        blurVolume.enabled = false;
        cFL.enabled = true;
        Bullet.instance.enabled = true;
        Bullet.instance.transform.position = initialPos;
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
        SceneManager.LoadScene(PlayerPrefs.GetString("Level"));
        }

    public void NewGame()
        {
        PlayerPrefs.SetString("Level", "Level-1 1");
        SceneManager.LoadScene(PlayerPrefs.GetString("Level"));
        //Borrar partida anterior
        }

    public void Settings()
        {
         //Abre la ventana de Settings
        }

    public void Exit()
        {
        Application.Quit();
        }


    void ManagerResolution()
        {
        resolutions = Screen.resolutions;             //Posibles resoluciones de unity
        ddResolution.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
            {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                if (PlayerPrefs.GetInt("firstResolution") != 1) currentResolutionIndex = i;
                else currentResolutionIndex = PlayerPrefs.GetInt("indexResolution");

                }
            }

        ddResolution.AddOptions(options);
        ddResolution.value = currentResolutionIndex;
        ddResolution.RefreshShownValue();

        PlayerPrefs.SetInt("firstResolution", 1);
        }

    public void SetResolution(int resolutionIndex)
        {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("indexResolution", resolutionIndex);
        }

    
    public void SetFullscreen(bool isFullscreen)
        {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        }

    }
