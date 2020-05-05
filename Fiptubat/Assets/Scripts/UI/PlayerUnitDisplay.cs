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

    private Image uiStatusImage;

    [Tooltip("The button that selects this unit")]
    public Image parentImage;

    public Image crosshairs;
    private Vector2 crosshairsCoordinates;
    private float crosshairsWidth;
    private float crosshairsHeight;

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
    }

    void Update() {
        healthBar.text = string.Format("HP: {0}/{1}", unit.health, maxHealth);
        actionPointsBar.text = string.Format("AP: {0}/{1}", unit.GetCurrentActionPoints(), maxPoints);
        armourBar.text = string.Format("Armour: {0}/{1}", unit.armour, maxArmour);
        ammoCounter.text = string.Format("Ammo: {0}", weapon.GetAmmoCounter());
        fireModeField.text = string.Format("Firemode: {0}", weapon.GetCurrentFireMode().name);

        bool isSelected = unit.IsSelected();
        ShowIfSelected(isSelected);
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

    public void ToggleUsingUi(bool isUsingUi) {
        usingUi = isUsingUi;
    }
}