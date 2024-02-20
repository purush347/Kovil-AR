using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explode : MonoBehaviour
{
    public float explosionForce = 10f;
    public float explosionRadius = 5f;
    public float[] modifiers = { 0.5f, 0.6f, 0.7f }; // Array of modifiers
    private int modifierIndex = 0; // Index to track which modifier to use

    private Transform[] children;

    // Start is called before the first frame update
    void Start()
    {
        // Invoke the Explode method after 2 seconds
        Invoke("Explode", 2f);
    }

    public void Explode()
    {
        // Get all children of the parent GameObject
        children = new Transform[transform.childCount];
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            // Add a rigidbody and box collider to each child
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            child.gameObject.AddComponent<BoxCollider>();
            // Disable gravity for the child
            rb.useGravity = false;
            // Set isKinematic to false
            rb.isKinematic = false;
        }

        foreach (Rigidbody rb in rigidbodies)
        {
            // Add an explosion force to each rigidbody
            rb.AddExplosionForce(explosionForce * modifiers[modifierIndex], transform.position, explosionRadius);
            modifierIndex = (modifierIndex + 1) % modifiers.Length;
        }

        // Invoke makeKinetic method after 2 seconds
        Invoke("makeKinetic", 1f);

        Debug.Log("Explode");
    }

    private void makeKinetic()
    {
        foreach (Transform child in children)
        {
            // Set isKinematic to true for each child
            child.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        Debug.Log("makeKinetic");
    }
}