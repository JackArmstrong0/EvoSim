using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Organism : MonoBehaviour
{
    public event Action<State> OnStateChanged;
    public event Action<string, float> OnStatChanged;
    public float speed;
    public float perception;

    public int age;
    public int maxAge;

    public float energy;
    public float maxEnergy;
    public float energyDecreaseRate = 0.1f;
    public float energyIncreaseAmount = 20f;

    public float moveInterval;
    private float timeSinceLastMove = 0f;
    private Vector3 moveDirection;
    private GameObject targetFood;
    
    public State activeState;
    public enum State
    {
        Wandering,
        FoundFood,
        SearchingForMate,
        Reproducing,
        Dying
    }

    public void Initialize(float speed, float perception, float maxEnergy)
    {
        age = 0;
        maxAge = 100;

        this.speed = speed;
        this.perception = perception;
        this.maxEnergy = maxEnergy;
        energy = maxEnergy;
    }

    void Start()
    {
        SetState(State.Wandering);
        ChangeRandomDirection();
        moveInterval = Random.Range(2f, 3f);
    }

    void Update()
    {
        switch (activeState)
        {
            case State.Wandering:
                WanderingBehavior(); break;
            case State.FoundFood:
                FoundFoodBehavior(); break;
        }

        energy -= energyDecreaseRate * Time.deltaTime;
        AlertStatChanged("energy", energy);
    }

    void ChangeRandomDirection()
    {

        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);

        moveDirection = new Vector3(randomX, 0f, randomZ).normalized;
    }

    void WanderingBehavior()
    {
        timeSinceLastMove += Time.deltaTime;

        if (timeSinceLastMove >= moveInterval)
        {
            ChangeRandomDirection();
            timeSinceLastMove = 0f;
            moveInterval = Random.Range(2f, 3f);
        }

        transform.position += moveDirection * speed * Time.deltaTime;

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
    void SetState(State newState)
    {
        if (activeState == newState) return;

        activeState = newState;
        OnStateChanged?.Invoke(activeState);
    }

    void AlertStatChanged(string statName, float value)
    {
        OnStatChanged?.Invoke(statName, value);
    }
}
