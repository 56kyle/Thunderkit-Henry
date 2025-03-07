﻿using System;
using System.Collections.Generic;
using System.IO;
using Path = System.IO.Path;
using RoR2.ContentManagement;
using System.Collections;
using System.Reflection;
using UnityEngine;
using System.Linq;
using R2API;
using RoR2;
using EntityStates;
using StubbedConverter;

namespace ModdedSurvivorCamel.Modules
{
    internal static class Assets
    {
		//The file name of your asset bundle
		internal const string assetBundleName = "myassetbundle";

		//Should be the same name as your SerializableContentPack in the asset bundle
		internal const string contentPackName = "ModdedSurvivorCamelContentPack";

		//Name of the your soundbank file, if any.
		internal const string soundBankName = ""; //ModdedSurvivorCamelBank

		internal static AssetBundle mainAssetBundle = null;
		internal static ContentPack mainContentPack = null;
		internal static SerializableContentPack serialContentPack = null;

		internal static List<EffectDef> effectDefs = new List<EffectDef>();

        internal static void Init()
        {
            if (assetBundleName == "thunderassetbundle")
            {
                Debug.LogError(ModdedSurvivorCamelPlugin.MODNAME + ": AssetBundle name hasn't been changed. Not loading any assets to avoid conflicts.");
				ModdedSurvivorCamelPlugin.cancel = true;
                return;
            }
			if (Modules.Config.characterEnabled.Value == false)
			{
				Debug.LogFormat(ModdedSurvivorCamelPlugin.MODNAME + ": Character enabled config value is false. Not loading mod.");
				ModdedSurvivorCamelPlugin.cancel = true;
				return;
			}

			LoadAssetBundle();
            LoadSoundBank();
            PopulateAssets();
        }

		// Any extra asset stuff not handled or loaded by the Asset Bundle should be sorted here.
		// This is also a good place to set up any references, if you need to.
		// References within SkillState scripts can be done through EntityStateConfigs instead.
		internal static void PopulateAssets()
		{
			if (!mainAssetBundle)
			{
				Debug.LogError(ModdedSurvivorCamelPlugin.MODNAME + ": AssetBundle not found. Unable to Populate Assets.");
				ModdedSurvivorCamelPlugin.cancel = true;
				return;
			}
		}

		// Loads the AssetBundle, which includes the Content Pack.
		internal static void LoadAssetBundle()
        {
            if (mainAssetBundle == null)
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(path, assetBundleName));

				if (!mainAssetBundle) 
				{
					Debug.LogError(ModdedSurvivorCamelPlugin.MODNAME + ": AssetBundle not found. File missing or assetBundleName is incorrect.");
					ModdedSurvivorCamelPlugin.cancel = true;
					return;
				}
				LoadContentPack();
			}
        }

		// Sorts out ContentPack related shenanigans.
		// Sets up variables for reference throughout the mod and initializes a new content pack based on the SerializableContentPack.
		internal static void LoadContentPack()
        {
			serialContentPack = mainAssetBundle.LoadAsset<SerializableContentPack>(contentPackName);
			mainContentPack = serialContentPack.CreateContentPack();
			AddEntityStateTypes();
			CreateEffectDefs();
			ContentPackProvider.contentPack = mainContentPack;
		}


		// Loads the sound bank for any custom sounds. 
        internal static void LoadSoundBank()
        {
			if (soundBankName == "mysoundbank")
			{
				Debug.LogError(ModdedSurvivorCamelPlugin.MODNAME + ": SoundBank name hasn't been changed - not loading SoundBank to avoid conflicts.");
				return;
			}

			if (soundBankName == "")
            {
				Debug.LogFormat(ModdedSurvivorCamelPlugin.MODNAME + ": SoundBank name is blank. Skipping loading SoundBank.");
				return;

			}

			using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream(ModdedSurvivorCamelPlugin.MODNAME + "." + soundBankName +".bnk"))
			{
				byte[] array = new byte[manifestResourceStream2.Length];
				manifestResourceStream2.Read(array, 0, array.Length);
				SoundAPI.SoundBanks.Add(array);
			}
		}

		// Gathers all GameObjects with VFXAttributes attached and creates an EffectDef for each one.
		// Without this, the Effect is unable to be spawned.
		// Any VFX elements must have a NetWorkIdentity, VFXAttributes and EffectComponent on the base in order to be usable.
		internal static void CreateEffectDefs()
        {
			List<GameObject> effects = new List<GameObject>();

			GameObject[] assets = mainAssetBundle.LoadAllAssets<GameObject>();
			foreach (GameObject g in assets)
            {
				if (g.GetComponent<EffectComponent>())
                {
					effects.Add(g);
                }
            }
			foreach (GameObject g in effects)
            {
				EffectDef def = new EffectDef();
				def.prefab = g;

				effectDefs.Add(def);
            }

			mainContentPack.effectDefs.Add(effectDefs.ToArray());
        }



		// Finds all Entity State Types within the mod and adds them to the content pack.
		// Saves fuss of having to add them manually. Credit to KingEnderBrine for this code.
		internal static void AddEntityStateTypes()
        {
			mainContentPack.entityStateTypes.Add(((IEnumerable<System.Type>)Assembly.GetExecutingAssembly().GetTypes()).Where<System.Type>
				((Func<System.Type, bool>)(type => typeof(EntityState).IsAssignableFrom(type))).ToArray<System.Type>());

			if (ModdedSurvivorCamelPlugin.debug)
			foreach (Type t in mainContentPack.entityStateTypes)
            {
					Debug.LogWarning(t);
			}
		}
    }

	public class ContentPackProvider : IContentPackProvider
	{
		public static SerializableContentPack serializedContentPack;
		public static ContentPack contentPack;

		public static string contentPackName = null;
		public string identifier
		{
			get
			{
				return ModdedSurvivorCamelPlugin.MODNAME;
			}
		}

		internal static void Initialize()
		{
			contentPackName = Assets.contentPackName;
			//contentPack = serializedContentPack.CreateContentPack();
			ContentManager.collectContentPackProviders += AddCustomContent;
		}

		private static void AddCustomContent(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
		{
			addContentPackProvider(new ContentPackProvider());
		}

		public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
		{
			ContentPack.Copy(contentPack, args.output);
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}
	}
}
