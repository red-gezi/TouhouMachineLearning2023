using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TouhouMachineLearningSummary.Manager
{
    public partial class KeyWordManager : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        public int width;
        static bool IsCheckHover = false;
        static KeyWordManager manager;
        void Awake() => manager = this;
        public GameObject TagPopup;
        public TextMeshProUGUI TagPopupText => TagPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textMesh => GetComponent<TextMeshProUGUI>();
        static List<KeyWordModel> words = new List<KeyWordModel>();
        public static string ReplaceAbilityKeyWord(string text)
        {
            words = TranslateManager.CheckKeyWord(text);
            words.Select(x => x.keyWord).Distinct().ToList().ForEach(keyWord =>
            {
                text = text.Replace(keyWord, $"<u>{keyWord}</u>");
            });

            return text;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            manager.TagPopup.SetActive(false);
            IsCheckHover = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsCheckHover = true;
            _ = CheakHover();

            async Task CheakHover()
            {
                while (IsCheckHover)
                {
                    Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
                    int linkIndex = TMP_TextUtilities.FindNearestCharacter(textMesh, pos, null, true);
                    if (linkIndex > -1)
                    {
                        TMP_CharacterInfo info = textMesh.textInfo.characterInfo[linkIndex];
                        var targetKeyWord = words.FirstOrDefault(word => word.startIndex <= linkIndex && word.endIndex > linkIndex);
                        if (targetKeyWord != null)
                        {
                            //Debug.Log("µã»÷ÁË" + targetKeyWord.keyWord);
                            manager.TagPopup.SetActive(true);
                            manager.TagPopupText.text = targetKeyWord.introduction;
                            manager.TagPopup.transform.position = Camera.main.ViewportToScreenPoint(Camera.main.ScreenToViewportPoint(Input.mousePosition) + new Vector3(0.1f, 0.05f));
                            manager.TagPopup.GetComponent<RectTransform>().sizeDelta = new Vector2(manager.TagPopupText.text.Count() * width, manager.TagPopup.GetComponent<RectTransform>().sizeDelta.y);
                        }
                        else
                        {
                            manager.TagPopup.SetActive(false);
                        }
                    }
                    else
                    {
                        manager.TagPopup.SetActive(false);
                    }
                    await Task.Delay(100);
                }
            }
        }
    }
}

