using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongSoundPlayer : UdonSharpBehaviour
    {
        public AudioSource[] audioSources;
        public AudioClip[] entry;
        public AudioClip[] tsumo;
        public AudioClip[] ron;
        public AudioClip[] pon;
        public AudioClip[] kan;
        public AudioClip[] chi;
        public AudioClip[] reach;
        public AudioClip discard;
        public AudioClip myTurn;

        public void OnEntry(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnEntry" + index);
        }

        public void OnEntry0()
        {
            PlayVoice(entry, 0);
        }

        public void OnEntry1()
        {
            PlayVoice(entry, 1);
        }

        public void OnEntry2()
        {
            PlayVoice(entry, 2);
        }

        public void OnEntry3()
        {
            PlayVoice(entry, 3);
        }

        public void OnTsumo(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnTsumo" + index);
        }

        public void OnTsumo0()
        {
            PlayVoice(tsumo, 0);
        }

        public void OnTsumo1()
        {
            PlayVoice(tsumo, 1);
        }

        public void OnTsumo2()
        {
            PlayVoice(tsumo, 2);
        }

        public void OnTsumo3()
        {
            PlayVoice(tsumo, 3);
        }

        public void OnRon(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnRon" + index);
        }

        public void OnRon0()
        {
            PlayVoice(ron, 0);
        }

        public void OnRon1()
        {
            PlayVoice(ron, 1);
        }

        public void OnRon2()
        {
            PlayVoice(ron, 2);
        }

        public void OnRon3()
        {
            PlayVoice(ron, 3);
        }

        public void OnPon(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnPon" + index);
        }

        public void OnPon0()
        {
            PlayVoice(pon, 0);
        }

        public void OnPon1()
        {
            PlayVoice(pon, 1);
        }

        public void OnPon2()
        {
            PlayVoice(pon, 2);
        }

        public void OnPon3()
        {
            PlayVoice(pon, 3);
        }

        public void OnKan(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnKan" + index);
        }

        public void OnKan0()
        {
            PlayVoice(kan, 0);
        }

        public void OnKan1()
        {
            PlayVoice(kan, 1);
        }

        public void OnKan2()
        {
            PlayVoice(kan, 2);
        }

        public void OnKan3()
        {
            PlayVoice(kan, 3);
        }

        public void OnChi(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnChi" + index);
        }

        public void OnChi0()
        {
            PlayVoice(chi, 0);
        }

        public void OnChi1()
        {
            PlayVoice(chi, 1);
        }

        public void OnChi2()
        {
            PlayVoice(chi, 2);
        }

        public void OnChi3()
        {
            PlayVoice(chi, 3);
        }

        public void OnReach(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnReach" + index);
        }

        public void OnReach0()
        {
            PlayVoice(reach, 0);
        }

        public void OnReach1()
        {
            PlayVoice(reach, 1);
        }

        public void OnReach2()
        {
            PlayVoice(reach, 2);
        }

        public void OnReach3()
        {
            PlayVoice(reach, 3);
        }

        public void OnDiscard(int index)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnDiscard" + index);
        }

        public void OnDiscard0()
        {
            PlaySound(discard, 0);
        }

        public void OnDiscard1()
        {
            PlaySound(discard, 1);
        }

        public void OnDiscard2()
        {
            PlaySound(discard, 2);
        }

        public void OnDiscard3()
        {
            PlaySound(discard, 3);
        }

        public void OnMyTurn(int index)
        {
            PlaySound(myTurn, index);
        }

        private void PlayVoice(AudioClip[] audioClips, int index)
        {
            audioSources[index].PlayOneShot(audioClips[index]);
        }

        private void PlaySound(AudioClip audioClip, int index)
        {
            audioSources[index].PlayOneShot(audioClip);
        }
    }
}
