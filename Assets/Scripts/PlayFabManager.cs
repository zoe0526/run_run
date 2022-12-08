using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;
using UnityEditor.PackageManager;
using run_run;
using System;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;
    public string message_string = string.Empty;
    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private string my_game_title_ID = "9C071";

    public void Register(string email, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail =false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request,OnRegisterSuccess,OnError);
    }
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("register is successfull!");

    }
    public void EmailLogin(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email=email,
            Password=password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    
    public void CustomIDLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }
    void OnLoginSuccess(LoginResult login_result)
    {
        Debug.Log(string.Format("ID {0} create/login is successful!!", login_result.PlayFabId));
        StatManager.Instance.set_character_ID(login_result.PlayFabId);

        message_string = "Login Success!!";
        FindObjectOfType<LoginSceneManager>().set_message_txt(message_string);

        GetCoin();
        //GetTitleData();
        SceneController.Instance.load_scene(scene_name.LOADINGScene);

    }
    public void ResetPassword(string email)
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = email,
            TitleId = my_game_title_ID
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);

    }
    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        message_string = "Password reset mail is sent to your email address";
        FindObjectOfType<LoginSceneManager>().set_message_txt(message_string);
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("PlayFabError !!");
        if (error.ErrorDetails != null)
        {
            foreach (var item in error.ErrorDetails)
            {
                message_string = item.Value[0];
                break;
            }
        }
        else
            message_string = error.ErrorMessage;

        FindObjectOfType<LoginSceneManager>().set_message_txt(message_string);
        Debug.Log(error.GenerateErrorReport());

    }
    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="Score",
                    Value=score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
    }

    void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("score sent successful");
    }
    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request,OnLeaderBoardGet,OnError);
    }
    public void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach(var player in result.Leaderboard)
        {
            Debug.Log(string.Format("Player ID : {0} , Rank : {1} , Score : {2}", player.Position, player.PlayFabId, player.StatValue));
        }
    }
    public void SaveCoin(long value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Coin", value.ToString()}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public  void GetCoin()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }
    void OnDataRecieved(GetUserDataResult result)
    {
        if(result.Data!=null && result.Data.ContainsKey("Coin"))
        {
            StatManager.Instance.set_character_coin(result.Data["Coin"].Value);
            Debug.Log("user data recieved successfull");
        }
        else
        {
            Debug.Log("user data recieve error");
        }

        SceneController.Instance.load_scene(scene_name.MAIN);
    }
    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("User Data send successful");
    }
    void GetTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnTitleDataRecieved, OnError);
    }
    void OnTitleDataRecieved(GetTitleDataResult result)
    {
        if(result.Data==null || result.Data.ContainsKey("Message")==false)
        {
            Debug.Log("No data !");
            return;
        }
        message_string = result.Data["Message"];
        FindObjectOfType<LoginSceneManager>().set_message_txt(message_string);

    }
}
