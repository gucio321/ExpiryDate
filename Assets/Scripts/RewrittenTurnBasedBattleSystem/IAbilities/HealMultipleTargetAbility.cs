using System.Collections.Generic;
using RewrittenTurnBasedBattleSystem.ScriptableObjects.BaseAbilityData_ChildClasses;
using UnityEngine;

namespace RewrittenTurnBasedBattleSystem.IAbilities
{
    public class HealMultipleTargetAbility : IAbility
    {
        private HealMultipleTargetAbilityData AbilityData { get;}
        
        public HealMultipleTargetAbility(HealMultipleTargetAbilityData abilityData)
        {
            this.AbilityData = abilityData;
        }
        
        public void Perform(Team casterTeam, Team enemyTeam, Character selectedTarget)
        {
            foreach (Character character in casterTeam.characters)
            {
                var healAmount = Random.Range(AbilityData.minHealAmount, AbilityData.maxHealAmount);
                Debug.Log($"Used {AbilityData.name} on {selectedTarget.CharacterData.characterName}");
                character.Heal(healAmount);
            }
            
        }
    }
}