using System.Collections.Generic;
using UnityEngine;

public class InkBolt : MonoBehaviour
{
    public GameObject puddlePrefab; 
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 pos = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;    

            GameObject puddle = Instantiate(puddlePrefab, pos + normal * 0.01f, Quaternion.LookRotation(normal));

            puddle.transform.Rotate(Vector3.up, Random.Range(0, 360), Space.Self);
            puddle.transform.localScale *= Random.Range(0.8f, 1.2f);

            Destroy(puddle, 10f);
        }
    }
}