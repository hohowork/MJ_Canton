using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace SuzuFactory.AozoraMahjong
{
    public enum TsumogiriDisplayMode
    {
        Never,
        OneRound,
        Always,
    }

    public enum HonbaMode
    {
        Mode1,
        Mode2,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Mahjong : UdonSharpBehaviour
    {
        [Tooltip("0 for Japanese, 1 for English")]
        public int initialLanguage = 0; // 0 - japanese; 1 - english
        public TsumogiriDisplayMode tsumogiriDisplayMode = TsumogiriDisplayMode.OneRound;
        private int language = 0;
        private const int NUM_LANGUAGES = 2;
        [Tooltip("Mode1:\n Increases when\n - Dealer wins hand\n - Dealer is tenpai at exhaustive draw\n Resets when the seats rotate\n\nMode2:\n Increases by\n - Exhaustive draw\n - Abortive draw\n - Dealer winning hand\n - Nagashi mangan\n Resets when any non-dealer wins the hand")]
        public HonbaMode honbaMode = HonbaMode.Mode1;
        public MahjongTranslation translation;
        public MahjongCommunicator comm;

        public GameObject[] entryButtons;
        public GameObject[] entryCpuButtons;
        public GameObject[] nextButtons;
        public GameObject startButton;
        public GameObject resetButton;
        public Transform tilesSet;
        public Transform pickTiles;
        public Transform turnArrow;
        public Transform statusTransform;
        public Text statusText;
        public Transform playerStatusTransform;
        public Text playerStatusText;
        public Transform honbaTransform;
        public Text honbaText;
        public Text kyotakuText;
        public Text[] pointTexts;

        public Transform playerUI;
        public GameObject tsumoButton;
        public GameObject ronButton;
        public GameObject ponButton;
        public GameObject kanButton;
        public GameObject chiPrevButton;
        public GameObject chiCenterButton;
        public GameObject chiNextButton;
        public Text chiPrevText;
        public Text chiCenterText;
        public Text chiNextText;
        public GameObject reachButton;
        public GameObject nineButton;
        public GameObject noButton;
        public GameObject[] nakinashiOnButtons;
        public GameObject[] nakinashiOffButtons;
        public GameObject[] tsumogiriOnButtons;
        public GameObject[] tsumogiriOffButtons;

        public Transform chicha;
        public GameObject chichaTon;
        public GameObject chichaNan;
        public GameObject chichaSha;
        public GameObject chichaPey;

        public Transform directions;
        public GameObject ownerMark;
        public GameObject[] reachBars;

        public MahjongSoundPlayer soundPlayer;

        public Transform spectator;

        private const int TILE_TYPE_M = 0;
        private const int TILE_TYPE_P = 1;
        private const int TILE_TYPE_S = 2;
        private const int TILE_TYPE_Z = 3;

        private const int TILE_NUMBER_TON = 1;
        private const int TILE_NUMBER_NAN = 2;
        private const int TILE_NUMBER_SHA = 3;
        private const int TILE_NUMBER_PEY = 4;
        private const int TILE_NUMBER_HAKU = 5;
        private const int TILE_NUMBER_HATSU = 6;
        private const int TILE_NUMBER_CHUN = 7;

        private const int NUM_ALL_TILES = 148;
        private const int NUM_ALL_WALL_TILES = 136;
        private const int NUM_NORMALIZED_TILES = 34;

        private const int PATTERN_HEAD = 0;
        private const int PATTERN_THREE = 1;
        private const int PATTERN_SEQUENCE = 2;
        private const int PATTERN_INCOMPLETE_HEAD = 3;
        private const int PATTERN_INCOMPLETE_THREE = 4;
        private const int PATTERN_INCOMPLETE_SEQUENCE = 5;
        private const int PATTERN_INCOMPLETE_SEQUENCE_SPLIT = 6;
        private const int PATTERN_TANKI = 7;
        private const int PATTERN_EATED_PON = 8;
        private const int PATTERN_EATED_ANKAN = 9;
        private const int PATTERN_EATED_MINKAN = 10;
        private const int PATTERN_EATED_CHI = 11;

        private const int HANDSTATE_INCOMPLETE = 0;
        private const int HANDSTATE_COMPLETE = 1;
        private const int HANDSTATE_COMPLETE_YAKUNASHI = 2;
        private const int HANDSTATE_COMPLETE_FURITEN = 3;

        private const int YAKU_MENZEN = 0;
        private const int YAKU_REACH = 1;
        private const int YAKU_DOUBLE_REACH = 2;
        private const int YAKU_IPPATSU = 3;
        private const int YAKU_TANYAO = 4;
        private const int YAKU_PINFU = 5;
        private const int YAKU_RYANPEHKOU = 6;
        private const int YAKU_IHPEHKOH = 7;
        private const int YAKU_YAKUHAI = 8;
        private const int YAKU_CHANKAN = 9;
        private const int YAKU_RINSHANKAIHOU = 10;
        private const int YAKU_HAITEIRAOYUE = 11;
        private const int YAKU_HOUTEIRAOYUI = 12;
        private const int YAKU_CHITOITSU = 13;
        private const int YAKU_RENPUHAI = 14;
        private const int YAKU_SUKANTSU = 15;
        private const int YAKU_SUANKO = 16;
        private const int YAKU_SUANKO_TANKI = 17;
        private const int YAKU_SANANKO = 18;
        private const int YAKU_SANKANTSU = 19;
        private const int YAKU_TOITOI = 20;
        private const int YAKU_SANSHOKUDOJUN = 21;
        private const int YAKU_SANSHOKUDOKO = 22;
        private const int YAKU_JUNCHAN = 23;
        private const int YAKU_HONROUTOU = 24;
        private const int YAKU_IKKITSUKAN = 25;
        private const int YAKU_CHANTA = 26;
        private const int YAKU_DAISANGEN = 27;
        private const int YAKU_SHOSANGEN = 28;
        private const int YAKU_TSUISO = 29;
        private const int YAKU_CHINIHSOU = 30;
        private const int YAKU_HONIHSOU = 31;
        private const int YAKU_KOKUSHIMUSO = 32;
        private const int YAKU_KOKUSHIMUSO13 = 33;
        private const int YAKU_DORA = 34;
        private const int YAKU_URA_DORA = 35;
        private const int YAKU_AKADORA = 36;
        private const int YAKU_TENHO = 37;
        private const int YAKU_CHIHO = 38;
        private const int YAKU_RENHO = 39;
        private const int YAKU_RYUISO = 40;
        private const int YAKU_DAISUSHI = 41;
        private const int YAKU_SHOSUSHI = 42;
        private const int YAKU_CHURENPOHTOU = 43;
        private const int YAKU_CHINROUTOU = 44;

        private const int GAMESTATE_INITIAL = 0;
        private const int GAMESTATE_ENTRY_COMPLETED = 1;
        private const int GAMESTATE_DRAWED = 2;
        private const int GAMESTATE_CONFIRM_RON = 3;
        private const int GAMESTATE_CONFIRM_KAN = 4;
        private const int GAMESTATE_CONFIRM_PON = 5;
        private const int GAMESTATE_CONFIRM_CHI = 6;
        private const int GAMESTATE_EATED = 7;
        private const int GAMESTATE_EXHAUSTIVE_DRAW = 8;
        private const int GAMESTATE_ABORTIVE_DRAW_4KAN = 9;
        private const int GAMESTATE_ABORTIVE_DRAW_4REACH = 10;
        private const int GAMESTATE_ABORTIVE_DRAW_SUFURENDA = 11;
        private const int GAMESTATE_ABORTIVE_DRAW_NINE = 12;
        private const int GAMESTATE_TSUMO = 13;
        private const int GAMESTATE_RON = 14;
        private const int GAMESTATE_SELECT_KAN = 15;
        private const int GAMESTATE_SELECT_REACH = 16;
        private const int GAMESTATE_CONFIRM_RON_CHANKAN = 17;

        private const int WAIT_TYPE_RYANMEN = 0;
        private const int WAIT_TYPE_KANCHAN = 1;
        private const int WAIT_TYPE_PENCHAN = 2;
        private const int WAIT_TYPE_SHANPON = 3;
        private const int WAIT_TYPE_TANKI = 4;

        private const int EATED_SET_TYPE_PON = 1;
        private const int EATED_SET_TYPE_ANKAN = 2;
        private const int EATED_SET_TYPE_MINKAN = 3;
        private const int EATED_SET_TYPE_KAKAN = 4;
        private const int EATED_SET_TYPE_CHI_PREV = 5;
        private const int EATED_SET_TYPE_CHI_CENTER = 6;
        private const int EATED_SET_TYPE_CHI_NEXT = 7;

        private const int EATED_SET_DIRECTION_SELF = 0;
        private const int EATED_SET_DIRECTION_LEFT = 1;
        private const int EATED_SET_DIRECTION_CENTER = 2;
        private const int EATED_SET_DIRECTION_RIGHT = 3;

        private const int GENERATE_WALL_TILES = 0;
        private const int GENERATE_INCLUDE_NO_USE = 1;
        private const int GENERATE_NORMALIZED = 2;

        private const int YAOCHU_M1 = 0;
        private const int YAOCHU_M9 = 1;
        private const int YAOCHU_P1 = 2;
        private const int YAOCHU_P9 = 3;
        private const int YAOCHU_S1 = 4;
        private const int YAOCHU_S9 = 5;
        private const int YAOCHU_TON = 6;
        private const int YAOCHU_NAN = 7;
        private const int YAOCHU_SHA = 8;
        private const int YAOCHU_PEY = 9;
        private const int YAOCHU_HAKU = 10;
        private const int YAOCHU_HATSU = 11;
        private const int YAOCHU_CHUN = 12;
        private const int YAOCHU_TYPES = 13;

        private const float tileWidth = 0.0212f;
        private const float tileHeight = 0.029f;
        private const float tileDepth = 0.0162f;
        private const float tileWidthHalf = tileWidth / 2.0f;
        private const float tileHeightHalf = tileHeight / 2.0f;
        private const float tileDepthHalf = tileDepth / 2.0f;

        private int numRedM = 1;
        private int numRedP = 1;
        private int numRedS = 1;

        private int currentStateIndex = -1;

        private int gameState = GAMESTATE_INITIAL;

        private bool[] entry;
        private bool[] nextReady;
        private bool[] entrySelf;
        private bool[] entryCpu;
        private int[] points;
        private int kyotaku = 0;

        private int round = 0;
        private int honba = 0;
        private int firstLeader = 0;
        private int leader = 0;
        private int turn = 0;
        private int eatTurn = 0;
        private int lastKakanIndex = -1;

        private int[] wallTiles = new int[NUM_ALL_WALL_TILES];
        private int numWallTiles = 0;
        private int numKan = 0;
        private int numDoraOpen = 0;
        private bool reached = false;

        private int[][] handTiles;
        private int[] numHandTiles;

        private int[][][] eatedSetsTiles;
        private int[][] eatedSetsTypes;
        private int[][] eatedSetsDirections;
        private int[] numEatedSets;

        private int[][] discardedTiles;
        private bool[][] discardedTilesTsumogiri;
        private int[] numDiscardedTiles;

        private int[] handState;
        private bool[] overlook;
        private int[] reachIndex;
        private bool[] ippatsu;
        private bool[] canDoubleReach;
        private bool[] doubleReach;
        private bool[] tenpai;
        private bool[] nagashimangan;
        private int[][] waitingTiles;

        private int[][] waitingTilesBy;
        private bool canReach = false;
        private bool canReachDirty = true;

        private int selectIndex = -1;
        private bool[] nakinashi;
        private bool[] tsumogiri;

        private float lastTime = 0.0f;
        private double[] cpuScores;
        private int cpuScoresIndex = -1;

        private int maxTileId = 0;
        private int maxNormalizedTileId = 0;

        private int[] allNormalizedTiles;

        private int[][][] validPatternsWin;
        private int[][][] validPatternsWait;

        private Transform[] tileTransforms;
        private Transform[] tileTransformMap;
        private Transform[] sampleTileTransforms;
        private Transform[] sampleTileTransformMap;
        private Transform[] pickTileTransformMap;

        private Transform[] transformsSource = new Transform[1000];
        private Transform[] transformsSpectator = new Transform[1000];
        private int numTransforms = 0;

        private Text[] textsSource = new Text[1000];
        private Text[] textsSpectator = new Text[1000];
        private int numTexts = 0;

        private const int MAX_INITIALIZE_COUNT = 11;

        private int initializeCount = 0;
        private bool initialized => initializeCount >= MAX_INITIALIZE_COUNT;
        private bool initializing = false;
        [UdonSynced] private bool ownerEnabled = false;

        private int[] _yaochuTiles = null;

        private int[] GetYaochuTiles()
        {
            if (_yaochuTiles == null)
            {
                _yaochuTiles = new int[YAOCHU_TYPES];
                _yaochuTiles[YAOCHU_M1] = GetTileId(0, TILE_TYPE_M, 1, false);
                _yaochuTiles[YAOCHU_M9] = GetTileId(0, TILE_TYPE_M, 9, false);
                _yaochuTiles[YAOCHU_P1] = GetTileId(0, TILE_TYPE_P, 1, false);
                _yaochuTiles[YAOCHU_P9] = GetTileId(0, TILE_TYPE_P, 9, false);
                _yaochuTiles[YAOCHU_S1] = GetTileId(0, TILE_TYPE_S, 1, false);
                _yaochuTiles[YAOCHU_S9] = GetTileId(0, TILE_TYPE_S, 9, false);
                _yaochuTiles[YAOCHU_TON] = GetTileId(0, TILE_TYPE_Z, TILE_NUMBER_TON, false);
                _yaochuTiles[YAOCHU_NAN] = GetTileId(0, TILE_TYPE_Z, TILE_NUMBER_NAN, false);
                _yaochuTiles[YAOCHU_SHA] = GetTileId(0, TILE_TYPE_Z, TILE_NUMBER_SHA, false);
                _yaochuTiles[YAOCHU_PEY] = GetTileId(0, TILE_TYPE_Z, TILE_NUMBER_PEY, false);
                _yaochuTiles[YAOCHU_HAKU] = GetTileId(0, TILE_TYPE_Z, TILE_NUMBER_HAKU, false);
                _yaochuTiles[YAOCHU_HATSU] = GetTileId(0, TILE_TYPE_Z, TILE_NUMBER_HATSU, false);
                _yaochuTiles[YAOCHU_CHUN] = GetTileId(0, TILE_TYPE_Z, TILE_NUMBER_CHUN, false);
            }

            return _yaochuTiles;
        }

        void Start()
        {
            SwitchLanguage(initialLanguage);
            Initialize();
        }

        public void SwitchLanguage(int language)
        {
            this.language = language;

            string[] prefixes = { "JA_", "EN_" };

            for (int i = 0; i < prefixes.Length; ++i)
            {
                ActivateLanguage(prefixes[i], i == language);
            }
        }

        public void SwitchLanguageNext()
        {
            var nextLanguage = language + 1;

            if (nextLanguage == NUM_LANGUAGES)
            {
                nextLanguage = 0;
            }

            SwitchLanguage(nextLanguage);
        }

        private void ActivateLanguage(string prefix, bool active)
        {
            var children = GetComponentsInChildren(typeof(Transform), true);

            foreach (var child in children)
            {
                var go = child.gameObject;

                if (go.name.StartsWith(prefix))
                {
                    go.SetActive(active);
                }
            }
        }

        private bool Initialize()
        {
            // 初期化処理完了していたらtrue
            if (initialized)
            {
                return true;
            }
            // 初期化処理開始していたらここでfalse
            if (initializing)
            {
                return false;
            }
            // 初期化処理開始フラグを立てる
            initializing = true;
            // 最低限の初期化
            validPatternsWin = new int[5][][];
            validPatternsWait = new int[5][][];
            // 次のフレームに処理を任せる
            SendCustomEventDelayedFrames(nameof(Initializing), 1);
            // 初期化処理完了していないのでfalse
            return false;
        }

        public void Initializing()
        {
            var firstTime = System.DateTime.Now;
            // 処理を11分割
            while (initializeCount < MAX_INITIALIZE_COUNT)
            {
                if (initializeCount < 5)
                {
                    validPatternsWin = BuildValidPatterns(true, validPatternsWin, initializeCount);
                }
                else if (initializeCount < 10)
                {
                    validPatternsWait = BuildValidPatterns(false, validPatternsWait, initializeCount - 5);
                }
                else
                {
                    // 最終初期化
                    LastInitialize();
                }
                initializeCount++;
                // 制限時間(3秒)を超過したらこのフレームの処理を完了する
                var sec = (System.DateTime.Now - firstTime).TotalSeconds;
                if (sec > 3.0)
                {
                    // Debug.Log($"Initializing break{initializeCount} {sec}");
                    break;
                }
            }
            if (!initialized)
            {
                // 処理が終わってなければ次のフレームに処理を任せる
                SendCustomEventDelayedFrames(nameof(Initializing), 1);
            }
        }

        private void LastInitialize()
        {
            if (initialized) { return; }

            // Debug.Log("LastInitialize 1");

            entry = new bool[4];
            nextReady = new bool[4];
            entrySelf = new bool[4];
            entryCpu = new bool[4];
            points = new int[4];

            handTiles = new int[4][];
            numHandTiles = new int[4];

            eatedSetsTiles = new int[4][][];
            eatedSetsTypes = new int[4][];
            eatedSetsDirections = new int[4][];
            numEatedSets = new int[4];

            discardedTiles = new int[4][];
            discardedTilesTsumogiri = new bool[4][];
            numDiscardedTiles = new int[4];

            handState = new int[0];
            overlook = new bool[4];
            reachIndex = new int[4];
            ippatsu = new bool[4];
            canDoubleReach = new bool[4];
            doubleReach = new bool[4];
            tenpai = new bool[4];
            nagashimangan = new bool[4];
            waitingTiles = new int[4][];
            waitingTilesBy = new int[14][];

            // Debug.Log("LastInitialize 2");

            for (int i = 0; i < 4; ++i)
            {
                handTiles[i] = new int[14];

                eatedSetsTypes[i] = new int[4];
                eatedSetsDirections[i] = new int[4];
                eatedSetsTiles[i] = new int[4][];

                for (int j = 0; j < eatedSetsTiles[i].Length; ++j)
                {
                    eatedSetsTiles[i][j] = new int[4];
                }

                discardedTiles[i] = new int[NUM_ALL_WALL_TILES];
                discardedTilesTsumogiri[i] = new bool[NUM_ALL_WALL_TILES];
                waitingTiles[i] = new int[0];
                waitingTilesBy[i] = new int[0];
            }

            nakinashi = new bool[4];
            tsumogiri = new bool[4];
            cpuScores = new double[14];

            // Debug.Log("LastInitialize 3");

            var ids = GenerateTileIds(GENERATE_INCLUDE_NO_USE);

            for (int i = 0; i < ids.Length; ++i)
            {
                maxTileId = Mathf.Max(maxTileId, ids[i]);
                maxNormalizedTileId = Mathf.Max(maxNormalizedTileId, NormalizeTileId(ids[i]));
            }

            allNormalizedTiles = GenerateTileIds(GENERATE_NORMALIZED);

            // Debug.Log("LastInitialize 4");

            tileTransforms = new Transform[ids.Length];
            tileTransformMap = new Transform[maxTileId + 1];

            for (int i = 0; i < ids.Length; ++i)
            {
                var id = ids[i];
                var tile = GetTile("Tiles", id);
                tileTransforms[i] = tile;
                tileTransformMap[id] = tile;
            }

            sampleTileTransforms = new Transform[allNormalizedTiles.Length];
            sampleTileTransformMap = new Transform[maxNormalizedTileId + 1];

            for (int i = 0; i < allNormalizedTiles.Length; ++i)
            {
                var id = allNormalizedTiles[i];
                var tile = GetTile("SampleTiles", id);
                sampleTileTransforms[i] = tile;
                sampleTileTransformMap[id] = tile;
            }

            // Debug.Log("LastInitialize 5");

            pickTileTransformMap = new Transform[pickTiles.childCount];

            for (int i = 0; i < pickTileTransformMap.Length; ++i)
            {
                var name = "PickTile" + i;
                var tile = pickTiles.Find(name);

                if (tile == null)
                {
                    Debug.LogError(name + " not found");
                }

                pickTileTransformMap[i] = tile;
            }

            ResetAllImpl();
            ResetGameLocal();
            DisableAllButtons();

            // {
            //     var data = new byte[] { };

            //     comm.SetMessage(data, data.Length);
            //     entrySelf[0] = true;
            //     entrySelf[1] = true;
            //     entrySelf[2] = true;
            //     entrySelf[3] = true;
            // }

            MapSpectator();

            // Debug.Log("LastInitialize 6");
        }

        private void MapSpectator()
        {
            // Debug.Log("MapSpectator 1");

            if (spectator == null)
            {
                return;
            }

            // Debug.Log("MapSpectator 2");

            Transform[] stackSource = new Transform[100];
            Transform[] stackSpectator = new Transform[100];
            int numStack = 0;

            stackSource[numStack] = transform;
            stackSpectator[numStack] = spectator;
            ++numStack;

            // Debug.Log("MapSpectator 3");

            while (numStack != 0)
            {
                --numStack;
                var tSource = stackSource[numStack];
                var tSpectator = stackSpectator[numStack];

                if (tSource != transform)
                {
                    transformsSource[numTransforms] = tSource;
                    transformsSpectator[numTransforms] = tSpectator;
                    ++numTransforms;

                    var textSource = tSource.GetComponent<Text>();
                    var textSpectator = tSpectator.GetComponent<Text>();

                    if (textSource != null && textSpectator != null)
                    {
                        textsSource[numTexts] = textSource;
                        textsSpectator[numTexts] = textSpectator;
                        ++numTexts;
                    }
                }

                for (int i = 0; i < tSpectator.childCount; ++i)
                {
                    var cSpectator = tSpectator.GetChild(i);
                    var cSource = tSource.Find(cSpectator.name);

                    if (cSource == null)
                    {
                        continue;
                    }

                    stackSource[numStack] = cSource;
                    stackSpectator[numStack] = cSpectator;
                    ++numStack;
                }
            }

            // Debug.Log("MapSpectator 5");
        }

        private void UpdateSpectator()
        {
            for (int i = 0; i < numTransforms; ++i)
            {
                var source = transformsSource[i];
                var spectator = transformsSpectator[i];

                spectator.gameObject.SetActive(source.gameObject.activeSelf);

                spectator.localPosition = source.localPosition;
                spectator.localRotation = source.localRotation;
            }

            for (int i = 0; i < numTexts; ++i)
            {
                var source = textsSource[i];
                var spectator = textsSpectator[i];

                spectator.text = source.text;
            }
        }

        private int[][][] BuildValidPatterns(bool win, int[][][] result, int i)
        {
            var firstPattern = Mahjong.PATTERN_HEAD;
            var lastPattern = win ? Mahjong.PATTERN_SEQUENCE : Mahjong.PATTERN_INCOMPLETE_SEQUENCE_SPLIT;

            int num = 0;
            int[] pattern = new int[5 - i];

            for (int j = 0; j < pattern.Length; ++j)
            {
                pattern[j] = firstPattern;
            }

            while (true)
            {
                bool valid = win ? IsValidPatternWin(pattern) : IsValidPatternWait(pattern);

                if (valid)
                {
                    ++num;
                }

                if (!AdvancePattern(pattern, firstPattern, lastPattern))
                {
                    break;
                }
            }

            result[i] = new int[num][];
            int dst = 0;

            for (int j = 0; j < pattern.Length; ++j)
            {
                pattern[j] = Mahjong.PATTERN_HEAD;
            }

            while (true)
            {
                bool valid = win ? IsValidPatternWin(pattern) : IsValidPatternWait(pattern);

                if (valid)
                {
                    result[i][dst++] = (int[])pattern.Clone();
                }

                if (!AdvancePattern(pattern, firstPattern, lastPattern))
                {
                    break;
                }
            }
            return result;
        }

        private bool IsValidPatternWin(int[] pattern)
        {
            int numHead = 0;

            for (int i = 0; i < pattern.Length; ++i)
            {
                if (pattern[i] == PATTERN_HEAD)
                {
                    ++numHead;
                }
            }

            return numHead == 1;
        }

        private bool IsValidPatternWait(int[] pattern)
        {
            int numHead = 0;
            int numIncomplete = 0;

            for (int i = 0; i < pattern.Length; ++i)
            {
                switch (pattern[i])
                {
                    case PATTERN_HEAD:
                    case PATTERN_INCOMPLETE_HEAD:
                        ++numHead;
                        break;
                }

                switch (pattern[i])
                {
                    case PATTERN_INCOMPLETE_HEAD:
                    case PATTERN_INCOMPLETE_THREE:
                    case PATTERN_INCOMPLETE_SEQUENCE:
                    case PATTERN_INCOMPLETE_SEQUENCE_SPLIT:
                        ++numIncomplete;
                        break;
                }
            }

            return numHead == 1 && numIncomplete == 1;
        }

        private bool AdvancePattern(int[] pattern, int firstPattern, int lastPattern)
        {
            ++pattern[0];

            int i = 0;

            while (pattern[i] > lastPattern)
            {
                ++i;

                for (int j = 0; j < i; ++j)
                {
                    pattern[j] = firstPattern;
                }

                if (i == pattern.Length)
                {
                    break;
                }

                ++pattern[i];
            }

            if (i == pattern.Length)
            {
                return false;
            }

            return true;
        }

        public void ResetAll()
        {
            if (!Initialize()) { return; }

            // Debug.Log("ResetAll 1");

            if (Networking.IsOwner(gameObject))
            {
                ResetAllImpl();
                Serialize(true);
            }

            // Debug.Log("ResetAll 2");

            for (int i = 0; i < 4; ++i)
            {
                entrySelf[i] = false;
            }

            ResetGameLocal();

            // Debug.Log("ResetAll 3");
        }

        private void ResetAllImpl()
        {
            gameState = GAMESTATE_INITIAL;

            round = 0;
            honba = 0;
            firstLeader = 0;
            leader = 0;
            turn = 0;
            eatTurn = 0;
            lastKakanIndex = -1;

            ResetGame();

            for (int i = 0; i < 4; ++i)
            {
                entry[i] = false;
                nextReady[i] = false;
                entryCpu[i] = false;
                points[i] = 25000;
            }

            kyotaku = 0;
        }

        private void ResetGame()
        {
            numWallTiles = 0;
            numKan = 0;
            numDoraOpen = 0;
            reached = false;
            handState = new int[0];

            for (int i = 0; i < 4; ++i)
            {
                nextReady[i] = false;
                numHandTiles[i] = 0;
                numDiscardedTiles[i] = 0;
                numEatedSets[i] = 0;
                overlook[i] = false;
                reachIndex[i] = -1;
                ippatsu[i] = false;
                canDoubleReach[i] = true;
                doubleReach[i] = false;
                tenpai[i] = false;
                nagashimangan[i] = true;
                waitingTiles[i] = new int[0];
            }

            cpuScoresIndex = -1;

            for (int i = 0; i < cpuScores.Length; ++i)
            {
                cpuScores[i] = 0.0;
            }
        }

        public void ResetGameLocal()
        {
            if (!Initialize()) { return; }

            selectIndex = -1;

            for (int i = 0; i < 4; ++i)
            {
                nakinashi[i] = false;
                tsumogiri[i] = false;
            }

            for (int i = 0; i < waitingTilesBy.Length; ++i)
            {
                waitingTilesBy[i] = new int[0];
            }

            canReach = false;
            canReachDirty = true;
        }

        public void ResetSelectIndex()
        {
            if (!Initialize()) { return; }

            selectIndex = -1;
        }

        public void DisableAllButtons()
        {
            // Debug.Log("DisableAllButtons 1");

            for (int i = 0; i < 4; ++i)
            {
                entryButtons[i].SetActive(false);
                entryCpuButtons[i].SetActive(false);
                nextButtons[i].SetActive(false);
            }

            startButton.SetActive(false);
            playerUI.gameObject.SetActive(false);
            tsumoButton.SetActive(false);
            ronButton.SetActive(false);
            ponButton.SetActive(false);
            kanButton.SetActive(false);
            chiPrevButton.SetActive(false);
            chiCenterButton.SetActive(false);
            chiNextButton.SetActive(false);
            chiPrevText.text = "";
            chiCenterText.text = "";
            chiNextText.text = "";
            reachButton.SetActive(false);
            nineButton.SetActive(false);
            noButton.SetActive(false);

            // Debug.Log("DisableAllButtons 2");

            for (int i = 0; i < 4; ++i)
            {
                nakinashiOnButtons[i].SetActive(false);
                nakinashiOffButtons[i].SetActive(false);
                tsumogiriOnButtons[i].SetActive(false);
                tsumogiriOffButtons[i].SetActive(false);
            }

            // Debug.Log("DisableAllButtons 3");

            for (int i = 0; i < pickTiles.childCount; ++i)
            {
                var pickTile = pickTiles.GetChild(i);
                pickTile.gameObject.SetActive(false);
            }

            // Debug.Log("DisableAllButtons 4");
        }

        void Update()
        {
            // Debug.Log("Update 1");
            if (!initialized) { return; }
            var localPlayer = Networking.LocalPlayer;

            if (localPlayer != null)
            {
                var delta = localPlayer.GetPosition() - transform.position;
                var trackRotation = Quaternion.Euler(0.0f, -Mathf.Atan2(delta.z, delta.x) * 180.0f / Mathf.PI, 0.0f);
                statusTransform.rotation = trackRotation;
            }

            // Debug.Log("Update 2");

            Deserialize();

            // Debug.Log("Update 3");

            if (Networking.IsOwner(gameObject))
            {
                var now = Time.time;

                if (now - lastTime > 1.0f)
                {
                    RunCpu();
                }
            }
        }

        // void OnEnable()
        // {
        //     if (Networking.IsOwner(gameObject))
        //     {
        //         ownerEnabled = true;
        //         RequestSerialization();
        //     }
        //     else
        //     {
        //         if (ownerEnabled)
        //         {
        //             SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(RequestRequestSerialization));
        //         }
        //         else
        //         {
        //             Networking.SetOwner(Networking.LocalPlayer, gameObject);
        //             comm.TakeOwnership();

        //             ownerEnabled = true;
        //             RequestSerialization();
        //         }
        //     }
        // }

        public void RequestRequestSerialization()
        {
            RequestSerialization();
        }

        public void OnStartButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnStartButton 1");

            if (gameState != GAMESTATE_ENTRY_COMPLETED)
            {
                return;
            }

            RandomizeFirstLeader();
            StartGame();
            Serialize(true);

            // Debug.Log("OnStartButton 2");
        }

        public void OnEntryButton0()
        {
            entrySelf[0] = true;
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(OnEntryButton0Owner));
        }

        public void OnEntryButton0Owner()
        {
            OnEntry(0);
        }

        public void OnEntryButton1()
        {
            entrySelf[1] = true;
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(OnEntryButton1Owner));
        }

        public void OnEntryButton1Owner()
        {
            OnEntry(1);
        }

        public void OnEntryButton2()
        {
            entrySelf[2] = true;
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(OnEntryButton2Owner));
        }

        public void OnEntryButton2Owner()
        {
            OnEntry(2);
        }

        public void OnEntryButton3()
        {
            entrySelf[3] = true;
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(OnEntryButton3Owner));
        }

        public void OnEntryButton3Owner()
        {
            OnEntry(3);
        }

        private void OnEntry(int seat)
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnEntry 1");

            if (entry[seat])
            {
                Serialize(false);
                return;
            }

            soundPlayer.OnEntry(seat);

            entry[seat] = true;

            if (gameState != GAMESTATE_INITIAL)
            {
                Serialize(false);
                return;
            }

            CheckEntry();

            // Debug.Log("OnEntry 2");
        }

        public void OnEntryCpuButton0()
        {
            OnEntryCpu(0);
        }

        public void OnEntryCpuButton1()
        {
            OnEntryCpu(1);
        }

        public void OnEntryCpuButton2()
        {
            OnEntryCpu(2);
        }

        public void OnEntryCpuButton3()
        {
            OnEntryCpu(3);
        }

        private void OnEntryCpu(int seat)
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnEntryCpu 1");

            if (entry[seat])
            {
                Serialize(false);
                return;
            }

            soundPlayer.OnEntry(seat);

            entry[seat] = true;
            entryCpu[seat] = true;

            if (gameState != GAMESTATE_INITIAL)
            {
                Serialize(false);
                return;
            }

            CheckEntry();

            // Debug.Log("OnEntryCpu 2");
        }

        private void CheckEntry()
        {
            if (entry[0] && entry[1] && entry[2] && entry[3])
            {
                gameState = GAMESTATE_ENTRY_COMPLETED;
            }

            Serialize(false);
        }

        public void OnResync()
        {
            if (!Initialize()) { return; }

            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ResetEntrySelf));

            for (int i = 0; i < 4; ++i)
            {
                entry[i] = false;
                entryCpu[i] = false;
            }

            Serialize(true);
        }

        public void ResetEntrySelf()
        {
            if (!Initialize()) { return; }

            for (int i = 0; i < 4; ++i)
            {
                entrySelf[i] = false;
            }
        }

        public void OnNextButton0()
        {
            OnNext(0);
        }

        public void OnNextButton1()
        {
            OnNext(1);
        }

        public void OnNextButton2()
        {
            OnNext(2);
        }

        public void OnNextButton3()
        {
            OnNext(3);
        }

        private void OnNext(int seat)
        {
            if (!Initialize()) { return; }

            if (nextReady[seat])
            {
                Serialize(false);
                return;
            }

            nextReady[seat] = true;
            CheckNext();
        }

        private void CheckNext()
        {
            bool ready = true;

            for (int i = 0; i < 4; ++i)
            {
                if (!nextReady[i] && !entryCpu[i])
                {
                    ready = false;
                    break;
                }
            }

            if (!ready)
            {
                Serialize(false);
                return;
            }

            // Debug.Log("CheckNext 1");

            switch (gameState)
            {
                case GAMESTATE_EXHAUSTIVE_DRAW:
                    {
                        // Debug.Log("CheckNext 2");

                        if (IsExistsNagashimangan())
                        {
                            int turn_ = leader;

                            while (true)
                            {
                                if (nagashimangan[turn_])
                                {
                                    // Debug.Log("CheckNext 3");

                                    if (turn_ == leader)
                                    {
                                        var delta = 4000 + honba * 100;

                                        points[turn_] += delta * 3;

                                        for (int i = 1; i < 4; ++i)
                                        {
                                            var t = (turn_ + i) % 4;
                                            points[t] -= delta;
                                        }
                                    }
                                    else
                                    {
                                        var delta = 2000 + honba * 100;
                                        var deltaLeader = 4000 + honba * 100;

                                        points[turn_] += delta * 2 + deltaLeader;

                                        for (int i = 1; i < 4; ++i)
                                        {
                                            var t = (turn_ + i) % 4;
                                            points[t] -= t == leader ? deltaLeader : delta;
                                        }

                                        AdvanceLeader();
                                    }

                                    points[turn_] += kyotaku;
                                    kyotaku = 0;

                                    switch (honbaMode)
                                    {
                                        case HonbaMode.Mode1:
                                            break;
                                        case HonbaMode.Mode2:
                                            ++honba;
                                            break;
                                    }

                                    break;
                                }

                                turn_ = (turn_ + 1) % 4;
                            }
                        }
                        else
                        {
                            // Debug.Log("CheckNext 4");

                            int numTenpai = 0;

                            for (int i = 0; i < 4; ++i)
                            {
                                if (tenpai[i])
                                {
                                    ++numTenpai;
                                }
                            }

                            for (int i = 0; i < 4; ++i)
                            {
                                switch (numTenpai)
                                {
                                    case 1:
                                        points[i] += tenpai[i] ? 3000 : -1000;
                                        break;
                                    case 2:
                                        points[i] += tenpai[i] ? 1500 : -1500;
                                        break;
                                    case 3:
                                        points[i] += tenpai[i] ? 1000 : -3000;
                                        break;
                                }
                            }

                            switch (honbaMode)
                            {
                                case HonbaMode.Mode1:
                                    if (tenpai[leader])
                                    {
                                        ++honba;
                                        break;
                                    }

                                    honba = 0;
                                    AdvanceLeader();
                                    break;
                                case HonbaMode.Mode2:
                                    ++honba;

                                    if (tenpai[leader])
                                    {
                                        break;
                                    }

                                    AdvanceLeader();
                                    break;
                            }
                        }
                        break;
                    }
                case GAMESTATE_ABORTIVE_DRAW_4KAN:
                case GAMESTATE_ABORTIVE_DRAW_4REACH:
                case GAMESTATE_ABORTIVE_DRAW_SUFURENDA:
                case GAMESTATE_ABORTIVE_DRAW_NINE:
                    switch (honbaMode)
                    {
                        case HonbaMode.Mode1:
                            break;
                        case HonbaMode.Mode2:
                            ++honba;
                            break;
                    }
                    break;
                case GAMESTATE_TSUMO:
                    {
                        // Debug.Log("CheckNext 5");

                        var point = GetPointFromHandState();

                        if (turn == leader)
                        {
                            var delta = Ceil100(point / 3) + honba * 100;

                            points[turn] += delta * 3;

                            for (int i = 1; i < 4; ++i)
                            {
                                var t = (turn + i) % 4;
                                points[t] -= delta;
                            }

                            ++honba;
                        }
                        else
                        {
                            var delta = Ceil100(point / 4) + honba * 100;
                            var deltaLeader = Ceil100(point / 2) + honba * 100;

                            points[turn] += delta * 2 + deltaLeader;

                            for (int i = 1; i < 4; ++i)
                            {
                                var t = (turn + i) % 4;
                                points[t] -= t == leader ? deltaLeader : delta;
                            }

                            honba = 0;
                            AdvanceLeader();
                        }

                        points[turn] += kyotaku;
                        kyotaku = 0;

                        break;
                    }
                case GAMESTATE_RON:
                    {
                        // Debug.Log("CheckNext 6");

                        var point = GetPointFromHandState();
                        var delta = point + honba * 300;

                        points[turn] -= delta;
                        points[eatTurn] += delta + kyotaku;
                        kyotaku = 0;

                        if (eatTurn == leader)
                        {
                            ++honba;
                        }
                        else
                        {
                            honba = 0;
                            AdvanceLeader();
                        }

                        break;
                    }
                default:
                    return;
            }

            // Debug.Log("CheckNext 7");

            StartGame();
            Serialize(true);

            // Debug.Log("CheckNext 8");
        }

        private int GetPointFromHandState()
        {
            int src = 3;
            var numPoint0 = (uint)handState[src++];
            var numPoint1 = (uint)handState[src++];
            var numPoint2 = (uint)handState[src++];
            var numPoint3 = (uint)handState[src++];
            var numPoint = (int)(numPoint0 | (numPoint1 << 8) | (numPoint2 << 16) | (numPoint3 << 24));

            return numPoint;
        }

        private void AdvanceLeader()
        {
            leader = (leader + 1) % 4;

            if (leader == firstLeader)
            {
                round = (round + 1) % 4;
            }
        }

        public void OnPickTile0Local()
        {
            OnPickTileLocal(0, false);
        }

        public void OnPickTile1Local()
        {
            OnPickTileLocal(1, false);
        }

        public void OnPickTile2Local()
        {
            OnPickTileLocal(2, false);
        }

        public void OnPickTile3Local()
        {
            OnPickTileLocal(3, false);
        }

        public void OnPickTile4Local()
        {
            OnPickTileLocal(4, false);
        }

        public void OnPickTile5Local()
        {
            OnPickTileLocal(5, false);
        }

        public void OnPickTile6Local()
        {
            OnPickTileLocal(6, false);
        }

        public void OnPickTile7Local()
        {
            OnPickTileLocal(7, false);
        }

        public void OnPickTile8Local()
        {
            OnPickTileLocal(8, false);
        }

        public void OnPickTile9Local()
        {
            OnPickTileLocal(9, false);
        }

        public void OnPickTile10Local()
        {
            OnPickTileLocal(10, false);
        }

        public void OnPickTile11Local()
        {
            OnPickTileLocal(11, false);
        }

        public void OnPickTile12Local()
        {
            OnPickTileLocal(12, false);
        }

        public void OnPickTile13Local()
        {
            OnPickTileLocal(13, false);
        }

        private void OnPickTileLocal(int index, bool force)
        {
            if (!force)
            {
                if (selectIndex != index)
                {
                    selectIndex = index;
                    UpdateView();
                    return;
                }
            }

            selectIndex = -1;

            SendCustomNetworkEvent(NetworkEventTarget.Owner, "OnPickTile" + index);
        }

        public void OnPickTile0()
        {
            OnPickTile(0);
        }

        public void OnPickTile1()
        {
            OnPickTile(1);
        }

        public void OnPickTile2()
        {
            OnPickTile(2);
        }

        public void OnPickTile3()
        {
            OnPickTile(3);
        }

        public void OnPickTile4()
        {
            OnPickTile(4);
        }

        public void OnPickTile5()
        {
            OnPickTile(5);
        }

        public void OnPickTile6()
        {
            OnPickTile(6);
        }

        public void OnPickTile7()
        {
            OnPickTile(7);
        }

        public void OnPickTile8()
        {
            OnPickTile(8);
        }

        public void OnPickTile9()
        {
            OnPickTile(9);
        }

        public void OnPickTile10()
        {
            OnPickTile(10);
        }

        public void OnPickTile11()
        {
            OnPickTile(11);
        }

        public void OnPickTile12()
        {
            OnPickTile(12);
        }

        public void OnPickTile13()
        {
            OnPickTile(13);
        }

        private void OnPickTile(int index)
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnPickTile 1");

            var tile = handTiles[turn][index];
            var normalizedTile = NormalizeTileId(tile);

            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                    {
                        ippatsu[turn] = false;
                        canDoubleReach[turn] = false;
                        var isTsumogiri = index == numHandTiles[turn] - 1;
                        DiscardTile(index, isTsumogiri);
                        EndTurn();
                        break;
                    }
                case GAMESTATE_EATED:
                    {
                        var eatedIndex = numEatedSets[turn] - 1;
                        var eatedTiles = eatedSetsTiles[turn][eatedIndex];
                        var eatedType = eatedSetsTypes[turn][eatedIndex];
                        var eatedDir = eatedSetsDirections[turn][eatedIndex];

                        bool reject = false;

                        switch (eatedType)
                        {
                            case EATED_SET_TYPE_PON:
                                if (NormalizeTileId(eatedTiles[0]) == normalizedTile)
                                {
                                    reject = true;
                                }
                                break;
                            case EATED_SET_TYPE_CHI_PREV:
                            case EATED_SET_TYPE_CHI_NEXT:
                                {
                                    int eatedTile1 = NormalizeTileId(eatedTiles[1]);
                                    int eatedTile2 = NormalizeTileId(eatedTiles[2]);

                                    if (normalizedTile == GetTilePrevNumber(eatedTile1) ||
                                        normalizedTile == GetTileNextNumber(eatedTile2))
                                    {
                                        reject = true;
                                    }
                                }
                                break;
                            case EATED_SET_TYPE_CHI_CENTER:
                                if (NormalizeTileId(eatedTiles[0]) == normalizedTile)
                                {
                                    reject = true;
                                }
                                break;
                        }

                        if (reject)
                        {
                            Serialize(false);
                            return;
                        }
                    }

                    DiscardTile(index, false);
                    EndTurn();
                    break;
                case GAMESTATE_SELECT_KAN:
                    {
                        if (CanSelfAnkanBy(index))
                        {
                            // Debug.Log("OnPickTile 1.1");

                            var eatedSetIndex = numEatedSets[turn];
                            eatedSetsDirections[turn][eatedSetIndex] = EATED_SET_DIRECTION_SELF;
                            eatedSetsTypes[turn][eatedSetIndex] = EATED_SET_TYPE_ANKAN;
                            eatedSetsTiles[turn][eatedSetIndex][0] = RemoveSameTile(turn, tile);
                            eatedSetsTiles[turn][eatedSetIndex][1] = RemoveSameTile(turn, tile);
                            eatedSetsTiles[turn][eatedSetIndex][2] = RemoveSameTile(turn, tile);
                            eatedSetsTiles[turn][eatedSetIndex][3] = RemoveSameTile(turn, tile);
                            ++numEatedSets[turn];

                            // Debug.Log("OnPickTile 1.2");

                            DrawTileFromDeadWall(true);
                            UpdateHandStateTsumo(true);
                            gameState = GAMESTATE_DRAWED;

                            // Debug.Log("OnPickTile 1.3");
                        }
                        else if (CanSelfMinkanBy(index))
                        {
                            int eatedSetIndex = 0;

                            for (; eatedSetIndex < numEatedSets[turn]; ++eatedSetIndex)
                            {
                                if (eatedSetsTypes[turn][eatedSetIndex] == EATED_SET_TYPE_PON)
                                {
                                    if (normalizedTile == NormalizeTileId(eatedSetsTiles[turn][eatedSetIndex][0]))
                                    {
                                        break;
                                    }
                                }
                            }

                            eatedSetsTypes[turn][eatedSetIndex] = EATED_SET_TYPE_KAKAN;
                            eatedSetsTiles[turn][eatedSetIndex][3] = RemoveSameTile(turn, tile);

                            lastKakanIndex = eatedSetIndex;

                            EndTurnKakan();
                        }
                        else
                        {
                            return;
                        }

                        break;
                    }
                case GAMESTATE_SELECT_REACH:
                    {
                        var isTsumogiri = index == numHandTiles[turn] - 1;
                        DiscardTile(index, isTsumogiri);
                        reachIndex[turn] = numDiscardedTiles[turn] - 1;
                        ippatsu[turn] = true;
                        doubleReach[turn] = canDoubleReach[turn];
                        EndTurn();
                        break;
                    }
            }

            // Debug.Log("OnPickTile 2");

            Serialize(false);

            // Debug.Log("OnPickTile 3");
        }

        private void DiscardTile(int index, bool isTsumogiri)
        {
            var tile = handTiles[turn][index];

            if (!IsTileYaochu(tile))
            {
                nagashimangan[turn] = false;
            }

            Remove(handTiles[turn], numHandTiles[turn], index, 1);
            --numHandTiles[turn];
            Sort(handTiles[turn], numHandTiles[turn]);

            discardedTiles[turn][numDiscardedTiles[turn]] = tile;
            discardedTilesTsumogiri[turn][numDiscardedTiles[turn]] = isTsumogiri;
            ++numDiscardedTiles[turn];

            if (reachIndex[turn] == -1)
            {
                overlook[turn] = false;
            }

            soundPlayer.OnDiscard(turn);
        }

        public void OnTsumoButton()
        {
            if (!Initialize()) { return; }

            soundPlayer.OnTsumo(turn);

            gameState = GAMESTATE_TSUMO;
            numDoraOpen = numKan;
            Serialize(false);
        }

        public void OnRonButton()
        {
            if (!Initialize()) { return; }

            soundPlayer.OnRon(eatTurn);

            gameState = GAMESTATE_RON;
            numDoraOpen = numKan;
            Serialize(false);
        }

        public void OnPonButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnPonButton 1");

            soundPlayer.OnPon(eatTurn);

            var tile = PopLastDiscardedTile();
            var index = numEatedSets[eatTurn];
            var dir = GetEatDirection();

            eatedSetsDirections[eatTurn][index] = GetEatDirection();
            eatedSetsTypes[eatTurn][index] = EATED_SET_TYPE_PON;
            eatedSetsTiles[eatTurn][index][0] = tile;
            eatedSetsTiles[eatTurn][index][1] = RemoveSameTile(eatTurn, tile);
            eatedSetsTiles[eatTurn][index][2] = RemoveSameTile(eatTurn, tile);
            ++numEatedSets[eatTurn];

            // Debug.Log("OnPonButton 2");

            OnEated(false);
            Serialize(false);

            // Debug.Log("OnPonButton 3");
        }

        public void OnKanButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnKanButton 1");

            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                    {
                        soundPlayer.OnKan(turn);

                        gameState = GAMESTATE_SELECT_KAN;
                        break;
                    }
                case GAMESTATE_CONFIRM_KAN:
                    {
                        soundPlayer.OnKan(eatTurn);

                        var tile = PopLastDiscardedTile();
                        var index = numEatedSets[eatTurn];
                        var dir = GetEatDirection();

                        eatedSetsDirections[eatTurn][index] = GetEatDirection();
                        eatedSetsTypes[eatTurn][index] = EATED_SET_TYPE_MINKAN;
                        eatedSetsTiles[eatTurn][index][0] = tile;
                        eatedSetsTiles[eatTurn][index][1] = RemoveSameTile(eatTurn, tile);
                        eatedSetsTiles[eatTurn][index][2] = RemoveSameTile(eatTurn, tile);
                        eatedSetsTiles[eatTurn][index][3] = RemoveSameTile(eatTurn, tile);
                        ++numEatedSets[eatTurn];

                        OnEated(true);
                        CheckKanDraw();
                        break;
                    }
            }

            // Debug.Log("OnKanButton 2");

            Serialize(false);

            // Debug.Log("OnKanButton 3");
        }

        private bool CheckKanDraw()
        {
            // Debug.Log("CheckKanDraw 1");

            if (numKan == 4)
            {
                var maxKan = 0;

                for (int i = 0; i < 4; ++i)
                {
                    var kan = 0;

                    for (int j = 0; j < numEatedSets[i]; ++j)
                    {
                        switch (eatedSetsTypes[i][j])
                        {
                            case EATED_SET_TYPE_ANKAN:
                            case EATED_SET_TYPE_MINKAN:
                            case EATED_SET_TYPE_KAKAN:
                                ++kan;
                                break;
                        }
                    }

                    maxKan = Mathf.Max(maxKan, kan);
                }

                if (maxKan != 4)
                {
                    gameState = GAMESTATE_ABORTIVE_DRAW_4KAN;
                    return true;
                }
            }

            // Debug.Log("CheckKanDraw 2");

            return false;
        }

        public void OnChiPrevButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnChiPrevButton 1");

            soundPlayer.OnChi(eatTurn);

            var tile = PopLastDiscardedTile();
            var index = numEatedSets[eatTurn];
            var dir = GetEatDirection();

            var prevTile = GetTilePrevNumber(tile);
            var prevPrevTile = GetTilePrevNumber(prevTile);

            eatedSetsDirections[eatTurn][index] = GetEatDirection();
            eatedSetsTypes[eatTurn][index] = EATED_SET_TYPE_CHI_PREV;
            eatedSetsTiles[eatTurn][index][0] = tile;
            eatedSetsTiles[eatTurn][index][1] = RemoveSameTile(eatTurn, prevPrevTile);
            eatedSetsTiles[eatTurn][index][2] = RemoveSameTile(eatTurn, prevTile);
            ++numEatedSets[eatTurn];

            // Debug.Log("OnChiPrevButton 2");

            OnEated(false);
            Serialize(false);

            // Debug.Log("OnChiPrevButton 3");
        }

        public void OnChiCenterButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnChiCenterButton 1");

            soundPlayer.OnChi(eatTurn);

            var tile = PopLastDiscardedTile();
            var index = numEatedSets[eatTurn];
            var dir = GetEatDirection();

            var prevTile = GetTilePrevNumber(tile);
            var nextTile = GetTileNextNumber(tile);

            eatedSetsDirections[eatTurn][index] = GetEatDirection();
            eatedSetsTypes[eatTurn][index] = EATED_SET_TYPE_CHI_CENTER;
            eatedSetsTiles[eatTurn][index][0] = tile;
            eatedSetsTiles[eatTurn][index][1] = RemoveSameTile(eatTurn, prevTile);
            eatedSetsTiles[eatTurn][index][2] = RemoveSameTile(eatTurn, nextTile);
            ++numEatedSets[eatTurn];

            // Debug.Log("OnChiCenterButton 2");

            OnEated(false);
            Serialize(false);

            // Debug.Log("OnChiCenterButton 3");
        }

        public void OnChiNextButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnChiNextButton 1");

            soundPlayer.OnChi(eatTurn);

            var tile = PopLastDiscardedTile();
            var index = numEatedSets[eatTurn];
            var dir = GetEatDirection();

            var nextTile = GetTileNextNumber(tile);
            var nextNextTile = GetTileNextNumber(nextTile);

            eatedSetsDirections[eatTurn][index] = GetEatDirection();
            eatedSetsTypes[eatTurn][index] = EATED_SET_TYPE_CHI_NEXT;
            eatedSetsTiles[eatTurn][index][0] = tile;
            eatedSetsTiles[eatTurn][index][1] = RemoveSameTile(eatTurn, nextTile);
            eatedSetsTiles[eatTurn][index][2] = RemoveSameTile(eatTurn, nextNextTile);
            ++numEatedSets[eatTurn];

            // Debug.Log("OnChiNextButton 2");

            OnEated(false);
            Serialize(false);

            // Debug.Log("OnChiNextButton 3");
        }

        private int GetEatDirection()
        {
            if ((eatTurn + 1) % 4 == turn)
            {
                return EATED_SET_DIRECTION_RIGHT;
            }
            else if ((eatTurn + 2) % 4 == turn)
            {
                return EATED_SET_DIRECTION_CENTER;
            }
            else if ((eatTurn + 3) % 4 == turn)
            {
                return EATED_SET_DIRECTION_LEFT;
            }
            else
            {
                return -1;
            }
        }

        private void OnEated(bool kan)
        {
            // Debug.Log("OnEated 1");

            nagashimangan[turn] = false;
            nagashimangan[eatTurn] = false;

            if (reached)
            {
                points[turn] -= 1000;
                kyotaku += 1000;
                reached = false;
            }

            turn = eatTurn;

            if (kan)
            {
                DrawTileFromDeadWall(false);
                UpdateHandStateTsumo(true);
                gameState = GAMESTATE_DRAWED;
            }
            else
            {
                handState = new int[0];
                gameState = GAMESTATE_EATED;
            }

            for (int i = 0; i < 4; ++i)
            {
                ippatsu[i] = false;
                canDoubleReach[i] = false;
            }

            // Debug.Log("OnEated 2");
        }

        public void OnReachButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnReachButton 1");

            soundPlayer.OnReach(turn);

            gameState = GAMESTATE_SELECT_REACH;
            reached = true;

            Serialize(false);

            // Debug.Log("OnReachButton 2");
        }

        public void OnNineButton()
        {
            if (!Initialize()) { return; }

            gameState = GAMESTATE_ABORTIVE_DRAW_NINE;
            Serialize(false);
        }

        public void OnNoButton()
        {
            if (!Initialize()) { return; }

            // Debug.Log("OnNoButton 1");

            switch (gameState)
            {
                case GAMESTATE_CONFIRM_RON:
                case GAMESTATE_CONFIRM_KAN:
                case GAMESTATE_CONFIRM_PON:
                case GAMESTATE_CONFIRM_CHI:
                    EndTurn();
                    break;
                case GAMESTATE_CONFIRM_RON_CHANKAN:
                    EndTurnKakan();
                    break;
            }

            Serialize(false);

            // Debug.Log("OnNoButton 2");
        }

        public void OnNakinashiOnButton0()
        {
            nakinashi[0] = true;
            RunAutomatedAction();
        }

        public void OnNakinashiOnButton1()
        {
            nakinashi[1] = true;
            RunAutomatedAction();
        }

        public void OnNakinashiOnButton2()
        {
            nakinashi[2] = true;
            RunAutomatedAction();
        }

        public void OnNakinashiOnButton3()
        {
            nakinashi[3] = true;
            RunAutomatedAction();
        }

        public void OnNakinashiOffButton0()
        {
            nakinashi[0] = false;
            RunAutomatedAction();
        }

        public void OnNakinashiOffButton1()
        {
            nakinashi[1] = false;
            RunAutomatedAction();
        }

        public void OnNakinashiOffButton2()
        {
            nakinashi[2] = false;
            RunAutomatedAction();
        }

        public void OnNakinashiOffButton3()
        {
            nakinashi[3] = false;
            RunAutomatedAction();
        }

        public void OnTsumogiriOnButton0()
        {
            tsumogiri[0] = true;
            RunAutomatedAction();
        }

        public void OnTsumogiriOnButton1()
        {
            tsumogiri[1] = true;
            RunAutomatedAction();
        }

        public void OnTsumogiriOnButton2()
        {
            tsumogiri[2] = true;
            RunAutomatedAction();
        }

        public void OnTsumogiriOnButton3()
        {
            tsumogiri[3] = true;
            RunAutomatedAction();
        }

        public void OnTsumogiriOffButton0()
        {
            tsumogiri[0] = false;
            RunAutomatedAction();
        }

        public void OnTsumogiriOffButton1()
        {
            tsumogiri[1] = false;
            RunAutomatedAction();
        }

        public void OnTsumogiriOffButton2()
        {
            tsumogiri[2] = false;
            RunAutomatedAction();
        }

        public void OnTsumogiriOffButton3()
        {
            tsumogiri[3] = false;
            RunAutomatedAction();
        }

        private void EndTurn()
        {
            if (!Initialize()) { return; }

            // Debug.Log("EndTurn 1");

            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ResetSelectIndex));

            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                case GAMESTATE_EATED:
                case GAMESTATE_SELECT_REACH:
                    gameState = GAMESTATE_CONFIRM_RON;
                    eatTurn = turn;
                    break;
            }

            if (gameState == GAMESTATE_CONFIRM_RON)
            {
                while (true)
                {
                    AdvanceEatTurn();

                    if (eatTurn == turn)
                    {
                        break;
                    }

                    var tile = GetLastDiscardedTile();
                    UpdateHandStateRon(tile, false);

                    if (CanEatRon())
                    {
                        return;
                    }
                }

                gameState = GAMESTATE_CONFIRM_KAN;
            }

            if (CheckKanDraw())
            {
                return;
            }

            numDoraOpen = numKan;

            if (reached)
            {
                points[turn] -= 1000;
                kyotaku += 1000;
                reached = false;
            }

            int numReach = 0;

            for (int i = 0; i < 4; ++i)
            {
                if (reachIndex[i] != -1)
                {
                    ++numReach;
                }
            }

            if (numReach == 4)
            {
                gameState = GAMESTATE_ABORTIVE_DRAW_4REACH;
                return;
            }

            // Debug.Log("EndTurn 2");

            if (gameState == GAMESTATE_CONFIRM_KAN)
            {
                while (true)
                {
                    AdvanceEatTurn();

                    if (eatTurn == turn)
                    {
                        break;
                    }

                    if (CanEatKan())
                    {
                        return;
                    }
                }

                gameState = GAMESTATE_CONFIRM_PON;
            }

            // Debug.Log("EndTurn 3");

            if (gameState == GAMESTATE_CONFIRM_PON)
            {
                while (true)
                {
                    AdvanceEatTurn();

                    if (eatTurn == turn)
                    {
                        break;
                    }

                    if (CanEatPon())
                    {
                        return;
                    }
                }

                gameState = GAMESTATE_CONFIRM_CHI;
            }

            // Debug.Log("EndTurn 4");

            if (gameState == GAMESTATE_CONFIRM_CHI)
            {
                AdvanceEatTurn();

                if (CanEatChi())
                {
                    return;
                }
            }

            {
                bool sufurenda = true;

                for (int i = 0; i < 4; ++i)
                {
                    if (numEatedSets[i] != 0 ||
                        numDiscardedTiles[i] != 1 ||
                        !IsTileKaze(discardedTiles[i][0]) ||
                        NormalizeTileId(discardedTiles[i][0]) != NormalizeTileId(discardedTiles[0][0]))
                    {
                        sufurenda = false;
                        break;
                    }
                }

                if (sufurenda)
                {
                    gameState = GAMESTATE_ABORTIVE_DRAW_SUFURENDA;
                    return;
                }
            }

            AdvanceTurn();

            // Debug.Log("EndTurn 5");

            if (DrawTileFromWall())
            {
                UpdateHandStateTsumo(false);
                gameState = GAMESTATE_DRAWED;
            }
            else
            {
                gameState = GAMESTATE_EXHAUSTIVE_DRAW;

                for (int i = 0; i < 4; ++i)
                {
                    var sortedHandTiles = NormalizeAndSort(handTiles[i], numHandTiles[i]);
                    var waiting = FindWaiting(sortedHandTiles, numHandTiles[i], numEatedSets[i]);
                    tenpai[i] = waiting.Length != 0;
                }
            }

            // Debug.Log("EndTurn 6");
        }

        private void EndTurnKakan()
        {
            // Debug.Log("EndTurnKakan 1");

            switch (gameState)
            {
                case GAMESTATE_SELECT_KAN:
                    gameState = GAMESTATE_CONFIRM_RON_CHANKAN;
                    eatTurn = turn;
                    break;
            }

            // Debug.Log("EndTurnKakan 2");

            var tile = eatedSetsTiles[turn][lastKakanIndex][3];

            if (gameState == GAMESTATE_CONFIRM_RON_CHANKAN)
            {
                while (true)
                {
                    AdvanceEatTurn();

                    if (eatTurn == turn)
                    {
                        break;
                    }

                    UpdateHandStateRon(tile, true);

                    if (CanEatRon())
                    {
                        return;
                    }
                }
            }

            lastKakanIndex = -1;

            for (int i = 0; i < 4; ++i)
            {
                ippatsu[i] = false;
            }

            // Debug.Log("EndTurnKakan 3");

            DrawTileFromDeadWall(false);
            UpdateHandStateTsumo(true);
            gameState = GAMESTATE_DRAWED;

            // Debug.Log("EndTurnKakan 4");
        }

        private void UpdateHandStateTsumo(bool rinshan)
        {
            // Debug.Log("UpdateHandState 1");

            var result = CheckHand(
                turn,
                handTiles[turn],
                numHandTiles[turn],
                eatedSetsTiles[turn],
                eatedSetsTypes[turn],
                numEatedSets[turn],
                discardedTiles[turn],
                numDiscardedTiles[turn],
                false,
                false,
                rinshan,
                overlook[turn]);

            handState = result;

            // Debug.Log("UpdateHandState 2");
        }

        private void UpdateHandStateRon(int tile, bool chankan)
        {
            // Debug.Log("UpdateHandStateRon 1 " + GetTileName(tile));

            var handTiles_ = (int[])handTiles[eatTurn].Clone();
            var num = numHandTiles[eatTurn];
            handTiles_[num++] = tile;

            handState = CheckHand(
                eatTurn,
                handTiles_,
                num,
                eatedSetsTiles[eatTurn],
                eatedSetsTypes[eatTurn],
                numEatedSets[eatTurn],
                discardedTiles[eatTurn],
                numDiscardedTiles[eatTurn],
                true,
                chankan,
                false,
                overlook[eatTurn]);

            switch (handState[0])
            {
                case HANDSTATE_COMPLETE:
                case HANDSTATE_COMPLETE_YAKUNASHI:
                case HANDSTATE_COMPLETE_FURITEN:
                    overlook[eatTurn] = true;
                    break;
            }

            // Debug.Log("UpdateHandStateRon 2");
        }

        private int GetNumEatedExceptAnkan(int turn_)
        {
            int result = 0;

            for (int i = 0; i < numEatedSets[turn_]; ++i)
            {
                if (eatedSetsTypes[turn_][i] != EATED_SET_TYPE_ANKAN)
                {
                    ++result;
                }
            }

            return result;
        }

        private int GetLastDiscardedTile()
        {
            // Debug.Log("GetLastDiscardedTile 1");

            var num = numDiscardedTiles[turn];

            if (num == 0)
            {
                return -1;
            }

            var ret = discardedTiles[turn][num - 1];

            // Debug.Log("GetLastDiscardedTile 2");

            return ret;
        }

        private int PopLastDiscardedTile()
        {
            // Debug.Log("PopLastDiscardedTile 1");

            var num = numDiscardedTiles[turn];

            if (num == 0)
            {
                return -1;
            }

            --numDiscardedTiles[turn];
            var ret = discardedTiles[turn][num - 1];

            // Debug.Log("PopLastDiscardedTile 2");

            return ret;
        }

        private void UpdateCanReach()
        {
            if (!canReachDirty)
            {
                return;
            }

            if (reachIndex[turn] == -1 &&
                GetNumEatedExceptAnkan(turn) == 0 &&
                GetWallRemain() >= 4)
            {
                // Debug.Log("UpdateHandState 3");

                bool tenpai = false;

                for (int i = 0; i < numHandTiles[turn]; ++i)
                {
                    waitingTilesBy[i] = CalculateWaitingTilesBy(i);

                    if (waitingTilesBy[i].Length != 0)
                    {
                        tenpai = true;
                    }
                }

                canReach = tenpai;
            }
            else
            {
                canReach = false;
            }

            canReachDirty = false;
        }

        private bool CanReach()
        {
            UpdateCanReach();
            return canReach;
        }

        private bool CanReachBy(int index)
        {
            if (!CanReach())
            {
                return false;
            }

            return waitingTilesBy[index].Length != 0;
        }

        private int[] CalculateWaitingTilesBy(int index)
        {
            if (GetNumEatedExceptAnkan(turn) != 0)
            {
                return new int[0];
            }

            var handTiles_ = (int[])handTiles[turn].Clone();
            var numHandTiles_ = numHandTiles[turn];
            var numEatedSets_ = numEatedSets[turn];

            Remove(handTiles_, numHandTiles_, index, 1);
            --numHandTiles_;

            var sortedHandTiles = NormalizeAndSort(handTiles_, numHandTiles_);
            var waiting = Unique(FindWaiting(sortedHandTiles, numHandTiles_, numEatedSets_));

            return waiting;
        }

        private bool CanNine()
        {
            if (numDiscardedTiles[turn] != 0)
            {
                return false;
            }

            for (int i = 0; i < 4; ++i)
            {
                if (numEatedSets[i] != 0)
                {
                    return false;
                }
            }

            var sortedHandTiles = NormalizeAndSort(handTiles[turn], numHandTiles[turn]);
            var numYaochuTypes = 0;

            for (int i = 0; i < sortedHandTiles.Length; ++i)
            {
                if (i != 0 && sortedHandTiles[i] == sortedHandTiles[i - 1])
                {
                    continue;
                }

                if (IsTileYaochu(sortedHandTiles[i]))
                {
                    ++numYaochuTypes;
                }
            }

            return numYaochuTypes >= 9;
        }

        private bool CanTsumo()
        {
            return handState.Length != 0 && handState[0] == HANDSTATE_COMPLETE;
        }

        private bool CanEatRon()
        {
            return handState.Length != 0 && handState[0] == HANDSTATE_COMPLETE;
        }

        private bool CanSelfKan()
        {
            return CanSelfAnkan() || CanSelfMinkan();
        }

        private bool CanSelfKanBy(int index)
        {
            return CanSelfAnkanBy(index) || CanSelfMinkanBy(index);
        }

        private bool CanSelfAnkan()
        {
            // Debug.Log("CanSelfAnkan 1");

            for (int i = 0; i < numHandTiles[turn]; ++i)
            {
                if (CanSelfAnkanBy(i))
                {
                    return true;
                }
            }

            // Debug.Log("CanSelfAnkan 3");

            return false;
        }

        private bool CanSelfAnkanBy(int index)
        {
            // Debug.Log("CanSelfAnkanBy 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            if (numKan == 4)
            {
                return false;
            }

            int count = 0;
            var tile = NormalizeTileId(handTiles[turn][index]);

            for (int i = 0; i < numHandTiles[turn]; ++i)
            {
                if (tile == NormalizeTileId(handTiles[turn][i]))
                {
                    ++count;
                }
            }

            // Debug.Log("CanSelfAnkanBy 2");

            if (count != 4)
            {
                return false;
            }

            // Debug.Log("CanSelfAnkanBy 3");

            if (reachIndex[turn] != -1)
            {
                if (tile != NormalizeTileId(handTiles[turn][numHandTiles[turn] - 1]))
                {
                    return false;
                }

                var handTiles_ = (int[])handTiles[turn].Clone();
                int numHandTiles_ = numHandTiles[turn] - 1;
                var numEatedSets_ = numEatedSets[turn];

                var orgSortedHandTiles = NormalizeAndSort(handTiles_, numHandTiles_);
                var orgWaiting = Unique(FindWaiting(orgSortedHandTiles, numHandTiles_, numEatedSets_));

                RemoveSameTileFrom(handTiles_, numHandTiles_, tile);
                --numHandTiles_;
                RemoveSameTileFrom(handTiles_, numHandTiles_, tile);
                --numHandTiles_;
                RemoveSameTileFrom(handTiles_, numHandTiles_, tile);
                --numHandTiles_;

                ++numEatedSets_;

                // Debug.Log("CanSelfAnkanBy 4");

                var newSortedHandTiles = NormalizeAndSort(handTiles_, numHandTiles_);
                var newWaiting = Unique(FindWaiting(newSortedHandTiles, numHandTiles_, numEatedSets_));

                // Debug.Log("CanSelfAnkanBy 5");

                if (orgWaiting.Length != newWaiting.Length)
                {
                    return false;
                }

                // Debug.Log("CanSelfAnkanBy 6");

                for (int i = 0; i < orgWaiting.Length; ++i)
                {
                    if (orgWaiting[i] != newWaiting[i])
                    {
                        return false;
                    }
                }
            }

            // Debug.Log("CanSelfAnkanBy 7");

            return true;
        }

        private bool CanSelfMinkan()
        {
            // Debug.Log("CanSelfMinkan 1");

            for (int i = 0; i < numHandTiles[turn]; ++i)
            {
                if (CanSelfMinkanBy(i))
                {
                    return true;
                }
            }

            // Debug.Log("CanSelfMinkan 2");

            return false;
        }

        private bool CanSelfMinkanBy(int index)
        {
            // Debug.Log("CanSelfMinkanBy 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            if (numKan == 4)
            {
                return false;
            }

            if (reachIndex[turn] != -1)
            {
                return false;
            }

            var tile = NormalizeTileId(handTiles[turn][index]);

            for (int j = 0; j < numEatedSets[turn]; ++j)
            {
                if (eatedSetsTypes[turn][j] == EATED_SET_TYPE_PON)
                {
                    if (tile == NormalizeTileId(eatedSetsTiles[turn][j][0]))
                    {
                        return true;
                    }
                }
            }

            // Debug.Log("CanSelfMinkanBy 2");

            return false;
        }

        private bool CanEatKan()
        {
            // Debug.Log("CanEatKan 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            if (numKan == 4)
            {
                return false;
            }

            if (reachIndex[eatTurn] != -1)
            {
                return false;
            }

            var tile = GetLastDiscardedTile();
            var ret = CountSameTileInHand(eatTurn, tile) == 3;

            // Debug.Log("CanEatKan 2");

            return ret;
        }

        private bool CanEatPon()
        {
            // Debug.Log("CanEatPon 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            if (reachIndex[eatTurn] != -1)
            {
                return false;
            }

            var tile = GetLastDiscardedTile();
            var ret = CountSameTileInHand(eatTurn, tile) >= 2;

            // Debug.Log("CanEatPon 2");

            return ret;
        }

        private int CountSameTileInHand(int player, int tile)
        {
            // Debug.Log("CountSameTileInHand 1");

            int count = 0;

            for (int i = 0; i < numHandTiles[player]; ++i)
            {
                if (NormalizeTileId(handTiles[player][i]) == NormalizeTileId(tile))
                {
                    ++count;
                }
            }

            // Debug.Log("CountSameTileInHand 2");

            return count;
        }

        private bool CanEatChi()
        {
            return CanEatChiPrev() || CanEatChiCenter() || CanEatChiNext();
        }

        private bool CanEatChiPrev()
        {
            // Debug.Log("CanEatChiPrev 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            if ((turn + 1) % 4 != eatTurn)
            {
                return false;
            }

            if (reachIndex[eatTurn] != -1)
            {
                return false;
            }

            var tile = GetLastDiscardedTile();
            var prevTile = GetTilePrevNumber(tile);
            var prevPrevTile = GetTilePrevNumber(prevTile);
            var ret = CountSameTileInHand(eatTurn, prevPrevTile) != 0 && CountSameTileInHand(eatTurn, prevTile) != 0;

            if (ret)
            {
                var prev3Tile = GetTilePrevNumber(prevPrevTile);
                ret = (numHandTiles[eatTurn] - CountSameTileInHand(eatTurn, tile) - CountSameTileInHand(eatTurn, prev3Tile)) > 2;
            }

            // Debug.Log("CanEatChiPrev 2");

            return ret;
        }

        private string GetChiPrevText()
        {
            var tile = GetLastDiscardedTile();
            var prevTile = GetTilePrevNumber(tile);
            var prevPrevTile = GetTilePrevNumber(prevTile);
            return GetTileNumber(prevPrevTile) + " " + GetTileNumber(prevTile) + " " + GetTileNumber(tile);
        }

        private bool CanEatChiCenter()
        {
            // Debug.Log("CanEatChiCenter 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            if ((turn + 1) % 4 != eatTurn)
            {
                return false;
            }

            if (reachIndex[eatTurn] != -1)
            {
                return false;
            }

            var tile = GetLastDiscardedTile();
            var prevTile = GetTilePrevNumber(tile);
            var nextTile = GetTileNextNumber(tile);
            var ret = CountSameTileInHand(eatTurn, prevTile) != 0 && CountSameTileInHand(eatTurn, nextTile) != 0;

            if (ret)
            {
                ret = (numHandTiles[eatTurn] - CountSameTileInHand(eatTurn, tile)) > 2;
            }

            // Debug.Log("CanEatChiCenter 2");

            return ret;
        }

        private string GetChiCenterText()
        {
            var tile = GetLastDiscardedTile();
            var prevTile = GetTilePrevNumber(tile);
            var nextTile = GetTileNextNumber(tile);
            return GetTileNumber(prevTile) + " " + GetTileNumber(tile) + " " + GetTileNumber(nextTile);
        }

        private bool CanEatChiNext()
        {
            // Debug.Log("CanEatChiNext 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            if ((turn + 1) % 4 != eatTurn)
            {
                return false;
            }

            if (reachIndex[eatTurn] != -1)
            {
                return false;
            }

            var tile = GetLastDiscardedTile();
            var nextTile = GetTileNextNumber(tile);
            var nextNextTile = GetTileNextNumber(nextTile);
            var ret = CountSameTileInHand(eatTurn, nextTile) != 0 && CountSameTileInHand(eatTurn, nextNextTile) != 0;

            if (ret)
            {
                var next3Tile = GetTileNextNumber(nextNextTile);
                ret = (numHandTiles[eatTurn] - CountSameTileInHand(eatTurn, tile) - CountSameTileInHand(eatTurn, next3Tile)) > 2;
            }

            // Debug.Log("CanEatChiNext 2");

            return ret;
        }

        private string GetChiNextText()
        {
            var tile = GetLastDiscardedTile();
            var nextTile = GetTileNextNumber(tile);
            var nextNextTile = GetTileNextNumber(nextTile);
            return GetTileNumber(tile) + " " + GetTileNumber(nextTile) + " " + GetTileNumber(nextNextTile);
        }

        private int RemoveSameTile(int player, int id)
        {
            var tile = RemoveSameTileFrom(handTiles[player], numHandTiles[player], id);
            --numHandTiles[player];

            return tile;
        }

        private int RemoveSameTileFrom(int[] handTiles_, int numHandTiles_, int id)
        {
            var index = -1;

            for (int i = 0; i < numHandTiles_; ++i)
            {
                if (NormalizeTileId(handTiles_[i]) == NormalizeTileId(id))
                {
                    index = i;
                }
            }

            if (index == -1)
            {
                return -1;
            }

            var tile = handTiles_[index];
            Remove(handTiles_, numHandTiles_, index, 1);

            return tile;
        }

        private void Remove(int[] a, int size, int index, int count)
        {
            for (int i = index; i < size - count; ++i)
            {
                a[i] = a[i + count];
            }
        }

        private void RandomizeFirstLeader()
        {
            firstLeader = Random.Range(0, 4);
            leader = firstLeader;
        }

        private void StartGame()
        {
            // Debug.Log("StartGame 1");

            wallTiles = GenerateTileIds(GENERATE_WALL_TILES);
            Shuffle(wallTiles);

            ResetGame();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ResetGameLocal));

            gameState = GAMESTATE_DRAWED;
            numWallTiles = wallTiles.Length;

            turn = leader;

            for (int i = 0; i < 13 * 4; ++i)
            {
                DrawTileFromWall();
                AdvanceTurn();
            }

            for (int i = 0; i < 4; ++i)
            {
                Sort(handTiles[i], numHandTiles[i]);
            }

            DrawTileFromWall();
            UpdateHandStateTsumo(false);

            // numEatedSets[0] = 4;

            // for (int i = 0; i < 4; ++i)
            // {
            //     eatedSetsTypes[0][i] = EATED_SET_TYPE_MINKAN;
            //     eatedSetsDirections[0][i] = EATED_SET_DIRECTION_LEFT;

            //     for (int j = 0; j < 4; ++j)
            //     {
            //         eatedSetsTiles[0][i][j] = wallTiles[numWallTiles - 1];
            //         wallTiles[numWallTiles - 1] = 0;
            //         --numWallTiles;
            //     }

            //     --numHandTiles[0];
            //     --numHandTiles[0];
            //     --numHandTiles[0];
            // }

            // Debug.Log("StartGame 2");
        }

        private int GetWallRemain()
        {
            return numWallTiles - numKan - 14;
        }

        private bool DrawTileFromWall()
        {
            // Debug.Log("DrawTileFromWall 1");

            if (GetWallRemain() == 0)
            {
                return false;
            }

            handTiles[turn][numHandTiles[turn]] = wallTiles[numWallTiles - 1];
            wallTiles[numWallTiles - 1] = 0;
            --numWallTiles;
            ++numHandTiles[turn];

            // Debug.Log("DrawTileFromWall 2");

            return true;
        }

        private void DrawTileFromDeadWall(bool ankan)
        {
            // Debug.Log("DrawTileFromWall 1");

            var index = numKan;

            if (index % 2 == 0)
            {
                ++index;
            }
            else
            {
                --index;
            }

            handTiles[turn][numHandTiles[turn]] = wallTiles[index];
            wallTiles[index] = 0;
            ++numKan;
            ++numHandTiles[turn];

            if (ankan)
            {
                numDoraOpen = numKan;
            }

            // Debug.Log("DrawTileFromWall 2");
        }

        private int[] CheckHand(
            int turn_,
            int[] handTiles_,
            int numHandTiles_,
            int[][] eatedSetsTiles_,
            int[] eatedSetsTypes_,
            int numEatedSets_,
            int[] discardedTiles_,
            int numDiscardedTiles_,
            bool eatLast,
            bool chankan,
            bool rinshan,
            bool overlook)
        {
            // Debug.Log("CheckHand 1");

            if (numHandTiles_ + numEatedSets_ * 3 != 14)
            {
                var result = new int[1];
                result[0] = HANDSTATE_INCOMPLETE;
                return result;
            }

            // Debug.Log("CheckHand 2");

            var lastHandTile = NormalizeTileId(handTiles_[numHandTiles_ - 1]);

            // Debug.Log("CheckHand 3");

            var sortedHandTiles = NormalizeAndSort(handTiles_, numHandTiles_);
            var elements = FindPattern(sortedHandTiles, numHandTiles_, numEatedSets_);

            if (elements == null)
            {
                var result = new int[1];
                result[0] = HANDSTATE_INCOMPLETE;
                return result;
            }
            else
            {
                // Debug.Log("CheckHand 4");

                if (eatLast)
                {
                    // Debug.Log("CheckHand FURITEN TEST 1");

                    if (overlook)
                    {
                        // Debug.Log("CheckHand FURITEN TEST 2");

                        var result = new int[1];
                        result[0] = HANDSTATE_COMPLETE_FURITEN;
                        return result;
                    }

                    // Debug.Log("CheckHand FURITEN TEST 3");

                    var discarded = GetDiscardedTilesIncludeEaten(turn_);

                    for (int i = 0; i < discarded.Length; ++i)
                    {
                        // Debug.Log("CheckHand FURITEN TEST 4 " + GetTileName(discarded[i]));

                        var ht = (int[])handTiles_.Clone();
                        ht[numHandTiles_ - 1] = discarded[i];

                        var sorted = NormalizeAndSort(ht, numHandTiles_);
                        var ele = FindPattern(sorted, numHandTiles_, numEatedSets_);

                        if (ele != null)
                        {
                            // Debug.Log("CheckHand FURITEN TEST 5");

                            var result = new int[1];
                            result[0] = HANDSTATE_COMPLETE_FURITEN;
                            return result;
                        }

                        // Debug.Log("CheckHand FURITEN TEST 6");
                    }

                    // Debug.Log("CheckHand FURITEN TEST 7");
                }

                var types = new int[14];
                var tiles = new int[14];
                int numTypesTiles = 0;

                for (int i = 0; i < elements.Length / 2; ++i)
                {
                    types[numTypesTiles] = elements[i * 2];
                    tiles[numTypesTiles] = elements[i * 2 + 1];
                    ++numTypesTiles;
                }

                // Debug.Log("CheckHand 5");

                for (int i = 0; i < numEatedSets_; ++i)
                {
                    int pattern = -1;
                    var minIndex = 0;

                    switch (eatedSetsTypes_[i])
                    {
                        case EATED_SET_TYPE_PON:
                            pattern = PATTERN_EATED_PON;
                            break;
                        case EATED_SET_TYPE_ANKAN:
                            pattern = PATTERN_EATED_ANKAN;
                            break;
                        case EATED_SET_TYPE_MINKAN:
                        case EATED_SET_TYPE_KAKAN:
                            pattern = PATTERN_EATED_MINKAN;
                            break;
                        case EATED_SET_TYPE_CHI_PREV:
                        case EATED_SET_TYPE_CHI_CENTER:
                        case EATED_SET_TYPE_CHI_NEXT:
                            pattern = PATTERN_EATED_CHI;

                            for (int j = 1; j < 3; ++j)
                            {
                                if (eatedSetsTiles_[i][j] < eatedSetsTiles_[i][minIndex])
                                {
                                    minIndex = j;
                                }
                            }

                            break;
                    }

                    types[numTypesTiles] = pattern;
                    tiles[numTypesTiles] = eatedSetsTiles_[i][minIndex];
                    ++numTypesTiles;
                }

                // Debug.Log("CheckHand 6");

                var combinedTiles = GetCombinedTiles(
                    handTiles_,
                    numHandTiles_,
                    eatedSetsTiles_,
                    eatedSetsTypes_,
                    numEatedSets_);

                var numEatedExceptAnkan = GetNumEatedExceptAnkan(turn_);
                int numHead = 0;
                int numHandThree = 0;
                int numThree = 0;
                int numFour = 0;
                int numSequence = 0;
                int numTypeM = 0;
                int numTypeP = 0;
                int numTypeS = 0;
                int numTypeZ = 0;
                int numYakuhaiThree = 0;
                int numRenpuhaiThree = 0;
                int sumNumEat = 0;
                int numFu = 0;

                if (numEatedExceptAnkan == 0 && eatLast)
                {
                    numFu += 30;
                }
                else
                {
                    numFu += 20;
                }

                for (int i = 0; i < numTypesTiles; ++i)
                {
                    switch (types[i])
                    {
                        case PATTERN_HEAD:
                            ++numHead;
                            break;
                        case PATTERN_THREE:
                            ++numHandThree;
                            ++numThree;
                            break;
                        case PATTERN_SEQUENCE:
                            ++numSequence;
                            break;
                        case PATTERN_EATED_PON:
                            ++numThree;
                            break;
                        case PATTERN_EATED_ANKAN:
                            ++numHandThree;
                            ++numThree;
                            ++numFour;
                            break;
                        case PATTERN_EATED_MINKAN:
                            ++numThree;
                            ++numFour;
                            break;
                        case PATTERN_EATED_CHI:
                            ++numSequence;
                            break;
                    }

                    switch (GetTileType(tiles[i]))
                    {
                        case TILE_TYPE_M:
                            ++numTypeM;
                            break;
                        case TILE_TYPE_P:
                            ++numTypeP;
                            break;
                        case TILE_TYPE_S:
                            ++numTypeS;
                            break;
                        case TILE_TYPE_Z:
                            ++numTypeZ;
                            break;
                    }

                    switch (types[i])
                    {
                        case PATTERN_THREE:
                        case PATTERN_EATED_PON:
                        case PATTERN_EATED_ANKAN:
                        case PATTERN_EATED_MINKAN:
                            if (IsTileYakuhai(tiles[i], turn_))
                            {
                                if (IsTileZikaze(tiles[i], turn_) && IsTileBakaze(tiles[i]))
                                {
                                    ++numRenpuhaiThree;
                                }
                                else
                                {
                                    ++numYakuhaiThree;
                                }
                            }
                            break;
                    }

                    switch (types[i])
                    {
                        case PATTERN_HEAD:
                            numFu += IsTileYakuhai(tiles[i], turn_) ? 2 : 0;
                            break;
                        case PATTERN_THREE:
                            if (eatLast && tiles[i] == lastHandTile)
                            {
                                numFu += IsTileYaochu(tiles[i]) ? 4 : 2;
                            }
                            else
                            {
                                numFu += IsTileYaochu(tiles[i]) ? 8 : 4;
                            }
                            break;
                        case PATTERN_SEQUENCE:
                            break;
                        case PATTERN_EATED_PON:
                            numFu += IsTileYaochu(tiles[i]) ? 4 : 2;
                            break;
                        case PATTERN_EATED_ANKAN:
                            numFu += IsTileYaochu(tiles[i]) ? 32 : 16;
                            break;
                        case PATTERN_EATED_MINKAN:
                            numFu += IsTileYaochu(tiles[i]) ? 16 : 8;
                            break;
                        case PATTERN_EATED_CHI:
                            break;
                    }
                }

                int numTypeExceptZ = 0;
                numTypeExceptZ += numTypeM != 0 ? 1 : 0;
                numTypeExceptZ += numTypeP != 0 ? 1 : 0;
                numTypeExceptZ += numTypeS != 0 ? 1 : 0;

                for (int i = 0; i < 4; ++i)
                {
                    sumNumEat += numEatedSets[i];
                }

                var waitType = GetWaitType(types, tiles, numTypesTiles, lastHandTile);

                switch (waitType)
                {
                    case WAIT_TYPE_RYANMEN:
                        numFu += eatLast ? 0 : 2;
                        break;
                    case WAIT_TYPE_KANCHAN:
                        numFu += eatLast ? 2 : 4;
                        break;
                    case WAIT_TYPE_PENCHAN:
                        numFu += eatLast ? 2 : 4;
                        break;
                    case WAIT_TYPE_SHANPON:
                        numFu += eatLast ? 0 : 2;
                        break;
                    case WAIT_TYPE_TANKI:
                        numFu += eatLast ? 2 : 4;
                        break;
                }

                if (eatLast)
                {
                    switch (waitType)
                    {
                        case WAIT_TYPE_SHANPON:
                            --numHandThree;
                            break;
                    }
                }

                // Debug.Log("CheckHand 7");

                int numYaochu = 0;
                int numYakuhai = 0;

                for (int i = 0; i < combinedTiles.Length; ++i)
                {
                    if (IsTileYaochu(combinedTiles[i]))
                    {
                        ++numYaochu;
                    }

                    if (IsTileYakuhai(combinedTiles[i], turn_))
                    {
                        ++numYakuhai;
                    }
                }

                // Debug.Log("CheckHand 8");

                var yaku = new int[100];
                var yakuValue = new int[100];
                int numYaku = 0;

                if (!eatLast && numEatedExceptAnkan == 0)
                {
                    yaku[numYaku] = YAKU_MENZEN;
                    yakuValue[numYaku] = 1;
                    ++numYaku;
                }

                if (reachIndex[turn_] != -1)
                {
                    if (doubleReach[turn_])
                    {
                        yaku[numYaku] = YAKU_DOUBLE_REACH;
                        yakuValue[numYaku] = 2;
                        ++numYaku;
                    }
                    else
                    {
                        yaku[numYaku] = YAKU_REACH;
                        yakuValue[numYaku] = 1;
                        ++numYaku;
                    }

                    if (ippatsu[turn_])
                    {
                        yaku[numYaku] = YAKU_IPPATSU;
                        yakuValue[numYaku] = 1;
                        ++numYaku;
                    }
                }

                if (numYaochu == 0)
                {
                    yaku[numYaku] = YAKU_TANYAO;
                    yakuValue[numYaku] = 1;
                    ++numYaku;
                }

                bool pinfu = false;

                if (numEatedExceptAnkan == 0 &&
                    numHead == 1 &&
                    numSequence == 4 &&
                    numYakuhai == 0 &&
                    IsWaitTypeRyanmen(types, tiles, numTypesTiles, lastHandTile))
                {
                    yaku[numYaku] = YAKU_PINFU;
                    yakuValue[numYaku] = 1;
                    ++numYaku;
                    pinfu = true;
                }

                if (numEatedExceptAnkan == 0)
                {
                    int numPehkoh = 0;

                    for (int i = 0; i < numTypesTiles - 1; ++i)
                    {
                        if (types[i] == PATTERN_SEQUENCE &&
                            types[i + 1] == PATTERN_SEQUENCE &&
                            tiles[i] == tiles[i + 1])
                        {
                            ++numPehkoh;
                            ++i;
                        }
                    }

                    switch (numPehkoh)
                    {
                        case 1:
                            yaku[numYaku] = YAKU_IHPEHKOH;
                            yakuValue[numYaku] = 1;
                            ++numYaku;
                            break;
                        case 2:
                            yaku[numYaku] = YAKU_RYANPEHKOU;
                            yakuValue[numYaku] = 3;
                            ++numYaku;
                            break;
                    }
                }

                if (numYakuhaiThree != 0)
                {
                    yaku[numYaku] = YAKU_YAKUHAI;
                    yakuValue[numYaku] = numYakuhaiThree;
                    ++numYaku;
                }

                if (numRenpuhaiThree != 0)
                {
                    yaku[numYaku] = YAKU_RENPUHAI;
                    yakuValue[numYaku] = numRenpuhaiThree * 2;
                    ++numYaku;
                }

                if (chankan)
                {
                    yaku[numYaku] = YAKU_CHANKAN;
                    yakuValue[numYaku] = 1;
                    ++numYaku;
                }

                if (rinshan)
                {
                    yaku[numYaku] = YAKU_RINSHANKAIHOU;
                    yakuValue[numYaku] = 1;
                    ++numYaku;
                }

                if (GetWallRemain() == 0 && !rinshan)
                {
                    yaku[numYaku] = eatLast ? YAKU_HOUTEIRAOYUI : YAKU_HAITEIRAOYUE;
                    yakuValue[numYaku] = 1;
                    ++numYaku;
                }

                bool chitoitsu = false;

                if (numHead == 7)
                {
                    yaku[numYaku] = YAKU_CHITOITSU;
                    yakuValue[numYaku] = 2;
                    ++numYaku;
                    chitoitsu = true;
                }

                if (numFour == 4)
                {
                    yaku[numYaku] = YAKU_SUKANTSU;
                    yakuValue[numYaku] = 13;
                    ++numYaku;
                }
                else if (numFour == 3)
                {
                    yaku[numYaku] = YAKU_SANKANTSU;
                    yakuValue[numYaku] = 2;
                    ++numYaku;
                }

                if (numHandThree == 4)
                {
                    bool tanki = false;

                    for (int i = 0; i < numTypesTiles; ++i)
                    {
                        if (types[i] == PATTERN_HEAD && tiles[i] == lastHandTile)
                        {
                            tanki = true;
                            break;
                        }
                    }

                    yaku[numYaku] = tanki ? YAKU_SUANKO_TANKI : YAKU_SUANKO;
                    yakuValue[numYaku] = tanki ? 26 : 13;
                    ++numYaku;
                }
                else
                {
                    if (numHandThree == 3)
                    {
                        yaku[numYaku] = YAKU_SANANKO;
                        yakuValue[numYaku] = 2;
                        ++numYaku;
                    }

                    if (numThree == 4)
                    {
                        yaku[numYaku] = YAKU_TOITOI;
                        yakuValue[numYaku] = 2;
                        ++numYaku;
                    }
                }

                for (int i = 1; i <= 9; ++i)
                {
                    bool existsM = false;
                    bool existsP = false;
                    bool existsS = false;

                    for (int j = 0; j < numTypesTiles; ++j)
                    {
                        switch (types[j])
                        {
                            case PATTERN_SEQUENCE:
                            case PATTERN_EATED_CHI:
                                if (GetTileNumber(tiles[j]) == i)
                                {
                                    switch (GetTileType(tiles[j]))
                                    {
                                        case TILE_TYPE_M:
                                            existsM = true;
                                            break;
                                        case TILE_TYPE_P:
                                            existsP = true;
                                            break;
                                        case TILE_TYPE_S:
                                            existsS = true;
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    if (existsM && existsP && existsS)
                    {
                        yaku[numYaku] = YAKU_SANSHOKUDOJUN;
                        yakuValue[numYaku] = numEatedExceptAnkan == 0 ? 2 : 1;
                        ++numYaku;
                    }
                }

                for (int i = 1; i <= 9; ++i)
                {
                    bool existsM = false;
                    bool existsP = false;
                    bool existsS = false;

                    for (int j = 0; j < numTypesTiles; ++j)
                    {
                        switch (types[j])
                        {
                            case PATTERN_THREE:
                            case PATTERN_EATED_PON:
                            case PATTERN_EATED_ANKAN:
                            case PATTERN_EATED_MINKAN:
                                if (GetTileNumber(tiles[j]) == i)
                                {
                                    switch (GetTileType(tiles[j]))
                                    {
                                        case TILE_TYPE_M:
                                            existsM = true;
                                            break;
                                        case TILE_TYPE_P:
                                            existsP = true;
                                            break;
                                        case TILE_TYPE_S:
                                            existsS = true;
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    if (existsM && existsP && existsS)
                    {
                        yaku[numYaku] = YAKU_SANSHOKUDOKO;
                        yakuValue[numYaku] = 2;
                        ++numYaku;
                    }
                }

                if (numTypesTiles == 13)
                {
                    bool wait13 = false;

                    for (int i = 0; i < numTypesTiles; ++i)
                    {
                        if (types[i] == PATTERN_HEAD)
                        {
                            wait13 = tiles[i] == lastHandTile;
                        }
                    }

                    yaku[numYaku] = wait13 ? YAKU_KOKUSHIMUSO13 : YAKU_KOKUSHIMUSO;
                    yakuValue[numYaku] = wait13 ? 26 : 13;
                    ++numYaku;
                }
                else if (numTypeExceptZ != 0)
                {
                    if (numYaochu == combinedTiles.Length)
                    {
                        if (numTypeZ == 0)
                        {
                            yaku[numYaku] = YAKU_CHINROUTOU;
                            yakuValue[numYaku] = 13;
                            ++numYaku;
                        }
                        else
                        {
                            yaku[numYaku] = YAKU_HONROUTOU;
                            yakuValue[numYaku] = 2;
                            ++numYaku;
                        }
                    }
                    else
                    {
                        bool fail = false;

                        for (int i = 0; i < numTypesTiles; ++i)
                        {
                            switch (types[i])
                            {
                                case PATTERN_HEAD:
                                case PATTERN_THREE:
                                case PATTERN_EATED_PON:
                                case PATTERN_EATED_ANKAN:
                                case PATTERN_EATED_MINKAN:
                                    if (!IsTileYaochu(tiles[i]))
                                    {
                                        fail = true;
                                    }
                                    break;
                                case PATTERN_SEQUENCE:
                                case PATTERN_EATED_CHI:
                                    if (!IsTileYaochu(tiles[i]) &&
                                        !IsTileYaochu(GetTileNextNumber(tiles[i])) &&
                                        !IsTileYaochu(GetTileNextNumber(GetTileNextNumber(tiles[i]))))
                                    {
                                        fail = true;
                                    }
                                    break;
                            }

                            if (fail)
                            {
                                break;
                            }
                        }

                        if (!fail)
                        {
                            if (numTypeZ == 0)
                            {
                                yaku[numYaku] = YAKU_JUNCHAN;
                                yakuValue[numYaku] = numEatedExceptAnkan == 0 ? 3 : 2;
                                ++numYaku;
                            }
                            else
                            {
                                yaku[numYaku] = YAKU_CHANTA;
                                yakuValue[numYaku] = numEatedExceptAnkan == 0 ? 2 : 1;
                                ++numYaku;
                            }
                        }
                    }
                }

                {
                    int numSangenThree = 0;
                    bool existsSangenHead = false;

                    for (int i = 0; i < numTypesTiles; ++i)
                    {
                        switch (types[i])
                        {
                            case PATTERN_HEAD:
                                if (IsTileSangen(tiles[i]))
                                {
                                    existsSangenHead = true;
                                }
                                break;
                            case PATTERN_THREE:
                            case PATTERN_EATED_PON:
                            case PATTERN_EATED_ANKAN:
                            case PATTERN_EATED_MINKAN:
                                if (IsTileSangen(tiles[i]))
                                {
                                    ++numSangenThree;
                                }
                                break;
                        }
                    }

                    if (numSangenThree == 3)
                    {
                        yaku[numYaku] = YAKU_DAISANGEN;
                        yakuValue[numYaku] = 13;
                        ++numYaku;
                    }
                    else if (numSangenThree == 2 && existsSangenHead)
                    {
                        yaku[numYaku] = YAKU_SHOSANGEN;
                        yakuValue[numYaku] = 2;
                        ++numYaku;
                    }
                }

                {
                    int numKazeThree = 0;
                    bool existsKazeHead = false;

                    for (int i = 0; i < numTypesTiles; ++i)
                    {
                        switch (types[i])
                        {
                            case PATTERN_HEAD:
                                if (IsTileKaze(tiles[i]))
                                {
                                    existsKazeHead = true;
                                }
                                break;
                            case PATTERN_THREE:
                            case PATTERN_EATED_PON:
                            case PATTERN_EATED_ANKAN:
                            case PATTERN_EATED_MINKAN:
                                if (IsTileKaze(tiles[i]))
                                {
                                    ++numKazeThree;
                                }
                                break;
                        }
                    }

                    if (numKazeThree == 4)
                    {
                        yaku[numYaku] = YAKU_DAISUSHI;
                        yakuValue[numYaku] = 26;
                        ++numYaku;
                    }
                    else if (numKazeThree == 3 && existsKazeHead)
                    {
                        yaku[numYaku] = YAKU_SHOSUSHI;
                        yakuValue[numYaku] = 13;
                        ++numYaku;
                    }
                }

                for (int i = TILE_TYPE_M; i <= TILE_TYPE_S; ++i)
                {
                    bool exists123 = false;
                    bool exists456 = false;
                    bool exists789 = false;

                    for (int j = 0; j < numTypesTiles; ++j)
                    {
                        if (GetTileType(tiles[j]) != i)
                        {
                            continue;
                        }

                        switch (types[j])
                        {
                            case PATTERN_SEQUENCE:
                            case PATTERN_EATED_CHI:
                                switch (GetTileNumber(tiles[j]))
                                {
                                    case 1:
                                        exists123 = true;
                                        break;
                                    case 4:
                                        exists456 = true;
                                        break;
                                    case 7:
                                        exists789 = true;
                                        break;
                                }
                                break;
                        }
                    }

                    if (exists123 && exists456 && exists789)
                    {
                        yaku[numYaku] = YAKU_IKKITSUKAN;
                        yakuValue[numYaku] = numEatedExceptAnkan == 0 ? 2 : 1;
                        ++numYaku;
                    }
                }

                switch (numTypeExceptZ)
                {
                    case 0:
                        yaku[numYaku] = YAKU_TSUISO;
                        yakuValue[numYaku] = 13;
                        ++numYaku;
                        break;
                    case 1:
                        if (numTypeZ == 0)
                        {
                            yaku[numYaku] = YAKU_CHINIHSOU;
                            yakuValue[numYaku] = numEatedExceptAnkan == 0 ? 6 : 5;
                            ++numYaku;
                        }
                        else
                        {
                            yaku[numYaku] = YAKU_HONIHSOU;
                            yakuValue[numYaku] = numEatedExceptAnkan == 0 ? 3 : 2;
                            ++numYaku;
                        }

                        break;
                }

                if (numDiscardedTiles_ == 0 && sumNumEat == 0)
                {
                    if (turn_ == leader)
                    {
                        yaku[numYaku] = YAKU_TENHO;
                        yakuValue[numYaku] = 13;
                        ++numYaku;
                    }
                    else if (eatLast)
                    {
                        yaku[numYaku] = YAKU_RENHO;
                        yakuValue[numYaku] = 13;
                        ++numYaku;
                    }
                    else
                    {
                        yaku[numYaku] = YAKU_CHIHO;
                        yakuValue[numYaku] = 13;
                        ++numYaku;
                    }
                }

                {
                    int numExceptGreen = 0;

                    for (int i = 0; i < combinedTiles.Length; ++i)
                    {
                        if (!IsTileGreen(combinedTiles[i]))
                        {
                            ++numExceptGreen;
                        }
                    }

                    if (numExceptGreen == 0)
                    {
                        yaku[numYaku] = YAKU_RYUISO;
                        yakuValue[numYaku] = 13;
                        ++numYaku;
                    }
                }

                if (numTypeZ == 0 && numTypeExceptZ == 1 && numEatedExceptAnkan == 0)
                {
                    var nums = new int[10];

                    for (int i = 0; i < combinedTiles.Length; ++i)
                    {
                        var n = GetTileNumber(combinedTiles[i]);
                        ++nums[n];
                    }

                    if (nums[1] >= 3 && nums[9] >= 3)
                    {
                        bool fail = false;

                        for (int i = 2; i <= 8; ++i)
                        {
                            if (nums[i] != 1 && nums[i] != 2)
                            {
                                fail = true;
                                break;
                            }
                        }

                        if (!fail)
                        {
                            bool junsei = nums[GetTileNumber(lastHandTile)] == 2;

                            yaku[numYaku] = YAKU_CHURENPOHTOU;
                            yakuValue[numYaku] = junsei ? 26 : 13;
                            ++numYaku;
                        }
                    }
                }

                {
                    var doras = GetDoras();
                    int numDora = 0;

                    for (int i = 0; i < combinedTiles.Length; ++i)
                    {
                        for (int j = 0; j < doras.Length; ++j)
                        {
                            if (NormalizeTileId(combinedTiles[i]) == doras[j])
                            {
                                ++numDora;
                            }
                        }
                    }

                    if (numDora != 0)
                    {
                        yaku[numYaku] = YAKU_DORA;
                        yakuValue[numYaku] = numDora;
                        ++numYaku;
                    }
                }

                if (reachIndex[turn_] != -1)
                {
                    var doras = GetUraDoras();
                    int numDora = 0;

                    for (int i = 0; i < combinedTiles.Length; ++i)
                    {
                        for (int j = 0; j < doras.Length; ++j)
                        {
                            if (NormalizeTileId(combinedTiles[i]) == doras[j])
                            {
                                ++numDora;
                            }
                        }
                    }

                    if (numDora != 0)
                    {
                        yaku[numYaku] = YAKU_URA_DORA;
                        yakuValue[numYaku] = numDora;
                        ++numYaku;
                    }
                }

                {
                    int numAkadora = 0;

                    for (int i = 0; i < combinedTiles.Length; ++i)
                    {
                        if (GetTileRed(combinedTiles[i]))
                        {
                            ++numAkadora;
                        }
                    }

                    if (numAkadora != 0)
                    {
                        yaku[numYaku] = YAKU_AKADORA;
                        yakuValue[numYaku] = numAkadora;
                        ++numYaku;
                    }
                }

                if (pinfu)
                {
                    numFu = eatLast ? 30 : 20;
                }
                else if (chitoitsu)
                {
                    numFu = 25;
                }
                else if (numFu < 30)
                {
                    numFu = 30;
                }
                else if (numFu % 10 != 0)
                {
                    numFu = (numFu / 10 + 1) * 10;
                }

                int totalYakuValue = 0;
                int totalYakuValueWithoutDora = 0;

                for (int i = 0; i < numYaku; ++i)
                {
                    totalYakuValue += yakuValue[i];

                    switch (yaku[i])
                    {
                        case YAKU_DORA:
                        case YAKU_URA_DORA:
                        case YAKU_AKADORA:
                            break;
                        default:
                            totalYakuValueWithoutDora += yakuValue[i];
                            break;
                    }
                }

                var numPoint = CalculatePoint(totalYakuValue, numFu, turn_);

                // Debug.Log("CheckHand 9 " + totalYakuValue + " " + numFu + " " + numPoint);

                {
                    var result = new int[7 + numYaku * 2];
                    int resultDst = 0;
                    result[resultDst++] = totalYakuValueWithoutDora == 0 ? HANDSTATE_COMPLETE_YAKUNASHI : HANDSTATE_COMPLETE;
                    result[resultDst++] = numYaku;
                    result[resultDst++] = numFu;
                    result[resultDst++] = (int)((uint)numPoint & 0xff);
                    result[resultDst++] = (int)(((uint)numPoint >> 8) & 0xff);
                    result[resultDst++] = (int)(((uint)numPoint >> 16) & 0xff);
                    result[resultDst++] = (int)(((uint)numPoint >> 24) & 0xff);

                    for (int i = 0; i < numYaku; ++i)
                    {
                        result[resultDst++] = yaku[i];
                        result[resultDst++] = yakuValue[i];
                    }

                    // Debug.Log("CheckHand 10");

                    return result;
                }
            }
        }

        private int[] GetDiscardedTilesIncludeEaten(int turn_)
        {
            var eatedTiles = new int[12];
            int numEatedTiles = 0;

            var rightTurn = (turn_ + 1) % 4;
            var centerTurn = (turn_ + 2) % 4;
            var leftTurn = (turn_ + 3) % 4;

            for (int j = 0; j < numEatedSets[rightTurn]; ++j)
            {
                var tile = GetEatedTile(rightTurn, j, EATED_SET_DIRECTION_LEFT);

                if (tile != -1)
                {
                    eatedTiles[numEatedTiles++] = tile;
                }
            }

            for (int j = 0; j < numEatedSets[centerTurn]; ++j)
            {
                var tile = GetEatedTile(centerTurn, j, EATED_SET_DIRECTION_CENTER);

                if (tile != -1)
                {
                    eatedTiles[numEatedTiles++] = tile;
                }
            }

            for (int j = 0; j < numEatedSets[leftTurn]; ++j)
            {
                var tile = GetEatedTile(leftTurn, j, EATED_SET_DIRECTION_RIGHT);

                if (tile != -1)
                {
                    eatedTiles[numEatedTiles++] = tile;
                }
            }

            var result = new int[numDiscardedTiles[turn_] + numEatedTiles];
            int dst = 0;

            for (int i = 0; i < numDiscardedTiles[turn_]; ++i)
            {
                result[dst++] = discardedTiles[turn_][i];
            }

            for (int i = 0; i < numEatedTiles; ++i)
            {
                result[dst++] = eatedTiles[i];
            }

            return result;
        }

        private int GetEatedTile(int turn_, int index, int direction)
        {
            if (index < numEatedSets[turn_] &&
                    eatedSetsDirections[turn_][index] == direction)
            {
                return eatedSetsTiles[turn_][index][0];
            }

            return -1;
        }

        private int[] GetDoras()
        {
            var result = new int[1 + numDoraOpen];

            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = NormalizeTileId(GetTileNextNumberLoop(wallTiles[5 + i * 2]));
            }

            return result;
        }

        private int[] GetUraDoras()
        {
            var result = new int[1 + numDoraOpen];

            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = NormalizeTileId(GetTileNextNumberLoop(wallTiles[4 + i * 2]));
            }

            return result;
        }

        private int GetWaitType(int[] types, int[] tiles, int numTypesTiles, int lastHandTile)
        {
            if (IsWaitTypeKanchan(types, tiles, numTypesTiles, lastHandTile))
            {
                return WAIT_TYPE_KANCHAN;
            }
            else if (IsWaitTypePenchan(types, tiles, numTypesTiles, lastHandTile))
            {
                return WAIT_TYPE_PENCHAN;
            }
            else if (IsWaitTypeTanki(types, tiles, numTypesTiles, lastHandTile))
            {
                return WAIT_TYPE_TANKI;
            }
            else if (IsWaitTypeRyanmen(types, tiles, numTypesTiles, lastHandTile))
            {
                return WAIT_TYPE_RYANMEN;
            }
            else if (IsWaitTypeShanpon(types, tiles, numTypesTiles, lastHandTile))
            {
                return WAIT_TYPE_SHANPON;
            }

            return -1;
        }

        private bool IsWaitTypeKanchan(int[] types, int[] tiles, int numTypesTiles, int lastHandTile)
        {
            for (int i = 0; i < numTypesTiles; ++i)
            {
                if (types[i] == PATTERN_SEQUENCE)
                {
                    if (lastHandTile == GetTileNextNumber(tiles[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsWaitTypePenchan(int[] types, int[] tiles, int numTypesTiles, int lastHandTile)
        {
            for (int i = 0; i < numTypesTiles; ++i)
            {
                if (types[i] == PATTERN_SEQUENCE)
                {
                    if (GetTileNumber(tiles[i]) == 7 &&
                        lastHandTile == tiles[i])
                    {
                        return true;
                    }

                    if (GetTileNumber(tiles[i]) == 1 &&
                        lastHandTile == GetTileNextNumber(GetTileNextNumber(tiles[i])))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsWaitTypeTanki(int[] types, int[] tiles, int numTypesTiles, int lastHandTile)
        {
            for (int i = 0; i < numTypesTiles; ++i)
            {
                if (types[i] == PATTERN_HEAD)
                {
                    if (lastHandTile == tiles[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsWaitTypeRyanmen(int[] types, int[] tiles, int numTypesTiles, int lastHandTile)
        {
            for (int i = 0; i < numTypesTiles; ++i)
            {
                if (types[i] == PATTERN_SEQUENCE)
                {
                    if (GetTileNumber(tiles[i]) != 7 &&
                        lastHandTile == tiles[i])
                    {
                        return true;
                    }

                    if (GetTileNumber(tiles[i]) != 1 &&
                        lastHandTile == GetTileNextNumber(GetTileNextNumber(tiles[i])))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsWaitTypeShanpon(int[] types, int[] tiles, int numTypesTiles, int lastHandTile)
        {
            for (int i = 0; i < numTypesTiles; ++i)
            {
                if (types[i] == PATTERN_THREE)
                {
                    if (lastHandTile == tiles[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int CalculatePoint(int numYaku, int numFu, int turn_)
        {
            int numPoint = 0;

            if (numYaku >= 13)
            {
                numPoint = (numYaku / 13) * 32000;
            }
            else if (numYaku >= 11)
            {
                numPoint = 24000;
            }
            else if (numYaku >= 8)
            {
                numPoint = 16000;
            }
            else if (numYaku >= 6)
            {
                numPoint = 12000;
            }
            else if (numYaku == 5)
            {
                numPoint = 8000;
            }
            else if (true)
            {
                numPoint = Mathf.Min(8000, numFu * 4 * (1 << (numYaku + 2)));
            }

            if (turn_ == leader)
            {
                numPoint *= 3;
                numPoint /= 2;
            }

            numPoint = Ceil100(numPoint);

            return numPoint;
        }

        private int Ceil100(int x)
        {
            if (x % 100 != 0)
            {
                x = (x / 100 + 1) * 100;
            }

            return x;
        }

        private bool IsTileSangen(int tile)
        {
            switch (GetTileType(tile))
            {
                case TILE_TYPE_Z:
                    switch (GetTileNumber(tile))
                    {
                        case TILE_NUMBER_HAKU:
                        case TILE_NUMBER_HATSU:
                        case TILE_NUMBER_CHUN:
                            return true;
                    }
                    break;
            }

            return false;
        }

        private bool IsTileYakuhai(int tile, int turn_)
        {
            if (IsTileZikaze(tile, turn_))
            {
                return true;
            }

            if (IsTileBakaze(tile))
            {
                return true;
            }

            switch (GetTileType(tile))
            {
                case TILE_TYPE_Z:
                    switch (GetTileNumber(tile))
                    {
                        case TILE_NUMBER_HAKU:
                        case TILE_NUMBER_HATSU:
                        case TILE_NUMBER_CHUN:
                            return true;
                    }
                    break;
            }

            return false;
        }

        private bool IsTileKaze(int id)
        {
            switch (GetTileType(id))
            {
                case TILE_TYPE_Z:
                    switch (GetTileNumber(id))
                    {
                        case TILE_NUMBER_TON:
                        case TILE_NUMBER_NAN:
                        case TILE_NUMBER_SHA:
                        case TILE_NUMBER_PEY:
                            return true;
                    }
                    break;
            }

            return false;
        }

        private bool IsTileYaochu(int id)
        {
            switch (GetTileType(id))
            {
                case TILE_TYPE_Z:
                    return true;
                default:
                    switch (GetTileNumber(id))
                    {
                        case 1:
                        case 9:
                            return true;
                    }
                    break;
            }

            return false;
        }

        private bool IsTileZikaze(int tile, int turn_)
        {
            int myKaze = (4 + turn_ - leader) % 4;

            switch (GetTileType(tile))
            {
                case TILE_TYPE_Z:
                    switch (GetTileNumber(tile))
                    {
                        case TILE_NUMBER_TON:
                            return myKaze == 0;
                        case TILE_NUMBER_NAN:
                            return myKaze == 1;
                        case TILE_NUMBER_SHA:
                            return myKaze == 2;
                        case TILE_NUMBER_PEY:
                            return myKaze == 3;
                    }
                    break;
            }

            return false;
        }

        private bool IsTileBakaze(int tile)
        {
            switch (GetTileType(tile))
            {
                case TILE_TYPE_Z:
                    switch (GetTileNumber(tile))
                    {
                        case TILE_NUMBER_TON:
                            return round == 0;
                        case TILE_NUMBER_NAN:
                            return round == 1;
                        case TILE_NUMBER_SHA:
                            return round == 2;
                        case TILE_NUMBER_PEY:
                            return round == 3;
                    }
                    break;
            }

            return false;
        }

        private bool IsTileGreen(int tile)
        {
            switch (GetTileType(tile))
            {
                case TILE_TYPE_S:
                    switch (GetTileNumber(tile))
                    {
                        case 2:
                        case 3:
                        case 4:
                        case 6:
                        case 8:
                            return true;
                    }
                    break;
                case TILE_TYPE_Z:
                    switch (GetTileNumber(tile))
                    {
                        case TILE_NUMBER_HATSU:
                            return true;
                    }
                    break;
            }

            return false;
        }

        private int GetTileNumDora(int tile)
        {
            int result = 0;

            if (GetTileRed(tile))
            {
                ++result;
            }

            for (int i = 0; i < 1 + numDoraOpen; ++i)
            {
                if (NormalizeTileId(tile) == NormalizeTileId(GetTileNextNumberLoop(wallTiles[5 + i * 2])))
                {
                    ++result;
                }
            }

            return result;
        }

        private int[] NormalizeAndSort(int[] handTiles_, int numHandTiles_)
        {
            var result = (int[])handTiles_.Clone();

            for (int i = 0; i < numHandTiles_; ++i)
            {
                result[i] = NormalizeTileId(handTiles_[i]);
            }

            Sort(result, numHandTiles_);

            return result;
        }

        private int[] GetCombinedTiles(
            int[] handTiles_,
            int numHandTiles_,
            int[][] eatedSetsTiles_,
            int[] eatedSetsTypes_,
            int numEatedSets_)
        {
            // Debug.Log("GetCombinedTiles 1");

            var combinedTiles = new int[18];
            int numCombinedTiles = 0;

            for (int i = 0; i < numHandTiles_; ++i)
            {
                combinedTiles[numCombinedTiles++] = handTiles_[i];
            }

            for (int i = 0; i < numEatedSets_; ++i)
            {
                var num = GetNumTilesEated(eatedSetsTypes_[i]);

                for (int j = 0; j < num; ++j)
                {
                    combinedTiles[numCombinedTiles++] = eatedSetsTiles_[i][j];
                }
            }

            var result = new int[numCombinedTiles];

            for (int i = 0; i < numCombinedTiles; ++i)
            {
                result[i] = combinedTiles[i];
            }

            // Debug.Log("GetCombinedTiles 2");

            return result;
        }

        private int[] FindPattern(int[] handTiles_, int numHandTiles_, int numEatedSets_)
        {
            // Debug.Log("FindPattern 1");

            if (numHandTiles_ + numEatedSets_ * 3 != 14)
            {
                return null;
            }

            var ids = new int[numHandTiles_];
            var counts = new int[numHandTiles_];
            int numIdsCounts = BuildIdsCounts(handTiles_, numHandTiles_, ids, counts);

            // Debug.Log("FindPattern 2");

            var patterns = validPatternsWin[numEatedSets_];

            // Debug.Log("FindPattern 3");

            for (int patternIndex = 0; patternIndex < patterns.Length; ++patternIndex)
            {
                var pattern = patterns[patternIndex];

                var cloneCounts = (int[])counts.Clone();
                var result = new int[pattern.Length * 2];
                int dst = 0;
                bool fail = false;
                int firstIndex = 0;

                for (int i = 0; i < pattern.Length; ++i)
                {
                    while (cloneCounts[firstIndex] == 0)
                    {
                        ++firstIndex;
                    }

                    switch (pattern[i])
                    {
                        case PATTERN_HEAD:
                            fail = !ConsumeHead(ids, cloneCounts, numIdsCounts, firstIndex);
                            break;
                        case PATTERN_THREE:
                            fail = !ConsumeThree(ids, cloneCounts, numIdsCounts, firstIndex);
                            break;
                        case PATTERN_SEQUENCE:
                            fail = !ConsumeSequence(ids, cloneCounts, numIdsCounts, firstIndex);
                            break;
                    }

                    if (fail)
                    {
                        break;
                    }

                    result[dst++] = pattern[i];
                    result[dst++] = ids[firstIndex];
                }

                if (!fail)
                {
                    return result;
                }
            }

            // Debug.Log("FindPattern 4");

            switch (numIdsCounts)
            {
                case 7:
                    if (IsChitoitsu(ids, counts, numIdsCounts))
                    {
                        var result = new int[7 * 2];
                        int dst = 0;

                        for (int i = 0; i < numIdsCounts; ++i)
                        {
                            result[dst++] = PATTERN_HEAD;
                            result[dst++] = ids[i];
                        }

                        return result;
                    }
                    break;
                case 13:
                    if (IsKokushimuso(ids, counts, numIdsCounts))
                    {
                        var result = new int[13 * 2];
                        int dst = 0;

                        for (int i = 0; i < numIdsCounts; ++i)
                        {
                            result[dst++] = counts[i] == 1 ? PATTERN_TANKI : PATTERN_HEAD;
                            result[dst++] = ids[i];
                        }

                        return result;
                    }
                    break;
            }

            // Debug.Log("FindPattern 5");

            return null;
        }

        private string Dump(int[] x, int len = -1)
        {
            string str = "";

            if (len == -1)
            {
                len = x.Length;
            }

            for (int i = 0; i < len; ++i)
            {
                str += x[i];
                str += ",";
            }

            return str;
        }

        private int[] FindWaiting(int[] handTiles_, int numHandTiles_, int numEatedSets_)
        {
            // Debug.Log("FindWaiting 1 " + Dump(handTiles_, numHandTiles_) + "  " + numEatedSets_);

            if (numHandTiles_ + numEatedSets_ * 3 != 13)
            {
                return new int[0];
            }

            var ids = new int[numHandTiles_];
            var counts = new int[numHandTiles_];
            int numIdsCounts = BuildIdsCounts(handTiles_, numHandTiles_, ids, counts);

            // Debug.Log("FindWaiting 2 " + Dump(ids) + " " + Dump(counts) + " " + numIdsCounts);

            var patterns = validPatternsWait[numEatedSets_];

            // Debug.Log("FindWaiting 3");

            var waitingTiles = new int[2 * patterns.Length];
            int numWaitingTiles = 0;

            for (int patternIndex = 0; patternIndex < patterns.Length; ++patternIndex)
            {
                var pattern = patterns[patternIndex];

                var cloneCounts = (int[])counts.Clone();
                int waitingTile1 = -1;
                int waitingTile2 = -1;
                bool fail = false;
                int firstIndex = 0;

                for (int i = 0; i < pattern.Length; ++i)
                {
                    while (cloneCounts[firstIndex] == 0)
                    {
                        ++firstIndex;
                    }

                    var number = GetTileNumber(ids[firstIndex]);

                    switch (pattern[i])
                    {
                        case PATTERN_HEAD:
                            fail = !ConsumeHead(ids, cloneCounts, numIdsCounts, firstIndex);
                            break;
                        case PATTERN_THREE:
                            fail = !ConsumeThree(ids, cloneCounts, numIdsCounts, firstIndex);
                            break;
                        case PATTERN_SEQUENCE:
                            fail = !ConsumeSequence(ids, cloneCounts, numIdsCounts, firstIndex);
                            break;
                        case PATTERN_INCOMPLETE_HEAD:
                            if (!ConsumeIncompleteHead(ids, cloneCounts, numIdsCounts, firstIndex))
                            {
                                fail = true;
                                break;
                            }

                            waitingTile1 = ids[firstIndex];
                            break;
                        case PATTERN_INCOMPLETE_THREE:
                            if (!ConsumeIncompleteThree(ids, cloneCounts, numIdsCounts, firstIndex))
                            {
                                fail = true;
                                break;
                            }

                            waitingTile1 = ids[firstIndex];
                            break;
                        case PATTERN_INCOMPLETE_SEQUENCE:
                            if (!ConsumeIncompleteSequence(ids, cloneCounts, numIdsCounts, firstIndex))
                            {
                                fail = true;
                                break;
                            }

                            if (number != 1)
                            {
                                waitingTile1 = GetTileChangeNumber(ids[firstIndex], number - 1);
                            }

                            if (number != 8)
                            {
                                waitingTile2 = GetTileChangeNumber(ids[firstIndex], number + 2);
                            }
                            break;
                        case PATTERN_INCOMPLETE_SEQUENCE_SPLIT:
                            if (!ConsumeIncompleteSequenceSplit(ids, cloneCounts, numIdsCounts, firstIndex))
                            {
                                fail = true;
                                break;
                            }

                            waitingTile1 = GetTileChangeNumber(ids[firstIndex], number + 1);
                            break;
                    }

                    if (fail)
                    {
                        break;
                    }
                }

                if (!fail)
                {
                    if (waitingTile1 != -1)
                    {
                        waitingTiles[numWaitingTiles++] = waitingTile1;
                    }

                    if (waitingTile2 != -1)
                    {
                        waitingTiles[numWaitingTiles++] = waitingTile2;
                    }
                }
            }

            // Debug.Log("FindWaiting 4 " + Dump(waitingTiles, numWaitingTiles));

            switch (numIdsCounts)
            {
                case 7:
                    {
                        int numHead = 0;
                        int numTanki = 0;
                        int waitingTile = -1;

                        for (int i = 0; i < numIdsCounts; ++i)
                        {
                            switch (counts[i])
                            {
                                case 1:
                                    ++numTanki;
                                    waitingTile = ids[i];
                                    break;
                                case 2:
                                    ++numHead;
                                    break;
                            }
                        }

                        if (numHead == 6 && numTanki == 1)
                        {
                            waitingTiles[numWaitingTiles++] = waitingTile;
                        }
                    }
                    break;
                case 12:
                case 13:
                    {
                        var countKokushi = CountKokushi(ids, counts, numIdsCounts);

                        if (countKokushi != null)
                        {
                            int numEmpty = 0;
                            int emptyTile = -1;

                            for (int i = 0; i < countKokushi.Length; ++i)
                            {
                                if (countKokushi[i] == 0)
                                {
                                    ++numEmpty;
                                    emptyTile = GetYaochuTiles()[i];
                                }
                            }

                            switch (numEmpty)
                            {
                                case 0:
                                    for (int i = 0; i < GetYaochuTiles().Length; i++)
                                    {
                                        waitingTiles[numWaitingTiles++] = GetYaochuTiles()[i];
                                    }
                                    break;
                                case 1:
                                    waitingTiles[numWaitingTiles++] = emptyTile;
                                    break;
                            }
                        }

                        break;
                    }
            }

            // Debug.Log("FindWaiting 6 " + Dump(waitingTiles, numWaitingTiles));

            var result = new int[numWaitingTiles];

            for (int i = 0; i < numWaitingTiles; ++i)
            {
                result[i] = waitingTiles[i];
            }

            // Debug.Log("FindWaiting 7 " + Dump(result));

            return result;
        }

        private double CalculateCpuScoreBy(int index, int[] numRemainTiles)
        {
            var handTiles_ = (int[])handTiles[turn].Clone();
            var numHandTiles_ = numHandTiles[turn];
            var numEatedSets_ = numEatedSets[turn];
            var tileLastDiscard = NormalizeTileId(handTiles_[index]);

            Remove(handTiles_, numHandTiles_, index, 1);
            --numHandTiles_;

            var score = CalculateCpuScore(turn, handTiles_, numHandTiles_, numEatedSets_, numRemainTiles, tileLastDiscard);

            return score;
        }

        private const double UNCERTAINTY_RATIO = 0.5;
        private const double YAKU_MULTIPLIER = 1.5;

        private double CalculateCpuScore(int turn_, int[] handTiles_, int numHandTiles_, int numEatedSets_, int[] remainTiles, int tileLastDiscard)
        {
            // Debug.Log("CalculateCpuScore 1");

            if (numHandTiles_ + numEatedSets_ * 3 != 13)
            {
                return 0.0;
            }

            var sortedHandTiles = NormalizeAndSort(handTiles_, numHandTiles_);

            var ids = new int[numHandTiles_];
            var counts = new int[numHandTiles_];
            int numIdsCounts = BuildIdsCounts(sortedHandTiles, numHandTiles_, ids, counts);

            // Debug.Log("CalculateCpuScore 2");

            var patterns = validPatternsWin[numEatedSets_];

            // Debug.Log("CalculateCpuScore 3");

            var maxProbability = 0.0;

            for (int patternIndex = 0; patternIndex < patterns.Length; ++patternIndex)
            {
                var pattern = patterns[patternIndex];

                var cloneCounts = (int[])counts.Clone();
                int firstIndex = 0;

                var cloneRemainTiles = (int[])remainTiles.Clone();
                cloneRemainTiles[tileLastDiscard] = 0;

                double probability = 1.0;

                for (int i = 0; i < pattern.Length; ++i)
                {
                    while (cloneCounts[firstIndex] == 0)
                    {
                        ++firstIndex;
                    }

                    var id = ids[firstIndex];
                    double prob = 1.0;

                    switch (pattern[i])
                    {
                        case PATTERN_HEAD:
                            if (cloneCounts[firstIndex] < 2)
                            {
                                if (cloneRemainTiles[id] >= 1)
                                {
                                    prob = CalculateProbability(cloneRemainTiles[id], 1) * UNCERTAINTY_RATIO;
                                    --cloneRemainTiles[id];
                                    --cloneCounts[firstIndex];
                                }
                                else
                                {
                                    prob = 0.0;
                                }
                            }
                            else
                            {
                                cloneCounts[firstIndex] -= 2;
                            }
                            break;
                        case PATTERN_THREE:
                            if (cloneCounts[firstIndex] < 2)
                            {
                                if (cloneRemainTiles[id] >= 2)
                                {
                                    prob = CalculateProbability(cloneRemainTiles[id], 2) * UNCERTAINTY_RATIO;
                                    cloneRemainTiles[id] -= 2;
                                    --cloneCounts[firstIndex];
                                }
                                else
                                {
                                    prob = 0.0;
                                }
                            }
                            else if (cloneCounts[firstIndex] < 3)
                            {
                                if (cloneRemainTiles[id] >= 1)
                                {
                                    prob = CalculateProbability(cloneRemainTiles[id], 1) * UNCERTAINTY_RATIO;
                                    --cloneRemainTiles[id];
                                    cloneCounts[firstIndex] -= 2;
                                }
                                else
                                {
                                    prob = 0.0;
                                }
                            }
                            else
                            {
                                cloneCounts[firstIndex] -= 3;
                            }
                            break;
                        case PATTERN_SEQUENCE:
                            if (GetTileType(id) == TILE_TYPE_Z)
                            {
                                prob = 0.0;
                            }
                            else
                            {
                                --cloneCounts[firstIndex];

                                double probPrevPrev = 0.0;
                                double probPrev = 0.0;
                                double probNext = 0.0;
                                double probNextNext = 0.0;

                                var num = GetTileNumber(id);

                                if (num >= 3)
                                {
                                    var idPrevPrev = GetTileChangeNumber(id, num - 2);

                                    if (cloneRemainTiles[idPrevPrev] >= 1)
                                    {
                                        probPrevPrev = CalculateProbability(cloneRemainTiles[idPrevPrev], 1) * UNCERTAINTY_RATIO;
                                        --cloneRemainTiles[idPrevPrev];
                                    }
                                }

                                if (num >= 2)
                                {
                                    var idPrev = GetTileChangeNumber(id, num - 1);

                                    if (cloneRemainTiles[idPrev] >= 1)
                                    {
                                        probPrev = CalculateProbability(cloneRemainTiles[idPrev], 1) * UNCERTAINTY_RATIO;
                                        --cloneRemainTiles[idPrev];
                                    }
                                }

                                if (num <= 8)
                                {
                                    if (firstIndex < numIdsCounts - 1 &&
                                        ids[firstIndex + 1] == GetTileNextNumber(id) &&
                                        cloneCounts[firstIndex + 1] != 0)
                                    {
                                        probNext = 1.0;
                                        --cloneCounts[firstIndex + 1];
                                    }
                                    else
                                    {
                                        var idNext = GetTileChangeNumber(id, num + 1);

                                        if (cloneRemainTiles[idNext] >= 1)
                                        {
                                            probNext = CalculateProbability(cloneRemainTiles[idNext], 1) * UNCERTAINTY_RATIO;
                                            --cloneRemainTiles[idNext];
                                        }
                                    }
                                }

                                if (num <= 7)
                                {
                                    if (firstIndex < numIdsCounts - 1 &&
                                        ids[firstIndex + 1] == GetTileChangeNumber(id, num + 2) &&
                                        cloneCounts[firstIndex + 1] != 0)
                                    {
                                        probNextNext = 1.0;
                                        --cloneCounts[firstIndex + 1];
                                    }
                                    else if (firstIndex < numIdsCounts - 2 &&
                                        ids[firstIndex + 2] == GetTileChangeNumber(id, num + 2) &&
                                        cloneCounts[firstIndex + 2] != 0)
                                    {
                                        probNextNext = 1.0;
                                        --cloneCounts[firstIndex + 2];
                                    }
                                    else
                                    {
                                        var idNextNext = GetTileChangeNumber(id, num + 2);

                                        if (cloneRemainTiles[idNextNext] >= 1)
                                        {
                                            probNextNext = CalculateProbability(cloneRemainTiles[idNextNext], 1) * UNCERTAINTY_RATIO;
                                            --cloneRemainTiles[idNextNext];
                                        }
                                    }
                                }

                                prob = (probPrevPrev * probPrev) + (probPrev * probNext) + (probNext * probNextNext);
                            }
                            break;
                    }

                    if (prob < 0.01)
                    {
                        prob = 0.01;
                    }

                    probability *= prob;

                    if (probability < maxProbability)
                    {
                        break;
                    }
                }

                if (maxProbability < probability)
                {
                    maxProbability = probability;
                }
            }

            for (int i = 0; i < numHandTiles_; ++i)
            {
                var n = GetTileNumDora(handTiles_[i]);

                for (int j = 0; j < n; ++j)
                {
                    maxProbability *= 1.5;
                }
            }

            for (int i = 0; i < numIdsCounts; ++i)
            {
                var tile = ids[i];
                int n = 0;

                if (IsTileYakuhai(tile, turn_))
                {
                    if (IsTileZikaze(tile, turn_) && IsTileBakaze(tile))
                    {
                        n = 2;
                    }
                    else
                    {
                        n = 1;
                    }
                }

                if (counts[i] >= 3)
                {
                    maxProbability *= Mathf.Pow((float)YAKU_MULTIPLIER, n);
                }
                else if (counts[i] >= 2)
                {
                    maxProbability *= Mathf.Pow((float)YAKU_MULTIPLIER, n / 2.0f);
                }
            }

            // Debug.Log("CalculateCpuScore 4");

            return maxProbability;
        }

        private int BuildIdsCounts(int[] handTiles_, int numHandTiles_, int[] ids, int[] counts)
        {
            int numIdsCounts = 0;

            for (int i = 0; i < numHandTiles_; ++i)
            {
                if (numIdsCounts != 0 && ids[numIdsCounts - 1] == handTiles_[i])
                {
                    ++counts[numIdsCounts - 1];
                }
                else
                {
                    ids[numIdsCounts] = handTiles_[i];
                    counts[numIdsCounts] = 1;
                    ++numIdsCounts;
                }
            }

            return numIdsCounts;
        }

        private bool IsChitoitsu(int[] ids, int[] counts, int numIdsCounts)
        {
            for (int i = 0; i < numIdsCounts; ++i)
            {
                if (counts[i] != 2)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsKokushimuso(int[] ids, int[] counts, int numIdsCounts)
        {
            var countKokushi = CountKokushi(ids, counts, numIdsCounts);

            if (countKokushi == null)
            {
                return false;
            }

            for (int i = 0; i < countKokushi.Length; ++i)
            {
                if (countKokushi[i] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private int[] CountKokushi(int[] ids, int[] counts, int numIdsCounts)
        {
            int[] yaochuCounts = new int[YAOCHU_TYPES];

            for (int i = 0; i < numIdsCounts; ++i)
            {
                switch (GetTileType(ids[i]))
                {
                    case TILE_TYPE_M:
                        switch (GetTileNumber(ids[i]))
                        {
                            case 1: yaochuCounts[YAOCHU_M1] = counts[i]; break;
                            case 9: yaochuCounts[YAOCHU_M9] = counts[i]; break;
                            default: return null;
                        }
                        break;
                    case TILE_TYPE_P:
                        switch (GetTileNumber(ids[i]))
                        {
                            case 1: yaochuCounts[YAOCHU_P1] = counts[i]; break;
                            case 9: yaochuCounts[YAOCHU_P9] = counts[i]; break;
                            default: return null;
                        }
                        break;
                    case TILE_TYPE_S:
                        switch (GetTileNumber(ids[i]))
                        {
                            case 1: yaochuCounts[YAOCHU_S1] = counts[i]; break;
                            case 9: yaochuCounts[YAOCHU_S9] = counts[i]; break;
                            default: return null;
                        }
                        break;
                    case TILE_TYPE_Z:
                        switch (GetTileNumber(ids[i]))
                        {
                            case TILE_NUMBER_TON: yaochuCounts[YAOCHU_TON] = counts[i]; break;
                            case TILE_NUMBER_NAN: yaochuCounts[YAOCHU_NAN] = counts[i]; break;
                            case TILE_NUMBER_SHA: yaochuCounts[YAOCHU_SHA] = counts[i]; break;
                            case TILE_NUMBER_PEY: yaochuCounts[YAOCHU_PEY] = counts[i]; break;
                            case TILE_NUMBER_HAKU: yaochuCounts[YAOCHU_HAKU] = counts[i]; break;
                            case TILE_NUMBER_HATSU: yaochuCounts[YAOCHU_HATSU] = counts[i]; break;
                            case TILE_NUMBER_CHUN: yaochuCounts[YAOCHU_CHUN] = counts[i]; break;
                            default: return null;
                        }
                        break;
                }
            }

            return yaochuCounts;
        }

        private bool ConsumeHead(int[] ids, int[] counts, int numIdsCounts, int firstIndex)
        {
            if (counts[firstIndex] < 2)
            {
                return false;
            }

            counts[firstIndex] -= 2;
            return true;
        }

        private bool ConsumeThree(int[] ids, int[] counts, int numIdsCounts, int firstIndex)
        {
            if (counts[firstIndex] < 3)
            {
                return false;
            }

            counts[firstIndex] -= 3;
            return true;
        }

        private bool ConsumeSequence(int[] ids, int[] counts, int numIdsCounts, int firstIndex)
        {
            if (firstIndex >= numIdsCounts - 2)
            {
                return false;
            }

            if (ids[firstIndex + 1] != GetTileNextNumber(ids[firstIndex]) ||
                counts[firstIndex + 1] == 0)
            {
                return false;
            }

            if (ids[firstIndex + 2] != GetTileNextNumber(ids[firstIndex + 1]) ||
                counts[firstIndex + 2] == 0)
            {
                return false;
            }

            --counts[firstIndex];
            --counts[firstIndex + 1];
            --counts[firstIndex + 2];
            return true;
        }

        private bool ConsumeIncompleteHead(int[] ids, int[] counts, int numIdsCounts, int firstIndex)
        {
            if (counts[firstIndex] < 1)
            {
                return false;
            }

            counts[firstIndex] -= 1;
            return true;
        }

        private bool ConsumeIncompleteThree(int[] ids, int[] counts, int numIdsCounts, int firstIndex)
        {
            if (counts[firstIndex] < 2)
            {
                return false;
            }

            counts[firstIndex] -= 2;
            return true;
        }

        private bool ConsumeIncompleteSequence(int[] ids, int[] counts, int numIdsCounts, int firstIndex)
        {
            if (firstIndex >= numIdsCounts - 1)
            {
                return false;
            }

            if (ids[firstIndex + 1] != GetTileNextNumber(ids[firstIndex]) ||
                counts[firstIndex + 1] == 0)
            {
                return false;
            }

            --counts[firstIndex];
            --counts[firstIndex + 1];
            return true;
        }

        private bool ConsumeIncompleteSequenceSplit(int[] ids, int[] counts, int numIdsCounts, int firstIndex)
        {
            if (firstIndex >= numIdsCounts - 1)
            {
                return false;
            }

            if (ids[firstIndex + 1] == GetTileNextNumber(GetTileNextNumber(ids[firstIndex])) &&
                counts[firstIndex + 1] != 0)
            {
                --counts[firstIndex];
                --counts[firstIndex + 1];
                return true;
            }

            if (firstIndex >= numIdsCounts - 2)
            {
                return false;
            }

            if (ids[firstIndex + 2] == GetTileNextNumber(GetTileNextNumber(ids[firstIndex])) &&
                counts[firstIndex + 2] != 0)
            {
                --counts[firstIndex];
                --counts[firstIndex + 2];
                return true;
            }

            return false;
        }

        private void AdvanceTurn()
        {
            turn = (turn + 1) % 4;
        }

        private void AdvanceEatTurn()
        {
            eatTurn = (eatTurn + 1) % 4;
        }

        private int GetNumTilesEated(int type)
        {
            switch (type)
            {
                case EATED_SET_TYPE_PON:
                case EATED_SET_TYPE_CHI_PREV:
                case EATED_SET_TYPE_CHI_CENTER:
                case EATED_SET_TYPE_CHI_NEXT:
                    return 3;
                case EATED_SET_TYPE_ANKAN:
                case EATED_SET_TYPE_MINKAN:
                case EATED_SET_TYPE_KAKAN:
                    return 4;
            }

            return 0;
        }

        private void Sort(int[] a, int size)
        {
            if (size == 0)
            {
                return;
            }

            int[] stackLeft = new int[size];
            int[] stackRight = new int[size];
            int count = 0;

            stackLeft[0] = 0;
            stackRight[0] = size - 1;
            ++count;

            while (count != 0)
            {
                var left = stackLeft[count - 1];
                var right = stackRight[count - 1];
                --count;

                if (left < right)
                {
                    var i = left;
                    var j = right;

                    var pivot = Med3(a[i], a[i + (j - i) / 2], a[j]);

                    while (true)
                    {
                        while (a[i] < pivot)
                        {
                            ++i;
                        }

                        while (pivot < a[j])
                        {
                            --j;
                        }

                        if (i >= j)
                        {
                            break;
                        }

                        var temp = a[i];
                        a[i] = a[j];
                        a[j] = temp;

                        ++i;
                        --j;
                    }

                    stackLeft[count] = left;
                    stackRight[count] = i - 1;
                    ++count;

                    stackLeft[count] = j + 1;
                    stackRight[count] = right;
                    ++count;
                }
            }
        }

        private int Med3(int x, int y, int z)
        {
            if (x < y)
            {
                if (y < z)
                {
                    return y;
                }
                else if (z < x)
                {
                    return x;
                }
                else
                {
                    return z;
                }
            }
            else
            {
                if (z < y)
                {
                    return y;
                }
                else if (x < z)
                {
                    return x;
                }
                else
                {
                    return z;
                }
            }
        }

        private int NormalizeTileId(int id)
        {
            return GetTileId(0, GetTileType(id), GetTileNumber(id), false);
        }

        private int GetTileNextNumber(int id)
        {
            var type = GetTileType(id);

            if (type == TILE_TYPE_Z)
            {
                return -1;
            }

            var num = GetTileNumber(id);

            if (num == 9)
            {
                return -1;
            }

            return GetTileId(GetTileIndex(id), type, num + 1, false);
        }

        private int GetTileNextNumberLoop(int id)
        {
            var type = GetTileType(id);

            switch (type)
            {
                case TILE_TYPE_Z:
                    switch (GetTileNumber(id))
                    {
                        case TILE_NUMBER_TON:
                            return GetTileChangeNumber(id, TILE_NUMBER_NAN);
                        case TILE_NUMBER_NAN:
                            return GetTileChangeNumber(id, TILE_NUMBER_SHA);
                        case TILE_NUMBER_SHA:
                            return GetTileChangeNumber(id, TILE_NUMBER_PEY);
                        case TILE_NUMBER_PEY:
                            return GetTileChangeNumber(id, TILE_NUMBER_TON);
                        case TILE_NUMBER_HAKU:
                            return GetTileChangeNumber(id, TILE_NUMBER_HATSU);
                        case TILE_NUMBER_HATSU:
                            return GetTileChangeNumber(id, TILE_NUMBER_CHUN);
                        case TILE_NUMBER_CHUN:
                            return GetTileChangeNumber(id, TILE_NUMBER_HAKU);
                    }
                    break;
                default:
                    {
                        var num = GetTileNumber(id);

                        if (num == 9)
                        {
                            return GetTileChangeNumber(id, 1);
                        }
                        else
                        {
                            return GetTileChangeNumber(id, num + 1);
                        }
                    }
            }

            return -1;
        }

        private int GetTilePrevNumber(int id)
        {
            var type = GetTileType(id);

            if (type == TILE_TYPE_Z)
            {
                return -1;
            }

            var num = GetTileNumber(id);

            if (num == 1)
            {
                return -1;
            }

            return GetTileId(GetTileIndex(id), type, num - 1, false);
        }

        private int GetTileChangeNumber(int id, int number)
        {
            return GetTileId(GetTileIndex(id), GetTileType(id), number, false);
        }

        private int GetTileId(int index, int type, int number, bool red)
        {
            if (number >= 6 || red)
            {
                ++number;
            }

            return (type << 6) | (number << 2) | index;
        }

        private int GetTileType(int id)
        {
            return id >> 6;
        }

        private int GetTileNumber(int id)
        {
            int num = (id >> 2) & 0x0f;

            if (num >= 6)
            {
                --num;
            }

            return num;
        }

        private int GetTileIndex(int id)
        {
            return id & 0x03;
        }

        private bool GetTileRed(int id)
        {
            if (GetTileType(id) == TILE_TYPE_Z)
            {
                return false;
            }

            int num = (id >> 2) & 0x0f;
            return num == 6;
        }

        private Transform GetTile(string baseName, int id)
        {
            var index = GetTileIndex(id);

            var tilesName = baseName + index;
            var tiles = tilesSet.Find(tilesName);

            if (tiles == null)
            {
                Debug.LogError(tilesName + " not found");
            }

            string name = GetTileName(id);

            var tile = tiles.Find(name);

            if (tile == null)
            {
                Debug.LogError(name + " not found");
            }

            return tile;
        }

        private string GetTileName(int id)
        {
            var index = GetTileIndex(id);
            var type = GetTileType(id);
            var number = GetTileNumber(id);
            var red = GetTileRed(id);

            if (type == TILE_TYPE_Z)
            {
                switch (number)
                {
                    case TILE_NUMBER_TON: return "Ton";
                    case TILE_NUMBER_NAN: return "Nan";
                    case TILE_NUMBER_SHA: return "Sha";
                    case TILE_NUMBER_PEY: return "Pey";
                    case TILE_NUMBER_HAKU: return "Haku";
                    case TILE_NUMBER_HATSU: return "Hatsu";
                    case TILE_NUMBER_CHUN: return "Chun";
                }
            }
            else
            {
                string name = "";

                switch (type)
                {
                    case TILE_TYPE_M: name = "M"; break;
                    case TILE_TYPE_P: name = "P"; break;
                    case TILE_TYPE_S: name = "S"; break;
                }

                name += number.ToString();

                if (red)
                {
                    name += "R";
                }

                return name;
            }

            return "UNKNOWN";
        }

        private int[] GenerateTileIds(int mode)
        {
            // Debug.Log("GenerateTileIds 1");

            int numIds = 0;

            switch (mode)
            {
                case GENERATE_WALL_TILES:
                    numIds = NUM_ALL_WALL_TILES;
                    break;
                case GENERATE_INCLUDE_NO_USE:
                    numIds = NUM_ALL_TILES;
                    break;
                case GENERATE_NORMALIZED:
                    numIds = NUM_NORMALIZED_TILES;
                    break;
            }

            int[] ids = new int[numIds];
            int dst = 0;

            int numIndex = mode == GENERATE_NORMALIZED ? 1 : 4;

            for (int index = 0; index < numIndex; ++index)
            {
                for (int type = TILE_TYPE_M; type <= TILE_TYPE_Z; ++type)
                {
                    if (type == TILE_TYPE_Z)
                    {
                        for (int number = TILE_NUMBER_TON; number <= TILE_NUMBER_CHUN; ++number)
                        {
                            ids[dst++] = GetTileId(index, type, number, false);
                        }
                    }
                    else
                    {
                        if (mode == GENERATE_INCLUDE_NO_USE)
                        {
                            for (int number = 1; number <= 9; ++number)
                            {
                                ids[dst++] = GetTileId(index, type, number, false);
                            }

                            ids[dst++] = GetTileId(index, type, 5, true);
                        }
                        else
                        {
                            int numRed = 0;

                            switch (type)
                            {
                                case TILE_TYPE_M: numRed = numRedM; break;
                                case TILE_TYPE_P: numRed = numRedP; break;
                                case TILE_TYPE_S: numRed = numRedS; break;
                            }

                            for (int number = 1; number <= 9; ++number)
                            {
                                bool red = false;

                                if (mode != GENERATE_NORMALIZED)
                                {
                                    red = number == 5 && index < numRed;
                                }

                                ids[dst++] = GetTileId(index, type, number, red);
                            }
                        }
                    }
                }
            }

            // Debug.Log("GenerateTileIds 2");

            return ids;
        }

        private void Serialize(bool resetLocal)
        {
            // Debug.Log("Serialize 1");

            var data = new byte[1000];
            int dst = 0;

            int version = 9;

            data[dst++] = (byte)version;
            data[dst++] = (byte)gameState;
            data[dst++] = (byte)numWallTiles;
            data[dst++] = (byte)round;
            data[dst++] = (byte)honba;
            data[dst++] = (byte)firstLeader;
            data[dst++] = (byte)leader;
            data[dst++] = (byte)turn;
            data[dst++] = (byte)eatTurn;
            data[dst++] = (byte)ConvertNegativeToFF(lastKakanIndex);
            data[dst++] = (byte)numKan;
            data[dst++] = (byte)numDoraOpen;
            data[dst++] = reached ? (byte)1 : (byte)0;

            // Debug.Log("Serialize 2");

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = entry[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = nextReady[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = entryCpu[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                var point = Mathf.Abs(points[i]);
                data[dst++] = points[i] < 0 ? (byte)1 : (byte)0;
                data[dst++] = (byte)((uint)point & 0xff);
                data[dst++] = (byte)(((uint)point >> 8) & 0xff);
                data[dst++] = (byte)(((uint)point >> 16) & 0xff);
                data[dst++] = (byte)(((uint)point >> 24) & 0xff);
            }

            {
                data[dst++] = (byte)((uint)kyotaku & 0xff);
                data[dst++] = (byte)(((uint)kyotaku >> 8) & 0xff);
                data[dst++] = (byte)(((uint)kyotaku >> 16) & 0xff);
                data[dst++] = (byte)(((uint)kyotaku >> 24) & 0xff);
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = overlook[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = (byte)ConvertNegativeToFF(reachIndex[i]);
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = ippatsu[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = canDoubleReach[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = doubleReach[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = tenpai[i] ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = nagashimangan[i] ? (byte)1 : (byte)0;
            }

            // Debug.Log("Serialize 2.1");

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = (byte)waitingTiles[i].Length;

                for (int j = 0; j < waitingTiles[i].Length; ++j)
                {
                    data[dst++] = (byte)waitingTiles[i][j];
                }
            }

            // Debug.Log("Serialize 2.2");

            // Debug.Log("Serialize 3");

            for (int i = 0; i < numWallTiles; ++i)
            {
                data[dst++] = (byte)wallTiles[i];
            }

            // Debug.Log("Serialize 4");

            for (int i = 0; i < 4; ++i)
            {
                data[dst++] = (byte)numHandTiles[i];

                for (int j = 0; j < numHandTiles[i]; ++j)
                {
                    data[dst++] = (byte)handTiles[i][j];
                }

                data[dst++] = (byte)numEatedSets[i];

                for (int j = 0; j < numEatedSets[i]; ++j)
                {
                    data[dst++] = (byte)eatedSetsTypes[i][j];
                    data[dst++] = (byte)eatedSetsDirections[i][j];

                    int num = GetNumTilesEated(eatedSetsTypes[i][j]);

                    for (int k = 0; k < num; ++k)
                    {
                        data[dst++] = (byte)eatedSetsTiles[i][j][k];
                    }
                }

                data[dst++] = (byte)numDiscardedTiles[i];

                for (int j = 0; j < numDiscardedTiles[i]; ++j)
                {
                    data[dst++] = (byte)discardedTiles[i][j];
                    data[dst++] = discardedTilesTsumogiri[i][j] ? (byte)1 : (byte)0;
                }
            }

            // Debug.Log("Serialize 5");

            data[dst++] = (byte)handState.Length;

            for (int i = 0; i < handState.Length; ++i)
            {
                data[dst++] = (byte)handState[i];
            }

            data[dst++] = resetLocal ? (byte)1 : (byte)0;

            // Debug.Log("Serialize 6");

            comm.SetMessage(data, dst);

            // Debug.Log("Serialize 7");
        }

        private void Deserialize()
        {
            // Debug.Log("Deserialize 1");

            var stateIndex = comm.GetMessageId();

            if (stateIndex == currentStateIndex)
            {
                return;
            }

            // Debug.Log("Deserialize 2");

            var data = comm.GetMessage();

            if (data.Length != 0)
            {
                // Debug.Log("Deserialize 3");

                int src = 0;

                string str = "DECODED";

                for (int i = 0; i < data.Length; ++i)
                {
                    str += ", " + data[i];
                }

                Debug.Log(str);

                int version = data[src++];
                gameState = data[src++];
                numWallTiles = data[src++];
                round = data[src++];
                honba = data[src++];
                firstLeader = data[src++];
                leader = data[src++];
                turn = data[src++];
                eatTurn = data[src++];

                if (version >= 8)
                {
                    lastKakanIndex = ConvertFFToNegative(data[src++]);
                }

                numKan = data[src++];
                numDoraOpen = data[src++];

                if (version >= 7)
                {
                    reached = data[src++] != 0;
                }

                // Debug.Log("Deserialize 4");

                for (int i = 0; i < 4; ++i)
                {
                    entry[i] = data[src++] != 0;
                }

                if (version >= 6)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        nextReady[i] = data[src++] != 0;
                    }
                }

                if (version >= 3)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        entryCpu[i] = data[src++] != 0;
                    }
                }

                for (int i = 0; i < 4; ++i)
                {
                    bool pointSign = false;

                    if (version >= 2)
                    {
                        pointSign = data[src++] != 0;
                    }

                    var point0 = (uint)data[src++];
                    var point1 = (uint)data[src++];
                    var point2 = (uint)data[src++];
                    var point3 = (uint)data[src++];
                    points[i] = (int)(point0 | (point1 << 8) | (point2 << 16) | (point3 << 24));

                    if (pointSign)
                    {
                        points[i] = -points[i];
                    }
                }

                {
                    var kyotaku0 = (uint)data[src++];
                    var kyotaku1 = (uint)data[src++];
                    var kyotaku2 = (uint)data[src++];
                    var kyotaku3 = (uint)data[src++];
                    kyotaku = (int)(kyotaku0 | (kyotaku1 << 8) | (kyotaku2 << 16) | (kyotaku3 << 24));
                }

                for (int i = 0; i < 4; ++i)
                {
                    overlook[i] = data[src++] != 0;
                }

                for (int i = 0; i < 4; ++i)
                {
                    reachIndex[i] = ConvertFFToNegative(data[src++]);
                }

                for (int i = 0; i < 4; ++i)
                {
                    ippatsu[i] = data[src++] != 0;
                }

                if (version >= 9)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        canDoubleReach[i] = data[src++] != 0;
                    }

                    for (int i = 0; i < 4; ++i)
                    {
                        doubleReach[i] = data[src++] != 0;
                    }
                }
                else
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        canDoubleReach[i] = true;
                    }

                    for (int i = 0; i < 4; ++i)
                    {
                        doubleReach[i] = false;
                    }
                }

                for (int i = 0; i < 4; ++i)
                {
                    tenpai[i] = data[src++] != 0;
                }

                for (int i = 0; i < 4; ++i)
                {
                    nagashimangan[i] = data[src++] != 0;
                }

                // Debug.Log("Deserialize 4.1");

                for (int i = 0; i < 4; ++i)
                {
                    waitingTiles[i] = new int[data[src++]];

                    for (int j = 0; j < waitingTiles[i].Length; ++j)
                    {
                        waitingTiles[i][j] = data[src++];
                    }
                }

                // Debug.Log("Deserialize 4.1");

                // Debug.Log("Deserialize 5");

                for (int i = 0; i < numWallTiles; ++i)
                {
                    wallTiles[i] = data[src++];
                }

                // Debug.Log("Deserialize 6");

                for (int i = 0; i < 4; ++i)
                {
                    numHandTiles[i] = data[src++];

                    for (int j = 0; j < numHandTiles[i]; ++j)
                    {
                        handTiles[i][j] = data[src++];
                    }

                    numEatedSets[i] = data[src++];

                    for (int j = 0; j < numEatedSets[i]; ++j)
                    {
                        eatedSetsTypes[i][j] = data[src++];
                        eatedSetsDirections[i][j] = data[src++];

                        int num = GetNumTilesEated(eatedSetsTypes[i][j]);

                        for (int k = 0; k < num; ++k)
                        {
                            eatedSetsTiles[i][j][k] = data[src++];
                        }
                    }

                    numDiscardedTiles[i] = data[src++];

                    for (int j = 0; j < numDiscardedTiles[i]; ++j)
                    {
                        discardedTiles[i][j] = data[src++];
                        discardedTilesTsumogiri[i][j] = data[src++] != 0;
                    }
                }

                // Debug.Log("Deserialize 7");

                handState = new int[data[src++]];

                for (int i = 0; i < handState.Length; ++i)
                {
                    handState[i] = data[src++];
                }

                if (version >= 5)
                {
                    var resetLocal = data[src++] != 0;

                    if (resetLocal)
                    {
                        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ResetGameLocal));
                    }
                }

                // Debug.Log("Deserialize 8");
            }

            // Debug.Log("Deserialize 9");

            currentStateIndex = stateIndex;

            canReachDirty = true;
            cpuScoresIndex = 0;

            if (RunAutomatedAction())
            {
                return;
            }

            // Debug.Log("Deserialize 24");

            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                    if (entrySelf[turn])
                    {
                        soundPlayer.OnMyTurn(turn);
                    }
                    break;
                case GAMESTATE_CONFIRM_RON:
                case GAMESTATE_CONFIRM_KAN:
                case GAMESTATE_CONFIRM_PON:
                case GAMESTATE_CONFIRM_CHI:
                case GAMESTATE_CONFIRM_RON_CHANKAN:
                    if (entrySelf[eatTurn])
                    {
                        soundPlayer.OnMyTurn(eatTurn);
                    }
                    break;
                case GAMESTATE_EATED:
                    break;
            }

            // Debug.Log("Deserialize 25");
        }

        private bool RunAutomatedAction()
        {
            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                    if (entrySelf[turn] && tsumogiri[turn] && !CanTsumo())
                    {
                        var index = numHandTiles[turn] - 1;
                        OnPickTileLocal(index, true);
                        DisableAllButtons();
                        return true;
                    }
                    break;
                case GAMESTATE_CONFIRM_KAN:
                case GAMESTATE_CONFIRM_PON:
                case GAMESTATE_CONFIRM_CHI:
                    if (entrySelf[eatTurn] && nakinashi[eatTurn])
                    {
                        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(OnNoButton));
                        DisableAllButtons();
                        return true;
                    }
                    break;
            }

            if (Networking.IsOwner(gameObject))
            {
                lastTime = Time.time;
            }

            UpdateView();

            return false;
        }

        private void RunCpu()
        {
            // Debug.Log("RunCpu 1");

            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                case GAMESTATE_EATED:
                    if (entryCpu[turn])
                    {
                        // Debug.Log("RunCpu 2");

                        if (CanTsumo())
                        {
                            OnTsumoButton();
                        }
                        else if (reachIndex[turn] != -1)
                        {
                            OnPickTileLocal(numHandTiles[turn] - 1, true);
                        }
                        else if (CanReach())
                        {
                            OnReachButton();
                        }
                        else
                        {
                            var index = CalculateCpuSelection(false);

                            if (index != -1)
                            {
                                OnPickTileLocal(index, true);
                                lastTime = Time.time;
                            }
                        }
                    }
                    break;
                case GAMESTATE_CONFIRM_RON:
                case GAMESTATE_CONFIRM_RON_CHANKAN:
                    if (entryCpu[eatTurn])
                    {
                        // Debug.Log("RunCpu 3");

                        OnRonButton();
                    }
                    break;
                case GAMESTATE_CONFIRM_KAN:
                    if (entryCpu[eatTurn])
                    {
                        // Debug.Log("RunCpu 4");

                        OnNoButton();
                    }
                    break;
                case GAMESTATE_CONFIRM_PON:
                    if (entryCpu[eatTurn])
                    {
                        // Debug.Log("RunCpu 5");

                        var tile = GetLastDiscardedTile();

                        if (CountSameTileInHand(eatTurn, tile) == 2 && IsTileYakuhai(tile, eatTurn))
                        {
                            OnPonButton();
                        }
                        else
                        {
                            OnNoButton();
                        }
                    }
                    break;
                case GAMESTATE_CONFIRM_CHI:
                    if (entryCpu[eatTurn])
                    {
                        // Debug.Log("RunCpu 6");

                        OnNoButton();
                    }
                    break;
                case GAMESTATE_SELECT_REACH:
                    if (entryCpu[turn])
                    {
                        // Debug.Log("RunCpu 7");

                        var index = CalculateCpuSelection(true);

                        if (index != -1)
                        {
                            OnPickTileLocal(index, true);
                            lastTime = Time.time;
                        }
                    }
                    break;
            }

            switch (gameState)
            {
                case GAMESTATE_EXHAUSTIVE_DRAW:
                case GAMESTATE_ABORTIVE_DRAW_4KAN:
                case GAMESTATE_ABORTIVE_DRAW_4REACH:
                case GAMESTATE_ABORTIVE_DRAW_SUFURENDA:
                case GAMESTATE_ABORTIVE_DRAW_NINE:
                case GAMESTATE_TSUMO:
                case GAMESTATE_RON:
                    CheckNext();
                    break;
            }

            // Debug.Log("RunCpu 8");
        }

        private int CalculateCpuSelection(bool reach)
        {
            // Debug.Log("CalculateCpuSelection 1");

            if (cpuScoresIndex == -1)
            {
                return -1;
            }

            var startIndex = cpuScoresIndex;
            var endIndex = cpuScoresIndex + 1;

            var numRemainTiles = GetNumRemainTiles(turn);

            // Debug.Log("CalculateCpuSelection 2");

            for (int i = startIndex; i < endIndex; ++i)
            {
                if (reach && !CanReachBy(i))
                {
                    continue;
                }

                var score = CalculateCpuScoreBy(i, numRemainTiles);
                cpuScores[i] = score;
            }

            // Debug.Log("CalculateCpuSelection 3");

            if (endIndex == numHandTiles[turn])
            {
                // Debug.Log("CalculateCpuSelection 4");

                cpuScoresIndex = -1;

                int maxScoreIndex = -1;
                double maxScore = 0.0;

                for (int i = 0; i < numHandTiles[turn]; ++i)
                {
                    if (reach && !CanReachBy(i))
                    {
                        continue;
                    }

                    var score = cpuScores[i];

                    if (maxScoreIndex == -1 || score > maxScore)
                    {
                        maxScoreIndex = i;
                        maxScore = score;
                    }
                }

                return maxScoreIndex;
            }
            else
            {
                // Debug.Log("CalculateCpuSelection 5");

                cpuScoresIndex = endIndex;
                return -1;
            }
        }

        private int[] GetNumRemainTiles(int turn_)
        {
            var result = new int[maxNormalizedTileId + 1];

            for (int i = 0; i < maxNormalizedTileId; ++i)
            {
                result[i] = 4;
            }

            for (int i = 0; i < numHandTiles[turn_]; ++i)
            {
                var id = NormalizeTileId(handTiles[turn_][i]);
                --result[id];
            }

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < numDiscardedTiles[i]; ++j)
                {
                    var id = NormalizeTileId(discardedTiles[i][j]);

                    if (i == turn_)
                    {
                        result[id] = 0;
                    }
                    else
                    {
                        --result[id];
                    }
                }

                for (int j = 0; j < numEatedSets[i]; ++j)
                {
                    var num = GetNumTilesEated(eatedSetsTypes[i][j]);

                    for (int k = 0; k < num; ++k)
                    {
                        var id = NormalizeTileId(eatedSetsTiles[i][j][k]);
                        --result[id];
                    }
                }
            }

            for (int i = 0; i < 1 + numDoraOpen; ++i)
            {
                var id = NormalizeTileId(wallTiles[5 + i * 2]);
                --result[id];
            }

            return result;
        }

        private void UpdateView()
        {
            DisableAllButtons();
            turnArrow.gameObject.SetActive(false);
            chicha.gameObject.SetActive(false);
            chichaTon.SetActive(false);
            chichaNan.SetActive(false);
            chichaSha.SetActive(false);
            chichaPey.SetActive(false);
            directions.gameObject.SetActive(false);
            honbaTransform.gameObject.SetActive(false);

            for (int i = 0; i < 4; ++i)
            {
                pointTexts[i].enabled = false;
                reachBars[i].SetActive(false);
            }

            for (int i = 0; i < tileTransforms.Length; ++i)
            {
                var tile = tileTransforms[i];
                tile.gameObject.SetActive(false);

                var meshRenderer = tile.GetComponent<MeshRenderer>();
                meshRenderer.SetPropertyBlock(null);
            }

            for (int i = 0; i < sampleTileTransforms.Length; ++i)
            {
                sampleTileTransforms[i].gameObject.SetActive(false);
            }

            // Debug.Log("UpdateView 11");

            var turnRotation = Quaternion.Euler(0.0f, turn * -90.0f, 0.0f);
            var eatTurnRotation = Quaternion.Euler(0.0f, eatTurn * -90.0f, 0.0f);

            turnArrow.localRotation = turnRotation;

            for (int i = 0; i < 4; ++i)
            {
                entryButtons[i].SetActive(!entry[i]);
                entryCpuButtons[i].SetActive(!entry[i]);
            }

            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                case GAMESTATE_CONFIRM_RON:
                case GAMESTATE_CONFIRM_KAN:
                case GAMESTATE_CONFIRM_PON:
                case GAMESTATE_CONFIRM_CHI:
                case GAMESTATE_CONFIRM_RON_CHANKAN:
                case GAMESTATE_EATED:
                    turnArrow.gameObject.SetActive(true);
                    break;
                case GAMESTATE_ENTRY_COMPLETED:
                    startButton.SetActive(true);
                    break;
                case GAMESTATE_EXHAUSTIVE_DRAW:
                case GAMESTATE_ABORTIVE_DRAW_4KAN:
                case GAMESTATE_ABORTIVE_DRAW_4REACH:
                case GAMESTATE_ABORTIVE_DRAW_SUFURENDA:
                case GAMESTATE_ABORTIVE_DRAW_NINE:
                case GAMESTATE_TSUMO:
                case GAMESTATE_RON:
                    for (int i = 0; i < 4; ++i)
                    {
                        nextButtons[i].SetActive(entrySelf[i] && !nextReady[i]);
                    }

                    turnArrow.gameObject.SetActive(true);
                    break;
            }

            switch (gameState)
            {
                case GAMESTATE_INITIAL:
                case GAMESTATE_ENTRY_COMPLETED:
                    break;
                default:
                    {
                        var firstLeaderRot = Quaternion.Euler(0.0f, firstLeader * -90.0f, 0.0f);
                        var leaderRot = Quaternion.Euler(0.0f, leader * -90.0f, 0.0f);

                        chicha.gameObject.SetActive(true);
                        chicha.localRotation = firstLeaderRot;

                        switch (round)
                        {
                            case 0:
                                chichaTon.SetActive(true);
                                break;
                            case 1:
                                chichaNan.SetActive(true);
                                break;
                            case 2:
                                chichaSha.SetActive(true);
                                break;
                            case 3:
                                chichaPey.SetActive(true);
                                break;
                        }

                        directions.gameObject.SetActive(true);
                        directions.localRotation = leaderRot;

                        honbaTransform.gameObject.SetActive(true);
                        honbaTransform.rotation = transform.rotation * firstLeaderRot;

                        honbaText.text = honba + translation.HONBA(language);
                        kyotakuText.text = translation.DEPOSIT(language) + kyotaku + translation.POINTS(language);

                        for (int i = 0; i < 4; ++i)
                        {
                            pointTexts[i].enabled = true;
                            pointTexts[i].text = points[i] + translation.POINTS2(language);
                        }

                        break;
                    }
            }

            // Debug.Log("UpdateView 10");

            string statusStr = "";
            string playerStatusStr = "";

            switch (gameState)
            {
                case GAMESTATE_EXHAUSTIVE_DRAW:
                    {
                        statusStr += translation.EXHAUSTIVE_DRAW(language);

                        if (IsExistsNagashimangan())
                        {
                            statusStr += "\n\n" + translation.NAGASHI_MANGAN(language);
                        }
                    }
                    break;
                case GAMESTATE_ABORTIVE_DRAW_4KAN:
                    statusStr += translation.ABORTIVE_DRAW_4KAN(language);
                    break;
                case GAMESTATE_ABORTIVE_DRAW_4REACH:
                    statusStr += translation.ABORTIVE_DRAW_4REACH(language);
                    break;
                case GAMESTATE_ABORTIVE_DRAW_SUFURENDA:
                    statusStr += translation.ABORTIVE_DRAW_SUFURENDA(language);
                    break;
                case GAMESTATE_ABORTIVE_DRAW_NINE:
                    statusStr += translation.ABORTIVE_DRAW_NINE(language);
                    break;
                case GAMESTATE_TSUMO:
                case GAMESTATE_RON:
                    {
                        int src = 1;
                        var numYaku = handState[src++];
                        var numFu = handState[src++];
                        var numPoint0 = (uint)handState[src++];
                        var numPoint1 = (uint)handState[src++];
                        var numPoint2 = (uint)handState[src++];
                        var numPoint3 = (uint)handState[src++];
                        var numPoint = (int)(numPoint0 | (numPoint1 << 8) | (numPoint2 << 16) | (numPoint3 << 24));
                        int sumYakuValue = 0;

                        for (int i = 0; i < numYaku; ++i)
                        {
                            switch (handState[src++])
                            {
                                case YAKU_MENZEN:
                                    statusStr += translation.YAKU_MENZEN(language);
                                    break;
                                case YAKU_REACH:
                                    statusStr += translation.YAKU_REACH(language);
                                    break;
                                case YAKU_DOUBLE_REACH:
                                    statusStr += translation.YAKU_DOUBLE_REACH(language);
                                    break;
                                case YAKU_IPPATSU:
                                    statusStr += translation.YAKU_IPPATSU(language);
                                    break;
                                case YAKU_TANYAO:
                                    statusStr += translation.YAKU_TANYAO(language);
                                    break;
                                case YAKU_PINFU:
                                    statusStr += translation.YAKU_PINFU(language);
                                    break;
                                case YAKU_RYANPEHKOU:
                                    statusStr += translation.YAKU_RYANPEHKOU(language);
                                    break;
                                case YAKU_IHPEHKOH:
                                    statusStr += translation.YAKU_IHPEHKOH(language);
                                    break;
                                case YAKU_CHANKAN:
                                    statusStr += translation.YAKU_CHANKAN(language);
                                    break;
                                case YAKU_CHANTA:
                                    statusStr += translation.YAKU_CHANTA(language);
                                    break;
                                case YAKU_SHOSUSHI:
                                    statusStr += translation.YAKU_SHOSUSHI(language);
                                    break;
                                case YAKU_TENHO:
                                    statusStr += translation.YAKU_TENHO(language);
                                    break;
                                case YAKU_SUANKO:
                                    statusStr += translation.YAKU_SUANKO(language);
                                    break;
                                case YAKU_JUNCHAN:
                                    statusStr += translation.YAKU_JUNCHAN(language);
                                    break;
                                case YAKU_RYUISO:
                                    statusStr += translation.YAKU_RYUISO(language);
                                    break;
                                case YAKU_URA_DORA:
                                    statusStr += translation.YAKU_URA_DORA(language);
                                    break;
                                case YAKU_HONIHSOU:
                                    statusStr += translation.YAKU_HONIHSOU(language);
                                    break;
                                case YAKU_AKADORA:
                                    statusStr += translation.YAKU_AKADORA(language);
                                    break;
                                case YAKU_HONROUTOU:
                                    statusStr += translation.YAKU_HONROUTOU(language);
                                    break;
                                case YAKU_CHINIHSOU:
                                    statusStr += translation.YAKU_CHINIHSOU(language);
                                    break;
                                case YAKU_SUKANTSU:
                                    statusStr += translation.YAKU_SUKANTSU(language);
                                    break;
                                case YAKU_SANANKO:
                                    statusStr += translation.YAKU_SANANKO(language);
                                    break;
                                case YAKU_CHITOITSU:
                                    statusStr += translation.YAKU_CHITOITSU(language);
                                    break;
                                case YAKU_SHOSANGEN:
                                    statusStr += translation.YAKU_SHOSANGEN(language);
                                    break;
                                case YAKU_YAKUHAI:
                                    statusStr += translation.YAKU_YAKUHAI(language);
                                    break;
                                case YAKU_IKKITSUKAN:
                                    statusStr += translation.YAKU_IKKITSUKAN(language);
                                    break;
                                case YAKU_TSUISO:
                                    statusStr += translation.YAKU_TSUISO(language);
                                    break;
                                case YAKU_CHINROUTOU:
                                    statusStr += translation.YAKU_CHINROUTOU(language);
                                    break;
                                case YAKU_CHIHO:
                                    statusStr += translation.YAKU_CHIHO(language);
                                    break;
                                case YAKU_RENHO:
                                    statusStr += translation.YAKU_RENHO(language);
                                    break;
                                case YAKU_DAISUSHI:
                                    statusStr += translation.YAKU_DAISUSHI(language);
                                    break;
                                case YAKU_HAITEIRAOYUE:
                                    statusStr += translation.YAKU_HAITEIRAOYUE(language);
                                    break;
                                case YAKU_CHURENPOHTOU:
                                    statusStr += translation.YAKU_CHURENPOHTOU(language);
                                    break;
                                case YAKU_RENPUHAI:
                                    statusStr += translation.YAKU_RENPUHAI(language);
                                    break;
                                case YAKU_KOKUSHIMUSO13:
                                    statusStr += translation.YAKU_KOKUSHIMUSO13(language);
                                    break;
                                case YAKU_SANSHOKUDOJUN:
                                    statusStr += translation.YAKU_SANSHOKUDOJUN(language);
                                    break;
                                case YAKU_KOKUSHIMUSO:
                                    statusStr += translation.YAKU_KOKUSHIMUSO(language);
                                    break;
                                case YAKU_RINSHANKAIHOU:
                                    statusStr += translation.YAKU_RINSHANKAIHOU(language);
                                    break;
                                case YAKU_DAISANGEN:
                                    statusStr += translation.YAKU_DAISANGEN(language);
                                    break;
                                case YAKU_SANSHOKUDOKO:
                                    statusStr += translation.YAKU_SANSHOKUDOKO(language);
                                    break;
                                case YAKU_SUANKO_TANKI:
                                    statusStr += translation.YAKU_SUANKO_TANKI(language);
                                    break;
                                case YAKU_HOUTEIRAOYUI:
                                    statusStr += translation.YAKU_HOUTEIRAOYUI(language);
                                    break;
                                case YAKU_DORA:
                                    statusStr += translation.YAKU_DORA(language);
                                    break;
                                case YAKU_SANKANTSU:
                                    statusStr += translation.YAKU_SANKANTSU(language);
                                    break;
                                case YAKU_TOITOI:
                                    statusStr += translation.YAKU_TOITOI(language);
                                    break;
                            }

                            var yakuValue = handState[src++];

                            statusStr += " ";
                            statusStr += yakuValue + translation.YAKU_VALUE(language);
                            sumYakuValue += yakuValue;
                        }

                        statusStr += translation.TOTAL_VALUE(language);

                        if (sumYakuValue >= 13 * 4)
                        {
                            statusStr += sumYakuValue / 13 + translation.COUNT_YAKUMAN(language);
                        }
                        else if (sumYakuValue >= 13 * 3)
                        {
                            statusStr += translation.TRIPLE_YAKUMAN(language);
                        }
                        else if (sumYakuValue >= 13 * 2)
                        {
                            statusStr += translation.DOUBLE_YAKUMAN(language);
                        }
                        else if (sumYakuValue >= 13)
                        {
                            statusStr += translation.YAKUMAN(language);
                        }
                        else
                        {
                            statusStr += sumYakuValue + translation.HAN(language) + numFu + translation.FU(language);
                        }

                        statusStr += "\n";
                        statusStr += numPoint + translation.POINTS(language);

                        break;
                    }
            }

            // Debug.Log("UpdateView 20");

            if (IsGameStateSelf() && entrySelf[turn])
            {
                playerUI.gameObject.SetActive(true);
                playerUI.rotation = transform.rotation * turnRotation;

                switch (gameState)
                {
                    case GAMESTATE_DRAWED:
                        if (handState.Length != 0)
                        {
                            tsumoButton.SetActive(CanTsumo());
                            kanButton.SetActive(CanSelfKan());
                            reachButton.SetActive(CanReach());
                            nineButton.SetActive(CanNine());

                            switch (handState[0])
                            {
                                case HANDSTATE_COMPLETE:
                                    playerStatusStr = translation.COMPLETE_HAND(language);
                                    break;
                                case HANDSTATE_COMPLETE_YAKUNASHI:
                                    playerStatusStr = translation.NO_YAKU(language);
                                    break;
                            }
                        }
                        break;
                    case GAMESTATE_SELECT_KAN:
                        playerStatusStr = translation.GAMESTATE_SELECT_KAN(language);
                        break;
                    case GAMESTATE_SELECT_REACH:
                        playerStatusStr = translation.GAMESTATE_SELECT_REACH(language);
                        break;
                }
            }

            // Debug.Log("UpdateView 21");

            if (IsGameStateEat() && entrySelf[eatTurn])
            {
                playerUI.gameObject.SetActive(true);
                playerUI.rotation = transform.rotation * eatTurnRotation;

                switch (gameState)
                {
                    case GAMESTATE_CONFIRM_RON:
                    case GAMESTATE_CONFIRM_RON_CHANKAN:
                        ronButton.SetActive(CanEatRon());
                        playerStatusStr = translation.GAMESTATE_CONFIRM_RON_CHANKAN(language);
                        break;
                    case GAMESTATE_CONFIRM_PON:
                        ponButton.SetActive(CanEatPon());
                        playerStatusStr = translation.GAMESTATE_CONFIRM_PON(language);
                        break;
                    case GAMESTATE_CONFIRM_KAN:
                        kanButton.SetActive(CanEatKan());
                        playerStatusStr = translation.GAMESTATE_CONFIRM_KAN(language);
                        break;
                    case GAMESTATE_CONFIRM_CHI:
                        playerStatusStr = translation.GAMESTATE_CONFIRM_CHI(language);

                        if (CanEatChiPrev())
                        {
                            chiPrevButton.SetActive(true);
                            chiPrevText.text = GetChiPrevText();
                        }

                        if (CanEatChiCenter())
                        {
                            chiCenterButton.SetActive(true);
                            chiCenterText.text = GetChiCenterText();
                        }

                        if (CanEatChiNext())
                        {
                            chiNextButton.SetActive(true);
                            chiNextText.text = GetChiNextText();
                        }

                        break;
                }

                noButton.SetActive(true);
            }

            statusTransform.gameObject.SetActive(statusStr.Length != 0);
            statusText.text = statusStr;

            playerStatusTransform.gameObject.SetActive(playerStatusStr.Length != 0);
            playerStatusText.text = playerStatusStr;

            // Debug.Log("UpdateView 12");

            for (int i = 0; i < numWallTiles; ++i)
            {
                // Debug.Log("UpdateView 13");

                if (wallTiles[i] == 0)
                {
                    continue;
                }

                var tile = tileTransformMap[wallTiles[i]];

                var side = i * 4 / NUM_ALL_WALL_TILES;
                var indexInSide = i % (NUM_ALL_WALL_TILES / 4);
                bool flip = false;

                var x = indexInSide / 2 - (NUM_ALL_WALL_TILES / 16) + 0.5f;
                var y = indexInSide % 2;
                var z = 0;

                if (i >= 4)
                {
                    if (i % 2 == 1)
                    {
                        var index = i / 2 - 2;
                        flip = index >= 0 && index <= numDoraOpen;
                    }
                    else
                    {
                        switch (gameState)
                        {
                            case GAMESTATE_TSUMO:
                            case GAMESTATE_RON:
                                {
                                    var index = i / 2 - 2;
                                    flip = index >= 0 && index <= numDoraOpen;

                                    if (flip)
                                    {
                                        z += 1;
                                    }

                                    break;
                                }
                        }
                    }
                }

                if (numKan % 2 == 0 && i == numKan + 1)
                {
                    x -= 1;
                    y -= 1;
                }

                if (side == 0 && i < 14 + numKan)
                {
                    x -= 1;
                }

                var position = new Vector3(0.25f + z * -tileHeight, y * tileDepth + tileDepthHalf, x * tileWidth);
                var rotation = Quaternion.Euler(flip ? -90.0f : 90.0f, 90.0f, 0.0f);

                var rot = Quaternion.Euler(0.0f, (leader + side) * -90.0f, 0.0f);
                position = rot * position;
                rotation = rot * rotation;

                tile.gameObject.SetActive(true);
                tile.transform.position = TransformPosition(position);
                tile.transform.rotation = transform.rotation * rotation;

                // Debug.Log("UpdateView 14");
            }

            // Debug.Log("UpdateView 15");

            for (int i = 0; i < 4; ++i)
            {
                // Debug.Log("UpdateView 17");

                float eatedTileX = 0.37f;

                for (int j = 0; j < numEatedSets[i]; ++j)
                {
                    var num = GetNumTilesEated(eatedSetsTypes[i][j]);

                    for (int k = num - 1; k >= 0; --k)
                    {
                        var tile = tileTransformMap[eatedSetsTiles[i][j][k]];
                        bool rotate = false;
                        bool flip = false;

                        var eatedType = eatedSetsTypes[i][j];

                        if (eatedType == EATED_SET_TYPE_ANKAN)
                        {
                            flip = k == 0 || k == 3;
                        }
                        else
                        {
                            if (eatedType == EATED_SET_TYPE_KAKAN && k == 3)
                            {
                                continue;
                            }

                            switch (eatedSetsDirections[i][j])
                            {
                                case EATED_SET_DIRECTION_LEFT:
                                    rotate = k == 0;
                                    break;
                                case EATED_SET_DIRECTION_CENTER:
                                    rotate = k == 1;
                                    break;
                                case EATED_SET_DIRECTION_RIGHT:
                                    rotate = k == (eatedType == EATED_SET_TYPE_MINKAN ? 3 : 2);
                                    break;
                            }
                        }

                        var width = rotate ? tileHeight : tileWidth;
                        var rot = Quaternion.Euler(0.0f, i * -90.0f, 0.0f);

                        eatedTileX -= width * 0.5f;

                        float x = rotate ? (tileHeightHalf - tileWidthHalf) : 0.0f;

                        {
                            var position = new Vector3(0.35f + x, tileDepthHalf, eatedTileX);
                            var rotation = Quaternion.Euler(flip ? 90.0f : 270.0f, rotate ? 0.0f : 90.0f, 0.0f);

                            position = rot * position;
                            rotation = rot * rotation;

                            tile.gameObject.SetActive(true);
                            tile.transform.position = TransformPosition(position);
                            tile.transform.rotation = transform.rotation * rotation;
                        }

                        if (eatedType == EATED_SET_TYPE_KAKAN && rotate)
                        {
                            var position = new Vector3(0.35f + x - tileWidth, tileDepthHalf, eatedTileX);
                            var rotation = Quaternion.Euler(flip ? 90.0f : 270.0f, rotate ? 0.0f : 90.0f, 0.0f);

                            position = rot * position;
                            rotation = rot * rotation;

                            var lastTile = tileTransformMap[eatedSetsTiles[i][j][3]];

                            lastTile.gameObject.SetActive(true);
                            lastTile.transform.position = TransformPosition(position);
                            lastTile.transform.rotation = transform.rotation * rotation;
                        }

                        eatedTileX -= width * 0.5f;
                    }

                    eatedTileX -= 0.005f;
                }

                // Debug.Log("UpdateView 16");

                // Debug.Log("eatedTileX: " + eatedTileX);

                var handTileX = Mathf.Min(eatedTileX, 0.05f) - 0.05f;

                for (int j = 0; j < numHandTiles[i]; ++j)
                {
                    var tile = tileTransformMap[handTiles[i][j]];
                    var x = j - Mathf.Min(13 - numEatedSets[i] * 3, numHandTiles[i]) / 2.0f - 0.5f;

                    bool open = false;
                    bool pick = false;

                    switch (gameState)
                    {
                        case GAMESTATE_TSUMO:
                            open = turn == i;
                            break;
                        case GAMESTATE_RON:
                            open = eatTurn == i;
                            break;
                        case GAMESTATE_EXHAUSTIVE_DRAW:
                            {
                                if (!IsExistsNagashimangan())
                                {
                                    open = tenpai[i];
                                }
                            }
                            break;
                        case GAMESTATE_ABORTIVE_DRAW_NINE:
                            open = turn == i;
                            break;
                        case GAMESTATE_ABORTIVE_DRAW_4REACH:
                            open = true;
                            break;
                    }

                    switch (gameState)
                    {
                        case GAMESTATE_DRAWED:
                        case GAMESTATE_TSUMO:
                        case GAMESTATE_SELECT_REACH:
                            if (turn == i && j == numHandTiles[i] - 1)
                            {
                                x += 1.0f;
                            }
                            break;
                    }

                    if (turn == i && entrySelf[i] && j == selectIndex)
                    {
                        pick = true;
                    }

                    var position = new Vector3(0.35f, open ? tileDepthHalf : tileHeightHalf, x * tileWidth + handTileX);
                    var positionPick = position + new Vector3(0.0f, pick ? 0.01f : 0.0f, 0.0f);
                    var rotation = Quaternion.Euler(open ? 270.0f : 0.0f, 90.0f, 0.0f);

                    var rot = Quaternion.Euler(0.0f, i * -90.0f, 0.0f);
                    position = rot * position;
                    positionPick = rot * positionPick;
                    rotation = rot * rotation;

                    tile.gameObject.SetActive(true);
                    tile.transform.position = TransformPosition(positionPick);
                    tile.transform.rotation = transform.rotation * rotation;

                    if (gameState == GAMESTATE_SELECT_REACH && pick)
                    {
                        var waitingTiles = waitingTilesBy[selectIndex];

                        for (int k = 0; k < waitingTiles.Length; ++k)
                        {
                            var t = sampleTileTransformMap[waitingTiles[k]];
                            var tx = k - (waitingTiles.Length - 1) * 0.5f;
                            t.gameObject.SetActive(true);
                            t.position = TransformPosition(position + rot * new Vector3(0.0f, tileHeight * 2.0f, tx * tileWidth));
                            t.rotation = transform.rotation * rotation;
                        }
                    }

                    if (i == turn && entrySelf[turn])
                    {
                        var pickTile = pickTiles.GetChild(j);
                        pickTile.position = TransformPosition(position);
                        pickTile.rotation = transform.rotation * rotation;

                        switch (gameState)
                        {
                            case GAMESTATE_DRAWED:
                                if (reachIndex[i] == -1)
                                {
                                    pickTile.gameObject.SetActive(true);
                                }
                                else
                                {
                                    pickTile.gameObject.SetActive(j == numHandTiles[i] - 1);
                                }
                                break;
                            case GAMESTATE_EATED:
                                pickTile.gameObject.SetActive(true);
                                break;
                            case GAMESTATE_SELECT_KAN:
                                pickTile.gameObject.SetActive(CanSelfKanBy(j));
                                break;
                            case GAMESTATE_SELECT_REACH:
                                pickTile.gameObject.SetActive(CanReachBy(j));
                                break;
                        }
                    }
                }

                // Debug.Log("UpdateView 18");

                float discardedTileX = 0.0f;

                for (int j = 0; j < numDiscardedTiles[i]; ++j)
                {
                    var tile = tileTransformMap[discardedTiles[i][j]];

                    if (j % 6 == 0)
                    {
                        discardedTileX = 0.0f;
                    }

                    var rotate = reachIndex[i] != -1 && j == reachIndex[i];
                    var width = rotate ? tileHeight : tileWidth;

                    discardedTileX += width * 0.5f;

                    var x = discardedTileX + -3.0f * tileWidth;
                    var z = (j / 6) * tileHeight + 0.10f;

                    discardedTileX += width * 0.5f;

                    var position = new Vector3(z, tileDepthHalf, x);
                    var rotation = Quaternion.Euler(270.0f, rotate ? 0.0f : 90.0f, 0.0f);

                    var rot = Quaternion.Euler(0.0f, i * -90.0f, 0.0f);
                    position = rot * position;
                    rotation = rot * rotation;

                    tile.gameObject.SetActive(true);
                    tile.transform.position = TransformPosition(position);
                    tile.transform.rotation = transform.rotation * rotation;

                    bool showTsumogiri = false;

                    switch (tsumogiriDisplayMode)
                    {
                        case TsumogiriDisplayMode.Never:
                            break;
                        case TsumogiriDisplayMode.OneRound:
                            showTsumogiri = j == numDiscardedTiles[i] - 1;
                            break;
                        case TsumogiriDisplayMode.Always:
                            showTsumogiri = true;
                            break;
                    }

                    if (showTsumogiri)
                    {
                        var isTsumogiri = discardedTilesTsumogiri[i][j];

                        if (isTsumogiri)
                        {
                            var materialPropertyBlock = new MaterialPropertyBlock();
                            materialPropertyBlock.SetColor("_Color", new Color(0.7f, 0.7f, 0.7f, 1.0f));
                            materialPropertyBlock.SetColor("_EmissionColor", new Color(0.7f, 0.7f, 0.7f, 1.0f));

                            var meshRenderer = tile.GetComponent<MeshRenderer>();
                            meshRenderer.SetPropertyBlock(materialPropertyBlock);
                        }
                    }

                    if (selectIndex != -1)
                    {
                        var selectedTileId = NormalizeTileId(handTiles[turn][selectIndex]);
                        var discardedTileId = NormalizeTileId(discardedTiles[i][j]);

                        if (selectedTileId == discardedTileId)
                        {
                            var materialPropertyBlock = new MaterialPropertyBlock();
                            materialPropertyBlock.SetColor("_Color", new Color(1.0f, 0.7f, 0.7f, 1.0f));
                            materialPropertyBlock.SetColor("_EmissionColor", new Color(1.0f, 0.7f, 0.7f, 1.0f));

                            var meshRenderer = tile.GetComponent<MeshRenderer>();
                            meshRenderer.SetPropertyBlock(materialPropertyBlock);
                        }
                    }
                }

                // Debug.Log("UpdateView 19");
            }

            // Debug.Log("UpdateView 23");

            for (int i = 0; i < 4; ++i)
            {
                nakinashiOnButtons[i].SetActive(false);
                nakinashiOffButtons[i].SetActive(false);
                tsumogiriOnButtons[i].SetActive(false);
                tsumogiriOffButtons[i].SetActive(false);
            }

            switch (gameState)
            {
                case GAMESTATE_INITIAL:
                case GAMESTATE_ENTRY_COMPLETED:
                    break;
                default:
                    for (int i = 0; i < 4; ++i)
                    {
                        nakinashiOnButtons[i].SetActive(entrySelf[i] && !nakinashi[i]);
                        nakinashiOffButtons[i].SetActive(entrySelf[i] && nakinashi[i]);
                        tsumogiriOnButtons[i].SetActive(entrySelf[i] && !tsumogiri[i]);
                        tsumogiriOffButtons[i].SetActive(entrySelf[i] && tsumogiri[i]);
                        reachBars[i].SetActive(reachIndex[i] != -1);
                    }
                    break;
            }

            UpdateSpectator();
        }

        private bool IsGameStateSelf()
        {
            switch (gameState)
            {
                case GAMESTATE_DRAWED:
                case GAMESTATE_SELECT_KAN:
                case GAMESTATE_SELECT_REACH:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsGameStateEat()
        {
            switch (gameState)
            {
                case GAMESTATE_CONFIRM_RON:
                case GAMESTATE_CONFIRM_PON:
                case GAMESTATE_CONFIRM_KAN:
                case GAMESTATE_CONFIRM_CHI:
                case GAMESTATE_CONFIRM_RON_CHANKAN:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsExistsNagashimangan()
        {
            var result = false;

            for (int i = 0; i < 4; ++i)
            {
                if (nagashimangan[i])
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private void Shuffle(int[] v)
        {
            for (int i = 0; i < v.Length - 1; ++i)
            {
                int index = Random.Range(i, v.Length);
                int temp = v[i];
                v[i] = v[index];
                v[index] = temp;
            }
        }

        private int[] Unique(int[] v)
        {
            // Debug.Log("Unique 1");

            var temp = (int[])v.Clone();

            Sort(temp, temp.Length);

            int src = 0;
            int dst = 0;

            // Debug.Log("Unique 2");

            while (src != temp.Length)
            {
                if (dst == 0 || temp[dst - 1] != temp[src])
                {
                    if (src != dst)
                    {
                        temp[dst] = temp[src];
                    }

                    ++dst;
                }

                ++src;
            }

            // Debug.Log("Unique 3");

            var result = new int[dst];

            for (int i = 0; i < dst; ++i)
            {
                result[i] = temp[i];
            }

            // Debug.Log("Unique 4");

            return result;
        }

        private int ConvertNegativeToFF(int x)
        {
            return x < 0 ? 0xff : x;
        }

        private int ConvertFFToNegative(int x)
        {
            return x == 0xff ? -1 : x;
        }

        private int Factorial(int x)
        {
            int result = 1;

            for (int i = 2; i <= x; ++i)
            {
                result *= i;
            }

            return result;
        }

        private int Combination(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        private double CalculateProbability(int remain, int required)
        {
            return Combination(remain, required) * Mathf.Pow(0.25f, required) * Mathf.Pow(0.75f, remain - required);
        }

        private Vector3 TransformPosition(Vector3 position)
        {
            return transform.TransformPoint(position);
        }
    }
}
