using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class InitializeTags {
	/// <summary>
	/// The required tags for the NearbyDroid sample.
	/// </summary>
	private static string[] requiredTags =
	{
		"Projectile"
	};

	/// <summary>
	/// Initializes static members of the <see cref="InitializeTags"/> class.
	/// The static constructor is called by the Unity editor. because of the
	/// initializeOnLoad directive.
	/// </summary>
	static InitializeTags()
	{
		Debug.Log("Checking for custom tags and layers");

		// Open tag manager
		SerializedObject tagManager = 
			new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(
				"ProjectSettings/TagManager.asset")[0]);

		CheckTags(tagManager);

		// save our work!
		tagManager.ApplyModifiedProperties();
	}

	/// <summary>
	/// Checks the tags to make sure they are defined.
	/// </summary>
	/// <param name="tagManager">Tag manager.</param>
	private static void CheckTags(SerializedObject tagManager)
	{
		SerializedProperty tagsProp = tagManager.FindProperty("tags");

		for (int index = 0; index < requiredTags.Length; index++)
		{
			string tag = requiredTags[index];
			bool found = false;
			for (int i = 0; i < tagsProp.arraySize; i++)
			{
				SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
				if (t.stringValue == tag)
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				tagsProp.InsertArrayElementAtIndex(0);
				SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
				n.stringValue = tag;
				Debug.Log("Adding tag: " + tag);
			}
		}
	}

}
