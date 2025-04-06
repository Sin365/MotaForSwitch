using UnityEngine;

public class DebugOpen : MonoBehaviour {

	public Debugger debugger;

	void Awake()
	{
		//debugger.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		//按住上方向键+X建，再按下B键
		if (Input.GetKey(KeyCode.JoystickButton15) && Input.GetKey(KeyCode.JoystickButton3))
		{
			if (Input.GetKeyDown(KeyCode.JoystickButton0))
			{
				debugger.gameObject.SetActive(true);
			}
		}
	}
}
