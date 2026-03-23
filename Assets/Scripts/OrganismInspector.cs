using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrganismInspector : MonoBehaviour
{

    public Camera mainCamera;

    
    public float maxHighlightDistance = 50f; // HMM

    
    public Material outlineMaterial;

    private bool isTracking = false;

    private GameObject currentTarget = null;
    private Renderer currentRenderer = null;

    // Store the original material so it can be restored.
    private Material originalMaterial = null;

    public GameObject organismInspector;
    private UIManager ui;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI maxEnergyText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI perceptionText;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI generationText;
    public TextMeshProUGUI childrenText;

    public Image statusDot;
    public Image energyBarFill;

    public GameObject perceptionRingPrefab;
    private GameObject perceptionRingInstance = null;

    Organism organism;
    private void Start()
    {
        ui = organismInspector.GetComponent<UIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTracking();
        }

        // Only need to update highlighting if not tracking already tracking something
        if (!isTracking)
        {
            UpdateHighlighting();
        }
        else if (currentTarget != null)
        {
            FollowTarget();
        }
    }

    private void UpdateHighlighting()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxHighlightDistance))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag("Organism"))
            {
                if (currentTarget != hitObj)
                {
                    ClearHighlight();
                    ApplyHighlight(hitObj);
                }

                // Still looking at an organism or just switched to a new one
                return;
            }
        }

        // Organism no longer being looked at
        ClearHighlight();
    }

    private void ApplyHighlight(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();

        currentTarget = target;
        currentRenderer = renderer;

        // Save material and apply outline
        originalMaterial = renderer.material;
        renderer.material = outlineMaterial;
    }

    private void ClearHighlight()
    {
        if (currentTarget == null) return;

        if (currentRenderer != null && originalMaterial != null)
        {
            currentRenderer.material = originalMaterial;
        }

        currentTarget = null;
        currentRenderer = null;
        originalMaterial = null;
    }

    private void ToggleTracking()
    {
        if (isTracking)
        {
            isTracking = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ui.Hide();
            organism.OnStatChanged -= UpdateOrganismData;
            organism.OnStateChanged -= UpdateStatusDot;

            if (perceptionRingInstance != null)
            {
                Destroy(perceptionRingInstance);
                perceptionRingInstance = null;
            }
        }
        else if (currentTarget != null)
        {
            // Turn tracking on (only if we have a target)
            isTracking = true;
            organism = currentTarget.GetComponent<Organism>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            ui.Show();
            organism.OnStatChanged += UpdateOrganismData;
            organism.OnStateChanged += UpdateStatusDot;
            UpdateOrganismData(null, 0f); // Force update all data immediately
            UpdateStatusDot(organism.activeState); // Force update status dot immediately

            perceptionRingInstance = Instantiate(perceptionRingPrefab, currentTarget.transform);
            perceptionRingInstance.transform.localPosition = Vector3.zero; // Center on parent organism
            perceptionRingInstance.transform.position -= new Vector3(0f, 1f, 0f); // Lower slightly so it's on the ground
            float radius = organism.perception;
            perceptionRingInstance.transform.localScale = new Vector3(radius * 2f, 0.01f, radius * 2f);

        }
    }

    private void FollowTarget()
    {
        if (mainCamera == null || currentTarget == null) return;

        Vector3 targetPos = currentTarget.transform.position;
        mainCamera.transform.LookAt(targetPos);
    }
    private void UpdateOrganismData(string statName, float newValue)
    {
        if (currentTarget == null) return;

        if (organism != null)
        {
            switch (statName)
            {
                case null: // Update all stats if statName is null
                    energyText.text = organism.energy.ToString("F1");
                    maxEnergyText.text = organism.maxEnergy.ToString("F1");
                    speedText.text = organism.speed.ToString("F2");
                    perceptionText.text = organism.perception.ToString("F2");
                    ageText.text = organism.age.ToString("F0");
                    generationText.text = organism.generation.ToString("F0");
                    childrenText.text = organism.childrenCount.ToString("F0");
                    UpdateEnergyBar();
                    break;
                case "energy":
                    energyText.text = newValue.ToString("F1");
                    UpdateEnergyBar();
                    break;
                case "maxEnergy":
                    maxEnergyText.text = newValue.ToString("F1");
                    UpdateEnergyBar();
                    break;
                case "speed":
                    speedText.text = newValue.ToString("F2");
                    break;
                case "perception":
                    perceptionText.text = newValue.ToString("F2");
                    float radius = organism.perception;
                    perceptionRingInstance.transform.localScale = new Vector3(radius * 2f, 0.01f, radius * 2f);

                    break;
                case "age":
                    ageText.text = newValue.ToString("F0");
                    break;
                case "generation":
                    generationText.text = newValue.ToString("F0");
                    break;
                case "children":
                    childrenText.text = newValue.ToString("F0");
                    break;
            }
        }
    }
    private void UpdateStatusDot(Organism.State newState)
    {
            switch (newState)
            {
                case Organism.State.Wandering:
                    statusDot.color = Color.yellow;
                    break;
                case Organism.State.FoundFood:
                    statusDot.color = Color.green;
                    break;
                case Organism.State.SearchingForMate:
                    statusDot.color = Color.magenta;
                    break;
                case Organism.State.Reproducing:
                    statusDot.color = Color.red;
                    break;
                case Organism.State.Dying:
                    statusDot.color = Color.gray;
                    break;
            }
    }
    void UpdateEnergyBar()
    {
        if (organism != null)
        {
            float energyRatio = Mathf.Clamp(organism.energy / organism.maxEnergy, 0f, 1f); // Ensure ratio between 0 and 1
		    energyBarFill.fillAmount = energyRatio;
        }
    }
}
