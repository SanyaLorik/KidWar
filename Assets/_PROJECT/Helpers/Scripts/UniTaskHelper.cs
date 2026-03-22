using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _PROJECT.Scripts.Helpers {
    public static class UniTaskHelper {
        
        
        public static void DisposeTask(ref CancellationTokenSource tokenSource) {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = null;
        }

        public static async UniTask TimerAction(float duration, Action action, CancellationToken token) {
            await UniTask.WaitForSeconds(duration, cancellationToken: token);
            action?.Invoke();
        }
        
        
        
    }
}
