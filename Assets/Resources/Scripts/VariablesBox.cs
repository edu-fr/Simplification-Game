using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class VariablesBox : FillableBox
    {
        [SerializeField] private GameObject variableBoxes;
        public List<char> variableList;
        public List<Transform> variableBoxList;
        public Transform variableBoxPrefab;
        private float _variableBoxHeight;
        
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            _variableBoxHeight = variableBoxPrefab.GetComponent<RectTransform>().sizeDelta.y * Utils.ScreenDif;
        }

        protected override void SetGrayScale(bool option)
        {
            base.SetGrayScale(option);
            

        }
        
        public void FillWithVariables(IEnumerable<char> variables)
        {
            var variableBoxesRectTransform = variableBoxes.GetComponent<RectTransform>();
            var variableCounter = 0;
            foreach (var variable in variables)
            {
                // Increase the scroll rect size
                variableBoxesRectTransform.sizeDelta += new Vector2(0, _variableBoxHeight / Utils.ScreenDif);
                // Instantiate a new production box
                var variablesBoxTransform = variableBoxes.transform;
                var variablesBoxPosition = variablesBoxTransform.position;
                var newVariableBox = Instantiate(variableBoxPrefab,
                    new Vector3(variablesBoxPosition.x, variablesBoxPosition.y, variablesBoxPosition.z),
                    Quaternion.identity, variableBoxes.transform);
                var newVariableBoxDraggable = newVariableBox.GetComponent<Draggable>();
                newVariableBoxDraggable.CanBeDragged = true;
                newVariableBoxDraggable.AttachedTo = variableBoxes; // create a reference to this object
                newVariableBox.GetComponent<BoxContent>().SetVariable(variable);
                var newVariableTransform = newVariableBox.transform;
                var newVariablePosition = newVariableTransform.position;
                newVariablePosition -= new Vector3(0, _variableBoxHeight * variableCounter, 0);
                newVariableTransform.position = newVariablePosition;
                // Set new original position
                newVariableBox.GetComponent<Draggable>().OriginalPosition = newVariableTransform.localPosition;
                // Change it's text
                newVariableBox.GetComponentInChildren<TextMeshProUGUI>().SetText(variable.ToString());
                AddToLists(newVariableBox.gameObject);
                variableCounter++;
            }
            SetGrayScale(false);
        }
        
        public override void RemoveFromLists(GameObject box)
        {
            variableBoxList.Remove(box.transform);
            variableList.Remove(box.GetComponent<BoxContent>().Variable);
        }

        public override void AddToLists(GameObject box)
        {
            variableBoxList.Add(box.transform);
            variableList.Add(box.GetComponent<BoxContent>().Variable);
        }
        
        public override void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            if (eventData.pointerDrag.CompareTag("Variable"))
            {
                print("On valid position!");
                AddToLists(eventData.pointerDrag);
                eventData.pointerDrag.transform.SetParent(variableBoxes.transform);
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                    GetComponent<RectTransform>().anchoredPosition;
                eventData.pointerDrag.GetComponent<Draggable>().IsOnValidPositionToDrop = true;
            }
          
            
        }
    }
}
