using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public GameObject pause_menu;
    public static bool is_paused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause_menu.SetActive(false);
        is_paused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (is_paused){
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Pause(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pause_menu.SetActive(true);
        Time.timeScale = 0;

        is_paused = true;
    }
    public void Resume(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pause_menu.SetActive(false);
        Time.timeScale = 1f;

        is_paused = false;
    }

}
