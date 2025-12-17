using Unity.ProjectAuditor.Editor;
using UnityEngine;

namespace ProjectAuditorDemos.Editor.CodeAnalysis
{
    [CreateAssetMenu(
        fileName = "FindApiRule",
        menuName = "Project Auditor/Code Analysis/Find API Rule")]
    public sealed class FindApiRule : ScriptableObject
    {
        [Tooltip("Check for API Rule")]
        public bool ShouldCheckAPIRule = true;
        
        [Tooltip("Descriptor ID to raise when matched, this is required to be unique")]
        public string DescriptorId;
        
        [Tooltip("Fully qualified declaring type name, e.g. UnityEngine.GameObject")]
        public string DeclaringType;

        [Tooltip("Method name, e.g. FindWithTag")]
        public string MethodName;

        [Tooltip("Performance areas that are affected by this rule")]
        public Areas PerformanceAreas = Areas.CPU | Areas.Memory;
        
        [Tooltip("Optional base type match (inheritance check)")]
        public bool AllowInheritance = true;

        void OnValidate()
        {
            FindApiRuleRegistry.MarkDirty();
        }
    }
}