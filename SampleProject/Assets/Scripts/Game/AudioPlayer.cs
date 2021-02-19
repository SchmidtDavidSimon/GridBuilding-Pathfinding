using UnityEngine;

namespace Game
{
    /// <summary>
    /// Handles the correct playing of sounds and sound-effects
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        #region fields

        [SerializeField] private AudioClip placementSound;
        
        private AudioSource _audioSource;

        #endregion

        #region private methods

        /// <summary>
        /// Set up needed components
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        #endregion

        /// <summary>
        /// Play sound
        /// TODO: Going forward i'll be adding new soundtrack, will ad a parameter to select soundtrack to play then
        /// </summary>
        public void PlayPlacementSound()
        {
            if(placementSound != null)
            {
                _audioSource.PlayOneShot(placementSound);
            }
        }
    }
}