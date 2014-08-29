using UnityEngine;
using System.Collections;

public class AutoDestory : MonoBehaviour {
	public float Delay = 0;
	
	void Awake()
	{
		if (Delay <= 0)
		{
			DontDestroyOnLoad(gameObject);
            Destroy(this);
		}
		else
		{
			Destroy(gameObject, Delay);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
