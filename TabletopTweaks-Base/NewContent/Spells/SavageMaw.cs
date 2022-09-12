using Kingmaker.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Base.Main;

namespace TabletopTweaks.Base.NewContent.Spells {
    static class SavageMaw {

        public static void AddSavageMaw() {
            var icon = AssetLoader.LoadInternal(TTTContext, folder: "Abilities", file: "Icon_SavageMaw.png");
            var buffClone = BlueprintTools.GetBlueprint<BlueprintBuff>("a67b51a8074ae47438280be0a87b01b6");//animal fury
            buffClone.name = "SavageMawBuff";
            var bleed1d4buff = BlueprintTools.GetBlueprint<BlueprintBuff>("5eb68bfe186d71a438d4f85579ce40c1");
            var apply_bleed = createContextActionApplyBuff(bleed1d4buff, CreateContextDuration(), is_permanent: true, dispellable: false);
            buffClone.AddComponent(createAddInitiatorAttackWithWeaponTriggerWithCategory(Helpers.CreateActionList(apply_bleed), critical_hit: true, weapon_category: WeaponCategory.Bite));

            var apply_buff = createContextActionApplyBuff(buffClone, CreateContextDuration(CreateContextValue(AbilityRankType.Default), DurationRate.Minutes), is_from_spell: true);

            var SavageMawAbility = Helpers.CreateBlueprint<BlueprintAbility>(TTTContext, "SavageMaw", bp => {
                bp.SetName(TTTContext, "Savage Maw");
                bp.SetDescription(TTTContext, "Your teeth extend and sharpen, transforming your mouth into a maw of razor - sharp fangs.You gain a secondary bite attack that deals 1d4 points of damage plus your Strength modifier.");
                bp.LocalizedDuration = Helpers.CreateString(TTTContext, "LongArmAbility.Duration", "1 minute/level");
                bp.LocalizedSavingThrow = new Kingmaker.Localization.LocalizedString();

                bp.m_Icon = icon;
                bp.Type = AbilityType.Spell;
                bp.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                bp.AvailableMetamagic = Metamagic.Extend | Metamagic.Heighten | Metamagic.Quicken | Metamagic.CompletelyNormal;
                bp.Range = AbilityRange.Personal;
                bp.ResourceAssetIds = new string[0];
                bp.EffectOnAlly = AbilityEffectOnUnit.Helpful;
                bp.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Self;
                bp.AddComponent(Helpers.Create<AbilityEffectRunAction>(c => {
                    c.Actions = new ActionList {
                        Actions = new GameAction[] { apply_buff }
                    };
                }));
                bp.AddComponent(Helpers.Create<SpellComponent>(c => {
                    c.School = SpellSchool.Transmutation;
                }));
                bp.AddComponent(Helpers.CreateContextRankConfig());
            });

            SavageMawAbility.setMiscAbilityParametersSelfOnly();

            SavageMawAbility.AddToSpellList(SpellTools.SpellList.DruidSpellList, 2);
            SavageMawAbility.AddToSpellList(SpellTools.SpellList.ClericSpellList, 2);
            SavageMawAbility.AddToSpellList(SpellTools.SpellList.InquisitorSpellList, 2);
            SavageMawAbility.AddToSpellList(SpellTools.SpellList.MagusSpellList, 2);
            SavageMawAbility.AddToSpellList(SpellTools.SpellList.RangerSpellList, 1);

        }

        static public Kingmaker.UnitLogic.Mechanics.Actions.ContextActionApplyBuff createContextActionApplyBuff(BlueprintBuff buff, ContextDurationValue duration, bool is_from_spell = false,
                                                                                                          bool is_child = false, bool is_permanent = false, bool dispellable = true,
                                                                                                          int duration_seconds = 0) {
            var apply_buff = Helpers.Create<Kingmaker.UnitLogic.Mechanics.Actions.ContextActionApplyBuff>();
            apply_buff.IsFromSpell = is_from_spell;
            apply_buff.m_Buff = buff.ToReference<BlueprintBuffReference>();
            apply_buff.Permanent = is_permanent;
            apply_buff.DurationValue = duration;
            apply_buff.IsNotDispelable = !dispellable;
            apply_buff.UseDurationSeconds = duration_seconds > 0;
            apply_buff.DurationSeconds = duration_seconds;
            apply_buff.AsChild = is_child;
            apply_buff.ToCaster = false;
            return apply_buff;
        }

        public static ContextDurationValue CreateContextDuration(ContextValue bonus = null, DurationRate rate = DurationRate.Rounds, DiceType diceType = DiceType.Zero, ContextValue diceCount = null) {
            return new ContextDurationValue() {
                BonusValue = bonus ?? CreateContextValueRank(),
                Rate = rate,
                DiceCountValue = diceCount ?? 0,
                DiceType = diceType
            };
        }

        public static ContextValue CreateContextValueRank(AbilityRankType value = AbilityRankType.Default) => value.CreateContextValue();

        public static ContextValue CreateContextValue(this AbilityRankType value) {
            return new ContextValue() { ValueType = ContextValueType.Rank, ValueRank = value };
        }


        static public AddInitiatorAttackWithWeaponTrigger createAddInitiatorAttackWithWeaponTriggerWithCategory(Kingmaker.ElementsSystem.ActionList action, bool only_hit = true, bool critical_hit = false,
                                                                                              bool check_weapon_range_type = false, bool reduce_hp_to_zero = false,
                                                                                              bool on_initiator = false,
                                                                                              Kingmaker.Enums.WeaponRangeType range_type = Kingmaker.Enums.WeaponRangeType.Melee,
                                                                                              bool wait_for_attack_to_resolve = false, bool only_first_hit = false,
                                                                                              WeaponCategory weapon_category = WeaponCategory.UnarmedStrike) {
            var t = Helpers.Create<AddInitiatorAttackWithWeaponTrigger>();
            t.Action = action;
            t.OnlyHit = only_hit;
            t.CriticalHit = critical_hit;
            t.CheckWeaponRangeType = check_weapon_range_type;
            t.RangeType = range_type;
            t.ReduceHPToZero = reduce_hp_to_zero;
            t.ActionsOnInitiator = on_initiator;
            t.WaitForAttackResolve = wait_for_attack_to_resolve;
            t.OnlyOnFirstAttack = only_first_hit;
            t.CheckWeaponCategory = true;
            t.Category = weapon_category;
            return t;
        }


        public static void setMiscAbilityParametersSelfOnly(this BlueprintAbility ability,
                                                       Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Self,
                                                       Kingmaker.View.Animation.CastAnimationStyle animation_style = Kingmaker.View.Animation.CastAnimationStyle.CastActionSelf) {
            ability.CanTargetFriends = false;
            ability.CanTargetEnemies = false;
            ability.CanTargetSelf = true;
            ability.CanTargetPoint = false;
            ability.EffectOnEnemy = AbilityEffectOnUnit.None;
            ability.EffectOnAlly = AbilityEffectOnUnit.Helpful;
            ability.Animation = animation;
            ability.AnimationStyle = animation_style;
        }
    }
}
