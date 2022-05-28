using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.BattleScene;
using Other.Enums;
using ScriptableObjects;
using UnityEngine;

namespace Classes
{
    public class BattleActions
    {
        private NotificationsHandler notificationHandlerReference;
        /// <summary>
        ///     Deploy a chosen action - damage/heal/buff - onto a chosen target
        /// </summary>
        public void MakeAction(Character target, Ability selectedAbility,
            IEnumerable<Character> allCharacters, bool isPlayerTurn)
        {
            var finalAttackTargets = new List<Character>();
            finalAttackTargets.AddRange(allCharacters.Where(x => !x.IsDead).ToList());
            finalAttackTargets.RemoveAll(x => x.DodgeEverythingUntilNextTurn);

            // If the ability should target only own team
            if (selectedAbility.abilityTarget is TargetType.SingleTeammate or TargetType.MultipleTeammates)
            {
                if (isPlayerTurn) RemoveAllEnemyCharactersFromTargets(finalAttackTargets);
                else RemoveAllPlayerCharactersFromTargets(finalAttackTargets);
            }
            else
            {
                if (isPlayerTurn) RemoveAllPlayerCharactersFromTargets(finalAttackTargets);
                else RemoveAllEnemyCharactersFromTargets(finalAttackTargets);
            }

            if (selectedAbility.abilityTarget is TargetType.MultipleEnemies or TargetType.MultipleTeammates)
            {
                foreach (Character thisIterationTarget in finalAttackTargets)
                {
                    switch (selectedAbility.abilityType)
                    {
                        case AbilityType.Status:
                            ApplyStatus(thisIterationTarget, selectedAbility);
                            break;
                        case AbilityType.DamageOnly:
                            Deal(thisIterationTarget, selectedAbility);
                            break;
                        case AbilityType.Heal:
                            Heal(thisIterationTarget, selectedAbility);
                            break;
                        case AbilityType.Shield:
                            Shield(thisIterationTarget, selectedAbility);
                            break;
                        default:
                            Debug.LogError("Ability type not found");
                            break;
                    }
                }
            }
            else
            {
                switch (selectedAbility.abilityType)
                {
                    case AbilityType.Status:
                        ApplyStatus(target, selectedAbility);
                        break;
                    case AbilityType.DamageOnly:
                        Deal(target, selectedAbility);
                        break;
                    case AbilityType.Heal:
                        Heal(target, selectedAbility);
                        break;
                    case AbilityType.Shield:
                        Shield(target, selectedAbility);
                        break;
                    default:
                        Debug.LogError("Ability type not found");
                        break;
                }
            }
        }

        /// <summary>
        /// This is so wrong my eyes bleed, but we have two days to complete the game... sorry future me and anyone who sees this
        /// </summary>
        /// <param name="character">Character GameObject</param>
        public void AssignNotificationHandlerReference(GameObject character)
        {
            notificationHandlerReference = character.GetComponent<NotificationsHandler>();
        }

        private void VisualizeAction(AbilityType abilityType, string value)
        {
            notificationHandlerReference.HandleNotification(abilityType, value);
        }

        private static void RemoveAllEnemyCharactersFromTargets(List<Character> finalAttackTargets)
        {
            for (var i = 0; i < finalAttackTargets.Count; i++)
            {
                if (i >= finalAttackTargets.Count) break;

                if (!finalAttackTargets[i].isOwnedByPlayer)
                {
                    finalAttackTargets.Remove(finalAttackTargets[i]);
                    i--;
                }
            }
        }

        private static void RemoveAllPlayerCharactersFromTargets(List<Character> finalAttackTargets)
        {
            for (var i = 0; i < finalAttackTargets.Count; i++)
            {
                if (i >= finalAttackTargets.Count) break;

                if (finalAttackTargets[i].isOwnedByPlayer)
                {
                    finalAttackTargets.Remove(finalAttackTargets[i]);
                    i--;
                }
            }
        }

        private void Deal(Character target, Ability selectedAbility)
        {
            if ((target.Health + target.ShieldPoints) - selectedAbility.damageAmount <= 0)
            {
                target.ShieldPoints = 0;
                target.Health = 0;
                target.IsDead = true;
            }
            else
            {
                if (target.ShieldPoints - selectedAbility.damageAmount < 0)
                {
                    target.ShieldPoints = 0;
                    target.Health -= selectedAbility.damageAmount - target.ShieldPoints;
                }
                else
                {
                    target.ShieldPoints -= selectedAbility.damageAmount;
                }
            }

            VisualizeAction(selectedAbility.abilityType, selectedAbility.damageAmount.ToString());
            Debug.Log($"Dealt {selectedAbility.damageAmount} damage to {target.name}!");
        }
        private void Heal(Character target, Ability selectedAbility)
        {
            if (target.currentlyAppliedStatuses.Contains(StatusType.Bleed))
            {
                Debug.Log($"Healed bleeding on {target}!");
                target.BleedDurationLeft = 0;
            }
            
            if (target.Health + selectedAbility.healAmount > target.maxHealth)
            {
                target.Health = target.maxHealth;
            }
            else
            {
                target.Health += selectedAbility.healAmount;
            }
            VisualizeAction(selectedAbility.abilityType, selectedAbility.healAmount.ToString());
            Debug.Log($"{target.name} has been healed for {selectedAbility.healAmount}!");
        }
        private void Shield(Character target, Ability selectedAbility)
        {
            if (target.ShieldPoints + selectedAbility.shieldAmount > target.maxShield)
            {
                target.ShieldPoints= target.maxShield;
            }
            else
            {
                target.ShieldPoints += selectedAbility.shieldAmount;
            }
            VisualizeAction(selectedAbility.abilityType, selectedAbility.shieldAmount.ToString());
            Debug.Log($"{target.name} has been shielded for {selectedAbility.healAmount}!");
        }
        

        private void ApplyStatus(Character target, Ability selectedStatus)
        {
            if(!target.currentlyAppliedStatuses.Contains(selectedStatus.statusType))
            {
                target.currentlyAppliedStatuses.Add(selectedStatus.statusType);
            }
            string finalText = null;
            switch (selectedStatus.statusType)
            {
                case StatusType.Bleed:
                    target.BleedDurationLeft += selectedStatus.bleedDuration;
                    target.CumulatedBleedDmg += selectedStatus.bleedDmgAmount;
                    finalText = $"-{selectedStatus.damageAmount} HP, Bleed";
                    break;
                case StatusType.Dodge:
                    target.DodgeEverythingUntilNextTurn = true;
                    finalText = "Dodge";
                    break;
                case StatusType.Stun:
                    target.StunDurationLeft += selectedStatus.stunDuration;
                    finalText = $"-{selectedStatus.damageAmount} HP, Stun ({selectedStatus.stunDuration} turns)";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(message:"Status type not found", innerException: null);
            }
            VisualizeAction(AbilityType.Status, finalText);
            Debug.Log($"Applied {selectedStatus.statusType} to {target}!");
        }
    }
}