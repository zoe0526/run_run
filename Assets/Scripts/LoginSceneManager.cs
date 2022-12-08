using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{

    [SerializeField]
    private Text _message_text;
    [SerializeField]
    private TMP_InputField _email_input;
    [SerializeField]
    private TMP_InputField _password_input;

    private PlayFabManager _playfab_manager;
    private void Awake()
    {
        GameObject obj = Instantiate( Resources.Load("ImmutableObject")) as GameObject;
        obj.name = "ImmutableObject";
        DontDestroyOnLoad(obj);
    }
    public void set_message_txt(string text)
    {
        _message_text.text = text;

    }

    public void on_click_register()
    {
        PlayFabManager.Instance.Register(_email_input.text,_password_input.text);
    }
    public void on_click_email_login()
    {
        PlayFabManager.Instance.EmailLogin(_email_input.text,_password_input.text);
    }
    public void on_click_enter_guest()
    {
        PlayFabManager.Instance.CustomIDLogin();
    }
    public void on_click_reset_password()
    {
        PlayFabManager.Instance.ResetPassword(_email_input.text);
    }
}
