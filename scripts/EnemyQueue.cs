using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace StarWreck.scripts;

[GlobalClass]
public partial class EnemyQueue : Resource
{
    public enum EnemyQueueOrder
    {
        Sequential,
        Interleaved,
        Random
    }

    [Export(PropertyHint.Range, "0,10,or_greater")] public float SpawnRate { get; private set; } = 1f;

    [Export]
    public EnemyQueueOrder Order { get; private set; }

    [Export] public EnemyBatch[] EnemyBatches { get; private set; } = new EnemyBatch[1];

    public PriorityQueue<PackedScene, float> GetQueue()
    {
        PriorityQueue<PackedScene, float> queue = new();
        switch (Order)
        {
            case EnemyQueueOrder.Sequential:
            {
                float priority = 0f;
                foreach (EnemyBatch batch in EnemyBatches)
                {
                    for (int _ = 0; _ < batch.Count; _++)
                    {
                        queue.Enqueue(batch.Enemy, priority);
                        priority++;
                    }
                }

                break;
            }
            case EnemyQueueOrder.Interleaved:
            {
                int maxCount = EnemyBatches.Select(batch => batch.Count).Max();
                float offsetSize = maxCount <= 1 ? 1f : (1f / (maxCount - 1)) / EnemyBatches.Length;

                for (int i = 0; i < EnemyBatches.Length; i++)
                {
                    EnemyBatch batch = EnemyBatches[i];
                    float priorityIntervalSize = batch.Count <= 1 ? 0f : 1f / (batch.Count - 1);
                    for (int j = 0; j < batch.Count; j++)
                    {
                        float priority = priorityIntervalSize * j + offsetSize * i;
                        queue.Enqueue(batch.Enemy, priority);
                    }
                }

                break;
            }
            case EnemyQueueOrder.Random:
                Random random = new();
                foreach (EnemyBatch batch in EnemyBatches)
                {
                    for (int j = 0; j < batch.Count; j++)
                    {
                        queue.Enqueue(batch.Enemy, random.NextSingle());
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return queue;
    }
}
