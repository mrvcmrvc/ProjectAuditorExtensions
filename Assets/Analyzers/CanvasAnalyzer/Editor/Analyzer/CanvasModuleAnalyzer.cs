using System;
using System.Collections.Generic;
using System.IO;
using Unity.ProjectAuditor.Editor.Core;
using UnityEditor;
using UnityEngine;

namespace Unity.ProjectAuditor.Editor.Modules
{
    public class CanvasModuleAnalyzer : AssetsModuleAnalyzer
    {
        private const string PrefabExtension = ".prefab";
        private ICanvasIssue[] issues;
        
        public override void Initialize(Action<Descriptor> registerDescriptor)
        {
            Initialize();

            foreach (ICanvasIssue issue in issues)
            {
                registerDescriptor(issue.Descriptor);
            }
        }

        private void Initialize()
        {
            List<ICanvasIssue> issueInstances = new();
            foreach (Type type in TypeCache.GetTypesDerivedFrom(typeof(ICanvasIssue)))
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                
                ICanvasIssue issueInstance = (ICanvasIssue)Activator.CreateInstance(type);
                issueInstances.Add(issueInstance);
            }
            issues = issueInstances.ToArray();
        }

        public override IEnumerable<ReportItem> Analyze(AssetAnalysisContext context)
        {
            string extension = Path.GetExtension(context.AssetPath);
            if (!extension.Equals(PrefabExtension))
            {
                yield break;
            }
            
            GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(context.AssetPath);
            if (!loadedPrefab.TryGetComponent(typeof(Canvas), out Component canvas))
            {
                yield break;
            }

            foreach (ICanvasIssue issue in issues)
            {
                ReportItem issueReport = issue.Analyze(canvas as Canvas, context);
                if (issueReport == null)
                {
                    continue;
                }

                yield return issueReport;
            }
        }
    }
}