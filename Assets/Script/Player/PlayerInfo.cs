using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

	public UISprite m_icon;
	public bool isKnocked = false;
	public bool isPulling = false;
	public float knockTime = 0f;
	public float pullingTime = 0f;
	
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
		if(isPulling)
		{
			pullingTime -= Time.deltaTime;
			if(pullingTime <= 0)
			{
				pullingTime = 0;
				isPulling = false;
			}
		}
		else
		{
			pullingTime = 0;
		}
	}
}
