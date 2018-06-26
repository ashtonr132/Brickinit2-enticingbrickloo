using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    [SerializeField]
    GameObject[] But;
	// Use this for initialization
	void Start () {
        But[0].GetComponent<Button>().onClick.AddListener(delegate { MenuButtons(But[0].name); });
        But[1].GetComponent<Button>().onClick.AddListener(delegate { MenuButtons(But[1].name); });
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }

    private void MenuButtons(string name)
    {
        switch (name)
        {
            case "Start":
                UnityEngine.SceneManagement.SceneManager.LoadScene("brickinit");
                break;
            case "Exit":
                Application.Quit();
                break;
        }
    }
}
