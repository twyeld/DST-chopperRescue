using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class UpdateSliderValueInStart : MonoBehaviour
{
	private Slider slider;
	// Use this for initialization
	private void Start ()
	{
		slider = GetComponent<Slider>();
		if (slider == null) return;
		
		var value = slider.value;
		slider.value += 0.01f;
		slider.value -= 0.01f;
		slider.value = value;
	}
}
