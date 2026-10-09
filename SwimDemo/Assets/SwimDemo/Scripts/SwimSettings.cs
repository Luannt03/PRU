using UnityEngine;
namespace SwimDemo
{
    [CreateAssetMenu(menuName = "SwimDemo/Settings", fileName = "SwimSettings")]
    public class SwimSettings : ScriptableObject
    {
        public GameObject CharacterModel;
        public Avatar CharacterAvatar;
        public AnimationClip SwimmingClip;
        [Range(.1f, 4f)] public float ModelScale = 1f;
        public Vector3 ModelEulerCorrection;
        [Range(.5f, 4f)] public float MoveSpeed = 1.8f;
        [Range(.2f, 2f)] public float VerticalSpeed = .8f;
        [Range(.4f, 2f)] public float AnimationSpeed = 1f;
        public bool IsReady => CharacterModel != null && CharacterAvatar != null && CharacterAvatar.isValid && CharacterAvatar.isHuman && SwimmingClip != null && SwimmingClip.isHumanMotion;
    }
}
