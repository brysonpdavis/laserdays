using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class ControlManager : MonoBehaviour
{

    public static ControlManager Instance;

    private static ControllerState controllerState;

    private Player controllerPlayer;

/*
    public KeyCode jump { get; set; }
    public KeyCode forward { get; set; }
    public KeyCode backward { get; set; }
    public KeyCode left { get; set; }
    public KeyCode right { get; set; }
    public KeyCode pickup { get; set; }
    public KeyCode flip { get; set; }
    public KeyCode select { get; set; }
    public KeyCode pause { get; set; }
    public KeyCode submit { get; set; }
*/

    public enum ControllerState
    {
        KeyboardAndMouse,
        JoystickPS4
    }

    void Awake()
    {
        //Singleton pattern
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        
        controllerPlayer = ReInput.players.GetPlayer(0);
        
        
        OnStartup();
    }
    
    public bool GetButton(string action)
    {
        return controllerPlayer.GetButton(action);
    }
    
    public bool GetButtonDown(string action)
    {
        return controllerPlayer.GetButtonDown(action);
    }

    public float GetAxis(string axis)
    {
        return controllerPlayer.GetAxis(axis);
    }
    
    private void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        //Debug.LogError("Controller Connected: " + args.name);
        
        if (args.controllerType == ControllerType.Joystick)
        {
            controllerPlayer.controllers.AddController(ReInput.controllers.GetController(args.controllerType, args.controllerId), true);
            
            RemoveKeyboard();
        }

    }
    
    private void OnControllerDisconnect(ControllerStatusChangedEventArgs args)
    {
        //Debug.LogError("Controller Disconnected: " + args.name);

        if (args.controllerType == ControllerType.Joystick)
        {
            ReInput.controllers.RemoveControllerFromAllPlayers(ReInput.controllers.GetController(args.controllerType ,args.controllerId));
        }

        if (controllerPlayer.controllers.joystickCount == 0)
        {
            AddKeyboard();
        }
    }

    public static ControllerState GetControllerState()
    {
        return controllerState;
    }
    
    private void AddKeyboard()
    {
        controllerPlayer.controllers.hasKeyboard = true;
        controllerPlayer.controllers.hasMouse = true;

        controllerState = ControllerState.KeyboardAndMouse;
        
        //Debug.LogError("Adding keyboard, joysticks: " + ReInput.controllers.joystickCount);
    }

    private void RemoveKeyboard()
    {
        controllerPlayer.controllers.hasKeyboard = false;
        controllerPlayer.controllers.hasMouse = false;

        controllerState = ControllerState.JoystickPS4;
        
        //Debug.LogError("Removing keyboard, joysticks: " + ReInput.controllers.joystickCount);

    }

    private void OnStartup()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnect;
        
        if (controllerPlayer.controllers.joystickCount > 0)
        {
            RemoveKeyboard();

            foreach (Joystick stick in controllerPlayer.controllers.Joysticks)
            {
                controllerPlayer.controllers.AddController(stick,true);
            }
        }
        else
        {
            AddKeyboard();
        }
    }
}
