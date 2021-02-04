using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Action<CellContentType?> contentSelected;
    // public Action<CellContentType> houseSelected;
    // public Action<CellContentType> specialSelected;

    [SerializeField] private Button street;
    [SerializeField] private Button residence1;
    [SerializeField] private Button residence2;
    [SerializeField] private Button special1;
    [SerializeField] private Button special2;
    [SerializeField] private Color outlineColor;

    private List<Button> _buttons;
    private Button _currentButton;

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

    private void ResetButtonColor()
    {
        foreach (var button in _buttons)
        {
            button.GetComponent<Outline>().enabled = false;
        }
    }
    
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

    private void ModifyButtonColor(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }
}
