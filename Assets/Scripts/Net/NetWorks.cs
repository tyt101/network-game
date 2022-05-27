using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Security.Cryptography;
using System;
using UnityEngine.SceneManagement;

public class NetWorks : MonoBehaviour
{
    string userName;
    string password;
    string passwordMD5;
    public static Socket clientSocket;
    public bool connected = false;
    int addPerFrame = 0;
    static byte[] readBuff = new byte[1024];
    static byte[] sendBuff = new byte[1024];
    public List<string> msgList = new List<string>();
    public string recvStr;
    public string whoAmI;

    public static NetWorks instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
        BtnConnectClicked();
        DontDestroyOnLoad(this);
    }
    public void BtnConnectClicked()
    {
        clientSocket.Connect("127.0.0.1", 10001);
        clientSocket.BeginReceive(readBuff, 0, 1024, 0, receiveCb, clientSocket);
        connected = true;
        Debug.Log("连接成功!");
    }
    public void RegistBtnClicked()
    {
        userName = GameObject.Find("Canvas/RegistPanel/usrPasbox/userInputField/Text").GetComponent<Text>().text;
        password = GameObject.Find("Canvas/RegistPanel/usrPasbox/passInputField/Text").GetComponent<Text>().text;
        var spaceIndex = userName.IndexOf(" ");
        if (spaceIndex != -1)
        {
            GameObject.Find("Canvas/RegistPanel/usrPasbox/userInputField").GetComponent<InputField>().text = "用户名不能有空格";
            return;
        }
        //密码加密
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] passWordMD5Byte = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(password)); //MD5 散列
        passwordMD5 = BitConverter.ToString(passWordMD5Byte).Replace("-", ""); //转换为字符串
        sendBuff = System.Text.Encoding.UTF8.GetBytes("Regist " + userName + " " + passwordMD5);
        clientSocket.Send(sendBuff);
    }
    
    public void LoginBtnClicked()
    {
        userName = GameObject.Find("Canvas/LoginPanel/usrPasbox/userInputField/Text").GetComponent<Text>().text;
        password = GameObject.Find("Canvas/LoginPanel/usrPasbox/passInputField/Text").GetComponent<Text>().text;
        var spaceIndex = userName.IndexOf(" ");
        if (spaceIndex != -1)
        {
            GameObject.Find("Canvas/LoginPanel/usrPasbox/userInputField").GetComponent<InputField>().text = "用户名不能有空格";
            return;
        }
        //密码加密
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] passWordMD5Byte = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(password)); //MD5 散列
        passwordMD5 = BitConverter.ToString(passWordMD5Byte).Replace("-", ""); //转换为字符串
        sendBuff = System.Text.Encoding.UTF8.GetBytes("Login " + userName + " " + passwordMD5);
        clientSocket.Send(sendBuff);
    }

    public void RegistBtnReturn()
    {
        SceneManager.LoadScene("Login");
    }
    public void RegistBtnTo()
    {
        SceneManager.LoadScene("Regist");
    }
    public void receiveCb(IAsyncResult ar)
    {
        int num = clientSocket.EndReceive(ar);//EndReceive 方法将读取所有可用的数据，直到达到 BeginReceive 方法的 size 参数所指定的字节数为止。
        string recvStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, num);
        msgList.Add(recvStr);
        clientSocket.BeginReceive(readBuff, 0, 1024, 0, receiveCb, clientSocket);
    }
    private void FixedUpdate()
    {
        addPerFrame++;
        if (addPerFrame % 50 == 0)
        {
            if (connected)
            {
                sendBuff = System.Text.Encoding.Default.GetBytes("heartBeat");
                Debug.LogError("心跳");
                clientSocket.Send(sendBuff);
            }
        }
        if (msgList.Count > 0)
        {
            Debug.Log("这里进来了吗?");
            string handleString = msgList[0];
            msgList.Remove(handleString);
            string[] recvStrs = handleString.Split(' ');
            if ("RegistFail" == recvStrs[0])
            {
                Debug.Log("用户名已存在");
                GameObject.Find("Canvas/RegistPanel/usrPasbox/userInputField").GetComponent<InputField>().text = "用户名已存在";
            }
            if ("RegistSuccess" == recvStrs[0])
            {
                Debug.Log("注册成功");
                SceneManager.LoadScene("Room");
            }
            if (recvStrs[0] == "roomList")
            {
                recvStr = handleString;
            }
            if ("beginGame" == recvStrs[0])
            {
                whoAmI = recvStrs[1];
                SceneManager.LoadScene("Level");
            }
            
            if (recvStrs[0] == "Position")
            {
                //位置同步
                float x = float.Parse(recvStrs[1]);
                float y = float.Parse(recvStrs[2]);
                Vector2 newPostion = new Vector2(x, y);
                GameObject.Find("hero2").transform.position = newPostion;
                float h = float.Parse(recvStrs[3]);
                //血量同步
                GameObject.Find("hero2").GetComponent<PlayerHealth2>().health = h;
                GameObject.Find("hero2").GetComponent<PlayerHealth2>().UpdateHealthBar();
                PlayerControl2 pl2 = GameObject.Find("hero2").GetComponent<PlayerControl2>();
                if (h <= 0 && !pl2.isDie)
                {
                    byte[] sendBuff = new byte[1024];
                    sendBuff = System.Text.Encoding.Default.GetBytes("Die ");
                    pl2.anim.SetTrigger("Die");
                    pl2.isDie = true;
                }
                else if(h > 0)
                {
                    pl2.isDie = false;
                }
            }
            //转身同步
            if (recvStrs[0] == "Flip")
            {
                GameObject.Find("hero2").GetComponent<PlayerControl2>().Flip();
            }
            //开火同步
            if (recvStrs[0] == "Fire")
            {
                GameObject.Find("hero2/Gun").GetComponent<Gun2>().Fire();
            }
            if(recvStrs[0] == "Die")
            {
                GameObject.Find("hero2").GetComponent<PlayerControl2>().QuitGame();
                GameObject.Find("hero").GetComponent<PlayerControl>().QuitGame();
                Debug.Log("结束游戏");
            }
            if(recvStrs[0] == "LoginSuccess")
            {
                    Debug.Log("登录成功");
                    SceneManager.LoadScene("Room");
                
            }
            if (recvStrs[0] == "LoginFail")
            {
                if (recvStrs[1] == "3")
                {
                    GameObject.Find("Canvas/LoginPanel/usrPasbox/userInputField").GetComponent<InputField>().text = "该用户已登录";
                    Debug.Log("用户名已登录");
                }else if (recvStrs[1] == "2")
                {
                    GameObject.Find("Canvas/LoginPanel/usrPasbox/passInputField").GetComponent<InputField>().text = "密码错误";
                    Debug.Log("密码错误");
                }
                else if(recvStrs[1] == "1")
                {
                    GameObject.Find("Canvas/LoginPanel/usrPasbox/userInputField").GetComponent<InputField>().text = "用户名不存在";
                }
            }
        }
    }
    public Socket getClientSocket()
    {
        return clientSocket;
    }
    private void OnDestroy()
    {
        connected = false;
        clientSocket.Close();
    }
}
