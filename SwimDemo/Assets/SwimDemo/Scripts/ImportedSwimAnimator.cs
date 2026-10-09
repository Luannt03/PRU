using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
namespace SwimDemo
{
    public class ImportedSwimAnimator : MonoBehaviour
    {
        PlayableGraph graph;
        AnimationClipPlayable playable;
        AnimationClip clip;
        Transform hips, anchor;
        double clock;
        public float PlaybackSpeed = 1f;
        public void Configure(Animator animator, AnimationClip swimming, Transform visualAnchor)
        {
            if (animator == null || animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman || swimming == null || !swimming.isHumanMotion)
                throw new System.InvalidOperationException("SwimDemo: FBX cần Avatar Humanoid hợp lệ và clip Humanoid.");
            clip = swimming; anchor = visualAnchor;
            animator.applyRootMotion = false; animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.runtimeAnimatorController = null; animator.Rebind();
            hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            if (hips == null) throw new System.InvalidOperationException("SwimDemo: Avatar thiếu xương Hips.");
            graph = PlayableGraph.Create("MixamoSwimming"); graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            playable = AnimationClipPlayable.Create(graph, swimming);
            playable.SetApplyFootIK(false); playable.SetApplyPlayableIK(false); playable.SetSpeed(0);
            var output = AnimationPlayableOutput.Create(graph, "YBotAnimator", animator); output.SetSourcePlayable(playable);
            graph.Play(); ResetPose();
        }
        void Sample()
        {
            playable.SetTime(clock % System.Math.Max(clip.length, .001f)); graph.Evaluate(0);
            // The uploaded clip travels forward. Anchor the hips after sampling so keyboard movement
            // controls position, and the loop never teleports the whole swimmer back to its start.
            transform.position += anchor.position - hips.position;
        }
        public void ResetPose() { clock = 0; if (graph.IsValid()) Sample(); }
        void LateUpdate()
        {
            if (!graph.IsValid() || Time.timeScale <= 0 || Time.deltaTime <= 0) return;
            clock += Time.deltaTime * Mathf.Clamp(PlaybackSpeed, .1f, 3f); Sample();
        }
        void OnDestroy() { if (graph.IsValid()) graph.Destroy(); }
    }
}
