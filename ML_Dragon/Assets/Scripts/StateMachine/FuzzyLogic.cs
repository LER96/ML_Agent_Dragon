using System.Collections.Generic;
using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
    private const float MaxHealth = 100f;
    private float MaxDistanceToMonster = 50f;

    public float LowHealth(float health)
    {
        return Mathf.Clamp01((MaxHealth - health) / MaxHealth);
    }

    public float HighHealth(float health)
    {
        return Mathf.Clamp01(health / MaxHealth);
    }

    public float CloseToMonster(float distance)
    {
        return Mathf.Clamp01((MaxDistanceToMonster - distance) / MaxDistanceToMonster);
    }

    public float FarFromMonster(float distance)
    {
        return Mathf.Clamp01(distance / MaxDistanceToMonster);
    }

    public string DetermineBehavior(float health, float distanceToMonster, float MaxDistanceToMonster, bool isBeingChased, float[] weights = null)
    {

         if (weights == null)
        {
            weights = new float[] { 1, 1, 1 };
        }
        this.MaxDistanceToMonster = MaxDistanceToMonster;
        float lowHealth = LowHealth(health);
        float highHealth = HighHealth(health);
        float closeToMonster = CloseToMonster(distanceToMonster);
        float farFromMonster = FarFromMonster(distanceToMonster);

        // Use weights passed in from the genetic algorithm
        float seekingHealthPriority = lowHealth * weights[0];
        float avoidingMonsterPriority = isBeingChased ? closeToMonster * weights[1] : 0;
        float seekingCoinsPriority = highHealth * farFromMonster * weights[2];

        Dictionary<string, float> behaviors = new Dictionary<string, float>
        {
            { "Seeking Health", seekingHealthPriority },
            { "Avoiding Monster", avoidingMonsterPriority },
            { "Seeking Coins", seekingCoinsPriority }
        };

        string selectedBehavior = null;
        float maxPriority = float.MinValue;

        foreach (var behavior in behaviors)
        {
            if (behavior.Value > maxPriority)
            {
                maxPriority = behavior.Value;
                selectedBehavior = behavior.Key;
            }
        }

        return selectedBehavior;
    }
}
