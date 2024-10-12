using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BigPresent.Items;
public class BigPresent : GrabbableObject
{
    private List<Item> items = new();
    private System.Random presentRandom = new();
    
    public override void Start()
    {
        base.Start();
        presentRandom = new System.Random(StartOfRound.Instance.randomMapSeed);
        foreach (SpawnableItemWithRarity spawnableItemWithRarity in RoundManager.Instance.currentLevel.spawnableScrap)
        {
            items.Add(spawnableItemWithRarity.spawnableItem);
        }
    }

    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);
        Plugin.Logger.LogInfo("Item Activated");
        if (IsServer) SpawnItemOrExplodeServerRpc();
        if (IsServer) StartCoroutine(DestroyPresentDelay());
        this.playerHeldBy.DiscardHeldObject();
        this.GetComponentInChildren<ParticleSystem>().Play();
        this.GetComponent<MeshFilter>().mesh = null;

    }

    private IEnumerator DestroyPresentDelay()
    {
        yield return new WaitForSeconds(2f);
        this.NetworkObject.Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemOrExplodeServerRpc()
    {
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
            // spawn 10 to 20 different scraps from the level it spawned from.
            int randomIterateNumber = presentRandom.Next(10, 21);
            Vector3 spawnPosition = this.transform.position;
            for (int i = 0; i < randomIterateNumber; i++)
            {
                Item item = items[presentRandom.Next(0, items.Count)];
                var obj = GameObject.Instantiate(item.spawnPrefab, spawnPosition, Quaternion.Euler(item.restingRotation), StartOfRound.Instance.propsContainer);
                obj.GetComponent<NetworkObject>().Spawn();
            }
        }
        else
        {
            SpawnExplosionClientRpc();
        }
    }

    [ClientRpc]
    private void SpawnExplosionClientRpc()
    {
        Landmine.SpawnExplosion(playerHeldBy.transform.position, true, 3, 20, 80, 100, null, true);
    }
}