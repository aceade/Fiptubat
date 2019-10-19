using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUnitDisplay: MonoBehaviour {

    private Text healthBar;

    private Text actionPointsBar;

    private Text armourBar;

    private Text ammoCounter;

    private Text nameField;

    private Image statusImage;

    [Tooltip("The button that selects this unit")]
    public Image parentImage;

    public Sprite healthyImage, damagedSprite, criticalSprite, deadSprite;

    public float criticalDamageThreshold = 0.3f;

    private BaseUnit unit;

    private int maxHealth, maxPoints, maxArmour, maxAmmo;

    void Start() {
        unit = GetComponent<BaseUnit>();
        maxHealth = unit.health;
        maxPoints = unit.actionPoints;
        maxArmour = unit.armour;
        statusImage = parentImage.GetComponentInChildren<Image>();
        Text[] texts = parentImage.GetComponentsInChildren<Text>();
        healthBar = texts[0];
        actionPointsBar = texts[1];
        armourBar = texts[2];
        ammoCounter = texts[3];
        nameField = texts[4];
        nameField.text = unit.unitName;
    }

    void Update() {
        healthBar.text = string.Format("{0}/{1}", unit.health, maxHealth);
        actionPointsBar.text = string.Format("{0}/{1}", unit.GetCurrentActionPoints(), maxPoints);
        armourBar.text = string.Format("{0}/{1}", unit.armour, maxArmour);
        ammoCounter.text = string.Format("{0}/{1}", 0, 0);

        if (unit.health < maxHealth) {
            SetImage(unit.health);
        }

        ShowIfSelected(unit.IsSelected());
    }

    private void ShowIfSelected(bool selected) {
        if (selected) {
            nameField.canvasRenderer.SetAlpha(1f);
            nameField.color = Color.red;
            nameField.fontStyle = FontStyle.Bold;
        } else {
            nameField.canvasRenderer.SetAlpha(0.5f);
            nameField.color = Color.white;
            nameField.fontStyle = FontStyle.Normal;
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