using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MahjongCommunicator : UdonSharpBehaviour
    {
        public Transform syncsParent;

        [UdonSynced] private int sendMessageId = 0;
        [UdonSynced] private int sendMessageLength = 0;

        private int receivedMessageId = 0;
        private byte[] receivedMessage = new byte[0];

        private GameObject[] syncs;
        private bool initialized = false;

        void Start()
        {
            syncs = new GameObject[syncsParent.childCount];

            for (int i = 0; i < syncsParent.childCount; ++i)
            {
                var name = "Sync (" + i + ")";
                var child = syncsParent.Find(name);

                if (!child)
                {
                    Debug.LogError(name + " not found");
                }

                syncs[i] = child.gameObject;
            }

            initialized = true;

            if (Networking.IsOwner(gameObject))
            {
                if (receivedMessageId != 0)
                {
                    WriteData(receivedMessage, receivedMessage.Length, receivedMessageId);
                }
            }
        }

        void Update()
        {
            var localPlayer = Networking.LocalPlayer;

            if (localPlayer == null)
            {
                return;
            }

            if (sendMessageId != receivedMessageId)
            {
                var message = ReadData(sendMessageLength, sendMessageId);

                if (message != null)
                {
                    receivedMessageId = sendMessageId;
                    receivedMessage = message;
                }
            }
        }

        public void TakeOwnership()
        {
            var localPlayer = Networking.LocalPlayer;

            Networking.SetOwner(localPlayer, gameObject);

            for (int i = 0; i < syncs.Length; ++i)
            {
                Networking.SetOwner(localPlayer, syncs[i].gameObject);
            }
        }

        public void SetMessage(byte[] message, int length)
        {
            if (Networking.IsOwner(gameObject))
            {
                ++sendMessageId;
                sendMessageLength = length;

                receivedMessage = new byte[length];
                receivedMessageId = sendMessageId;

                for (int i = 0; i < length; ++i)
                {
                    receivedMessage[i] = message[i];
                }

                if (initialized)
                {
                    WriteData(message, length, sendMessageId);
                }

                RequestSerialization();
            }
        }

        private void WriteData(byte[] data, int length, int messageId)
        {
            int numInts = length / 4 + (length % 4 != 0 ? 1 : 0);
            int numSyncs = numInts / 10 + (numInts % 10 != 0 ? 1 : 0);

            var remainBytes = length;
            var remainInts = numInts;
            int src = 0;

            for (int i = 0; i < numSyncs; ++i)
            {
                var sync = (UdonBehaviour)syncs[i].GetComponent(typeof(UdonBehaviour));
                var nInts = Mathf.Min(remainInts, 10);

                for (int j = 0; j < nInts; ++j)
                {
                    var nBytes = Mathf.Min(remainBytes, 4);
                    uint value = 0;

                    for (int k = 0; k < nBytes; ++k)
                    {
                        value |= (uint)data[src++] << (k * 8);
                    }

                    sync.SetProgramVariable("data" + j, value);

                    remainBytes -= nBytes;
                }

                sync.SetProgramVariable("messageId", messageId);

                sync.RequestSerialization();

                remainInts -= nInts;
            }
        }

        private byte[] ReadData(int length, int messageId)
        {
            int numInts = length / 4 + (length % 4 != 0 ? 1 : 0);
            int numSyncs = numInts / 10 + (numInts % 10 != 0 ? 1 : 0);

            for (int i = 0; i < numSyncs; ++i)
            {
                var sync = (UdonBehaviour)syncs[i].GetComponent(typeof(UdonBehaviour));

                if ((int)sync.GetProgramVariable("messageId") != messageId)
                {
                    return null;
                }
            }

            var remainBytes = length;
            var remainInts = numInts;
            var data = new byte[length];
            int dst = 0;

            for (int i = 0; i < numSyncs; ++i)
            {
                var sync = (UdonBehaviour)syncs[i].GetComponent(typeof(UdonBehaviour));
                var nInts = Mathf.Min(remainInts, 10);

                for (int j = 0; j < nInts; ++j)
                {
                    var nBytes = Mathf.Min(remainBytes, 4);
                    var value = (uint)sync.GetProgramVariable("data" + j);

                    for (int k = 0; k < nBytes; ++k)
                    {
                        data[dst++] = (byte)((value >> (k * 8)) & 0xff);
                    }

                    remainBytes -= nBytes;
                }

                remainInts -= nInts;
            }

            return data;
        }

        public int GetMessageId()
        {
            return receivedMessageId;
        }

        public byte[] GetMessage()
        {
            return receivedMessage;
        }
    }
}
