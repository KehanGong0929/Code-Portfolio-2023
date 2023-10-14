using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DisplayScene : MonoBehaviour {

    public enum Scenes {
        Serenity,
        Joy,
        Stress
    }

    [System.Serializable]
    public struct SceneInformation {
        public Scenes name;
        public GameObject scene;
        public Material skybox;
        public LightingSettings lightingSettings;
        public float sceneLaneCentre;
    }

    public SceneInformation[] sceneInformation;
    [Range(0, 2)]
    public int selectedScene = 0;
    [HideInInspector]
    public float serenityLaneCentre;
    [HideInInspector]
    public float joyLaneCentre;
    [HideInInspector]
    public float stressLaneCentre;
    
    private SceneInformation serenitySceneInformation;
    private SceneInformation joySceneInformation;
    private SceneInformation stressSceneInformation;

    private GameObject currentScene;
    
    private bool isSerenity;
    private bool isJoy;
    private bool isStress;

    private Scenes lastScene;

    private SoundManager soundManager;
    private GameObject playerObject;
    private Camera mainCamera;
    private GameObject environmentObject;
    private GameObject lighting;
    // public GameObject startTrigger;
    // public GameObject completeTrigger;


    void Start() {

        foreach (SceneInformation info in sceneInformation) {
            switch (info.name) {
                case Scenes.Serenity:
                    serenitySceneInformation = info;
                    break;

                case Scenes.Joy:
                    joySceneInformation = info;
                    break;

                case Scenes.Stress:
                    stressSceneInformation = info;
                    break;
            }
        }

        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        environmentObject = GameObject.FindGameObjectWithTag("Environment");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        lighting = GameObject.FindGameObjectWithTag("Lighting");

        serenityLaneCentre = serenitySceneInformation.sceneLaneCentre;
        joyLaneCentre = joySceneInformation.sceneLaneCentre;
        stressLaneCentre = stressSceneInformation.sceneLaneCentre;

        if (selectedScene == 0) {
            isSerenity = true;
            isJoy = false;
            isStress = false;
        }

        if (selectedScene == 1) {
            isSerenity = false;
            isJoy = true;
            isStress = false;
        }
        
        if (selectedScene == 2) {
            isSerenity = false;
            isJoy = false;
            isStress = true;
        }

        if (isSerenity) {
            currentScene = (GameObject)Instantiate(serenitySceneInformation.scene, new Vector3(-2500, 0, 0), Quaternion.identity, environmentObject.transform);
            currentScene.SetActive(true);

            mainCamera.farClipPlane = 750f;

            playerObject.transform.position = new Vector3(-3250, 0, -500);
            // Vector3 triggerPosition = new Vector3(-3250, 0, -500);
            // GameObject triggerInstance = Instantiate(startTrigger, triggerPosition, Quaternion.identity);
            // triggerInstance.transform.SetParent(currentScene.transform);

            RenderSettings.skybox = serenitySceneInformation.skybox;
            lighting.transform.eulerAngles = new Vector3(60, 165, 0);
            Lightmapping.lightingSettings = serenitySceneInformation.lightingSettings;

            lastScene = Scenes.Serenity;

            soundManager.PlaySerenity();
        }

        if (isJoy) {
            currentScene = (GameObject)Instantiate(joySceneInformation.scene, new Vector3(0, 0, 0), Quaternion.identity, environmentObject.transform);
            currentScene.SetActive(true);

            mainCamera.farClipPlane = 1000f;
            
            playerObject.transform.position = new Vector3(-750, 0, -500);

            RenderSettings.skybox = joySceneInformation.skybox;
            lighting.transform.eulerAngles = new Vector3(30, 180, 0);
            Lightmapping.lightingSettings = joySceneInformation.lightingSettings;

            lastScene = Scenes.Joy;

            soundManager.PlayJoy();
        }
        
        if (isStress) {
            currentScene = (GameObject)Instantiate(stressSceneInformation.scene, new Vector3(2500, 0, 0), Quaternion.identity, environmentObject.transform);
            currentScene.SetActive(true);

            mainCamera.farClipPlane = 500f;
            
            playerObject.transform.position = new Vector3(-1750, 0, -500);

            RenderSettings.skybox = stressSceneInformation.skybox;
            lighting.transform.eulerAngles = new Vector3(50, 200, 0);
            lighting.SetActive(false);
            Lightmapping.lightingSettings = stressSceneInformation.lightingSettings;

            lastScene = Scenes.Stress;

            soundManager.PlayStress();
        }
    }

    void Update() {
        if (selectedScene == 0) {
            isSerenity = true;
            isJoy = false;
            isStress = false;
        }

        if (selectedScene == 1) {
            isSerenity = false;
            isJoy = true;
            isStress = false;
        }

        if (selectedScene == 2) {
            isSerenity = false;
            isJoy = false;
            isStress = true;
        }

        if (isSerenity && lastScene != Scenes.Serenity) {
            soundManager.StopSounds();
            soundManager.PlaySerenity();

            Destroy(currentScene);
            currentScene = (GameObject)Instantiate(serenitySceneInformation.scene, new Vector3(-2500, 0, 0), Quaternion.identity, environmentObject.transform);
            currentScene.SetActive(true);

            mainCamera.farClipPlane = 750f;

            playerObject.transform.position = new Vector3(-3250, 0, -500);

            RenderSettings.skybox = serenitySceneInformation.skybox;
            lighting.SetActive(true);
            lighting.transform.eulerAngles = new Vector3(60, 165, 0);
            Lightmapping.lightingSettings = serenitySceneInformation.lightingSettings;

            lastScene = Scenes.Serenity;
        }

        if (isJoy && lastScene != Scenes.Joy) {
            soundManager.StopSounds();
            soundManager.PlayJoy();

            Destroy(currentScene);
            currentScene = (GameObject)Instantiate(joySceneInformation.scene, new Vector3(0, 0, 0), Quaternion.identity, environmentObject.transform);
            currentScene.SetActive(true);

            mainCamera.farClipPlane = 1000f;
            
            playerObject.transform.position = new Vector3(-750, 0, -500);

            RenderSettings.skybox = joySceneInformation.skybox;
            lighting.SetActive(true);
            lighting.transform.eulerAngles = new Vector3(30, 180, 0);
            Lightmapping.lightingSettings = joySceneInformation.lightingSettings;

            lastScene = Scenes.Joy;
        }
        
        if (isStress && lastScene != Scenes.Stress) {
            soundManager.StopSounds();
            soundManager.PlayStress();

            Destroy(currentScene);
            currentScene = (GameObject)Instantiate(stressSceneInformation.scene, new Vector3(2500, 0, 0), Quaternion.identity, environmentObject.transform);
            currentScene.SetActive(true);

            mainCamera.farClipPlane = 500f;
            
            playerObject.transform.position = new Vector3(-1750, 0, -500);

            RenderSettings.skybox = stressSceneInformation.skybox;
            lighting.SetActive(false);

            lastScene = Scenes.Stress;
        }
    }
}
