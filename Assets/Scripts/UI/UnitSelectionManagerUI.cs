using System;
using MonoBehaviours;
using UnityEngine;

namespace UI
{
    public class UnitSelectionManagerUI : MonoBehaviour
    { 
        [SerializeField] private RectTransform _selectionAreaRectTransform;
        [SerializeField] private Canvas _mainCanvas;
        private bool _isSelecting;

        private void Start()
        {
            UnitSelectionManager.Instance.OnSelectionAreaStart += SelectionStarted;
            UnitSelectionManager.Instance.OnSelectionAreaEnd += SelectionEnded;
            _selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
             if (_isSelecting)
                UpdateVisual();
        }

        private void UpdateVisual()
        {
            
            var selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
            float canvasScale = _mainCanvas.transform.localScale.x;
            
            _selectionAreaRectTransform.anchoredPosition = selectionAreaRect.position/canvasScale;
            _selectionAreaRectTransform.sizeDelta = selectionAreaRect.size/canvasScale;
            
        }
        private void SelectionStarted(object sender, EventArgs e)
        {
            _isSelecting = true;
            _selectionAreaRectTransform.gameObject.SetActive(_isSelecting);
        }
        private void SelectionEnded(object sender, EventArgs e)
        {
            
            _isSelecting = false;
            _selectionAreaRectTransform.gameObject.SetActive(_isSelecting);
        }

    }
}