using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Chat;
using Photon.Realtime;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using ExitGames.Client.Photon;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif


namespace Photon.Chat.Lobby
{
    public class ChatService : MonoBehaviour, IChatClientListener
    {
        bool chatStart = false;

        public ChatChannel channel;
        public Text CurrentChannelText;
        public GameObject buttonInput; 

        ChatClient chatClient;
        protected ChatAppSettings chatAppSettings;
        public InputField InputFieldChat;
        public InputField NameBlock;

        public int TestLength = 2048;
        private byte[] testBytes = new byte[2048];

        public bool lobbyStart = false;
        
        
        public void Connect()
        {
            Application.runInBackground = true;
            chatClient = new ChatClient(this);

            chatClient.UseBackgroundWorkerForSending = true;
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(PhotonNetwork.NickName));
    
        }
        public void OnEnterSend()
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                SendChatMessage(InputFieldChat.text);
                InputFieldChat.text = "";
            }
        }
        public void OnClickSend()
        {
            if (InputFieldChat != null)
            {
                SendChatMessage(this.InputFieldChat.text);
                InputFieldChat.text = "";
            }
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            // update text
            this.ShowChannel();
        }

        private void SendChatMessage(string inputLine)
        {
            if (string.IsNullOrEmpty(inputLine))
            {
                return;
            }

            if (this.TestLength != this.testBytes.Length)
            {
                this.testBytes = new byte[this.TestLength];
            }

            chatClient.PublishMessage("Lobby", inputLine);
            
        }

        public void DebugReturn(DebugLevel level, string message)
        {
        }

        public void OnDisconnected()
        {
        }

        public void OnConnected()
        {
            chatClient.Subscribe(new string[] { "Lobby" }, 10);
        }

        public void OnChatStateChange(ChatState state)
        {

        }

        public void ShowChannel()
        {
            if (channel != null)
                CurrentChannelText.text = channel.ToStringMessages();
        }


        public void OnPrivateMessage(string sender, object message, string channelName)
        {

        }

        public void OnSubscribed(string[] channels, bool[] results)
        {

        }

        public void OnUnsubscribed(string[] channels)
        {

        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {

        }

        public void OnUserSubscribed(string channel, string user)
        {

        }

        public void OnUserUnsubscribed(string channel, string user)
        {

        }

        void Start()
        {
            if (NameBlock)
            {
                NameBlock.text = PhotonNetwork.NickName;
            }
            if (lobbyStart)
                Connect();
        }

        private void Update()
        {
            if (chatClient != null)
            {
                chatClient.Service();

                if (channel == null)
                {
                    chatClient.TryGetChannel("Lobby", out channel);
                    if (channel != null)
                    {
                        channel.ClearMessages();

                        buttonInput.SetActive(true);
                        InputFieldChat.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}