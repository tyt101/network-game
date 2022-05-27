using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomsNetWork : MonoBehaviour
{
    GameObject netObject;
    public Socket tempSocket;
    byte[] readBuff = new byte[1024];
    string localAddressStr = "";
    NetWorks netWorks;
    void Start()
    {
        netObject = GameObject.FindGameObjectWithTag("netSocket");
        netWorks = netObject.GetComponent<NetWorks>();
        tempSocket = netWorks.getClientSocket();
        //tempSocket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCb, tempSocket);

        EndPoint localEndPoint = tempSocket.LocalEndPoint;
        localAddressStr = ((IPEndPoint)localEndPoint).Address.ToString() + ":" +((IPEndPoint)localEndPoint).Port.ToString();
    }
    //public void ReceiveCb(IAsyncResult ar)
    //{
    //    int num = tempSocket.EndReceive(ar);
    //    string receiveStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, num);
    //    BaseSocket.msgList.Add(receiveStr);
    //    tempSocket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCb, tempSocket);
    //}

    private void FixedUpdate()
    {
   
    }
    int onePerFrame = 0;
    void Update()
    {
        onePerFrame++;
        if (onePerFrame % 120 == 0)
        {
            //tempSocket.Send(System.Text.Encoding.Default.GetBytes("heartBeat "));
            onePerFrame = 0;
            refreshRooms(netWorks.recvStr);
        }
    }
    GameObject[] roomAdressTexts;
    GameObject[] enterRoomBtns;
    string[] strRooms;
    public void refreshRooms(string recvStr)
    {
        if (recvStr == null)
        {
            return;
        }
        //房间存在，则销毁
        if (roomAdressTexts != null) for (int i = 0; i < roomAdressTexts.Length; i++)
                Destroy(roomAdressTexts[i]);
        if (enterRoomBtns != null) for (int i = 0; i < enterRoomBtns.Length; i++)
                Destroy(enterRoomBtns[i]);
        //接收服务器发来的房间列表
        strRooms = recvStr.Split(' ');
        int roomCount = int.Parse(strRooms[1]);
        roomAdressTexts = new GameObject[roomCount];
        enterRoomBtns = new GameObject[roomCount];
        for (int i = 0; i < roomCount; i++)
        {
            roomAdressTexts[i] = GameObject.Instantiate(Resources.Load("Text",typeof(GameObject))) as GameObject;
            roomAdressTexts[i].transform.SetParent(GameObject.Find("Canvas/RoomPanel/content").transform, false);
            roomAdressTexts[i].GetComponent<Text>().text = strRooms[i + 2];
            enterRoomBtns[i] = GameObject.Instantiate(Resources.Load("Button",typeof(GameObject))) as GameObject;
            enterRoomBtns[i].transform.SetParent(GameObject.Find("Canvas/RoomPanel/content").transform, false);

            if (localAddressStr == strRooms[i + 2])
            {
                //不能点击自己的进入房间
                enterRoomBtns[i].gameObject.transform.Find("Text").GetComponent<Text>().text = "自己";
                enterRoomBtns[i].GetComponent<Button>().enabled = false;
            }
            else
            {
                //可以点击别人的进入房间
                enterRoomBtns[i].gameObject.transform.Find("Text").GetComponent<Text>().text = "enter";
                enterRoomBtns[i].GetComponent<Button>().enabled = true;
            }
            enterRoomBtns[i].GetComponent<EnterRoomBtnNum>().setNum(i);
            GameObject tempObj = enterRoomBtns[i];
            enterRoomBtns[i].GetComponent<Button>().onClick.AddListener(
            //给enter点击设置事件监听，点击了即调用onclick传入tempObj
            delegate ()
            {
                this.onClick(tempObj);
            }
            );
        }
    }
    public void onClick(GameObject sender)
    {
        //发送进入房间信息给服务器
        int btnIndex = sender.GetComponent<EnterRoomBtnNum>().getNum();
        string sendStr = "enterRoom " + strRooms[btnIndex + 2];
        tempSocket.Send(System.Text.Encoding.Default.GetBytes(sendStr));
        sender.GetComponent<Button>().enabled = false;
    }
    public void BtnCreateRoomClicked()
    {
        //点击创建房间，创建后(创建按钮)失效
        tempSocket.Send(System.Text.Encoding.Default.GetBytes("createRoom "));
        GameObject.Find("Canvas/btnCreate").GetComponent<Button>().enabled = false;
    }
}
