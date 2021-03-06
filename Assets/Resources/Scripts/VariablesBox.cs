using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Resources.Scripts.Utils;

namespace Resources.Scripts
{
    public class VariablesBox : FillableBox
    {
        [SerializeField] private GameObject variableBoxes;
        public List<char> variableList;
        public List<Transform> variableBoxList;
        public Transform variableBoxPrefab;
        private float _variableBoxHeight;
        private Vector2 _variableBoxesOriginalSize;
        private char _lastVariableInserted;
        
        protected override void Awake()
        {
            base.Awake();
            _variableBoxHeight = variableBoxPrefab.GetComponent<RectTransform>().sizeDelta.y * Utils.ScreenDif;
            _variableBoxesOriginalSize = variableBoxes.GetComponent<RectTransform>().sizeDelta;
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void SetGrayScale(bool? option)
        {
            if (option == null) return;
            base.SetGrayScale(option);
            var currentEffectMode = (bool) option ? EffectMode.Grayscale : EffectMode.None;
            var currentColorFactor = (bool) option ? 0 : 1;

            foreach (var variableBox in variableBoxList)
            {
                variableBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
                variableBox.GetComponent<UIEffect>().colorFactor = currentColorFactor;
            }

        }

        public void FillWithVariables(IEnumerable<char> variables)
        {
            if (variableBoxList.Count > 0 || variableList.Count > 0)
            {
                ClearList();
            }
            var variableBoxesRectTransform = variableBoxes.GetComponent<RectTransform>();
            var variableCounter = 0;
            var variablesList = variables.ToList();
            variablesList.Sort(ExtensionMethods.SortVariables);
            foreach (var variable in variablesList)
            {
                // Expanding boxes container
                variableBoxesRectTransform.sizeDelta += new Vector2(0, _variableBoxHeight / Utils.ScreenDif);
                // Instantiate a new production box
                var variablesBoxTransform = variableBoxes.transform;
                var variablesBoxPosition = variablesBoxTransform.position;
                var newVariableBox = Instantiate(variableBoxPrefab,
                    variablesBoxPosition, Quaternion.identity, variableBoxes.transform);
                newVariableBox.GetComponent<Draggable>().CanBeDragged = false;
                newVariableBox.GetComponent<Draggable>().AttachedTo = variableBoxes;
                newVariableBox.GetComponent<BoxContent>().SetVariable(variable);
                var newVariableTransform = newVariableBox.transform;
                var newVariablePosition = variablesBoxPosition;
                newVariablePosition -= new Vector3(0, _variableBoxHeight * variableCounter, 0);
                newVariableTransform.position = newVariablePosition;
                // Change it's text
                newVariableBox.GetComponentInChildren<TextMeshProUGUI>().SetText(variable.ToString());
                AddToLists(newVariableBox.gameObject);
                _lastVariableInserted = variable;
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
            if (eventData.pointerDrag.CompareTag("Variable") && eventData.pointerDrag.GetComponent<Draggable>().CanBeDragged)
            {
                eventData.pointerDrag.GetComponent<Draggable>().IsOnValidPositionToDrop = true;
                InsertAndReconstructList(eventData.pointerDrag.GetComponent<BoxContent>().Variable, draggable: true, deletable: false, grayscale: false);
                Destroy(eventData.pointerDrag);
            }
        }
        
        public override void ClearList()
        {
            while (variableBoxList.Count > 0)
            {
                var currentProductionBox = variableBoxList[0];
                RemoveFromLists(currentProductionBox.gameObject);
                Destroy(currentProductionBox.gameObject);
            }
            variableBoxes.GetComponent<RectTransform>().sizeDelta = _variableBoxesOriginalSize;
        }

        public override void InsertAndReconstructList(GrammarScript.Production productionToBeInserted, bool? draggable, bool? deletable, bool? grayscale)
        {
            throw new NotImplementedException();
        }

        public override void InsertAndReconstructList(char variableToBeInserted, bool? draggable, bool? deletable, bool? grayscale)
        {
            var variableListCopy = variableList.DeepClone();
            variableListCopy.Add(variableToBeInserted);
            ClearList();
            variableListCopy.Sort(ExtensionMethods.SortVariables);
            FillWithVariables(variableListCopy);
            SetAllVariablesDeletability(deletable);
            SetAllVariablesDraggability(draggable);
            SetGrayScale(grayscale);
        }
        
        public override void RemoveAndReconstructList(GameObject variableBoxToBeRemoved, bool? draggable, bool? deletable, bool? grayscale, bool destroy)
        {
            RemoveFromLists(variableBoxToBeRemoved);
            if(destroy)
                Destroy(variableBoxToBeRemoved.gameObject);
            var variableListCopy = variableList.DeepClone();
            ClearList();
            variableListCopy.Sort(ExtensionMethods.SortVariables);
            FillWithVariables(variableListCopy);
            SetAllVariablesDeletability(deletable);
            SetAllVariablesDraggability(draggable);
        }
        
        public void SetAllVariablesDeletability(bool? boolean)
        {
            if (boolean == null) return; 
            foreach (var variableBox in variableBoxList)
            {
                variableBox.GetComponent<Draggable>().CanBeDeleted = (bool) boolean;
            }
        }
        
        public void SetAllVariablesDraggability(bool? boolean)
        {
            if (boolean == null) return; 
            foreach (var variableBox in variableBoxList)
            {
                variableBox.GetComponent<Draggable>().CanBeDragged = (bool)  boolean;
            }
        }

    }
}
