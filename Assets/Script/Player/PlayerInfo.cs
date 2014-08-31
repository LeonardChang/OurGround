using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

	public UISprite m_icon;
	public bool isKnocked = false;
	public float knockTime = 0f;
    public UILabel m_Label;
	public bool isLeft = false;

	public bool isPulling = false;
	public float pullingTime = 0f;
	public bool hasSeed = false;

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
//				if(isLeft)
//					m_icon.spriteName = "LightSprite1";
//				else
//					m_icon.spriteName = "DarkSprite1";
			}
		}
	}
}
