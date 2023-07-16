using HarmonyLib;
using Oc;
using UnityEngine;
using System.Reflection;

namespace ReinaS_MagicShield_Adjustment
{
    [HarmonyPatch(typeof(AsPl_Skill_MagicShield), "enter")]
    static class MS_Adjustment
    {
        [HarmonyPostfix]
        static void Postfix(OcPlMaster ____Pl)
        {
            Animator Animator;
            var IsVRM = ____Pl.IsVRoid;
            var HaightScaleOffset = 1f;
            if (IsVRM)
            {
                Animator = ____Pl.VrmModel.GetComponent<Animator>();
                HaightScaleOffset = ____Pl.CharaMakeData.vroidScale;
            }
            else
            {
                Animator = ____Pl.Animator;
            }


            var MagicShieldGameObject = ____Pl.PlCommon.GetType().GetField("_MagicBarrier", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(____Pl.PlCommon) as GameObject;
            if (MagicShieldGameObject == null) return;

            var ScaleOffset = MSA_PluginCore.SizeOffset.Value;
            var HeightOffset = MSA_PluginCore.HeightOffset.Value;
            var IsCraftopiamodelSizeAdjust = MSA_PluginCore.SizeAdjustCraftopiaModel.Value;

            float MagicShieldSize = MagicShieldGameObject.transform.localScale.y;

            if (IsVRM || IsCraftopiamodelSizeAdjust)
            {
                MagicShieldSize = AdjustUtility.ScaleAdjustment(Animator, MagicShieldGameObject.transform, ScaleOffset, HaightScaleOffset);
            }

            AdjustUtility.PositionAdjustment(Animator, MagicShieldGameObject.transform, HeightOffset, MagicShieldSize);

        }



    }

}