using UnityEngine;

public class Organism : MonoBehaviour
{
    public float speed;
    public float perception;

    public int age;
    public int maxAge;

    public float energy;
    public float maxEnergy;
    public float energyDecreaseRate = 0.1f;

    public float moveInterval = 3f;
    private float timeSinceLastMove = 0f;
    private Vector3 moveDirection;
    State activeState;
    private enum State
    {
        Wandering
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
        activeState = State.Wandering;
        ChangeRandomDirection();
    }

    void Update()
    {
        switch (activeState)
        {
            case State.Wandering:
                WanderingBehavior(); break;
        }

        energy -= energyDecreaseRate * Time.deltaTime;
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
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
