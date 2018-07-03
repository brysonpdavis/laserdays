using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour {
	public static GameController control;
	PlayerCharge playercharge;
	// Use this for initialization
	void Awake () {
		if(control == null)
		{
			DontDestroyOnLoad(gameObject);
			control = this;
		}
		else if (control != this)
		{
			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	void OnGUI () {
		GUI.Label(new Rect(10,10,100,30),"Charge" + 0);
	}

	public void Save()
	{
		playercharge = GetComponent<PlayerCharge>();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
		PlayerData data = new PlayerData();
		data.charge = playercharge.chargeSlider.value ;
		bf.Serialize(file, data);
		file.Close();
	}

	public void Load()
	{
		if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
		{playercharge = GetComponent<PlayerCharge>();
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file= File.Open(Application. persistentDataPath +"/playerInfo.dat", FileMode.Open);
			PlayerData data =(PlayerData)bf.Deserialize(file);
			file.Close();
			playercharge.chargeSlider.value = data.charge;
		}
	}
}
[Serializable]
class PlayerData
{
	public float charge;
}
