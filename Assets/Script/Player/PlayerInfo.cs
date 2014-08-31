using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

	public UISprite m_icon;
	public bool isKnocked = false;
	public float knockTime = 0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isKnocked)
		{
			knockTime -= Time.deltaTime;
			if(knockTime <= 0)
			{
				knockTime = 0;
				isKnocked = false;
			}
		}
	}
}
