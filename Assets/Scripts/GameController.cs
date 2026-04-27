using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //The variables for the inspeactor to change
    [SerializeField] Slider fuelSlider;

    [SerializeField] TMP_Text fuelText;

    //Fuel drain rates
    float drainRateLight = 1f;
    float drainRateMedium = 5f;
    float drainRateHeavy = 8f;

    //private state variables
    float fuel = 100f;
    float currentBoulderDrain = 0f;
    int bouldersDelivered = 0;
    int totalBoulders = 3;

    //References for resetting
    RocketController rocketController;
    TetherController tetherController;
    Vector3 rocketStartPosition;

    //Boulder rest data
    GameObject[] lightBoulders;
    GameObject[] mediumBoulders;
    GameObject[] heavyBoulders;
    Vector3[] lightStartPositions;
    Vector3[] mediumStartPositions;
    Vector3[] heavyStartPositions;

    // Start is called before the first frame update
    void Start()
    {
        rocketController = FindObjectOfType<RocketController>();
        tetherController = FindObjectOfType<TetherController>();

        //Rememberint to where the rocket starts for resetting later
        rocketStartPosition = rocketController.transform.position;

        //Storing all boulder start positions
        StoreBoulderPositions();
        
        //setting up the UI
        fuelSlider.maxValue = 100f;
        fuelSlider.value = fuel;

    }

    // Update is called once per frame
    void Update()
    {
        //Vpntinuously drain fuel if carrying a boulder
        if (currentBoulderDrain > 0f)
        {
            DrainFuel(currentBoulderDrain * Time.deltaTime);
        }

        UpdateFuelUI();

        //Checking if fuel has run out
        if(fuel <= 0f)
        {
            ResetGame();
        }
    }

    void StoreBoulderPositions()
    {
        lightBoulders = GameObject.FindGameObjectsWithTag("BoulderLight");
        mediumBoulders = GameObject.FindGameObjectsWithTag("BoulderMedium");
        heavyBoulders = GameObject.FindGameObjectsWithTag("BoulderHeavy");

        lightStartPositions = GetPositions(lightBoulders);
        mediumStartPositions = GetPositions(mediumBoulders);
        heavyStartPositions = GetPositions(heavyBoulders);
    }

    Vector3[] GetPositions(GameObject[] objects)
    {
        //Creating an array the same length as those objects
        Vector3[] positions = new Vector3[objects.Length];

        //looping through and storing each position
        for (int i=0; i < objects.Length; i++)
        {
            positions[i] = objects[i].transform.position;
        }

        return positions;
    }

    void UpdateFuelUI()
    {
        fuelSlider.value = fuel;
        fuelText.text = "Fuel:" + Mathf.RoundToInt(fuel);
    }

    //called by RocketController and ObstacleController to drain fuel
    public void DrainFuel(float amount)
    {
        fuel -= amount;

        //Mathf.Max stops fuel from going below 0
        fuel = Mathf.Max(fuel, 0f);

    }

    //This method is called by TetherController when boulder is picked up to set the current fuel drain rate
    public void SetCurrentBoulder(string boulderTag)
    {
        if(boulderTag == "BoulderLight")
        {
            currentBoulderDrain = drainRateLight;
        }
        else if (boulderTag == "BoulderMedium")
        {
            currentBoulderDrain = drainRateMedium;
        }
        else if (boulderTag == "BoulderHeavy")
        {
            currentBoulderDrain = drainRateHeavy;
        }
    }

    //This method is called by TetherController when boulder reached drop zone
    public void BoulderDelivered()
    {
        bouldersDelivered++;
        currentBoulderDrain = 0f;

        Debug.Log("Delivered: " + bouldersDelivered + "/" + totalBoulders);

        if (bouldersDelivered>= totalBoulders)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("YOU WIN! YAYY!");
        Time.timeScale = 0f; //this will freeze the game  
    }

    void ResetGame()
    {
        Debug.Log("Out of Fuel! Game Resetting...");

        //Resetting Counters
        fuel = 100f;
        bouldersDelivered = 0;
        currentBoulderDrain = 0f;

        //resetting rocket
        rocketController.ResetRocket(rocketStartPosition);

        //Resetting tether
        tetherController.ResetTether();

        //Resseting all the boulders
        ResetBoulders(lightBoulders, lightStartPositions);
        ResetBoulders(mediumBoulders, mediumStartPositions);
        ResetBoulders(heavyBoulders, heavyStartPositions);
    }

    void ResetBoulders(GameObject[] boulders, Vector3[] positions)
    {
        for (int i = 0; i < boulders.Length; i++)
        {
            boulders[i].transform.position = positions[i];

            Rigidbody rb = boulders[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
