using Live2D.Cubism.Core;
using UnityEngine;
namespace TouhouMachineLearningSummary.Dialogue
{
    public class Live2dModelManager : MonoBehaviour
    {
        public bool FaceRank;
        public CubismModel _model;
        private CubismParameter eyeY;
        private CubismParameter eyeX;
        private CubismParameter cloth;
        private CubismParameter body;
        private CubismParameter hand;
        private CubismParameter head;
        public float a, b, c;
        public float num;
        [SerializeField]
        public string ParameterID = "ParamAngleX";

        public GameObject[] trigger;
        private void Start()
        {
            _model = this.FindCubismModel();
            eyeX = _model.Parameters.FindById("ParamEyeBallX");
            eyeY = _model.Parameters.FindById("ParamEyeBallY");
            cloth = _model.Parameters.FindById("ParamCloth");
            cloth = _model.Parameters.FindById("ParamCloth");
            //body = _model.Parameters.FindById("Param2");
            hand = _model.Parameters.FindById("Param");
            head = _model.Parameters.FindById("ParamAngleX");
            //_model.Moc.
            //_paramAngleZ.Value = 5;
        }
        public Vector3 vector3;
        float targetValue = 30;
        // Update is called once per frame
        void LateUpdate()
        {
            vector3 = Input.mousePosition;
            eyeX.Value = (vector3.x / 960) - 1;
            eyeY.Value = -((vector3.y / 540) - 1);
            //print(Mathf.Lerp(_paramAngleZ.Value, FaceRank == 0 ? 30 : -30, Time.deltaTime));
            //
            //num = Mathf.Lerp(num, FaceRank ? -10 : 10, Time.deltaTime * 2);
            //_paramAngleZ.Value = num;
            ModelTriggerArea modelTriggerArea = trigger[0].GetComponent<ModelTriggerArea>();
            if (modelTriggerArea.isClickDown)
            {
                head.Value += modelTriggerArea.mouseX * a;
            }
            else
            {
                head.Value = Mathf.Lerp(head.Value, 0, Time.deltaTime * 10);
            }
            modelTriggerArea = trigger[1].GetComponent<ModelTriggerArea>();
            if (modelTriggerArea.isClickDown)
            {
                hand.Value += modelTriggerArea.mouseX * b;
            }
            else
            {
                hand.Value = Mathf.Lerp(hand.Value, 0, Time.deltaTime * 10);
            }
            modelTriggerArea = trigger[2].GetComponent<ModelTriggerArea>();
            if (modelTriggerArea.isClickDown)
            {
                cloth.Value += modelTriggerArea.mouseY * c;
            }
            else
            {
                cloth.Value = Mathf.Lerp(cloth.Value, 0, Time.deltaTime * 10);
            }
            modelTriggerArea.isClickUp = false;
            //if (modelTriggerArea.isClickUp)
            //{
            //    modelTriggerArea.isClickUp = false;
            //    Task.Run(async () =>
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            targetValue = 0;
            //            await Task.Delay(300);
            //            targetValue = 30;
            //            await Task.Delay(300);
            //        }
            //    });
            //}
            //body.Value = Mathf.Lerp(body.Value, targetValue, Time.deltaTime*5);

            _model.ForceUpdateNow();

        }

    }

}
