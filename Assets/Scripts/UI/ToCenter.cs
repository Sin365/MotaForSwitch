using UnityEngine;

public class ToCenter : MonoBehaviour
{
	private void OnEnable()
	{
		this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, this.transform.localPosition.z);
	}
}
