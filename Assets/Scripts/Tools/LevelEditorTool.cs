using UnityEngine;
using UnityEditor;
using TowerDefense.Data;

public class LevelEditorTool : MonoBehaviour
{
    // This MonoBehaviour is just a handle to open the editor from the inspector.
    // All logic is in the EditorWindow.
}

#if UNITY_EDITOR

[CustomEditor(typeof(LevelEditorTool))]
public class LevelEditorToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Open Level Editor"))
        {
            LevelEditorWindow.ShowWindow();
        }
    }
}

public class LevelEditorWindow : EditorWindow
{
    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private LevelData currentLevelData;
    private Vector2Int gridSize = new Vector2Int(10, 10);
    private Vector2Int newGridSize;
    private Vector2 scrollPosition;
    private bool isDirty = false;

    // Colors for the grid buttons
    private readonly Color greenColor = new Color(0.4f, 0.9f, 0.4f, 1f);
    private readonly Color grayColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    private void OnGUI()
    {
        // --- Toolbar ---
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("New Level", EditorStyles.toolbarButton))
        {
            if (isDirty && !EditorUtility.DisplayDialog("Unsaved Changes", "You have unsaved changes. Do you want to discard them and create a new level?", "Yes, Discard", "No"))
            {
                return; // User cancelled
            }
            NewLevel();
        }
        if (GUILayout.Button("Save Level", EditorStyles.toolbarButton))
        {
            SaveLevel();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // --- Main Content ---
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

        LevelData previousLevelData = currentLevelData;
        currentLevelData = (LevelData)EditorGUILayout.ObjectField("Current Level Data", currentLevelData, typeof(LevelData), false);
 
        // If the user drags a new asset into the field, reset the dirty flag.
        if (currentLevelData != previousLevelData && currentLevelData != null)
        {
            isDirty = false;
            newGridSize = new Vector2Int(currentLevelData.width, currentLevelData.height);
        }

        if (currentLevelData == null)
        {
            EditorGUILayout.HelpBox("No level data loaded. Create a new level or load an existing one.", MessageType.Info);
            gridSize = EditorGUILayout.Vector2IntField("New Grid Size", gridSize);
        }
        else
        {
            // Display and edit grid size
            EditorGUILayout.BeginHorizontal();
            newGridSize = EditorGUILayout.Vector2IntField("Grid Size", newGridSize);
            if (GUILayout.Button("Apply Size", GUILayout.Width(80)))
            {
                if (newGridSize.x > 0 && newGridSize.y > 0)
                {
                    Undo.RecordObject(currentLevelData, "Resize Grid");
                    currentLevelData.ResizeGrid(newGridSize.x, newGridSize.y);
                    isDirty = true;
                    EditorUtility.SetDirty(currentLevelData);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Size", "Grid dimensions must be positive.", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();

            // Grid view
            if (currentLevelData.grid != null && currentLevelData.grid.Count > 0)
            {
                EditorGUILayout.LabelField("Grid - Toggle Obstacles", EditorStyles.boldLabel);

                // Calculate button size to fit window width while maintaining a square ratio
                float padding = 30f; // For scrollbar and margins
                float availableWidth = EditorGUIUtility.currentViewWidth - padding;
                // Ensure at least a minimum size, and also don't let them get too huge.
                float buttonSize = Mathf.Clamp(availableWidth / currentLevelData.width, 20f, 50f);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                Color originalColor = GUI.backgroundColor; // Store original color

                for (int y = 0; y < currentLevelData.height; y++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int x = 0; x < currentLevelData.width; x++)
                    {
                        TileData tile = currentLevelData.GetTile(x, y);
                        if (tile != null)
                        {
                            // Set button color based on obstacle state (Green = Placeable, Gray = Obstacle)
                            GUI.backgroundColor = tile.isObstacle ? grayColor : greenColor;

                            // The button itself
                            if (GUILayout.Button(" ", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                            {
                                Undo.RecordObject(currentLevelData, "Toggle Obstacle");
                                tile.isObstacle = !tile.isObstacle;
                                isDirty = true; // Mark changes as unsaved
                                EditorUtility.SetDirty(currentLevelData);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUI.backgroundColor = originalColor; // Restore original color
                EditorGUILayout.EndScrollView();
            }
        }
    }

    private void NewLevel()
    {
        currentLevelData = CreateInstance<LevelData>();
        currentLevelData.width = gridSize.x;
        currentLevelData.height = gridSize.y;
        currentLevelData.InitializeGrid();
        isDirty = false; // A new level is not dirty until modified
    }

    private void SaveLevel()
    {
        if (currentLevelData == null)
        {
            EditorUtility.DisplayDialog("No Level Loaded", "There is no level data to save. Please create or load a level first.", "OK");
            return;
        }

        // If the asset doesn't have a path, it's a new in-memory asset.
        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(currentLevelData)))
        {
            SaveLevelAs();
        }
        else
        {
            // Asset already exists, just save it.
            EditorUtility.SetDirty(currentLevelData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            isDirty = false;
            EditorUtility.DisplayDialog("Level Saved", $"Successfully saved '{currentLevelData.name}'.", "OK");
        }
    }

    private void SaveLevelAs()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save New Level Data",
            "NewLevelData.asset",
            "asset",
            "Please enter a file name to save the level data to.");

        if (string.IsNullOrEmpty(path)) return;

        AssetDatabase.CreateAsset(currentLevelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        isDirty = false;
        EditorUtility.DisplayDialog("Level Saved", $"Successfully saved level as '{path}'.", "OK");
    }

    // Handle unsaved changes on window close
    private void OnDestroy()
    {
        if (isDirty)
        {
            if (EditorUtility.DisplayDialog("Unsaved Changes", "You have unsaved changes. Do you want to save them before closing?", "Save", "Don't Save"))
            {
                SaveLevel();
            }
        }
    }
}
#endif
