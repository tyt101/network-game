using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Login : MonoBehaviour
{
    public static Socket clientSocket;
    void Start()
    {
        clientSocket = NetWorks.instance.getClientSocket();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
