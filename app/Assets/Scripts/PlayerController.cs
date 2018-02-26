using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TrialPlayerController : MonoBehaviour
{
	public string name;
	public bool externalPlayer;

	private void PlayClicked(string name)
	{
		if (!this.externalPlayer)
		{
			this.name = name;
			this.gameObject.transform.position = new Vector3(0, 0.5f, 0);
			this.gameObject.name = name;
		}
	}
}
