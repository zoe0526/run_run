using run_run;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum scene_name
{
    LOGINScene,
    LOADINGScene,
    MAIN,

}
public class SceneController : MonoBehaviour
{

    public static SceneController Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
 
    public void load_scene(scene_name scene)
    {
        SceneManager.LoadScene(scene.ToString());

    }
}
