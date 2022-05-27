using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class GameNetWork : MonoBehaviour
{
    GameObject netObj;
    GameObject hero1;
    GameObject hero2;
    string woAmI;
    ulong add1PerFrame = 0;
    byte[] readBuff = new byte[1024];
    byte[] sendBuff = new byte[1024];
    Vector3 newPostion;
    Socket tempSocket;
    void Start()
    {
        netObj = GameObject.FindGameObjectWithTag("netSocket");
        tempSocket = netObj.GetComponent<NetWorks>().getClientSocket();
        setPosition(short.Parse(netObj.GetComponent<NetWorks>().whoAmI));
        //tempSocket.BeginReceive(readBuff, 0, 1024, 0, receiveCb, tempSocket);
    }

    void setPosition(short woAmI)
    {
        //初始化玩家位置
        hero1 = GameObject.Find("hero");
        hero2 = GameObject.Find("hero2");
        float x1, x2, y1, y2;
        y1 = y2 = hero1.transform.position.y;
        if (woAmI == 0)
        {
            x1 = 17;
            x2 = -17;
        }
        else
        {
            x1 = -17;
            x2 = 17;
        }
        Vector2 position1 = new Vector2(x1, y1);
        Vector2 position2 = new Vector2(x2, y2);
        hero1.transform.position = position1;
        hero2.transform.position = position2;
    }
    void FixedUpdate()
    {

        add1PerFrame++;
        if (add1PerFrame % 5 == 0)
        {
            string sendStr = "Position ";
            if (hero1 == null)
            {
                hero1 = GameObject.Find("hero1");
            }
            if (hero1 != null)
            {
                sendStr += hero1.transform.position.x.ToString() + ' '; //!!!!!!!!!
                sendStr += hero1.transform.position.y.ToString() + ' ';
                //血量同步
                float h = hero1.GetComponent<PlayerHealth>().health;
                sendStr += h.ToString() + ' ';
                sendBuff = System.Text.Encoding.UTF8.GetBytes(sendStr); //!!!!!!!!!!!!!
                tempSocket.Send(sendBuff);
            }
        }
    }
    //void receiveCb(IAsyncResult ar)
    //{
    //    int num = tempSocket.EndReceive(ar);
    //    string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, num);
    //    BaseSocket.msgList.Add(recvStr);
    //    tempSocket.BeginReceive(readBuff, 0, 1024, 0, receiveCb, tempSocket);

    //}
}
