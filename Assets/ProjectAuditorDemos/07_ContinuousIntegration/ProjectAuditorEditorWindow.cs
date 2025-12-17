#if UNITY_EDITOR
using System.Linq;
using ProjectAuditorDemos._07_CI;
using UnityEditor;
using UnityEngine;
using Unity.ProjectAuditor.Editor;

public class ProjectAuditorWindow : EditorWindow
{
    private string reportPath = "ProjectAuditorReport.projectauditor";

    private IssueCategory[] allCategories;
    private bool[] categorySelections;

    private BuildTarget buildTarget;

    private Vector2 scrollPos;

    [MenuItem("Tools/Project Auditor/Run Auditor…")]
    public static void ShowWindow()
    {
        // Open the window and give it a title
        var window = GetWindow<ProjectAuditorWindow>("Project Auditor");
        window.minSize = new Vector2(400, 400);
    }

    private void OnEnable()
    {
        // Safe place to call Unity APIs like activeBuildTarget
        buildTarget = EditorUserBuildSettings.activeBuildTarget;

        // Populate IssueCategory toggles
        allCategories = (IssueCategory[])System.Enum.GetValues(typeof(IssueCategory));
        categorySelections = new bool[allCategories.Length];
        for (int i = 0; i < categorySelections.Length; i++)
            categorySelections[i] = true;
    }

    private void OnGUI()
    {
        GUILayout.Label("Project Auditor – Run Configuration", EditorStyles.boldLabel);

        // Report path entry
        GUILayout.Space(6);
        EditorGUILayout.BeginHorizontal();
        reportPath = EditorGUILayout.TextField("Report Path", reportPath);
        if (GUILayout.Button("Browse…", GUILayout.Width(80)))
        {
            string chosen = EditorUtility.SaveFilePanel(
                "Save Project Auditor Report",
                "",
                "ProjectAuditorReport",
                "projectauditor");

            if (!string.IsNullOrEmpty(chosen))
                reportPath = chosen;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Category selection
        GUILayout.Label("Include Issue Categories", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        for (int i = 0; i < allCategories.Length; i++)
        {
            categorySelections[i] = EditorGUILayout.ToggleLeft(
                allCategories[i].ToString(), categorySelections[i]);
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        // Target build platform
        GUILayout.Label("Target Platform", EditorStyles.boldLabel);
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildTarget);

        GUILayout.Space(16);

        if (GUILayout.Button("Run Auditor", GUILayout.Height(32)))
        {
            RunAuditor();
        }
    }

    private void RunAuditor()
    {
        // Collect selected categories
        var selectedCategories = allCategories
            .Where((cat, idx) => categorySelections[idx])
            .ToArray();

        Debug.Log(
            $"[Project Auditor] Running audit with categories: {string.Join(", ", selectedCategories)}");

        var analysisParams = new AnalysisParams
        {
            Categories = selectedCategories,
            Platform = buildTarget
        };

        ProjectAuditorCI.AuditAndExport(reportPath, analysisParams);
    }
}
#endif
