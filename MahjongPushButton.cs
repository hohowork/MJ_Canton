using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongPushButton : UdonSharpBehaviour
    {
        public UdonBehaviour udonBehaviour;
        public bool local = false;
        public bool disableOnPush = true;
        public NetworkEventTarget target = NetworkEventTarget.Owner;
        public string id = "";

        public override void Interact()
        {
            if (disableOnPush)
            {
                udonBehaviour.SendCustomEvent("DisableAllButtons");
            }

            if (local)
            {
                udonBehaviour.SendCustomEvent(id);
            }
            else
            {
                udonBehaviour.SendCustomNetworkEvent(target, id);
            }
        }
    }
}
