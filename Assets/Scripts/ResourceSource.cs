using UnityEngine;
using UnityEngine.Events;

public enum ResourceType
{
    food
}

public class ResourceSource : MonoBehaviour
{
    public ResourceType type;
    public int quantity;
    
    // events
    public UnityEvent onQuantityChange;

    public void GatherResource(int amount, Player gatheringPlayer)
    {
        quantity -= amount;

        int amountToGive = amount;

        if (quantity < 0)
            amountToGive = quantity + amount;
        
        gatheringPlayer.GainResource(type, amountToGive);
        
        if(quantity <= 0)
            Destroy(gameObject);
        
        onQuantityChange?.Invoke();
    }
}