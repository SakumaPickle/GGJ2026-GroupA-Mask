#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BatchAttachToPrefabs
{
	// ---- CONFIG ----
	// Set your target folder (Project folder path)
	private const string TargetFolder = "Assets/Prefabs/objects/terrain";

	// Set the component you want to add
	// Example: typeof(DespawnRootMarker)
	private static readonly Type ComponentTypeToAdd = typeof(DespawnEnemyMarker);

	// If true, will also search subfolders
	private const bool IncludeSubfolders = false;

	[MenuItem("Tools/Batch Attach/Add Component To Prefabs In Folder")]
	public static void AddComponentToPrefabsInFolder()
	{
		if (!AssetDatabase.IsValidFolder(TargetFolder))
		{
			Debug.LogError($"Folder not found: {TargetFolder}");
			return;
		}

		string[] searchInFolders = new[] { TargetFolder };

		// Find all prefabs under folder
		string[] guids = AssetDatabase.FindAssets("t:Prefab", searchInFolders);

		int changed = 0;
		int skipped = 0;

		try
		{
			AssetDatabase.StartAssetEditing();

			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);

				// If not including subfolders, skip those not directly under TargetFolder
				if (!IncludeSubfolders)
				{
					string dir = Path.GetDirectoryName(path)?.Replace("\\", "/");
					if (!string.Equals(dir, TargetFolder, StringComparison.Ordinal))
					{
						continue;
					}
				}

				// Load prefab contents (safe way to edit prefab asset)
				GameObject root = PrefabUtility.LoadPrefabContents(path);
				bool modified = false;

				try
				{
					if (root == null)
					{
						skipped++;
						continue;
					}

					// Add component only if missing
					if (root.GetComponent(ComponentTypeToAdd) == null)
					{
						Undo.RegisterFullObjectHierarchyUndo(root, "Batch Add Component");
						root.AddComponent(ComponentTypeToAdd);
						modified = true;
					}

					if (modified)
					{
						PrefabUtility.SaveAsPrefabAsset(root, path);
						changed++;
						Debug.Log($"Updated: {path}");
					}
					else
					{
						skipped++;
					}
				}
				finally
				{
					PrefabUtility.UnloadPrefabContents(root);
				}
			}
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		Debug.Log($"Done. Changed={changed}, Skipped={skipped}, Folder={TargetFolder}, Type={ComponentTypeToAdd.Name}");
	}
}
#endif
