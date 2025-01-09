using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using SFB;
using UnityEngine.Events;

public class FileImporter : MonoBehaviour
{
    [SerializeField]
    private Text displayText; // Reference to a UI Text component to display the file content

    public UnityEvent<string> OnImportFileListener = new UnityEvent<string>();

    public void ImportFile()
    {
        // Open file with filter
        var extensions = new [] {
            new ExtensionFilter("Text Files", "txt"),
            new ExtensionFilter("CSV Files", "csv"),
            new ExtensionFilter("All Files", "*"),
        };
        
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, false, (string[] paths) => {
            if (paths.Length > 0)
            {
                string path = paths[0];
                try
                {
                    string fileContent = File.ReadAllText(path);
                    Debug.Log(fileContent);
                    DisplayFileContent(fileContent);
                    Debug.Log("File imported successfully!");
                    OnImportFileListener.Invoke(fileContent);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error reading file: " + e.Message);
                }
            }
            else
            {
                Debug.Log("File import cancelled.");
            }
        });
    }

    private void DisplayFileContent(string content)
    {
        if (displayText != null)
        {
            displayText.text = content;
        }
        else
        {
            Debug.LogWarning("DisplayText is not assigned. Assign a UI Text component to display the file content.");
        }
    }
}