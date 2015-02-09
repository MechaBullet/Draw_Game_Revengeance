using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item {
	//Basic Item Info
	public string itemPrefab;
	public string itemName;
	public int itemID;
	public string itemDesc;
	public Texture2D itemIcon;
	public ItemType itemType;

	//Weapon Info
	public float weaponDamage;
	public float weaponRange;
	public float weaponRate;
	public float weaponReloadTime;
	public float weaponMaxClip;
	public Slot slot;
	public Base weaponBase;

	public enum Base {
		Nonexistant,
		Revolver,
		LeverRifle
	}

	public enum Slot {
		Primary,
		Secondary,
		Melee
	}

	public enum ItemType {
		Weapon,
		Consumable
	}

	public Item() {}
	public Item(string name, int id, string description, ItemType type) {
		itemName = name;
		itemID = id;
		itemDesc = description;
		itemType = type;
		itemIcon = Resources.Load<Texture2D>("Icons/" + name);
	}

	public void BuildGun(Base baseWeapon, float damage, float range, float rate, float reloadTime, float maxClip) {
		weaponDamage = damage;
		weaponRange = range;
		weaponRate = rate;
		weaponReloadTime = reloadTime;
		weaponMaxClip = maxClip;
		weaponBase = baseWeapon;
		switch(baseWeapon) {
			case Base.LeverRifle:
				slot = Slot.Primary;
				break;
			case Base.Revolver:
				slot = Slot.Secondary;
				break;
		}
	}

	public void SetObject(string prefabName) {
		itemPrefab = "Prefabs/Items/" + prefabName;
	}
}
