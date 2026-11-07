using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MahjongSync : UdonSharpBehaviour
    {
        [UdonSynced] public int messageId = 0;
        [UdonSynced] public uint data0 = 0;
        [UdonSynced] public uint data1 = 0;
        [UdonSynced] public uint data2 = 0;
        [UdonSynced] public uint data3 = 0;
        [UdonSynced] public uint data4 = 0;
        [UdonSynced] public uint data5 = 0;
        [UdonSynced] public uint data6 = 0;
        [UdonSynced] public uint data7 = 0;
        [UdonSynced] public uint data8 = 0;
        [UdonSynced] public uint data9 = 0;
    }
}
