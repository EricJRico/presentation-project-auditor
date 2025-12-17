using System.Collections.Generic;
using UnityEditor;

namespace ProjectAuditorDemos.Editor.CodeAnalysis
{
    /// <summary>
    /// Registry for FindApiRule ScriptableObjects.
    /// Handles lazy rebuild and method → rule lookup.
    /// </summary>
    static class FindApiRuleRegistry
    {
        static Dictionary<string, List<FindApiRule>> s_Rules;
        static bool s_Dirty = true;

        /// <summary>
        /// Marks the registry as dirty. Next GetRules will rebuild it.
        /// </summary>
        public static void MarkDirty() => s_Dirty = true;

        /// <summary>
        /// Returns the list of FindApiRule assets matching a method name.
        /// </summary>
        public static IReadOnlyList<FindApiRule> GetRules(string methodName)
        {
            if (s_Dirty)
                Rebuild();

            return s_Rules.TryGetValue(methodName, out var rules) ? rules : null;
        }

#if UNITY_EDITOR
        static void Rebuild()
        {
            s_Rules = new Dictionary<string, List<FindApiRule>>();

            // Find all FindApiRule assets in the project
            string[] guids = AssetDatabase.FindAssets("t:FindApiRule");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var rule = AssetDatabase.LoadAssetAtPath<FindApiRule>(path);
                if (rule == null) continue;

                if (!s_Rules.TryGetValue(rule.MethodName, out var list))
                {
                    list = new List<FindApiRule>();
                    s_Rules[rule.MethodName] = list;
                }

                list.Add(rule);
            }

            s_Dirty = false;
        }
#endif
    }
}