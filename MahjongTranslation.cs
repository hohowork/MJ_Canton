
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections.Generic;

namespace SuzuFactory.AozoraMahjong
{
    public class MahjongTranslation : UdonSharpBehaviour
    {

        public string EXHAUSTIVE_DRAW(int lang)
        {
            string[] text = {  "流局", "Exhaustive Draw", };
            return text[lang];
        }
        public string NAGASHI_MANGAN(int lang)
        {
            string[] text = { "流し満貫", "Nagashi Mangan" };
            return text[lang];
        }

        public string HONBA(int lang)
        {
            string[] text = { "本場", " Honba" };
            return text[lang];
        }

        public string DEPOSIT(int lang)
        {
            string[] text = { "供託 ", "Honba\n" };
            return text[lang];
        }

        public string POINTS(int lang)
        {
            string[] text = { "点 ", " Pts" };
            return text[lang];
        }
        public string POINTS2(int lang)
        {
            string[] text = { "点 ", "" };
            return text[lang];
        }

        public string ABORTIVE_DRAW_4KAN(int lang)
        {
            string[] text = { "流局\n\n四槓流れ", "Abortive Draw\n\nFour Kans" };
            return text[lang];
        }

        public string ABORTIVE_DRAW_4REACH(int lang)
        {
            string[] text = { "流局\n\n四家立直", "Abortive Draw\n\nFour Riichi" };
            return text[lang];
        }

        public string ABORTIVE_DRAW_SUFURENDA(int lang)
        {
            string[] text = { "流局\n\n四風連打", "Abortive Draw\n\nFour Winds" };
            return text[lang];
        }

        public string ABORTIVE_DRAW_NINE(int lang)
        {
            string[] text = { "流局\n\n九種九牌", "Abortive Draw\n\n9 Tiles, 9 Types" };
            return text[lang];
        }

        public string YAKU_MENZEN(int lang)
        {
            string[] text = { "門前自摸", "MENZEN TSUMO" };
            return text[lang];
        }
        public string YAKU_DOUBLE_REACH(int lang)
        {
            string[] text = { "ダブル立直", "DOUBLE RIICHI" };
            return text[lang];
        }

        public string YAKU_REACH(int lang)
        {
            string[] text = { "立直", "RIICHI" };
            return text[lang];
        }
        public string YAKU_TANYAO(int lang)
        {
            string[] text = { "断么九", "TANYAO" };
            return text[lang];
        }

        public string YAKU_IPPATSU(int lang)
        {
            string[] text = { "一発", "IPPATSU" };
            return text[lang];
        }

        public string YAKU_PINFU(int lang)
        {
            string[] text = { "平和", "PINFU" };
            return text[lang];
        }

        public string YAKU_RYANPEHKOU(int lang)
        {
            string[] text = { "二盃口", "RYANPEIKOU" };
            return text[lang];
        }
        public string YAKU_IHPEHKOH(int lang)
        {
            string[] text = { "一盃口", "IIPEIKOU" };
            return text[lang];
        }

        public string YAKU_CHANKAN(int lang)
        {
            string[] text = { "槍槓", "CHANKAN" };
            return text[lang];
        }

