using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Variables for inspector to change
    [SerializeField] Slider fuelSlider; 
    [SerializeField] TMP_Text fuelText;
    [SerializeField] TMP_Text statusText;

    // --- Fuel drain rates per boulder type (from brief) ---
    float drainRateLight  = 1f;
    float drainRateMedium = 5f;
    float drainRateHeavy  = 8f;

    // --- Private State variables ---
    float fuel = 100f;
    float currentBoulderDrain = 0f;
    int bouldersDelivered = 0;
    int totalBoulders = 3;
    bool gameWon = false;

    // References for resetting

    //storing references to the other scripts' components. hence declared as fields.
    RocketController rocketController; 
    TetherController tetherController;
    Vector3 rocketStartPosition;

    // Boulder reset data

    //arrays to store all boulders of each type and their original positions.
    GameObject[] lightBoulders;
    GameObject[] mediumBoulders;
    GameObject[] heavyBoulders;
    Vector3[] lightStartPositions;
    Vector3[] mediumStartPositions;
    Vector3[] heavyStartPositions;

    void Start()
    {
        //FindObjectOfType will return the first component of type its finds
        rocketController = FindObjectOfType<RocketController>();
        tetherController = FindObjectOfType<TetherController>();

        //this is to read the rocket's starting position (for resetting purposes too)
        rocketStartPosition = rocketController.transform.position;


        //this method will find all boulders and records their positions.
        StoreBoulderPositions();

        fuelSlider.maxValue = 100f;

        // this will force the UI to restart
        RefreshAllUI();
    }

    void Update()
    {
        if (gameWon) return; //this will make sure to stop everything once we won

        if (currentBoulderDrain > 0f)
        {
            DrainFuel(currentBoulderDrain * Time.deltaTime); //make sure to drain per second and not per frame by Time.deltaTime
        }

        UpdateFuelUI();


        //triggering reset when the fuel ran out
        if (fuel <= 0f)
        {
            ResetGame();
        }
    }

    void StoreBoulderPositions()
    {
        //finding the specific object with their tags
        lightBoulders  = GameObject.FindGameObjectsWithTag("BoulderLight");
        mediumBoulders = GameObject.FindGameObjectsWithTag("BoulderMedium");
        heavyBoulders  = GameObject.FindGameObjectsWithTag("BoulderHeavy");

        //getting the starting position of the boulders
        lightStartPositions  = GetPositions(lightBoulders);
        mediumStartPositions = GetPositions(mediumBoulders);
        heavyStartPositions  = GetPositions(heavyBoulders);
    }

    //this method returns an Vector3 array to get positions of the boulders
    Vector3[] GetPositions(GameObject[] objects)
    {
        //will returns an array of the exact amount of the input array
        Vector3[] positions = new Vector3[objects.Length];
        //looping through the array
        for (int i = 0; i < objects.Length; i++)
        {
            //read the boulders position and storing it in the matching index
            positions[i] = objects[i].transform.position;
        }
        return positions;
    }

    // Refreshes every UI element at once
    // Call this on Start and after every reset
    void RefreshAllUI()
    {
        //making sure the fuel rate doesn't go below 0
        fuel = Mathf.Max(fuel, 0f);
        fuelSlider.value = fuel;

        //updating the fuel status text rounded to the nearest integer
        if (fuelText != null)
            fuelText.text = "Fuel: " + Mathf.RoundToInt(fuel);

        //updating the boulder delivery status text
        if (statusText != null)
        {
            int remaining = totalBoulders - bouldersDelivered;
            statusText.text = "Boulders remaining: " + remaining + " / " + totalBoulders;
        }
    }

    //updating the fuel UI elements
    void UpdateFuelUI()
    {
        fuelSlider.value = fuel;

        if (fuelText != null)
            fuelText.text = "Fuel: " + Mathf.RoundToInt(fuel);
    }

    //method will draining the fuel while applying thrust and carrying bouldes
    public void DrainFuel(float amount)
    {
        fuel -= amount;
        //clamping to zero to avoid negative fuel value
        fuel = Mathf.Max(fuel, 0f);
    }

    //method is to set the current boulder drain rate based on the boulder tag that is being carried
    public void SetCurrentBoulder(string boulderTag)
    {
        if (boulderTag == "BoulderLight")
            currentBoulderDrain = drainRateLight;
        else if (boulderTag == "BoulderMedium")
            currentBoulderDrain = drainRateMedium;
        else if (boulderTag == "BoulderHeavy")
            currentBoulderDrain = drainRateHeavy;
        else
            currentBoulderDrain = 0f;
    }

    public void BoulderDelivered()
    {
        bouldersDelivered++;
        //stopping the boulder drain when it's delivered
        currentBoulderDrain = 0f;


    Debug.Log("Delivered: " + bouldersDelivered + "/" + totalBoulders);
        

        // Refresh UI immediately after delivery
        RefreshAllUI();

        //checking if all the boulders are delivered so it can trigger wingame method
        if (bouldersDelivered >= totalBoulders)
        {
            WinGame();
            return;
        }
    }

    void WinGame()
    {
        //stopping all the updates by flipping it to True
        gameWon = true;

        Debug.Log("YOU WIN! YAYY!");
        
        //setting the text components text directly to the win message
        if (statusText != null)
            statusText.text = "YOU WIN! All boulders delivered! Resetting in 3 seconds...";

        // Auto reset after 3 seconds
        //cancelling any pending invoke calls on the monobehaviour
        CancelInvoke();
        //invoking a method after 3 secs delay
        Invoke("ResetGame", 3f);
    }

    void ResetGame()
    {
        Debug.Log("Resetting game...");

        // Cancel any pending Invoke calls like the auto reset after wining
        CancelInvoke();

        // Resetting all states
        gameWon = false;
        fuel = 100f;
        bouldersDelivered = 0;
        currentBoulderDrain = 0f;

        // Resetting rocket position
        rocketController.ResetRocket(rocketStartPosition);

        // Resetting tether
        tetherController.ResetTether();

        //finding dropzonecontroller in the scene and calling its reset method
        FindObjectOfType<DropZoneController>().ResetDropZone();

        // Resetting all boulders
        ResetBoulders(lightBoulders,  lightStartPositions);
        ResetBoulders(mediumBoulders, mediumStartPositions);
        ResetBoulders(heavyBoulders,  heavyStartPositions);

        // Force=ing full UI refresh after reset
        RefreshAllUI();
    }

    void ResetBoulders(GameObject[] boulders, Vector3[] positions)
    {
        for (int i = 0; i < boulders.Length; i++)
        {
            //setting the transform position of each boulder back to start position
            boulders[i].transform.position = positions[i];

            Rigidbody rb = boulders[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                //since delivered boulders are frozen in place, this will unfreeze them
                rb.constraints = RigidbodyConstraints.None;

                //this will reset any residual momentum
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}