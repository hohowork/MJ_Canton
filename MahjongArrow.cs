using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongArrow : UdonSharpBehaviour
    {
        public override void Interact()
        {
            var localPlayer = Networking.LocalPlayer;

            if (localPlayer != null)
            {
                var pos = localPlayer.GetPosition() + transform.rotation * new Vector3(0.0f, 0.0f, -84.0f);
                localPlayer.TeleportTo(pos, localPlayer.GetRotation());
            }
        }
    }
}
