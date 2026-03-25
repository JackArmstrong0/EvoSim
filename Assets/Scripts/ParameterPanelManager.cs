using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Search;
using System;

public class ParameterPanelManager : MonoBehaviour
{
    public Slider foodSpawnRateSlider;
    public TextMeshProUGUI foodSpawnRateValueText;
    public Slider maxMutationSlider;
    public TextMeshProUGUI maxMutationValueText;
    UIManager uiManager;
    void Start()
    {
        // Initialize sliders with current values from managers
        foodSpawnRateSlider.value = FoodManager.foodSpawnRate;
        maxMutationSlider.value = OrganismManager.maxMutation;

        // Add listeners to sliders
        foodSpawnRateSlider.onValueChanged.AddListener(OnFoodSpawnRateChanged);
        maxMutationSlider.onValueChanged.AddListener(OnMaxMutationChanged);
        
        // Initialize value texts
        foodSpawnRateValueText.text = foodSpawnRateSlider.value.ToString("0.00");
        maxMutationValueText.text = maxMutationSlider.value.ToString("0.00");

        uiManager = GetComponent<UIManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (uiManager.isVisible)
            {
                uiManager.Hide();
            }
            else
            {
                uiManager.Show();
            }
        }
    }
    void OnFoodSpawnRateChanged(float value)
    {
        FoodManager.foodSpawnRate = value;
        foodSpawnRateValueText.text = value.ToString("0.00");
    }
    void OnMaxMutationChanged(float value)
    {
        OrganismManager.maxMutation = value;
        maxMutationValueText.text = value.ToString("0.00");
    }
}
