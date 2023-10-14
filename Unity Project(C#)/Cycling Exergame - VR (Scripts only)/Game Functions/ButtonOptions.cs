using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;
using Valve.VR.Extras;
using Valve.VR;
using System;

public class ButtonOptions : MonoBehaviour
{
    public UITimer uiTimer;
    public Button singleCompetitorButton;
    public Button multiCompetitorButton;
    public TMP_InputField nameInputField;
    public TMP_InputField ageInputField;
    public TMPro.TMP_Text cheerText;

    public SteamVR_LaserPointer laserPointer;
    private bool pointerOnButton = false;
    private Button currentlyPointedButton;
    // GameObject myEventSystem;
    public SteamVR_Action_Boolean triggerAction;

    void Start()
    {
        // Debug.Log("Start method in ButtonOptions was called.");
        // laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer = FindObjectOfType<SteamVR_LaserPointer>();
        laserPointer.PointerIn += LaserPointer_PointerIn;
        laserPointer.PointerOut += LaserPointer_PointerOut;
        // Debug.Log("Events have been bound.");
        UpdateButtonStatus();
        string playerName = PlayerPrefs.GetString("PlayerName", "Anonymous");
        cheerText.text = $"Go {playerName}!";
    }


    private void LaserPointer_PointerIn(object sender, PointerEventArgs e)
    {
        // Debug.Log("Entered LaserPointer_PointerIn.");
        if (e.target.gameObject.GetComponent<Button>() != null && currentlyPointedButton == null)
        {
            currentlyPointedButton = e.target.gameObject.GetComponent<Button>();
            currentlyPointedButton.Select();
            pointerOnButton = true;
            // Debug.Log("Pointer is over button: " + currentlyPointedButton.name);
            // Debug.Log("Pointer is over an object with a collider: " + e.target.gameObject.name);
        }
    }

    private void LaserPointer_PointerOut(object sender, PointerEventArgs e)
    {
        // Debug.Log("Entered LaserPointer_PointerOut.");
        if (currentlyPointedButton != null)
        {
            pointerOnButton = false;
            // myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            currentlyPointedButton = null;
            // Debug.Log("LaserPointer_PointerOut was called.");
        }
    }

    void Update()
    {
        if (triggerAction.GetStateDown(SteamVR_Input_Sources.LeftHand) && pointerOnButton)
        {
            currentlyPointedButton.onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (nameInputField.isFocused)
            {
                SavePlayerName();
            }
            else if (ageInputField.isFocused)
            {
                SavePlayerAge();
            }
        }
    }

    public void ActivateNameInput()
    {
        nameInputField.ActivateInputField();
        nameInputField.Select();
    }

    public void ActivateAgeInput()
    {
        ageInputField.ActivateInputField();
        ageInputField.Select();
    }

    public void PlayGame(){
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("PlayerAge");
        PlayerPrefs.Save();

        Application.Quit();
    }


    public void MainMenu(){
        SceneManager.LoadScene(1);
    }

    public void SinglePlayer(){
        SceneManager.LoadScene(2);
    }

    public void Relax(){
        SceneManager.LoadScene(3);
    }

    public void SingleCompetitor()
    {
        float bestTime = PlayerPrefs.GetFloat("RawTime", 0f);
        float playerAge = PlayerPrefs.GetFloat("PlayerAge", 0f);
        
        if(bestTime == 0 || playerAge == 0)
        {
            return;
        }
        else
        {
            SceneManager.LoadScene(4);
        }
    }


    public void MultipleCompetitors()
    {
        // Make sure there are at least 3 records in the Temp directory
        if (CheckMultiCompetitor())
        {
            SceneManager.LoadScene(6); 
        }
        else
        {
            Debug.Log("Not enough records to start a multiple competitors game.");
            return;
        }
    }

    public void ResetBestTimeButton()
    {
        PlayerPrefs.DeleteKey("RawTime");
        PlayerPrefs.DeleteKey("AverageHeartRate");
        // Update UI if necessary
        uiTimer.UpdateUI();
        UpdateButtonStatus();
        // Debug.Log("The button has been invoked!");
        // Debug.Log(Application.persistentDataPath);
        string tempFolderPath = Path.Combine(Application.persistentDataPath, "MyRecordings/Temp/");
        string bestRecordFolderPath = Path.Combine(Application.persistentDataPath, "MyRecordings/Temp/BestRecord/");

        DeleteFilesInDirectory(tempFolderPath);
        DeleteFilesInDirectory(bestRecordFolderPath);
    }

    public void SaveAgeButton()
    {
       SavePlayerAge(); 
       UpdateButtonStatus();
    }

    public void UpdateNameButton()
    {
       SavePlayerName(); 
    }

    public void UpdateButtonStatus()
    {
        float bestTime = PlayerPrefs.GetFloat("RawTime", 0f);
        float playerAge = PlayerPrefs.GetFloat("PlayerAge", 0f);
        // Debug.Log("Player Age: " + playerAge);
        if (bestTime == 0 || playerAge == 0)
        {
            singleCompetitorButton.interactable = false;  
        }
        else
        {
            singleCompetitorButton.interactable = true; 
        }
        
        // Check if there are enough records for the MultiCompetitor mode
        if (CheckMultiCompetitor() && playerAge != 0)
        {
            multiCompetitorButton.interactable = true; 
        }
        else
        {
            multiCompetitorButton.interactable = false; 
        }
    }

    public void SavePlayerAge()
    {
        float playerAge;
        
        // Try to convert the text to an integer
        if (float.TryParse(ageInputField.text, out playerAge))
        {
            PlayerPrefs.SetFloat("PlayerAge", playerAge);
            PlayerPrefs.Save();
            Debug.Log("Player Age: " + playerAge);
        }
        else
        {
            // Handle the error if the conversion fails, e.g., the input was not a valid number
            Debug.LogError("Invalid age input");
        }
    }


    public void SavePlayerName()
    {
        string playerName = nameInputField.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        Debug.Log("Player Name: " + playerName);
    }

    private void DeleteFilesInDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
        else
        {
            Debug.Log($"Directory not found: {directoryPath}");
        }
    }

    private bool CheckMultiCompetitor()
    {
        string tempFolderPath = Path.Combine(Application.persistentDataPath, "MyRecordings/Temp/");
        if (Directory.Exists(tempFolderPath))
        {
            var entries = Directory.GetFileSystemEntries(tempFolderPath);
            int fileCount = 0;

            foreach (string entry in entries)
            {
                // Only count the entry if it's a file
                if (File.Exists(entry))
                {
                    fileCount++;
                }
            }

            if (fileCount >= 3)
            {
                return true;
            }
        }

        return false;
    }
}
