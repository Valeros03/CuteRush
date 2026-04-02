using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public static class GameExtensions
{
    public static Vector3 WithY(this Vector3 original, float newY)
    {
        return new Vector3(original.x, newY, original.z);
    }

    public static bool HasReachedDestination(this NavMeshAgent agent)
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static T GetRandomWeightedItem<T>(this IList<T> list, System.Func<T, float> weightSelector)
    {
        if (list == null || list.Count == 0) return default(T);

        float totalWeight = 0f;
        foreach (var item in list)
        {
            totalWeight += weightSelector(item);
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (var item in list)
        {
            currentSum += weightSelector(item);
            if (randomValue <= currentSum)
            {
                return item;
            }
        }
        return default(T);
    }
}