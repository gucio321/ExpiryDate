using System;
using System.Collections.Generic;
using System.Linq;
using Classes;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class BattleMenuController : MonoBehaviour
    {
        public List<Character> playerCharacters;
        public List<Character> enemyCharacters;

        private List<Character> targetsForEnemyPool = new List<Character>();
        private List<Character> targetsForPlayerPool = new List<Character>();

        private List<Character> battleQueue = new List<Character>();
        
        private Enemy enemy = new Enemy();

        private void Start()
        {
            targetsForPlayerPool.AddRange(enemyCharacters);
            targetsForEnemyPool.AddRange(playerCharacters);
            
            CreateQueue();
            while (!CheckIfAnySideWon())
            {
                MakeTurn();
            }
        }

        private bool CheckIfAnySideWon()
        {
            // returns `true` if any player character is alive while all enemies are dead OR if all player characters are dead while any enemy is alive
            return playerCharacters.Any(character => character.health > 0) && enemyCharacters.All(character => character.health <= 0) ||
                   playerCharacters.All(character => character.health <= 0) && enemyCharacters.Any(character => character.health > 0);
        }

        private void CreateQueue()
        {
            // Merge playerCharacters and enemyCharacters into one array
            battleQueue = playerCharacters.Concat(enemyCharacters).ToList();
            // Sort the battle queue by initiative
            battleQueue = battleQueue.OrderByDescending(character => character.initiative).ToList();
        }

        private void MakeTurn()
        {
            // using `.ToList()` here to avoid "Collection was modified; enumeration operation may not execute." error
            // https://stackoverflow.com/a/27851493
            foreach (var character in battleQueue.ToList())
            {
                if (character.health <= 0)
                {
                    if (character.isOwnedByPlayer)
                    {
                        targetsForEnemyPool.Remove(character);
                    }
                    else
                    {
                        targetsForPlayerPool.Remove(character);
                    }
                    battleQueue.Remove(character);
                    continue;
                }
                if (character.isOwnedByPlayer)
                {
                    // TODO: Wait until player does his turn, then continue (State machine?)
                    // --- TEMPORARY
                    var randomTargetIndex = Random.Range(0, targetsForPlayerPool.Count);
                    enemy.MakeAttack(characterUsedForAttack:character, target:targetsForPlayerPool[randomTargetIndex]);
                    // ---
                }
                else
                {
                    var randomTargetIndex = Random.Range(0, targetsForEnemyPool.Count);
                    enemy.MakeAttack(characterUsedForAttack:character, target:targetsForEnemyPool[randomTargetIndex]);
                }
            }
        }
    }
}