using System;
using System.Collections.Generic;
using Grid;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Action<CellContentType> contentSelected;
    // public Action<CellContentType> houseSelected;
    // public Action<CellContentType> specialSelected;

    [SerializeField] private Button streetButton;
    [SerializeField] private Button houseButton;
    [SerializeField] private Button specialButton;
    [SerializeField] private Color outlineColor;

    private List<Button> _buttons;

    private void Awake()
    {
        _buttons = new List<Button>
        {
            streetButton,
            houseButton,
            specialButton
        };
    }

    private void Start()
    {
        streetButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyButtonColor(streetButton);
            contentSelected?.Invoke(CellContentType.Street);
        });
        // houseButton.onClick.AddListener(() =>
        // {
        //     ResetButtonColor();
        //     ModifyButtonColor(houseButton);
        //     houseSelected?.Invoke(CellContentType.House);
        // });
        // specialButton.onClick.AddListener(() =>
        // {
        //     ResetButtonColor();
        //     ModifyButtonColor(specialButton);
        //     specialSelected?.Invoke(CellContentType.Special);
        // });
    }

    private void ModifyButtonColor(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    private void ResetButtonColor()
    {
        foreach (var button in _buttons)
        {
            button.GetComponent<Outline>().enabled = false;
        }
    }
}
