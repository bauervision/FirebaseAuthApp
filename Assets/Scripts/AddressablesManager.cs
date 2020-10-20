using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : MonoBehaviour
{
    public AssetReference AudioRef;

    public AudioSource audioSource;

    private void Start()
    {
        HandleAddressablesAudio();
    }

    public void HandleAddressablesAudio()
    {
        AudioRef.LoadAssetAsync<AudioClip>().Completed += OnAudioLoaded;
    }

    void OnAudioLoaded(AsyncOperationHandle<AudioClip> handle)
    {
        audioSource.clip = handle.Result;
        audioSource.Play();
    }
}
