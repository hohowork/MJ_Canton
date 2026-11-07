using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongMirrorButton : UdonSharpBehaviour
    {
        public GameObject mirror;

        public override void Interact()
        {
            mirror.SetActive(!mirror.activeSelf);
        }
    }
}
