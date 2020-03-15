using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUnitDisplay: MonoBehaviour {

    private Text healthBar;

    private Text actionPointsBar;

    private Text armourBar;

    private Text ammoCounter;

    private Text nameField;

    private Text fireModeField;

    private Image statusImage;

    private Image uiStatusImage;

    [Tooltip("The button that selects this unit")]
    public Image parentImage;

    public Image crosshairs;
    private Vector2 crosshairsCoordinates;
    private float crosshairsWidth;
    private float crosshairsHeight;


    public Sprite healthyImage, damagedSprite, criticalSprite, deadSprite;

    public float criticalDamageThreshold = 0.3f;

    private BaseUnit unit;

    private WeaponBase weapon;

    private int maxHealth, maxPoints, maxArmour;

    private bool usingUi = false;

    void Start() {
        unit = GetComponent<BaseUnit>();
        maxHealth = unit.health;
        maxPoints = unit.actionPoints;
        maxArmour = unit.armour;
        statusImage = parentImage.rectTransform.Find("DisplayImage").GetComponent<Image>();
        uiStatusImage = parentImage.rectTransform.Find("UiStatusImage").GetComponent<Image>();
        Text[] texts = parentImage.GetComponentsInChildren<Text>();
        healthBar = texts[0];
        actionPointsBar = texts[1];
        armourBar = texts[2];
        ammoCounter = texts[3];
        nameField = texts[4];
        fireModeField = texts[5];
        nameField.text = unit.unitName;
        weapon = GetComponentInChildren<WeaponBase>();
        crosshairsWidth = crosshairs.rectTransform.rect.width;
        crosshairsHeight = crosshairs.rectTransform.rect.height;
    }

    void Update() {
        healthBar.text = string.Format("{0}/{1}", unit.health, maxHealth);
        actionPointsBar.text = string.Format("{0}/{1}", unit.GetCurrentActionPoints(), maxPoints);
        armourBar.text = string.Format("{0}/{1}", unit.armour, maxArmour);
        ammoCounter.text = weapon.GetAmmoCounter();
        fireModeField.text = weapon.GetCurrentFireMode().name;

        if (unit.health < maxHealth) {
            SetImage(unit.health);
        }

        bool isSelected = unit.IsSelected();
        ShowIfSelected(isSelected);
        if (isSelected) {
            ShowCrosshairs();
        }

        ShowUiStatus();
        
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

    private void ShowUiStatus() {
        float alpha = usingUi? 1f : 0f;
        uiStatusImage.CrossFadeAlpha(alpha, 0.1f, true);
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

    private void ShowCrosshairs() {
        float deviation = weapon.GetDeviation() * 100;
        // get the size
        crosshairs.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, crosshairsWidth + deviation);
        crosshairs.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, crosshairsHeight + deviation);
    }

    public void ToggleUsingUi(bool isUsingUi) {
        usingUi = isUsingUi;
    }
}