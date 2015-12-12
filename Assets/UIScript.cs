using UnityEngine;
using System.Collections;

public class UIScript : MonoBehaviour {

	public void UIStart()
    {
        Application.LoadLevel("Tuukka");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
