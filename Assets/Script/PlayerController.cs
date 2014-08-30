using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	protected List<string> playID = new List<string>();

	// Use this for initialization
	void Start () 
	{
		playID.Clear();
		foreach (var kp in MessageCenter.Instance.mPlayers)
		{
			playID.Add(kp.Key);
		}

		//MessageCenter.Instance.mPlayerTeam
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GreatePlayers()
	{
		for(int i = 0; i < playID.Count; i++)
		{
			if(MessageCenter.Instance.mPlayerTeam.ContainsKey(playID[i]))
			{

			}
		}
	}
}
