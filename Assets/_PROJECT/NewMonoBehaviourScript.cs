using Architecture_M;
using UnityEngine;
using Zenject;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public int test;

    [Inject]
    private void Construct(IGameSave gameSave)
    {
        print(gameSave);
        gameSave.GetSave<GameSave>().REMOVE_THIS_VARIABLE = test;
        gameSave.Save();
    }
}