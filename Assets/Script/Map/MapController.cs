using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour {

	public int MAX_ROW;
	public int MAX_COL;

	public Map m_MAPLeft;
	public Map m_MAPRight;
	// Use this for initialization
	void Start () {
		m_MAPLeft = new Map(MAX_ROW, MAX_COL);
		m_MAPRight = new Map(MAX_ROW, MAX_COL);
		m_MAPLeft.SetOpponentTile(m_MAPRight, MAX_ROW, MAX_COL);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
