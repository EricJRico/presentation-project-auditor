using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.ProjectAuditor.Editor;
using Unity.ProjectAuditor.Editor.CodeAnalysis;
using Unity.ProjectAuditor.Editor.Core;
using UnityEngine;

namespace ProjectAuditorDemos.Editor.CodeAnalysis
{
    /// <summary>
    /// Analyzer for detecting usage of Unity Object search APIs
    /// like GameObject.FindWithTag, GameObject.FindGameObjectsWithTag, Object.FindObjectsByType, etc.
    /// </summary>
    sealed class UnityObjectSearchCallAnalyzer : CodeModuleInstructionAnalyzer
    {
        static readonly OpCode[] k_OpCodes =
        {
            OpCodes.Call,
            OpCodes.Callvirt
        };

        Dictionary<string, Descriptor> m_Descriptors;

        public override IReadOnlyCollection<OpCode> opCodes => k_OpCodes;

        /// <summary>
        /// Initializes the analyzer by creating descriptors from FindApiRule assets.
        /// </summary>
        public override void Initialize(Action<Descriptor> registerDescriptor)
        {
            m_Descriptors = new Dictionary<string, Descriptor>();

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:FindApiRule");
            foreach (var guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var rule = UnityEditor.AssetDatabase.LoadAssetAtPath<FindApiRule>(path);
                if (rule == null || string.IsNullOrEmpty(rule.DescriptorId))
                    continue;

                if (m_Descriptors.ContainsKey(rule.DescriptorId))
                {
                    Debug.LogWarning(
                        $"Duplicate DescriptorId '{rule.DescriptorId}' detected in UnityObjectSearchCallAnalyzer");
                    continue;
                }
                
                // Create Descriptor dynamically
                var descriptor = new Descriptor(
                    rule.DescriptorId,
                    "CUSTOM_" + rule.DeclaringType + "." + rule.MethodName,
                    rule.PerformanceAreas,
                    $"<b>{rule.DeclaringType}.{rule.MethodName}</b> allocates managed memory and can be slow",
                    $"Try to avoid calling this <b>{rule.MethodName}</b> in frequently-updated code. Ideally, this method should only be used during initialisation, and the results should be cached if they need to be re-used."
                )
                {
                    Type = rule.DeclaringType,
                    Method = rule.MethodName
                };

                // Register with Project Auditor
                registerDescriptor(descriptor);

                // Cache locally
                m_Descriptors[descriptor.Id] = descriptor;
            }

            // Mark registry dirty in case rules change later
            FindApiRuleRegistry.MarkDirty();
#endif
        }

        /// <summary>
        /// Analyzes a single IL instruction for calls to Unity Object search APIs.
        /// </summary>
        public override ReportItemBuilder Analyze(InstructionAnalysisContext context)
        {
            if (!(context.Instruction.Operand is MethodReference callee))
                return null;

            var rules = FindApiRuleRegistry.GetRules(callee.Name);
            if (rules == null)
                return null;

            var declaringType = callee.DeclaringType;

            foreach (var rule in rules)
            {
                if (!MatchesDeclaringType(declaringType, rule))
                    continue;

                if (!m_Descriptors.TryGetValue(rule.DescriptorId, out var descriptor))
                    continue;

                if (!rule.ShouldCheckAPIRule)
                    continue;
                
                // Build full method name including generics
                string fullMethodName = $"{declaringType.FullName}.{callee.Name}";
                if (callee is GenericInstanceMethod genericInstanceMethod && genericInstanceMethod.HasGenericArguments)
                {
                    var genericTypeNames = genericInstanceMethod.GenericArguments.Select(a => a.FullName);
                    fullMethodName += $"<{string.Join(", ", genericTypeNames)}>";
                }

                return context.CreateIssue(IssueCategory.Code, descriptor.Id)
                    .WithDescription($"'{fullMethodName}' usage");
            }

            return null;
        }

        static bool MatchesDeclaringType(TypeReference actualType, FindApiRule rule)
        {
            if (actualType.FullName == rule.DeclaringType)
                return true;

            if (!rule.AllowInheritance)
                return false;

            return MonoCecilHelper.IsOrInheritedFrom(actualType, rule.DeclaringType);
        }
    }
}