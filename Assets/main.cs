using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class main : MonoBehaviour
{
    // Direct light
    public GameObject directLight;
    public AudioClip cannonFire;
    public AudioClip MainStory;
    public GameObject Bhitti;
    public GameObject Devakosthas;
    public GameObject Adhisthana;
    public GameObject Vimana;
    public GameObject objectToExplode;
    public GameObject mainObject;


    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    public float explosionForce = 50f;
    public float explosionRadius = 10f;
    public float[] modifiers; // Array of modifiers
    private int modifierIndex = 0; // Index to track which modifier to use
    private GameObject duplicateObject;
    private GameObject duplicatemainObject;
    private bool isPlaying = false;

    private Transform[] children;
    private AudioSource audioSource;
    // Attach the GameObject you want to explode in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Invoke the Explode method after 2 seconds
        // Invoke("ExplodeF", 2f);
        Anchor();
        audioSource = GetComponent<AudioSource>();
    }

    public void ExplodeT(){
        Explode(true);
    }

    public void ExplodeF(){
        Explode(false);
    }

    private void GenerateRandomModifiers(int count)
    {
        modifiers = new float[count];
        for (int i = 0; i < count; i++)
        {
            modifiers[i] = Random.Range(0.0f, 1.0f); // Adjust the range as needed
        }
    }
    public void Explode(bool applyGravity)
    {
        // if duplicateObject is not null, return
        if (duplicateObject != null)
        {
            return;
        }

        audioSource.PlayOneShot(cannonFire);

        // Start a coroutine to introduce a delay
        StartCoroutine(DelayCoroutine(applyGravity));
    }

    private IEnumerator DelayCoroutine(bool applyGravity)
    {
        // Wait for one second
        yield return new WaitForSeconds(0.5f);

        GenerateRandomModifiers(7);
        // Check if the objectToExplode is assigned

        if (objectToExplode == null)
        {
            Debug.LogError("objectToExplode is not assigned!");
            yield break; // Exit the coroutine
        }

        // Instantiate a duplicate of the objectToExplode
        duplicateObject = Instantiate(objectToExplode, objectToExplode.transform.position, objectToExplode.transform.rotation, objectToExplode.transform.parent);
        mainObject.SetActive(false);
        duplicateObject.SetActive(true);
        duplicateObject.name = objectToExplode.name + "_Duplicate"; // Optional: Rename the duplicate for clarity

        // Parent the duplicate object to the original object's parent
        duplicateObject.transform.parent = objectToExplode.transform.parent;

        duplicateObject.transform.localPosition = mainObject.transform.localPosition;
        duplicateObject.transform.localRotation = mainObject.transform.localRotation;
        duplicateObject.transform.localScale = mainObject.transform.localScale;

        // remove box colider , nearinteractiongrabbable and object manipulator
        // deactivate box colider , nearinteractiongrabbable and object manipulator
        // duplicateObject.GetComponent<BoxCollider>().enabled = false;
        // duplicateObject.GetComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>().enabled = false;
        // duplicateObject.GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>().enabled = false;



        // Get all children of the duplicate GameObject
        children = new Transform[duplicateObject.transform.childCount];
        Rigidbody[] rigidbodies = duplicateObject.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < duplicateObject.transform.childCount; i++)
        {
            children[i] = duplicateObject.transform.GetChild(i);
        }
        // play just the audio clip
        foreach (Transform child in children)
        {
            // Add a rigidbody and box collider to each child
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            child.gameObject.AddComponent<BoxCollider>();
            // Set gravity state based on parameter
            rb.useGravity = applyGravity;
            // Set isKinematic to false if gravity is enabled
            rb.isKinematic = false;

            // Point each child upwards (modify as needed)
            rb.transform.up = Vector3.up;
            
        }

        // Deactivate the original object
        objectToExplode.SetActive(false);

        foreach (Rigidbody rb in rigidbodies)
        {
            // Add an explosion force to each rigidbody at a random position pointing upwards
            Vector3 randomExplosionPosition = duplicateObject.transform.position + Random.onUnitSphere * explosionRadius;
            rb.AddExplosionForce(explosionForce * modifiers[modifierIndex], randomExplosionPosition, explosionRadius);
            modifierIndex = (modifierIndex + 1) % modifiers.Length;
            
        }

        foreach (Transform child in children){
            // Add nearInteractionGrabbable to each child
            child.gameObject.AddComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>();
            // Add objectManipulator to each child
            child.gameObject.AddComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();

        }


        // Invoke makeKinetic method after 2 seconds if applyGravity is false
        if (!applyGravity)
        {
            Invoke(nameof(MakeKinetic), 1f);
        }
    }



    private void MakeKinetic()
    {
        // Activate the original object
        // objectToExplode.SetActive(true);

        foreach (Transform child in children)
        {
            // Set isKinematic to true for each child
            child.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        Debug.Log("MakeKinetic");
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Reset();
    }

    public void Reset()
    {

        // Reset the position, rotation, and scale
        objectToExplode.transform.localPosition = originalPosition;
        objectToExplode.transform.localRotation = originalRotation;
        objectToExplode.transform.localScale = originalScale;

        mainObject.transform.localPosition = originalPosition;
        mainObject.transform.localRotation = originalRotation;
        mainObject.transform.localScale = originalScale;

        // if duplicateObject is null, return
        if (duplicateObject == null)
        {
            return;
        }
        // Activate the original object
        Debug.Log("reset");
        Debug.Log("objectToExplode: " + objectToExplode);
        Destroy(duplicateObject); // Adjust the time as needed
        mainObject.SetActive(true);
        // Destroy the duplicate object after the explosion
        Debug.Log("duplicateObject: " + duplicateObject);
    }

    public void Anchor()
    {
        // Save the original position, rotation, and scale
        originalPosition = mainObject.transform.localPosition;
        originalRotation = mainObject.transform.localRotation;
        originalScale = mainObject.transform.localScale;
        Debug.Log("Anchor");
        Debug.Log("originalPosition: " + originalPosition);
        Debug.Log("originalRotation: " + originalRotation);
        
    }

    /*
    Welcome to the enchanting Mixed Reality immersive experience of Muvar Koil temple in Tamil Nadu. This sacred site boasts three Siva temples dating back to the ninth century, standing as a timeless testament to early Chola architecture. However, the passage of time has not been kind to this architectural marvel. As a result of neglect, invasion, disasters, and the relentless impact of climate change, only two of the three temples have managed to survive.

    The temple's foundation showcases a captivating padma-pushkala-style adhisthana, while the bhitti, or wall, is adorned with exquisite projections and devakoshthas niches. As your gaze ascends, the two-story vimana unfolds with intricate carvings, each niche revealing the mastery of Chola art. These sculptures are celebrated as some of the finest specimens from that era.
    Embark on a captivating augmented reality expedition as we revive and restore Muvar Koil, preserving its ancient charm. Immerse yourself in the rich history and interact with the temple, unlocking a unique journey through time.

    */

    public void Play()
    {
        /*
        6 lights on
        25 blast
        36 reset
        41 adhistana
        44 bhitti
        50 -devakoahtas
        54 Vimana
        1.28 StoryEnd
        */
        if (isPlaying)
        {
            return;
        }
        isPlaying = true;
        // stop the light
        SetStoryMode();
        directLight.SetActive(false);
        audioSource.PlayOneShot(MainStory);
        StartCoroutine(ActivateLightAfterDelay(7f));

        StartCoroutine(BlastAfterDelay(25f));
        StartCoroutine(ResetAfterDelay(36f));
        StartCoroutine(SetTooltipActiveAfterDelay(Adhisthana, 43f));
        StartCoroutine(SetTooltipActiveAfterDelay(Bhitti, 45f));
        StartCoroutine(SetTooltipActiveAfterDelay(Devakosthas, 50f));
        StartCoroutine(SetTooltipActiveAfterDelay(Vimana, 54f));


        StartCoroutine(StoryEndAfterDelay(90f));
    }

    IEnumerator ActivateLightAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActivateLight();
    }

    private void ActivateLight()
    {
        directLight.SetActive(true);
    }

    private void SetStoryMode()
    {
        // remove box colider , nearinteractiongrabbable and object manipulator
        // deactivate box colider , nearinteractiongrabbable and object manipulator
        mainObject.GetComponent<BoxCollider>().enabled = false;
        mainObject.GetComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>().enabled = false;
        mainObject.GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>().enabled = false;

    }

    private void StoryModeEnd()
    {
        // remove box colider , nearinteractiongrabbable and object manipulator
        // deactivate box colider , nearinteractiongrabbable and object manipulator
        mainObject.GetComponent<BoxCollider>().enabled = true;
        mainObject.GetComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>().enabled = true;
        mainObject.GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>().enabled = true;

    }

    private void playCanon()
    {
        AudioSource.PlayClipAtPoint(cannonFire, duplicateObject.transform.position);
    }

    IEnumerator BlastAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Blast();
    }

    private void Blast()
    {
        if (!isPlaying)
        {
            return;
        }
        Explode(false);
    }

    IEnumerator SetTooltipActiveAfterDelay(GameObject tooltip, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTooltipActive(tooltip);
    }

    IEnumerator SetTooltipInactiveAfterDelay(GameObject tooltip, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTooltipInactive(tooltip);
    }

    private void SetTooltipActive(GameObject tooltip)
    {
        if (!isPlaying)
        {
            return;
        }
        tooltip.SetActive(true);
    }

    private void SetTooltipInactive(GameObject tooltip)
    {
        if (!isPlaying)
        {
            return;
        }
        tooltip.SetActive(false);
    }

    IEnumerator StoryEndAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopThePlay();
    }

    public void StopThePlay()
    {
        if (!isPlaying)
        {
            return;
        }
        StopAllCoroutines();
        Reset();
        StoryModeEnd();
        audioSource.Stop();
        directLight.SetActive(true);
        Adhisthana.SetActive(false);
        Bhitti.SetActive(false);
        Devakosthas.SetActive(false);
        Vimana.SetActive(false);
        isPlaying = false;
        Debug.Log("StopThePlay");
    }

}