using System;
using UnityEngine;
using UnityEngine.UI;

// drop it onto the camera
// change new Rect (x, y, w, h) values to move text around


public class FPSCounterx : MonoBehaviour
{
	public Text fpsText;
	private const float FPS_UPDATE_INTERVAL = 0.5f;
	private float fpsAccum = 0;
	private int fpsFrames = 0;
	private float fpsTimeLeft = FPS_UPDATE_INTERVAL;
	private float fps = 0;
	private GUIStyle guiStyle = new GUIStyle(); 


    void Update()
	{
		fpsTimeLeft -= Time.deltaTime;
		fpsAccum += Time.timeScale / Time.deltaTime;
		fpsFrames++;

		if (fpsTimeLeft <= 0)
		{
			//fps = fpsAccum / fpsFrames;
			fps = (int)(1f / Time.unscaledDeltaTime);
			fpsTimeLeft = FPS_UPDATE_INTERVAL;
			fpsAccum = 0;
			fpsFrames = 0;
		}

		fpsText.text = "FPS: " + fps.ToString("f2");
	}

	/*
    void OnGUI()
	{
		//GUI.contentColor = Color.white;		
		guiStyle.fontSize = 24; //font size
		guiStyle.normal.textColor = Color.white; //font colour
		GUILayout.BeginArea(new Rect(240, 5, 500, 500));
		GUILayout.Label("FPS: " + fps.ToString("f2"), guiStyle);
		GUILayout.EndArea();
	}*/
}