        public string YAKU_CHANTA(int lang)
        {
            string[] text = { "全帯幺九", "CHANTA" };
            return text[lang];
        }
        public string YAKU_SHOSUSHI(int lang)
        {
            string[] text = { "小四喜", "SHOUSUUSHI" };
            return text[lang];
        }
        public string YAKU_TENHO(int lang)
        {
            string[] text = { "天和", "TENHOU" };
            return text[lang];
        }
        public string YAKU_SUANKO(int lang)
        {
            string[] text = { "四暗刻", "SUUANKOU" };
            return text[lang];
        }
        public string YAKU_JUNCHAN(int lang)
        {
            string[] text = { "純全帯么九", "JUNCHAN" };
            return text[lang];
        }
        public string YAKU_RYUISO(int lang)
        {
            string[] text = { "緑一色", "RYUUIISOU" };
            return text[lang];
        }
        public string YAKU_URA_DORA(int lang)
        {
            string[] text = { "裏ドラ", "URADORA" };
            return text[lang];
        }
        public string YAKU_HONIHSOU(int lang)
        {
            string[] text = { "混一色", "HONITSU" };
            return text[lang];
        }
        public string YAKU_AKADORA(int lang)
        {
            string[] text = { "赤ドラ", "AKADORA" };
            return text[lang];
        }
        public string YAKU_HONROUTOU(int lang)
        {
            string[] text = { "混老頭", "HONROUTOU" };
            return text[lang];
        }
        public string YAKU_CHINIHSOU(int lang)
        {
            string[] text = { "清一色", "CHINITSU" };
            return text[lang];
        }
        public string YAKU_SUKANTSU(int lang)
        {
            string[] text = { "四槓子", "SUUKANTSU" };
            return text[lang];
        }
        public string YAKU_SANANKO(int lang)
        {
            string[] text = { "三暗刻", "SANANKOU" };
            return text[lang];
        }
        public string YAKU_CHITOITSU(int lang)
        {
            string[] text = { "七対子", "CHIITOITSU" };
            return text[lang];
        }
        public string YAKU_SHOSANGEN(int lang)
        {
            string[] text = { "小三元", "SHOSANGEN" };
            return text[lang];
        }
        public string YAKU_YAKUHAI(int lang)
        {
            string[] text = { "役牌", "YAKUHAI" };
            return text[lang];
        }
        public string YAKU_IKKITSUKAN(int lang)
        {
            string[] text = { "一気通貫", "ITTSUU" };
            return text[lang];
        }
        public string YAKU_TSUISO(int lang)
        {
            string[] text = { "字一色", "TSUUIISOU" };
            return text[lang];
        }
        public string YAKU_CHINROUTOU(int lang)
        {
            string[] text = { "清老頭", "CHINROUTOU" };
            return text[lang];
        }
        public string YAKU_CHIHO(int lang)
        {
            string[] text = { "地和", "CHIIHOU" };
            return text[lang];
        }
        public string YAKU_RENHO(int lang)
        {
            string[] text = { "人和", "RENHOU" };
            return text[lang];
        }
        public string YAKU_DAISUSHI(int lang)
        {
            string[] text = { "大四喜", "DAISUUSHI" };
            return text[lang];
        }
        public string YAKU_HAITEIRAOYUE(int lang)
        {
            string[] text = { "海底撈月", "HAITEI" };
            return text[lang];
        }
        public string YAKU_CHURENPOHTOU(int lang)
        {
            string[] text = { "九蓮宝燈", "CHUUREN" };
            return text[lang];
        }
        public string YAKU_RENPUHAI(int lang)
        {
            string[] text = { "連風牌", "RENPUHAI" };
            return text[lang];
        }
        public string YAKU_KOKUSHIMUSO13(int lang)
        {
            string[] text = { "国士無双十三面待ち", "KOKUSHIMUSOU" };
            return text[lang];
        }
        public string YAKU_SANSHOKUDOJUN(int lang)
        {
            string[] text = { "三色同順", "SANSHOKU" };
            return text[lang];
        }
        public string YAKU_KOKUSHIMUSO(int lang)
        {
            string[] text = { "国士無双", "KOKUSHIMUSOU" };
            return text[lang];
        }
        public string YAKU_RINSHANKAIHOU(int lang)
        {
            string[] text = { "嶺上開花", "RINSHAN" };
            return text[lang];
        }
        public string YAKU_DAISANGEN(int lang)
        {
            string[] text = { "大三元", "DAISANGEN" };
            return text[lang];
        }
        public string YAKU_SANSHOKUDOKO(int lang)
        {
            string[] text = { "三色同刻", "SANSHOKU" };
            return text[lang];
        }
        public string YAKU_SUANKO_TANKI(int lang)
        {
            string[] text = { "四暗刻単騎", "SUUANKOU" };
            return text[lang];
        }
        public string YAKU_HOUTEIRAOYUI(int lang)
        {
            string[] text = { "河底撈魚", "HOUTEI" };
            return text[lang];
        }
        public string YAKU_DORA(int lang)
        {
            string[] text = { "ドラ", "DORA" };
            return text[lang];
        }
        public string YAKU_SANKANTSU(int lang)
        {
            string[] text = { "三槓子", "SANKANTSU" };
            return text[lang];
        }
        public string YAKU_TOITOI(int lang)
        {
            string[] text = { "対々和", "TOITOI" };
            return text[lang];
        }

        public string YAKU_VALUE(int lang)
        {
            string[] text = { "役\n", " HAN\n" };
            return text[lang];
        }

        public string TOTAL_VALUE(int lang)
        {
            string[] text = { "\n合計 ", "\n\n\nTotal: " };
            return text[lang];
        }

        public string COUNT_YAKUMAN(int lang)
        {
            string[] text = { "倍役満", "-COUNT YAKUMAN" };
            return text[lang];
        }

        public string DOUBLE_YAKUMAN(int lang)
        {
            string[] text = { "ダブル役満", "DOUBLE YAKUMAN" };
            return text[lang];
        }

        public string TRIPLE_YAKUMAN(int lang)
        {
            string[] text = { "トリプル役満", "TRIPLE YAKUMAN" };
            return text[lang];
        }

        public string YAKUMAN(int lang)
        {
            string[] text = { "役満", "YAKUMAN" };
            return text[lang];
        }

        public string HAN(int lang)
        {
            string[] text = { "役 ", " HAN " };
            return text[lang];
        }

        public string FU(int lang)
        {
            string[] text = { "符", " Fu" };
            return text[lang];
        }

        public string COMPLETE_HAND(int lang)
        {
            string[] text = { "アガリ", "WIN" };
            return text[lang];
        }

        public string NO_YAKU(int lang)
        {
            string[] text = { "役無し", "NO YAKU" };
            return text[lang];
        }

        public string GAMESTATE_SELECT_KAN(int lang)
        {
            string[] text = { "カンする牌を選んでください", "Please choose a tile to Kan." };
            return text[lang];
        }

        public string GAMESTATE_SELECT_REACH(int lang)
        {
            string[] text = { "リーチします", "Declare Riichi" };
            return text[lang];
        }

        public string GAMESTATE_CONFIRM_RON_CHANKAN(int lang)
        {
            string[] text = { "ロンしますか？", "Declare Ron?" };
            return text[lang];
        }

        public string GAMESTATE_CONFIRM_PON(int lang)
        {
            string[] text = { "ポンしますか？", "Declare Pon?" };
            return text[lang];
        }

        public string GAMESTATE_CONFIRM_KAN(int lang)
        {
            string[] text = { "カンしますか？", "Declare Kan?" };
            return text[lang];
        }

        public string GAMESTATE_CONFIRM_CHI(int lang)
        {
            string[] text = { "チーしますか？", "Declare Chii?" };
            return text[lang];
        }
    }
}
