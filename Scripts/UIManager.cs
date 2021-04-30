using System;
using System.IO;
using Spotboo.Unity.Methods;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour, IInitializable
{
    public static UIManager UIM { get; private set; }
    public TextMeshProUGUI textGeneration;
    public TextMeshProUGUI textSpeed;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textAlive;
    public TextMeshProUGUI textStartButton;
    public TextMeshProUGUI textRestartButton;
    public TMP_InputField inputPopulationSize;
    public TMP_InputField inputMaxSpeed;
    public TMP_InputField inputTimeToMaxSpeed;
    public TMP_InputField inputAIInterval;
    public TMP_InputField inputMutationChance;
    public TMP_InputField inputSeed;

    public static string SettingsFilePath { get => Path.Combine(Application.dataPath, "Settings.json"); }

    public void PreInit()
    {
        UIM = this;
    }

    public void Init()
    {
        LoadSettings();
        UpdateAll();
    }

    public void PostInit() { }

    private void Update()
    {
        textGeneration.text = $"Generation: {SimulationController.Generation}";
        textSpeed.text = $"Speed: {SimulationController.GameSpeed:F3}";
        textScore.text = $"Score: {(int) SimulationController.Score}";
        textAlive.text = $"Alive: {SimulationController.AliveDinos.Count}";
        textStartButton.text = SimulationController.CurrentState switch
        {
            SimulationState.Running => "Stop Simulation",
            SimulationState.Stopped => "Start Simulation",
            _ => "Stopping Simulation"
        };
        textRestartButton.text = SimulationController.CurrentState == SimulationState.Stopped ? "Restart Simulation" : "Can't restart now";
    }

    public void InputUpdate(string id)
    {
        switch (id.ToLower())
        {
            case "population_size":
                UpdatePopulationSize();
                break;
            case "max_speed":
                UpdateMaxSpeed();
                break;
            case "time_to_max_speed":
                UpdateTimeToMaxSpeed();
                break;
            case "ai_interval":
                UpdateAIInterval();
                break;
            case "mutation_chance":
                UpdateMutationChance();
                break;
            case "seed":
                UpdateSeed();
                break;
        }

        SaveSettings();
    }

    private void SaveSettings()
    {
        SettingsData saveData = new SettingsData
        {
            dinoCount = SimulationController.PopulationSize,
            aiInterval = SimulationController.AIInterval,
            maxSpeed = SimulationController.MaxSpeed,
            mutationChance = SimulationController.MutationChance,
            timeToMaxSpeed = SimulationController.TimeToMaxSpeed,
            seed = SimulationController.Seed
        };
        SaveSystem.SaveJson(saveData, SettingsFilePath);
    }

    private void LoadSettings()
    {
        SettingsData saveData = SaveSystem.LoadJson<SettingsData>(SettingsFilePath);

        if (saveData == null) return;
        inputPopulationSize.text = saveData.dinoCount + "";
        inputAIInterval.text = saveData.aiInterval + "";
        inputMaxSpeed.text = saveData.maxSpeed + "";
        inputMutationChance.text = saveData.mutationChance + "";
        inputTimeToMaxSpeed.text = saveData.timeToMaxSpeed + "";
        inputSeed.text = saveData.seed + "";
    }

    public void ButtonStartSimulation()
    {
        switch (SimulationController.CurrentState)
        {
            case SimulationState.Running:
                SimulationController.CurrentState = SimulationState.Stopping;
                break;
            case SimulationState.Stopping:
                SimulationController.CurrentState = SimulationState.Running;
                break;
            default:
                SimulationController.SM.StartSimulation();
                break;
        }
    }

    public void RestartSimulation()
    {
        if (SimulationController.CurrentState == SimulationState.Stopped)
            SceneManager.LoadScene(0);
    }

    public void QuitSimulation()
    {
        Application.Quit();
    }

    private void UpdateAll()
    {
        UpdatePopulationSize();
        UpdateMaxSpeed();
        UpdateTimeToMaxSpeed();
        UpdateAIInterval();
        UpdateMutationChance();
        UpdateSeed();
    }

    private void UpdatePopulationSize()
    {
        if (int.TryParse(inputPopulationSize.text, out int value))
            SimulationController.PopulationSize = Mathf.Max(value, 0);
        inputPopulationSize.text = SimulationController.PopulationSize + "";
    }

    private void UpdateMaxSpeed()
    {
        if (int.TryParse(inputMaxSpeed.text, out int value))
            SimulationController.MaxSpeed = Mathf.Max(value, 1);
        inputMaxSpeed.text = SimulationController.MaxSpeed + "";
    }

    private void UpdateTimeToMaxSpeed()
    {
        if (int.TryParse(inputTimeToMaxSpeed.text, out int value))
            SimulationController.TimeToMaxSpeed = Mathf.Max(value, 0);
        inputTimeToMaxSpeed.text = SimulationController.TimeToMaxSpeed + "";
    }

    private void UpdateAIInterval()
    {
        if (int.TryParse(inputAIInterval.text, out int value))
            SimulationController.AIInterval = Mathf.Max(value, 1);
        inputAIInterval.text = SimulationController.AIInterval + "";
    }

    private void UpdateMutationChance()
    {
        if (float.TryParse(inputMutationChance.text, out float value))
            SimulationController.MutationChance = Mathf.Max(value, 1);
        inputMutationChance.text = SimulationController.MutationChance + "";
    }

    private void UpdateSeed()
    {
        if (int.TryParse(inputSeed.text, out int value))
            SimulationController.Seed = value;
        inputSeed.text = SimulationController.Seed + "";
    }
}

[Serializable]
public class SettingsData
{
    public int dinoCount;
    public int aiInterval;
    public float maxSpeed;
    public float mutationChance;
    public float timeToMaxSpeed;
    public float seed;
}