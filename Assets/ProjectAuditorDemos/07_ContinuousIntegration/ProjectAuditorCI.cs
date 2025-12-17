#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.ProjectAuditor.Editor;

namespace ProjectAuditorDemos._07_CI
{
    public static class ProjectAuditorCI
    {
        public static void AuditAndExport(
            string reportPath,
            AnalysisParams analysisParams)
        {
            if (string.IsNullOrEmpty(reportPath))
            {
                reportPath = "ProjectAuditorReport.projectauditor";
            }

            Debug.Log($"[Project Auditor] Running audit → {reportPath}");

            string directory = Path.GetDirectoryName(reportPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var auditor = new ProjectAuditor();
            var report = auditor.Audit(analysisParams);
            report.Save(reportPath);

            Debug.Log("[Project Auditor] Audit complete.");

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }
        
        /// <summary>
        /// CLI entry point for Project Auditor.
        /// Parses command-line arguments and calls AuditAndExport.
        /// </summary>
        public static void RunFromCommandLine()
        {
            // Default values
            string reportPath = "ProjectAuditorReport.projectauditor";
            IssueCategory[] categories = Enum.GetValues(typeof(IssueCategory)).Cast<IssueCategory>().ToArray();

            // Parse CLI arguments
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-reportPath" && i + 1 < args.Length)
                    reportPath = args[i + 1];
                else if (args[i] == "-categories" && i + 1 < args.Length)
                    categories = args[i + 1]
                        .Split(',')
                        .Select(s => (IssueCategory)Enum.Parse(typeof(IssueCategory), s.Trim()))
                        .ToArray();
            }

            // Build AnalysisParams
            var analysisParams = new AnalysisParams
            {
                Categories = categories,
                Platform = EditorUserBuildSettings.activeBuildTarget
            };

            // Run the audit and export the report
            AuditAndExport(reportPath, analysisParams);
        }
    }
}
#endif