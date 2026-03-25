using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using System;
using System.Numerics;

public class Organism : MonoBehaviour
{
    public event Action<State> OnStateChanged;
    public event Action<string, float> OnStatChanged;
    Rigidbody rb;
    Renderer rend;

    public float speed;
    public float perception;

    public int age;
    public int maxAge;
    private float birthTime;

    public float energy;
    public float maxEnergy;
    private float energyDecreaseRate = 2f;
    private float energyIncreaseAmount = 20f;

    public float moveInterval;
    private float timeSinceLastMove = 0f;
    private Vector3 moveDirection;
    private GameObject targetFood;

    private float energyReproduceThreshold = 50f;
    private float ageReproduceThreshold;
    private float reproductionCooldown = 10f;
    private float lastReproductionTime = -Mathf.Infinity;
    private GameObject mate;
    public int childrenCount = 0;
    public int generation;
    public OrganismManager orgManager;
    
    public State activeState;
    public enum State
    {
        Wandering,
        FoundFood,
        SearchingForMate,
        Reproducing,
        Dying
    }

    public void Initialize(float speed, float perception, float maxEnergy, int generation)
    {
        age = 0;
        maxAge = 200;
        this.generation = generation;

        this.speed = speed;
        this.perception = perception;
        this.maxEnergy = maxEnergy;
        energy = maxEnergy;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

        SetState(State.Wandering);
        ChangeRandomDirection();
        moveInterval = Random.Range(2f, 3f);
        birthTime = Time.time;

        orgManager = FindAnyObjectByType<OrganismManager>();
        ageReproduceThreshold = maxAge * 0.3f;
    }

    void Update()
    {
        if (activeState == State.Dying)
            return; // Don't do anything if dying

        switch (activeState)
        {
            case State.Wandering:
                WanderingBehavior(); break;
            case State.FoundFood:
                FoundFoodBehavior(); break;
            case State.SearchingForMate:
                SearchingForMateBehavior(); break;
        }

        age = Mathf.FloorToInt(Time.time - birthTime);
        AlertStatChanged("age", age);
        energy -= energyDecreaseRate * Time.deltaTime;
        AlertStatChanged("energy", energy);
        
        if (age >= ageReproduceThreshold && energy >= energyReproduceThreshold && Time.time - lastReproductionTime >= reproductionCooldown && activeState != State.Reproducing)
        {
            SetState(State.SearchingForMate);
        }

        if (age >= maxAge || energy <= 0)
        {
            Die();
        }
    }

    void ChangeRandomDirection()
    {

        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);

