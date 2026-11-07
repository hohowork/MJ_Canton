using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongAdjusterResetButton : UdonSharpBehaviour
    {
        public Transform col;

        public override void Interact()
        {
            col.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
