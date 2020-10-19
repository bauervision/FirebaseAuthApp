using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : MonoBehaviour
{
    public AssetReference SceneRef;
    public AssetReference AudioRef;

    public AssetReferenceGameObject PrefabRef;
    public AudioSource CameraAudioSource;

    public void HandleAddressablesPrefab()
    {
        Addressables.InstantiateAsync(PrefabRef);
    }

    public void HandleAddressablesScene()
    {
        SceneRef.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void HandleAddressablesAudio()
    {
        AudioRef.LoadAssetAsync<AudioClip>().Completed += OnAudioLoaded;
    }

    void OnAudioLoaded(AsyncOperationHandle<AudioClip> handle)
    {
        CameraAudioSource.clip = handle.Result;
        CameraAudioSource.Play();
    }
}
