using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {

    private static MySceneManager _instance;

    public static MySceneManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MySceneManager>();
                if(_instance == null)
                {
                    GameObject tempGO = new GameObject("MySceneManager");
                    _instance = tempGO.AddComponent<MySceneManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLevelByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
