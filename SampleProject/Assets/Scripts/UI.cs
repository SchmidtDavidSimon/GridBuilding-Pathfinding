using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to handle UI interactions
/// </summary>
public class UI : MonoBehaviour
{
    public Action<CellContentType?> contentSelected;

    [SerializeField] private Button street;
    [SerializeField] private Button residence1;
    [SerializeField] private Button residence2;
    [SerializeField] private Button special1;
    [SerializeField] private Button special2;
    [SerializeField] private Color outlineColor;

    private List<Button> _buttons;
    private Button _currentButton;

    /// <summary>
    /// Adding buttons to list
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
    /// Adding listeners to buttons
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
   /// Set the outline of the buttons to false
   /// </summary>
    private void ResetButtonColor()
    {
        foreach (var button in _buttons)
        {
            button.GetComponent<Outline>().enabled = false;
        }
    }
    
   /// <summary>
   ///  Set the selected button either to the pressed one or to null
   /// </summary>
   /// <param name="button">The button that was just pressed</param>
   /// <param name="cellContentType">The content type that is selected to be build</param>
   private void SetSelectedButton(Button button, CellContentType cellContentType)
    {
        if (button == _currentButton)
        {
            contentSelected?.Invoke(null);
            _currentButton = null;
        }
        else
        {
            ModifyButtonColor(button);
            contentSelected?.Invoke(cellContentType);
            _currentButton = button;
        }
    }

   /// <summary>
   /// Set the outline of the selected button to the chosen outlineColor
   /// </summary>
   /// <param name="button">The just selected button</param>
    private void ModifyButtonColor(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }
}
