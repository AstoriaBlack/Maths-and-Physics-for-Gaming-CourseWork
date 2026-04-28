using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // --- Inspector Settings ---
    [SerializeField] Slider fuelSlider;
    [SerializeField] TMP_Text fuelText;
    [SerializeField] TMP_Text statusText;

    // --- Fuel drain rates per boulder type (from brief) ---
    float drainRateLight  = 1f;
    float drainRateMedium = 5f;
    float drainRateHeavy  = 8f;

    // --- Private State ---
    float fuel = 100f;
    float currentBoulderDrain = 0f;
    int bouldersDelivered = 0;
    int totalBoulders = 3;
    bool gameWon = false;

    // References for resetting
    RocketController rocketController;
    TetherController tetherController;
    Vector3 rocketStartPosition;

    // Boulder reset data
    GameObject[] lightBoulders;
    GameObject[] mediumBoulders;
    GameObject[] heavyBoulders;
    Vector3[] lightStartPositions;
    Vector3[] mediumStartPositions;
    Vector3[] heavyStartPositions;

    void Start()
    {
        rocketController = FindObjectOfType<RocketController>();
        tetherController = FindObjectOfType<TetherController>();

        rocketStartPosition = rocketController.transform.position;

        StoreBoulderPositions();

        fuelSlider.maxValue = 100f;

        // Force full UI refresh at start
        RefreshAllUI();
    }

    void Update()
    {
        if (gameWon) return;

        if (currentBoulderDrain > 0f)
        {
            DrainFuel(currentBoulderDrain * Time.deltaTime);
        }

        UpdateFuelUI();

        if (fuel <= 0f)
        {
            ResetGame();
        }
    }

    void StoreBoulderPositions()
    {
        lightBoulders  = GameObject.FindGameObjectsWithTag("BoulderLight");
        mediumBoulders = GameObject.FindGameObjectsWithTag("BoulderMedium");
        heavyBoulders  = GameObject.FindGameObjectsWithTag("BoulderHeavy");

        lightStartPositions  = GetPositions(lightBoulders);
        mediumStartPositions = GetPositions(mediumBoulders);
        heavyStartPositions  = GetPositions(heavyBoulders);
    }

    Vector3[] GetPositions(GameObject[] objects)
    {
        Vector3[] positions = new Vector3[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            positions[i] = objects[i].transform.position;
        }
        return positions;
    }

    // Refreshes every UI element at once
    // Call this on Start and after every reset
    void RefreshAllUI()
    {
        fuel = Mathf.Max(fuel, 0f);
        fuelSlider.value = fuel;

        if (fuelText != null)
            fuelText.text = "Fuel: " + Mathf.RoundToInt(fuel);

        if (statusText != null)
        {
            int remaining = totalBoulders - bouldersDelivered;
            statusText.text = "Boulders remaining: " + remaining + " / " + totalBoulders;
        }
    }

    void UpdateFuelUI()
    {
        fuelSlider.value = fuel;

        if (fuelText != null)
            fuelText.text = "Fuel: " + Mathf.RoundToInt(fuel);
    }

    public void DrainFuel(float amount)
    {
        fuel -= amount;
        fuel = Mathf.Max(fuel, 0f);
    }

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
        currentBoulderDrain = 0f;


    Debug.Log("Delivered: " + bouldersDelivered + "/" + totalBoulders);
        

        // Refresh UI immediately after delivery
        RefreshAllUI();

        if (bouldersDelivered >= totalBoulders)
        {
            WinGame();
            return;
        }
    }

    void WinGame()
    {
        gameWon = true;

        Debug.Log("YOU WIN! YAYY!");

        if (statusText != null)
            statusText.text = "YOU WIN! All boulders delivered! Resetting in 3 seconds...";

        // Auto reset after 3 seconds
        CancelInvoke();
        Invoke("ResetGame", 3f);
    }

    void ResetGame()
    {
        Debug.Log("Resetting game...");

        // Cancel any pending Invoke calls (e.g. WinGame's 3 second timer)
        CancelInvoke();

        // Reset all state
        gameWon = false;
        fuel = 100f;
        bouldersDelivered = 0;
        currentBoulderDrain = 0f;

        // Reset rocket
        rocketController.ResetRocket(rocketStartPosition);

        // Reset tether
        tetherController.ResetTether();

        FindObjectOfType<DropZoneController>().ResetDropZone();

        // Reset all boulders
        ResetBoulders(lightBoulders,  lightStartPositions);
        ResetBoulders(mediumBoulders, mediumStartPositions);
        ResetBoulders(heavyBoulders,  heavyStartPositions);

        // Force full UI refresh after reset
        RefreshAllUI();
    }

    void ResetBoulders(GameObject[] boulders, Vector3[] positions)
    {
        for (int i = 0; i < boulders.Length; i++)
        {
            boulders[i].transform.position = positions[i];

            Rigidbody rb = boulders[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Unfreeze first — delivered boulders get frozen, must undo on reset
                rb.constraints = RigidbodyConstraints.None;

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}