using System;
using MonoBehaviours;
using UnityEngine;

namespace UI
{
    public class UnitSelectionManagerUI : MonoBehaviour
    { 
        [SerializeField] private RectTransform _selectionAreaRectTransform;

        private void Start()
        {
            UnitSelectionManager.Instance.OnSelectionAreaStart += SelectionStarted;
            UnitSelectionManager.Instance.OnSelectionAreaEnd += SelectionEnded;
            _selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void SelectionStarted(object sender, EventArgs e)
        {
            _selectionAreaRectTransform.gameObject.SetActive(true);
        }
        private void SelectionEnded(object sender, EventArgs e)
        {
            _selectionAreaRectTransform.gameObject.SetActive(false);
        }

    }
}