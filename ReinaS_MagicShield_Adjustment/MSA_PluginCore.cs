using System;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using UnityEngine.Animations;

namespace ReinaS_MagicShield_Adjustment
{
    [BepInPlugin("net.rs64.plugins.MagicShieldAdjustment", "ReinaS_MagicShield_Adjustment", "1.0.0.0")]
    public class MSA_PluginCore : BaseUnityPlugin
    {
        public static ConfigEntry<float> HeightOffset;
        public static ConfigEntry<float> SizeOffset;


        private void Awake()
        {
            HeightOffset = Config.Bind("General", "HeightOffset", -0.6f, "The smaller the value, the lower it goes.");
            SizeOffset = Config.Bind("General", "SizeOffset", 1.2f, "Size offset");


            var harmony = new Harmony("net.rs64.plugins.MagicShieldAdjustment");
            harmony.PatchAll();

            Debug.Log("Endable MagicShield_Adjustment_Plugin");

        }
    }


    public static class AdjustUtility
    {
        public static void Adjustment(Animator AvatarAnimator, Transform MagicShiled, float ScaleOffset, float HeightOffset)
        {
            var Pconstraint = MagicShiled.gameObject.GetComponent<PositionConstraint>();
            if (Pconstraint != null) return;
            Pconstraint = MagicShiled.gameObject.AddComponent<PositionConstraint>();

            var MagicShieldSize = (GetHaight(AvatarAnimator.avatar) / 2) * ScaleOffset;


            MagicShiled.localScale = new Vector3(MagicShieldSize, MagicShieldSize, MagicShieldSize);

            var HipBone = AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
            Pconstraint.AddSource(new ConstraintSource() { sourceTransform = HipBone, weight = 1f });
            Pconstraint.translationOffset = new Vector3(0, HeightOffset * MagicShieldSize, 0);
            Pconstraint.weight = 1f;
            Pconstraint.locked = true;
            Pconstraint.enabled = true;
            Pconstraint.constraintActive = true;

        }

        public static float GetHaight(Avatar Avatar)
        {
            var avatarhumanDescription = Avatar.humanDescription;
            var avatarHuman = avatarhumanDescription.human;
            var avatarSkelton = avatarhumanDescription.skeleton;

            var hight = 0f;


            var SerchHumanNames = new string[] { "Hips", "Spine", "Chest", "UpperChest", "Neck", "Head" };
            foreach (var HumanName in SerchHumanNames)
            {
                var BoneIndex = Array.FindIndex(avatarHuman, I => I.humanName == HumanName);
                if (BoneIndex == -1) continue;
                var bonename = avatarHuman[BoneIndex].boneName;
                var SkeltonBone = Array.Find(avatarSkelton, I => I.name == bonename);
                hight += SkeltonBone.position.y;

                if (SerchHumanNames[0] == HumanName && SkeltonBone.position.y < 0.1f)//腰が埋まっているモデル用(主に標準モデル用)
                {
                    hight += 1;
                }

            }


            return hight;
        }
    }
}
