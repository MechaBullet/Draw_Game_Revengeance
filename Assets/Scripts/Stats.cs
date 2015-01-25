using UnityEngine;
using System.Collections;
	
[System.Serializable]
public class Stats {
	public int level;
	public int experience;

	//Statistics
	public Archetype playerClass;

	public int cunning; //Stealth
	public int resolve; //Defense
	public int dexterity; //Speed
	public int hitDie;

	//Classes
	public enum Archetype {
		Guardian, Tempest, Sharpshooter
	}

	// Level * 100
	private int experienceToNext;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Stats (int level, Archetype playerClass) {
		this.level = level;
		experience = (level - 1) * 100;
		experienceToNext = level * 100;

		this.playerClass = playerClass;
		switch(playerClass) {
		case Archetype.Sharpshooter:
			hitDie = 8;
			cunning = 10;
			resolve = 5;
			dexterity = 5;
			break;
		case Archetype.Tempest:
			hitDie = 10;
			dexterity = 10;
			cunning = 5;
			resolve = 5;
			break;
		case Archetype.Guardian:
			hitDie = 12;
			resolve = 10;
			cunning = 5;
			dexterity = 5;
			break;
		}
	}
}
