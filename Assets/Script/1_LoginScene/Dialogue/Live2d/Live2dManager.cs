using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    public class Live2dManager : MonoBehaviour
    {
        CubismModel model;
        //public AnimatorController animator;
        public Animator animator;
        public List<AnimationClip> faces;
        public List<AnimationClip> action;

        public Vector3 defaultSightPoint;
        public Vector3 biasSightPoint;
        CubismParameter ParamAngleX;
        CubismParameter ParamAngleY;
        // Start is called before the first frame update
        void Awake()
        {
            model = this.FindCubismModel();
            ParamAngleX = model.Parameters.FindById("ParamAngleX");
            ParamAngleY = model.Parameters.FindById("ParamAngleY");
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Input.GetMouseButton(0))
            {

                Vector3 vector3 = Camera.main.WorldToViewportPoint(transform.position);
                var targetSightPoint = (Camera.main.ScreenToViewportPoint(Input.mousePosition) - vector3) * 60;
                biasSightPoint = Vector3.Lerp(biasSightPoint, targetSightPoint, Time.deltaTime * 5);
            }
            else
            {
                biasSightPoint = Vector3.Lerp(biasSightPoint, defaultSightPoint, Time.deltaTime * 5);
            }
            ParamAngleX.Value = biasSightPoint.x;
            ParamAngleY.Value = biasSightPoint.y;
            model.ForceUpdateNow();
        }
        #if UNITY_EDITOR
        [Button("初始化动画片段")]
        public void Init()
        {
            animator = GetComponent<Animator>();
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            animatorController.parameters.ToList().ForEach(parameter => animatorController.RemoveParameter(parameter));
            faces.ForEach(clip => animatorController.AddParameter(clip.name, AnimatorControllerParameterType.Trigger));
            action.ForEach(clip => animatorController.AddParameter(clip.name, AnimatorControllerParameterType.Trigger));
            AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
            stateMachine.states.ToList().ForEach(state => stateMachine.RemoveState(state.state));
            stateMachine.states = new ChildAnimatorState[0];
            AnimatorState state = stateMachine.AddState("默认");
            state.motion = faces[0];
            stateMachine.AddEntryTransition(state);
            int position = 0;
            //导入表情动画，并为循环播放
            faces.ForEach(clip =>
            {
                position++;
                AnimatorState state = stateMachine.AddState(clip.name, new Vector3(50, 50 * position));
                state.motion = clip;
                AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
                transition.AddCondition(AnimatorConditionMode.If, 1, clip.name);
            });
            position = 0;
            //导入动作动画，只播放一次
            action.ForEach(clip =>
            {
                AnimatorState state = stateMachine.AddState(clip.name, new Vector3(100, 50 * position));
                state.motion = clip;
                AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
                transition.AddCondition(AnimatorConditionMode.If, 1, clip.name);
                stateMachine.AddEntryTransition(state);
            });
            AssetDatabase.SaveAssets();
        }
        #endif
        [Button("播放")]
        public void Play(string tag) => GetComponent<Animator>().SetTrigger(tag);
        public void ToActive(float process)
        {
            float value = process * 0.8f + 0.2f;
            model.Drawables.ToList().ForEach(drawable => drawable.GetComponent<CubismRenderer>().Color = new Color(value, value, value));
        }
    }
}