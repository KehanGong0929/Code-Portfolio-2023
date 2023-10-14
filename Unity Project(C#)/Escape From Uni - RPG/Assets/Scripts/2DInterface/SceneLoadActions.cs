using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadActions : MonoBehaviour
{
    [SerializeField] private Transform player; //drag player reference onto here
    private Vector3 targetPosition2 = new Vector3(1.09000003f, 1.64999998f, -23.2600002f); //here you store the position you want to teleport your player to
    private Vector3 targetPosition3; //here you store the position you want to teleport your player to

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded; //You add your method to the delegate
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    //After adding this method to the delegate, this method will be called every time
    //that a new scene is loaded. You can then compare the scene loaded to your desired
    //scenes and do actions according to the scene loaded.
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 2) //use your desired check here to compare your scene
            player.position = targetPosition2;
        if (scene.buildIndex == 3)
            player.position = targetPosition3;
    }
}