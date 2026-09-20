// Copyright (c) Meta Platforms, Inc. and affiliates.
// Original Source code from Oculus Starter Samples (https://github.com/oculus-samples/Unity-StarterSamples)

using System;
using System.Collections.Generic;
using System.IO;
using Meta.XR.Samples;
using PassthroughCameraSamples.MultiObjectDetection;
using Unity.InferenceEngine;
using UnityEngine;

namespace PassthroughCameraSamples.StartScene
{
    // Create menu of all scenes included in the build.
    [MetaCodeSample("PassthroughCameraApiSamples-StartScene")]
    public class StartMenu : MonoBehaviour
    {
        public OVROverlay Overlay;
        public OVROverlay Text;
        public OVRCameraRig VrRig;

        [SerializeField] private ModelAsset m_objectDetectionModel;

        private void Awake()
        {
            Debug.Log("=== [DEBUG] StartMenu: Awake called ===");
            
            if (m_objectDetectionModel == null)
            {
                Debug.LogError("=== [DEBUG] ERROR: m_objectDetectionModel is NULL! Check Inspector settings. ===");
            }
            else
            {
                Debug.Log($"=== [DEBUG] Preloading Model: {m_objectDetectionModel.name} ===");
            }

            // SentisInferenceRunManager.PreloadModel(m_objectDetectionModel);
        }

        private void Start()
        {
            Debug.Log("=== [DEBUG] StartMenu: Start called ===");

            var generalScenes = new List<Tuple<int, string>>();
            var passthroughScenes = new List<Tuple<int, string>>();
            var proControllerScenes = new List<Tuple<int, string>>();

            var n = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            Debug.Log($"=== [DEBUG] Total scenes in Build Settings: {n} ===");

            for (var sceneIndex = 1; sceneIndex < n; ++sceneIndex)
            {
                var path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(sceneIndex);
                Debug.Log($"=== [DEBUG] Checking Scene Index {sceneIndex}: {path} ===");

                if (path.Contains("Passthrough"))
                {
                    Debug.Log("   -> Categorized as: Passthrough Scene");
                    passthroughScenes.Add(new Tuple<int, string>(sceneIndex, path));
                }
                else if (path.Contains("TouchPro"))
                {
                    Debug.Log("   -> Categorized as: TouchPro Scene");
                    proControllerScenes.Add(new Tuple<int, string>(sceneIndex, path));
                }
                else
                {
                    Debug.Log("   -> Categorized as: General Scene");
                    generalScenes.Add(new Tuple<int, string>(sceneIndex, path));
                }
            }
            

            Debug.Log($"=== [DEBUG] Scene List Summary: Passthrough={passthroughScenes.Count}, ProController={proControllerScenes.Count}, General={generalScenes.Count} ===");

            var uiBuilder = DebugUIBuilder.Instance;

            if (uiBuilder == null)
            {
                Debug.LogError("=== [DEBUG] FATAL ERROR: DebugUIBuilder.Instance is NULL! Is the prefab missing from the scene? ===");
                return;
            }

            if (passthroughScenes.Count > 0)
            {
                Debug.Log("=== [DEBUG] Adding Passthrough UI Buttons... ===");
                _ = uiBuilder.AddLabel("Passthrough Scenes", DebugUIBuilder.DEBUG_PANE_LEFT);
                foreach (var scene in passthroughScenes)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.Item2);
                    Debug.Log($"   -> Button Added: {sceneName} (Index: {scene.Item1})");
                    _ = uiBuilder.AddButton(sceneName, () => LoadScene(scene.Item1), -1, DebugUIBuilder.DEBUG_PANE_LEFT);
                }
            }

            if (proControllerScenes.Count > 0)
            {
                Debug.Log("=== [DEBUG] Adding ProController UI Buttons... ===");
                _ = uiBuilder.AddLabel("Pro Controller Scenes", DebugUIBuilder.DEBUG_PANE_RIGHT);
                foreach (var scene in proControllerScenes)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.Item2);
                    Debug.Log($"   -> Button Added: {sceneName} (Index: {scene.Item1})");
                    _ = uiBuilder.AddButton(sceneName, () => LoadScene(scene.Item1), -1, DebugUIBuilder.DEBUG_PANE_RIGHT);
                }
            }

            _ = uiBuilder.AddLabel("Press ☰ at any time to return to scene selection", DebugUIBuilder.DEBUG_PANE_CENTER);

            if (generalScenes.Count > 0)
            {
                Debug.Log("=== [DEBUG] Adding General UI Buttons... ===");
                _ = uiBuilder.AddDivider(DebugUIBuilder.DEBUG_PANE_CENTER);
                _ = uiBuilder.AddLabel("The Scenes", DebugUIBuilder.DEBUG_PANE_CENTER);
                foreach (var scene in generalScenes)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.Item2);
                    Debug.Log($"   -> Button Added: {sceneName} (Index: {scene.Item1})");
                    _ = uiBuilder.AddButton(sceneName, () => LoadScene(scene.Item1), -1, DebugUIBuilder.DEBUG_PANE_CENTER);
                }
            }

            Debug.Log("=== [DEBUG] Calling uiBuilder.Show() ===");
            uiBuilder.Show();
        }

        private void LoadScene(int idx)
        {
            Debug.Log($"=== [DEBUG] LoadScene Triggered! Loading Index: {idx} ===");
            
            if (DebugUIBuilder.Instance != null)
            {
                DebugUIBuilder.Instance.Hide();
            }
            else
            {
                Debug.LogWarning("=== [DEBUG] Warning: DebugUIBuilder.Instance is null during LoadScene. ===");
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(idx);
        }
    }
}