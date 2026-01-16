using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text display;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button button;

    private CharacterLogic _character;
    
    public Action<int> OnSkillCardSelected;
    
    public void SetSkillCard(CharacterLogic character)
    {
        _character = character;
        display.text = character.Name;
        costText.text = character.ExSkillCost.ToString();
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        button.interactable = true;
        
        gameObject.SetActive(true);
    }

    private void OnButtonClick()
    {
        int index = transform.GetSiblingIndex();
        OnSkillCardSelected?.Invoke(index);
    }

    public void DisableSkillCard()
    {
        gameObject.SetActive(false);
    }
}
