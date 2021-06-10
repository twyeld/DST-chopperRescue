using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphCollection : MonoBehaviour
{
	public static GraphCollection instance;

	public GLGraph redGraph;
	public GLGraph blueGraph;
	public GLGraph disabledSelf;
	public GLGraph disabledTarget;

	private void Awake()
	{
		instance = this;
	}
}
