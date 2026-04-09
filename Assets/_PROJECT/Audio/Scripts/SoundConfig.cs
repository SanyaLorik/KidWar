using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "SoundConfig", menuName = "Configs/SoundConfig")]
public class SoundConfig : ScriptableObject {
    [field: SerializeField] public SoundType SoundType { get; private set; }
    [field: SerializeField] public AudioClip[] AudioClips { get; private set; }
    [field: SerializeField, Range(0,1)] public float Volume { get; private set; }
    [field: SerializeField] public PairedValue<float> PitchDiapasone { get; private set; }
    [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
    [field: SerializeField] public bool Loop { get; private set; }
    [field: SerializeField, Range(0f,1f)] public float SpatialBlend { get; private set; }

}
