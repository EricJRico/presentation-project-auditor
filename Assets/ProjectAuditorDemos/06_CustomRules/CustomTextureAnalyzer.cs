// using System;
// using System.Collections.Generic;
// using Unity.ProjectAuditor.Editor;
// using Unity.ProjectAuditor.Editor.Core;
// using UnityEditor;
//
// namespace BoatAttack.ProjectAuditorAnalyzer
// {
//     class CustomTextureAnalyzer : TextureModuleAnalyzer
//     {
//         // Max allowed size for any Texture2D
//         const int k_MaxTextureSize = 2048;
//
//         // Descriptor declaration
//         const string k_TextureTooBigId = "EJR9001";
//         const string k_Title = "(Custom) Texture: Oversized Texture";
//
//         const string k_Description =
//             "This texture exceeds the allowed maximum size and may cause excessive memory usage.";
//
//         const string k_Recommendation =
//             "Resize the texture or lower the Max Size in the Import Settings.";
//
//         static readonly Descriptor k_TextureTooBigDescriptor = new Descriptor
//         (
//             k_TextureTooBigId,
//             k_Title,
//             Areas.Memory | Areas.Quality,
//             k_Description,
//             k_Recommendation
//         )
//         {
//             MessageFormat = "Texture '{0}' is oversized",
//
//             Fixer = (issue, analysisParams) =>
//             {
//                 var importer =
//                     AssetImporter.GetAtPath(issue.RelativePath) as TextureImporter;
//
//                 if (importer != null)
//                 {
//                     importer.maxTextureSize = k_MaxTextureSize;
//                     importer.SaveAndReimport();
//                 }
//             }
//         };
//
//         public override void Initialize(Action<Descriptor> registerDescriptor)
//         {
//             registerDescriptor(k_TextureTooBigDescriptor);
//         }
//
//         public override IEnumerable<ReportItem> Analyze(TextureAnalysisContext context)
//         {
//             // Only check non-sprite textures (Default = normal Texture2D)
//             if (context.Importer.textureType == TextureImporterType.Default)
//             {
//                 if (context.Texture.width > k_MaxTextureSize ||
//                     context.Texture.height > k_MaxTextureSize)
//                 {
//                     yield return context.CreateIssue(
//                         IssueCategory.AssetIssue,
//                         k_TextureTooBigDescriptor.Id,
//                         context.Name
//                     ).WithLocation(context.Importer.assetPath);
//                 }
//             }
//         }
//     }
// }