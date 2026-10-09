using UnityEngine;
namespace ChemLab9.Core
{
    public class AudioFeedback : MonoBehaviour
    {
        public AudioClip Click, Correct, Incorrect;
        AudioSource source;
        public void Configure(AudioClip click, AudioClip correct, AudioClip incorrect)
        { Click=click; Correct=correct; Incorrect=incorrect; source=gameObject.AddComponent<AudioSource>(); source.playOnAwake=false; source.spatialBlend=0; }
        public void PlayClick() { if(Click!=null) source.PlayOneShot(Click,.4f); }
        public void PlayAnswer(bool correct) { AudioClip clip=correct?Correct:Incorrect; if(clip!=null) source.PlayOneShot(clip,.65f); }
    }
}