        moveDirection = new Vector3(randomX, 0f, randomZ).normalized;
    }

    void WanderingBehavior()
    {
        MovementLogic();
        CheckForFood();
    }
    void FoundFoodBehavior()
    {
        if (targetFood != null)
        {
            Vector3 foodDirection = (targetFood.transform.position - transform.position).normalized;
            transform.position += foodDirection * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetFood.transform.position) < 1.5f)
            {
                Eat();
                targetFood = null;
                SetState(State.Wandering);
            }
        }
        else
        {
            SetState(State.Wandering);
        }
    }
    void SearchingForMateBehavior()
    {
        if (energy < energyReproduceThreshold - 20)
        {
            SetState(State.Wandering);
            return;
        }

        if (mate != null)
        {
            Vector3 mateDirection = (mate.transform.position - transform.position).normalized;
            transform.position += mateDirection * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, mate.transform.position) < 1.5f)
            {
                SetState(State.Reproducing);
                if (gameObject.name.CompareTo(mate.gameObject.name) < 0)
                {
                    Invoke(nameof(Reproduce), 5f);
                }
            }
        }
        else
        {
            MovementLogic();
            CheckForMate();
        }
    }
    void MovementLogic()
    {
        timeSinceLastMove += Time.deltaTime;

        if (timeSinceLastMove >= moveInterval)
        {
            ChangeRandomDirection();
            timeSinceLastMove = 0f;
            moveInterval = Random.Range(2f, 3f);
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }
    void CheckForFood()
    {
        // Circle hitbox at pos with radius perception
        Collider[] hitboxColliders = Physics.OverlapSphere(transform.position, perception);

        float minDistance = Mathf.Infinity;
        GameObject closestFood = null;

        foreach (Collider collider in hitboxColliders)
        {
            if (collider.CompareTag("Food"))
            {
                float foodDistance = Vector3.Distance(transform.position, collider.transform.position);

                if (foodDistance < minDistance)
                {
                    minDistance = foodDistance;
                    closestFood = collider.gameObject;
                }
            }
        }

        // If there is food nearby, change state
        if (closestFood != null)
        {
            targetFood = closestFood;
            SetState(State.FoundFood);
        }
    }
    
    void Eat()
    {
        energy += energyIncreaseAmount;

        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        // Change only after checking maxEnergy to avoid sending values above maxEnergy to the inspector
        AlertStatChanged("energy", energy);

        if (targetFood != null)
        {
            Destroy(targetFood);
        }
    }
    void CheckForMate()
    {
        Collider[] hitboxColliders = Physics.OverlapSphere(transform.position, perception);

        foreach (Collider collider in hitboxColliders)
        {
            if (collider.CompareTag("Organism") && collider.gameObject != gameObject)
            {
                Organism otherOrganism = collider.GetComponent<Organism>();

                if (otherOrganism.activeState == State.SearchingForMate)
                {
                    mate = collider.gameObject;
                    break;
                }
            }
        }
    }
    void Reproduce()
    {
        if (mate == null) return;
        Organism mateOrganism = mate.GetComponent<Organism>();

        float childSpeed = (speed + mateOrganism.speed) / 2f;
        float childPerception = (perception + mateOrganism.perception) / 2f;
        float childMaxEnergy = (maxEnergy + mateOrganism.maxEnergy) / 2f;

        Mutate();

        Vector3 childPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        orgManager.CreateOrganism("Organism" + OrganismManager.organismCount, childPosition, childSpeed, childPerception, childMaxEnergy, generation + 1);

        lastReproductionTime = Time.time;
        energy -= energyReproduceThreshold * 0.5f;
        AlertStatChanged("energy", energy);
        childrenCount++;
        mate = null;
        SetState(State.Wandering);

        mateOrganism.lastReproductionTime = Time.time;
        mateOrganism.energy -= energyReproduceThreshold * 0.5f;
        mateOrganism.AlertStatChanged("energy", mateOrganism.energy);
        mateOrganism.childrenCount++;
        mateOrganism.mate = null;
        mateOrganism.SetState(State.Wandering);
    }
    void Mutate()
    {
        speed *= 1 + Random.Range(-OrganismManager.maxMutation, OrganismManager.maxMutation) / 100f;
        perception *= 1 + Random.Range(-OrganismManager.maxMutation, OrganismManager.maxMutation) / 100f;
        maxEnergy *= 1 + Random.Range(-OrganismManager.maxMutation, OrganismManager.maxMutation) / 100f;
    }
    void Die()
    {
        SetState(State.Dying);
        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(moveDirection, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }
    void SetState(State newState)
    {
        activeState = newState;
        OnStateChanged?.Invoke(activeState);

        switch (newState)
        {
            case State.Wandering:
                rend.material.color = Color.yellow;
                break;
            case State.FoundFood:
                rend.material.color = Color.green;
                break;
            case State.SearchingForMate:
                rend.material.color = Color.magenta;
                break;
            case State.Reproducing:
                rend.material.color = Color.red; 
                break;
            case State.Dying:
                rend.material.color = Color.gray;
                break;
        }
    }

    void AlertStatChanged(string statName, float value)
    {
        OnStatChanged?.Invoke(statName, value);
    }
}
