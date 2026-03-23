using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshAgentExtensions {
    public static void SetDestinationSafety(this NavMeshAgent agent, Vector3 target, CancellationToken token) {
        if (agent == null || !agent.isActiveAndEnabled || token.IsCancellationRequested)
            return;
        agent.SetDestination(target);
    }
    
    public static void SafeStop(this NavMeshAgent agent) {
        if (agent == null || !agent.isActiveAndEnabled)
            return;
            
        agent.ResetPath();
    }
}