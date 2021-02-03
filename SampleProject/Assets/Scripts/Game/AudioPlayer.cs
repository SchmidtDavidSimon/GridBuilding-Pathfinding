using UnityEngine;

namespace Game
{
    /// <summary>
    /// Created by: Sunny Valley Studio 
    ///	https://svstudio.itch.io
    /// Altered by David Schmidt
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip placementSound;
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayPlacementSound()
        {
            if(placementSound != null)
            {
                _audioSource.PlayOneShot(placementSound);
            }
        }
    }
}