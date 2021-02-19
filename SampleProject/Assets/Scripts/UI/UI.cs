using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    /// <summary>
    /// Handling the Interaction with the User Interface
    /// </summary>
    public class UI : MonoBehaviour
    {
        #region actions

        public Action<CellContentType?> contentSelected;

        #endregion
    
        #region fields

        [SerializeField] private Button street;
        [SerializeField] private Button residence1;
        [SerializeField] private Button residence2;
        [SerializeField] private Button special1;
        [SerializeField] private Button special2;
        [SerializeField] private Color outlineColor;

        private List<Button> _buttons;
        private Button _currentButton;

        #endregion

        #region private methods

        /// <summary>
        /// Adding buttons to a list for easier excess afterwards
        /// </summary>
        private void Awake()
        {
            _buttons = new List<Button>
            {
                street,
                residence1,
                residence2,
                special1,
                special2
            };
        }

        /// <summary>
        /// Adding listeners to button actions to make them do something on Click
        /// </summary>
        private void Start()
        {
            street.onClick.AddListener(() =>
            {
                ResetButtonColor();
                SetSelectedButton(street, CellContentType.Street);
            });
            residence1.onClick.AddListener(() =>
            {
                ResetButtonColor();
                SetSelectedButton(residence1, CellContentType.Residence1);
            });
            residence2.onClick.AddListener(() =>
            {
                ResetButtonColor();
                SetSelectedButton(residence2, CellContentType.Residence2);
            });
            special1.onClick.AddListener(() =>
            {
                ResetButtonColor();
                SetSelectedButton(special1, CellContentType.Special1);
            });
        }

        /// <summary>
        /// Set the outline of the buttons to false to get rid of the highlight
        /// </summary>
        private void ResetButtonColor()
        {
            foreach (var button in _buttons)
            {
                button.GetComponent<Outline>().enabled = false;
            }
        }
    
        /// <summary>
        ///  Check if the selected button is the button clicked, if so, set the selected button to null and invoke the selectedContent with null. This is in order to be able to unselect any building type and not build anything on click.
        /// If the selected button isn't the one clicked, modify the buttons outline, invoke the the selectedContent with the appropriate type and set this button to the selectedButton
        /// </summary>
        /// <param name="button">The button that was just clicked</param>
        /// <param name="cellContentType">Appropriate type for this button</param>
        private void SetSelectedButton(Button button, CellContentType cellContentType)
        {
            if (button == _currentButton)
            {
                contentSelected?.Invoke(null);
                _currentButton = null;
            }
            else
            {
                ModifyButtonOutline(button);
                contentSelected?.Invoke(cellContentType);
                _currentButton = button;
            }
        }

        /// <summary>
        /// Enable the outline of the given button and set its color to the previously selected to highlight the button in the ui
        /// </summary>
        /// <param name="button">The just clicked button</param>
        private void ModifyButtonOutline(Button button)
        {
            var outline = button.GetComponent<Outline>();
            outline.effectColor = outlineColor;
            outline.enabled = true;
        }

        #endregion
    }
}
