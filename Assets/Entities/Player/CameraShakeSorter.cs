using System;
using System.ComponentModel.Design;
using NUnit.Framework.Internal;
using Unity.Cinemachine;
using Unity.Cinemachine.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraShakeSorter : MonoBehaviour
{
    //public CinemachineImpulseChannels impulseChannel;
    public CinemachineImpulseSource impulseSource;
    //private PlayerInput[] activePlayers = new PlayerInput[4]; 
    public PlayerInput playerInput;
    public CinemachineCamera playerCam;
    public CinemachineImpulseListener impulseListener;
    private int playerIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        playerCam.GetComponent<CinemachineImpulseListener>();
        //playerInput = GetComponent<PlayerInput>();
        playerIndex = playerInput.user.index;
        //impulseChannel.ImpulseChannels[0];
        CinemachineImpulseDefinition TEST = impulseSource.ImpulseDefinition;
        TEST.ImpulseChannel = Mathf.FloorToInt( Mathf.Pow(2,playerIndex));
        impulseSource.ImpulseDefinition = TEST;
        
       impulseListener.ChannelMask = TEST.ImpulseChannel;

        
        
    }

    // Update is called once per frame
    /*void Update()
    {
        if (playerInput.playerIndex == 0);
        {
            //impulseChannel.ImpulseChannels[1];
        }
    }*/
}
