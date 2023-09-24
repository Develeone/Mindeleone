using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldValidator : MonoBehaviour
{
    InputField inputField;

    void OnEnable()
    {
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(Validate);
    }

    public void Validate(string newValue = null) {
        inputField.text = CleanInput(newValue == null ? inputField.text : newValue);
    }

    static string CleanInput(string strIn)
    {
        return Regex.Replace(strIn, @"[^a-zA-Zа-яА-Я0-9`!@#$%^&*()_+ \-=\\{}\[\]:""'<>?,./]", "");
    }

    void OnDisable()
    {
        inputField.onValueChanged.RemoveAllListeners();
    }
}
