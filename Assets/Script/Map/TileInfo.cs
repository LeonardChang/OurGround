﻿using UnityEngine;
using System.Collections;

public class TileInfo : MonoBehaviour {

	public UISprite tileBG;
	public UISprite tileFlower;
	public UISprite tileUpBlock;
	public UISprite tileRightBlock;
	public UISprite tileDownBlock;
	public UISprite tileLeftBlock;
	public UISprite knockSprite;

	public bool isKnocked = false;
	public float cdTime = 0;
	public bool isGrowing = false;
	public bool hasFlower = false;
	public bool hasSeed = false;
	public float growTime = 0;
	public bool isLeft = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(hasSeed)
		{
			if(isGrowing)
			{
				growTime -= Time.deltaTime;
				if(growTime <= 0)
				{
					growTime = 0;
					hasFlower = true;
					hasSeed = false;
					isGrowing = false;
					if(isLeft)
					{
						tileFlower.spriteName = "LightFlower";
						tileFlower.MakePixelPerfect();
						//MapController.Instance.m_MAPLeft.m_score++;
						//TileIndex ti = MapController.Instance.pc.GetTileIndex(transform.localPosition, isLeft);
						//MapController.Instance.m_MAPLeft.m_tiles[ti.m_x, ti.m_y].canAward = true;
					}
					else
					{
						tileFlower.spriteName = "DarkFlower";
						tileFlower.MakePixelPerfect();
						//MapController.Instance.m_MAPRight.m_score++;
						//TileIndex ti = MapController.Instance.pc.GetTileIndex(transform.localPosition, isLeft);
						//MapController.Instance.m_MAPRight.m_tiles[ti.m_x, ti.m_y].canAward = true;
					}
				}
			}
		}

		if(isKnocked)
		{
			cdTime -= Time.deltaTime;
			if(cdTime <= 0)
			{
				cdTime = 0;
				isKnocked = false;
				knockSprite.gameObject.SetActive(false);
			}
		}
	}
}
