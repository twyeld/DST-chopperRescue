using UnityEngine;
using UnityEngine.UI;

public class TextUpater : MonoBehaviour
{
	private Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	public void SetText(float value)
	{
		text.text = value.ToString("0.000");
	}
}
