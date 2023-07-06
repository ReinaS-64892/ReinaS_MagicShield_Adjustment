using HarmonyLib;
using Oc;
using UnityEngine;

namespace ReinaS_MagicShield_Adjustment
{
    [HarmonyPatch(typeof(AsPl_Skill_MagicShield), "enter")]
    static class MS_Adjustment
    {
        [HarmonyPostfix]
        static void Postfix(OcPlMaster ____Pl)
        {
            Animator Animator;
            if (____Pl.IsVRoid)
            {
                Animator = ____Pl.VrmModel.GetComponent<Animator>();
            }
            else
            {
                Animator = ____Pl.Animator;
            }


            var MagicShieldInstans = ____Pl.PlCommon.transform.Find("Effect/MagicBarrier");
            if (MagicShieldInstans == null) return;

            var ScaleOffset = MSA_PluginCore.SizeOffset.Value;
            var HeightOffset = MSA_PluginCore.HeightOffset.Value;

            AdjustUtility.Adjustment(Animator, MagicShieldInstans, ScaleOffset, HeightOffset);

        }



    }

}