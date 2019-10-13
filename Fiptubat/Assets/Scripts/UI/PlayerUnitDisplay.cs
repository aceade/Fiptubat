using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUnitDisplay: MonoBehaviour {

    public Text healthBar;

    public Text actionPointsBar;

    public Text armourBar;

    public Text ammoCount;

    public Image statusImage;

    public Sprite healthyImage, damagedSprite, criticalSprite, deadSprite;

    public float criticalDamageThreshold = 0.3f;

    private BaseUnit unit;

    private int maxHealth, maxPoints, maxArmour, maxAmmo;

    void Start() {
        unit = GetComponent<BaseUnit>();
        maxHealth = unit.health;
        maxPoints = unit.actionPoints;
        maxArmour = unit.armour;
    }

    void Update() {
        healthBar.text = string.Format("{0}/{1}", unit.health, maxHealth);
        actionPointsBar.text = string.Format("{0}/{1}", unit.actionPoints, maxPoints);
        armourBar.text = string.Format("{0}/{1}", unit.armour, maxArmour);
        ammoCount.text = string.Format("{0}/{1}", 0, 0);

        if (unit.health < maxHealth) {
            SetImage(unit.health);
        }

    }

    private void SetImage(int health) {
        if (health <= (criticalDamageThreshold * maxHealth)) {
            statusImage.sprite = criticalSprite;
        }
        else if (health <= 0) {
            statusImage.sprite = deadSprite;
        } else {
            statusImage.sprite = damagedSprite;
        }
    }
}