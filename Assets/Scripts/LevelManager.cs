using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    
    private bool gameHasEnded = false;
    [SerializeField] private float restartLevelDelay = 5.0f;

    public void EndGame(){
        if(!gameHasEnded){
            gameHasEnded = true;
            Invoke("Restart", restartLevelDelay);
        }
    }

    void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
