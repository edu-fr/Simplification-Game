using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Resources.Scripts
{
    public abstract class FillableBox : MonoBehaviour, IDropHandler
    {
        protected RectTransform FillableBoxTransform;
        [SerializeField] protected GameObject titleBox;
        [SerializeField] protected float titleBoxOffset;
        [SerializeField] protected GameObject scrollBar;
        protected ColorBlock ScrollBarDefaultColorBlock;

        protected virtual void Awake()
        {
            FillableBoxTransform = GetComponent<RectTransform>();
        }
    
        protected virtual void Start()
        {
            ScrollBarDefaultColorBlock = scrollBar.GetComponent<Scrollbar>().colors;
        }
        
        public virtual void SetGrayScale(bool? option)
        {
            if (option == null) return;
            var currentEffectMode = (bool) option ? EffectMode.Grayscale : EffectMode.None;
            var currentColorBlock = (bool) option ? ColorBlock.defaultColorBlock : ScrollBarDefaultColorBlock;

            gameObject.GetComponent<UIEffect>().effectMode = currentEffectMode;
            titleBox.GetComponent<UIEffect>().effectMode = currentEffectMode;
            scrollBar.GetComponent<Scrollbar>().colors = currentColorBlock;
        }

        public abstract void ClearList();
        public abstract void RemoveFromLists(GameObject box);
        public abstract void AddToLists(GameObject box);
        public abstract void OnDrop(PointerEventData eventData);

        public abstract void RemoveAndReconstructList(GameObject boxToBeRemoved, bool? draggable, bool? deletable, bool? grayscale, bool destroy);

        public abstract void InsertAndReconstructList(GrammarScript.Production productionToBeInserted, bool? draggable, bool? deletable, bool? grayscale);
        public abstract void InsertAndReconstructList(char variableToBeInserted, bool? draggable, bool? deletable, bool? grayscale);
    }
}
