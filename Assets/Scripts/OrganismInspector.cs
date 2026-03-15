using TMPro;
using Unity.Collections;
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
            UpdateOrganismData();
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
        }
        else if (currentTarget != null)
        {
            // Turn tracking on (only if we have a target)
            isTracking = true;
            organism = currentTarget.GetComponent<Organism>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            ui.Show();
        }
    }

    private void FollowTarget()
    {
        if (mainCamera == null || currentTarget == null) return;

        Vector3 targetPos = currentTarget.transform.position;
        mainCamera.transform.LookAt(targetPos);
    }
    private void UpdateOrganismData()
    {
        if (currentTarget == null) return;

        if (organism != null)
        {
            energyText.text = organism.energy.ToString("F1");
            maxEnergyText.text = organism.maxEnergy.ToString("F1");
            speedText.text = organism.speed.ToString("F2");
            perceptionText.text = organism.perception.ToString("F2");
            ageText.text = organism.age.ToString("F0");
            // generationText.text = organism.generation.ToString("F0");
            // childrenText.text = organism.children.Count.ToString("F0");

            
        }
    }
}
