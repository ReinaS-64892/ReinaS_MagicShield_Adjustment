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
        public static ConfigEntry<bool> SizeAdjustCraftopiaModel;


        private void Awake()
        {
            HeightOffset = Config.Bind("General", "HeightOffset", -0.6f, "The smaller the value, the lower it goes.");
            SizeOffset = Config.Bind("General", "SizeOffset", 1.2f, "Size offset");
            SizeAdjustCraftopiaModel = Config.Bind("General", "SizeAdjustCraftpiaModel", true, "If false, the size adjustment is disabled when the Craftopia model is used.");





            var harmony = new Harmony("net.rs64.plugins.MagicShieldAdjustment");
            harmony.PatchAll();

            Debug.Log("Endable MagicShield_Adjustment_Plugin");

        }
    }


    public static class AdjustUtility
    {
        public static void PositionAdjustment(Animator AvatarAnimator, Transform MagicShiled, float HeightOffset, float MagicShieldSize)
        {
            var Pconstraint = MagicShiled.gameObject.GetComponent<PositionConstraint>();
            if (Pconstraint != null) return;
            Pconstraint = MagicShiled.gameObject.AddComponent<PositionConstraint>();

            var HipBone = AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
            Pconstraint.AddSource(new ConstraintSource() { sourceTransform = HipBone, weight = 1f });
            Pconstraint.translationOffset = new Vector3(0, HeightOffset * MagicShieldSize, 0);
            Pconstraint.weight = 1f;
            Pconstraint.locked = true;
            Pconstraint.enabled = true;
            Pconstraint.constraintActive = true;

        }

        public static float ScaleAdjustment(Animator AvatarAnimator, Transform MagicShiled, float ScaleOffset, float HaightScaleOffset = 1f)
        {
            var MagicShieldSize = ((GetHaight(AvatarAnimator.avatar) * HaightScaleOffset) / 2) * ScaleOffset;


            MagicShiled.localScale = new Vector3(MagicShieldSize, MagicShieldSize, MagicShieldSize);
            return MagicShieldSize;
        }

        public static string Hips = "Hips";
        public static string[] UpperBoneNames = new string[] { "Spine", "Chest", "UpperChest", "Neck", "Head" };
        public static string[] lowerBoneNames = new string[] { "LeftUpperLeg", "LeftLowerLeg", "LeftFoot", "LeftToes" };

        public static float GetHaight(Avatar Avatar)//Unity基準のY軸が高さとして認識する
        {
            var HumanDescription = Avatar.humanDescription;
            var hight = 0f;

            foreach (var Name in UpperBoneNames)
            {
                var Bone = HumanDescription.GetSkeletonBone(Name);
                if (Bone == null) continue;
                hight += Mathf.Abs(Bone.Value.position.y);
            }

            foreach (var Name in lowerBoneNames)
            {
                var Bone = HumanDescription.GetSkeletonBone(Name);
                if (Bone == null) continue;
                hight += Mathf.Abs(Bone.Value.position.y);
            }

            return hight;
        }

        public static SkeletonBone? GetSkeletonBone(this HumanDescription avatarhumanDescription, string HumanName)
        {
            var HumanBones = avatarhumanDescription.human;
            var Skeltons = avatarhumanDescription.skeleton;

            var BoneIndex = Array.FindIndex(HumanBones, I => I.humanName == HumanName);
            if (BoneIndex == -1) return null;
            var bonename = HumanBones[BoneIndex].boneName;
            var SkeltonBone = Array.Find(Skeltons, I => I.name == bonename);
            return SkeltonBone;
        }

    }
}
