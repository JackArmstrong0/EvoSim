using System.Net.NetworkInformation;
using UnityEngine;

public class OrganismManager : MonoBehaviour
{
public int startOrganismCount = 10;
public static int organismCount = 0;
public GameObject organismPrefab;
public static float maxMutation = 25f;

    void Start()
    {
        for (int i = 0; i < startOrganismCount; i++)
        {
            string name = "Organism" + i;
            Vector3 pos = GetRandomPos();
            
            // Assign slightly varying, randomised values for each stat
            float speed = Random.Range(3f, 7f); // 5±2
            float perception = Random.Range(6f, 10f); // 8±2
            float maxEnergy = Random.Range(90f, 110f); // 100±10
            int generation = 1; // First generation is generation 1
            CreateOrganism(name, pos, speed, perception, maxEnergy, generation);
        }
    }

    public void CreateOrganism(string name, Vector3 pos, float speed, float perception, float maxEnergy, int generation)
    {
        organismCount++;

        GameObject newOrganism = Instantiate(organismPrefab, pos, Quaternion.identity);
        newOrganism.tag = "Organism";

        //Access the Organism script of the newly created GameObject
        Organism organism = newOrganism.GetComponent<Organism>();

        if (organism != null) // Ensure the object has the script
        {
            organism.name = name;
            organism.Initialize(speed, perception, maxEnergy, generation);
        }
    }

    Vector3 GetRandomPos()
{
    float randomX = Random.Range(-50f, 50f);
    float randomZ = Random.Range(-50f, 50f);

    return new Vector3(randomX, 2f, randomZ);
}
}
