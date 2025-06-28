using UnityEngine;
// Removed using UnityEngine.AI; as NavMesh is no longer used

public class PatrolPointGenerator : MonoBehaviour
{
    [Header("Génération des Points")]
    // Removed patrol point generation settings as they are no longer relevant
    public bool generateOnStart = true; // Générer automatiquement au démarrage
    
    [Header("Visualisation")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.cyan;
    
    [Header("Référence Ennemi")]
    public EnemyAI enemyAI; // Référence à l'IA ennemie
    
    // Removed generatedPoints as they are no longer used
    
    void Start()
    {
        if (generateOnStart)
        {
            // No longer generating patrol points, just assigning enemyAI
            AssignEnemyAI();
        }
    }
    
    // Removed GeneratePatrolPoints and FindValidPatrolPoint methods
    // as they are no longer needed for a simple follow bot.

    void AssignEnemyAI()
    {
        if (enemyAI == null)
        {
            // Essayer de trouver l'IA ennemie automatiquement
            enemyAI = FindObjectOfType<EnemyAI>();
        }
        
        if (enemyAI != null)
        {
            Debug.Log($"Assigné l'ennemi {enemyAI.name} au générateur.");
            // No patrol points to assign for a follow bot
        }
        else
        {
            Debug.LogWarning("Aucune IA ennemie trouvée pour assigner.");
        }
    }
    
    // Removed ClearOldPoints method as there are no points to clear.
    
    // Removed RegeneratePoints context menu as it's not applicable.
    
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // No longer drawing patrol points or generation zones
        // You can add other gizmos here if needed for the follow bot, e.g., detection range.
    }
}
