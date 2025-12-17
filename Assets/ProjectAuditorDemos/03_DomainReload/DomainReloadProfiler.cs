#if UNITY_EDITOR
using UnityEditor;
using System.Diagnostics;

namespace ProjectAuditorDemos._03_DomainReload
{
    /// <summary>
    /// Measures the duration of Unity script domain reloads.
    /// Intended for correlating Project Auditor findings
    /// with actual editor reload cost.
    /// </summary>
    [InitializeOnLoad]
    public static class DomainReloadProfiler
    {
        private const string ReloadStartKey = "ProjectAuditorDemos.DomainReload.StartTicks";

        static DomainReloadProfiler()
        {
            // Defensive unsubscription to avoid duplicate registrations
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private static void OnBeforeAssemblyReload()
        {
            long startTicks = Stopwatch.GetTimestamp();
            EditorPrefs.SetString(ReloadStartKey, startTicks.ToString());
        }

        private static void OnAfterAssemblyReload()
        {
            if (!EditorPrefs.HasKey(ReloadStartKey))
            {
                UnityEngine.Debug.LogWarning(
                    "[Domain Reload] Start time missing. Reload may have been interrupted."
                );
                return;
            }

            long startTicks = long.Parse(EditorPrefs.GetString(ReloadStartKey));
            long endTicks = Stopwatch.GetTimestamp();

            long elapsedTicks = endTicks - startTicks;
            double elapsedSeconds = (double)elapsedTicks / Stopwatch.Frequency;

            UnityEngine.Debug.Log(
                $"[Domain Reload] Script domain reload took {elapsedSeconds:F2} seconds"
            );

            EditorPrefs.DeleteKey(ReloadStartKey);
        }
    }
}
#endif