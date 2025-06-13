using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Formats.Fbx.Exporter; // FBX exporter 4.1.0
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityObject = UnityEngine.Object; // Alias UnityEngine.Object to avoid ambiguity
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class BatchImportExportProcthor : EditorWindow
{
    private static readonly string[] cleanedNames = new string[] {"Structure", "Objects", "ProceduralLighting"};

    private static readonly string[] cleanedNamePrefixes = new string[] {"Structure_", "Objects_"};

    private static readonly string[] allowedExactNames = new string[] {"Structure", "Objects"};
    private static readonly string[] allowedNamePrefixes = new string[] {"Structure_", "Objects_"};

    // Flag to track if 'init p' has been executed
    private static bool isInitExecuted = false;


    [MenuItem("Tools/Batch_Convert_JSON_To_FBX")]
    public static void BatchProcess()
    {
        Debug.Log("[BatchProcess] Starting batch processing...");

        // Load the Procedural scene
        string proceduralScenePath = "Assets/Scenes/Procedural/Procedural.unity";
        if (!File.Exists(proceduralScenePath))
        {
            Debug.LogError($"[BatchProcess] Procedural scene not found at: {proceduralScenePath}");
            return;
        }

        // Open the Procedural scene
        Scene scene = EditorSceneManager.OpenScene(proceduralScenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError("[BatchProcess] Failed to open Procedural scene.");
            return;
        }
        Debug.Log($"[BatchProcess] Opened scene: {proceduralScenePath}");

        // global::System.Object value = EditorUtility.DisplayProgressBar("Batch Processing", "Initialized Procedural Scene", 0f);
        Debug.Log($"[BatchProcess] Initialized Procedural scene. Waiting for scene load...");


        // Check current Play Mode state
        bool wasPlaying = EditorApplication.isPlaying;
        Debug.Log("[BatchProcess] Current Play Mode state: " + (wasPlaying ? "Playing" : "Not Playing"));
        if (!wasPlaying)
        {
            // // Subscribe to Play Mode state changes
            // EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // Enter Play Mode
            EnterPlayMode();
        }


    }


    // Method called by PlayModeStateListener when Play Mode is entered
    public static void OnPlayModeEntered()
    {

        // TODO: specify your json files path and export path
        string roomsPath = "ai2thor/scenes_json";
        string exportFBXPath = "Assets/ExportedFBX";

        Debug.Log("[BatchProcess] Already in Play Mode. Executing 'initp'...");
        ExecuteCommand("initp");


        // Get all JSON files in the rooms directory
        string[] jsonFiles = Directory.GetFiles(roomsPath, "*.json");
        Debug.Log("[BatchProcess] Found JSON files in rooms directory: " + jsonFiles.Length);
        int totalFiles = jsonFiles.Length;
        for (int i = 0; i < totalFiles; i++)
        {
            string jsonFile = jsonFiles[i];
            string fileName = Path.GetFileNameWithoutExtension(jsonFile);
            Debug.Log($"[JSON] Processing JSON file: {fileName}");

            // Execute the "chp [fileName]" command
            ExecuteCommand($"chp {fileName}");
            Debug.Log($"[BatchProcess] Executed 'chp {fileName}' command. Waiting for scene load...");

            // Update progress bar
            float progress = (float)(i + 1) / totalFiles;
            EditorUtility.DisplayProgressBar("Batch Processing", $"Exporting {fileName}", progress);

            // Export the scene to FBX using updated single-file export
            ExportCurrentSceneToFBX(fileName, exportFBXPath);
        }
    }

    private static void ExportCurrentSceneToFBX(string fileName, string exportFBXPath)
    {
        if (!exportFBXPath.StartsWith("Assets/"))
        {
            Debug.LogError("[FBX Export] Export path must be within the 'Assets' folder. Please provide a valid path inside the Assets directory.");
            return;
        }

        Scene activeScene = EditorSceneManager.GetActiveScene();
        if (!activeScene.IsValid())
        {
            Debug.LogError("[FBX Export] No valid active scene found. Make sure a scene is loaded.");
            return;
        }

        // Log all root GameObjects for debugging
        LogAllRootGameObjects(activeScene);

        // Collect all eligible root GameObjects based on name criteria
        List<GameObject> modelObjects = new List<GameObject>();
        List<GameObject> cleanObjects = new List<GameObject>();
        foreach (GameObject rootObj in activeScene.GetRootGameObjects())
        {
            Debug.Log($"[FBX Export] The root GameObject included by this scene: {rootObj.name}.");

            if (IsNameEligible(rootObj.name))
            {
                modelObjects.Add(rootObj);
            }

            if (IsNameEligibleClean(rootObj.name))
            {
                cleanObjects.Add(rootObj);
            }
            
        }

        if (modelObjects.Count == 0)
        {
            Debug.LogWarning("[FBX Export] No eligible root GameObjects found in the active scene to export.");
            return;
        }

        // Create a temporary parent GameObject to hold all eligible GameObjects
        GameObject tempParent = new GameObject("TempExportParent");

        try
        {
            // Duplicate each eligible GameObject and parent it under the temporary parent
            foreach (GameObject obj in modelObjects)
            {
                if (obj == tempParent) continue; // Avoid self-parenting

                // Duplicate the GameObject and its entire hierarchy
                GameObject duplicate = GameObject.Instantiate(obj);
                duplicate.name = obj.name; // Preserve original name
                duplicate.transform.SetParent(tempParent.transform, false); // Preserve local transforms

                Debug.Log($"[Export] Duplicated GameObject: {duplicate.name} with children count: {duplicate.transform.childCount}");
            }

            // After duplication, log the tempParent's children for verification
            Debug.Log($"[Export] TempExportParent has {tempParent.transform.childCount} children after duplication.");
            for (int i = 0; i < tempParent.transform.childCount; i++)
            {
                Transform child = tempParent.transform.GetChild(i);
                Debug.Log($"[Export] - Child {i + 1}: {child.gameObject.name} with {child.childCount} children.");
            }

            // Optionally, adjust the position or other properties of tempParent if needed
            tempParent.transform.position = Vector3.zero;
            tempParent.transform.rotation = Quaternion.identity;
            tempParent.transform.localScale = Vector3.one;

            // Define the full export path for the single FBX file
            string FileExportPath = Path.Combine(exportFBXPath, $"{fileName}.fbx").Replace("\\", "/");

            // Ensure that tempParent has been correctly set up
            if (tempParent.transform.childCount == 0)
            {
                Debug.LogError("[Export] TempExportParent has no children to export.");
                return;
            }

            // Export the temporary parent (and all its children) as a single FBX
            ModelExporter.ExportObject(FileExportPath, tempParent);

            Debug.Log($"[Export] Successfully exported eligible GameObjects to FBX at: {FileExportPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Export] Failed to export eligible GameObjects to FBX: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            // Cleanup
            // 1. Destroy the temporary parent and its duplicates
            if (tempParent != null)
            {
                GameObject.DestroyImmediate(tempParent);
                Debug.Log("[Cleanup] Destroyed TempExportParent and its duplicates.");
            }

            // 2. Destroy the specified GameObjects to be removed
            foreach (GameObject obj in cleanObjects)
            {
                if (obj != null)
                {
                    Debug.Log($"[Cleanup] Destroyed original GameObject: {obj.name}");
                    GameObject.DestroyImmediate(obj);
                }
            }

            GameObject newObjects = new GameObject("Objects");
            Debug.Log("[Cleanup] Created new empty GameObject 'Objects'.");
            Debug.Log("[Cleanup] Finished cleaning up temporary and original GameObjects.");
        }
    }

    private static void OnInitpCompleted()
    {
        if (!isInitExecuted)
        {
            isInitExecuted = true;
            Debug.Log("[BatchProcess] 'initp' executed successfully. Proceeding with JSON loading...");

            // // Proceed with loading and processing JSON files
            // LoadAndProcessJSON();

            // Unsubscribe from OnInitialized to prevent multiple calls
            DebugInputField.OnInitialized -= OnInitpCompleted;

            // Optionally, exit Play Mode
            // EditorApplication.ExitPlaymode();
        }
    }

    private static void ExecuteCommand(string command)
    {
        try
        {
            Debug.Log($"[ExecuteCommand Batch1] Executing command: {command}");
            // Find the DebugInputField script in the scene
            DebugInputField debugInput = UnityObject.FindObjectOfType<DebugInputField>();
            if (debugInput != null)
            {
                debugInput.ExecuteCommand(command);
                Debug.Log($"[ExecuteCommand Batch2] Executed command: {command}");
            }
            else
            {
                Debug.LogError("[ExecuteCommand] DebugInputField not found in the scene. Ensure the scene is in Play mode and the component exists.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[ExecuteCommand] Error executing command '{command}': {e.Message}");
        }
    }

    private static void LogAllRootGameObjects(Scene scene)
    {
        Debug.Log("=== Root GameObjects in the Scene ===");
        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            Debug.Log($"[Scene] Root GameObject: {rootObj.name}");
        }
    }

    private static bool IsNameEligible(string name)
    {
        // Check for exact name matches
        foreach (string exactName in allowedExactNames)
        {
            if (name.Equals(exactName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // Check for name prefixes
        foreach (string prefix in allowedNamePrefixes)
        {
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
    private static bool IsNameEligibleClean(string name)
    {
        // Check for exact name matches
        foreach (string exactName in cleanedNames)
        {
            if (name.Equals(exactName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // Check for name prefixes
        foreach (string prefix in cleanedNamePrefixes)
        {
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    // [MenuItem("Tools/Enter Play Mode")] // Add this to create a menu item
    private static void EnterPlayMode()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.Log("[BatchProcess] Entering Play Mode...");
            EditorApplication.EnterPlaymode();
        }
        else
        {
            Debug.Log("[BatchProcess] Already in Play mode.");
        }
    }

}