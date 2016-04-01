using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private bool paused = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;       
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && paused == false)
        {            
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && paused == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            paused = !paused;
        }
    }


}
