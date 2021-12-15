using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [System.Serializable]
    public class SceneToLoad
    {
        public string SceneName;
        public bool Load;
        [HideInInspector] public bool IsLoaded;
    }

    public SceneToLoad[] Scenes;

    private void Update()
    {
        foreach(var s in Scenes)
        {
            if(s.Load && !s.IsLoaded)
            {
                SceneManager.LoadScene(s.SceneName, LoadSceneMode.Additive);
                s.IsLoaded = true;
            }
            else if(!s.Load && s.IsLoaded)
            {
                SceneManager.UnloadSceneAsync(s.SceneName);
                s.IsLoaded = false;
            }
        }
    }
}
