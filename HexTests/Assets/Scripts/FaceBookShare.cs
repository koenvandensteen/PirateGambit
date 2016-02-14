//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

public class FaceBookShare : MonoBehaviour
{
    public static FaceBookShare FBShare;

    private List<string> _permList = new List<string>() { "public_profile", "email", "user_friends", "publish_actions" };
    private bool _shareLink;
    private bool _loggedIn = false;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

        FBShare = GetComponent<FaceBookShare>();
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    void OnGUI()
    {
        if (_shareLink)
        {
            if (!FB.IsLoggedIn && !_loggedIn)
            {
                FB.LogInWithPublishPermissions(_permList,loginResult);
                _loggedIn = true;
            }
           
            if(FB.IsLoggedIn)
            {
                _shareLink = false;
                FB.ShareLink(new Uri("http://web.howest.be/dae.2015.team3/"), callback: ShareCallback);
            }
        }
    }

    private void loginResult(ILoginResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("login Error: " + result.Error);
        }
        else if(FB.IsLoggedIn)
        {
            // Share succeeded without postID
            Debug.Log("login success!");
        }
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }

    public void ShareFBLink()
    {
        _shareLink = true;
    }



}
