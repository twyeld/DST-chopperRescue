using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetFrameRate: MonoBehaviour {

    void Start () {


    //Screen.SetResolution (1920, 1080, false);
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = 1000;

		}
	}
