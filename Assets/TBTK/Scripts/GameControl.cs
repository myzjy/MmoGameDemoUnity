using System.Collections;
using UnityEngine;

namespace TBTK
{
    [RequireComponent(typeof(TurnControl))]
    public class GameControl : MonoBehaviour
    {
#if UNITY_EDITOR
        public static bool inspector = false;
#endif


        public static GameControl instance;

        public static int factionWon_ID = -1; //for loading/saving data from cache

        public bool useGlobalSetting = false;

        public bool enableUnitDeployment = true;

        public bool autoEndTurn = false;

        public bool endMoveAfterAttack = false;

        public bool enableCounterAttack = false;

        public bool restoreAPOnTurn;

        public bool useAPToMove;

        public bool useAPToAttack;


        public int apPerMove = 1;

        //public int apPerNode=0;
        public int apPerAttack = 1;


        public bool enableCoverSystem;
        public float coverCritBonus = 0.25f;
        public float coverDodgeBonus = 0.3f;


        public bool enableSideStepping;

        public bool enableFogOfWar;

        public bool actionInProgress = false;

        //~ public static void CollectibleUseAbility(Ability ability, Node node){
        //~ instance.StartCoroutine(instance.ColAbilityRoutine(ability, node));
        //~ //GridManager.ClearSelectUnit();
        //~ }
        //~ public IEnumerator ColAbilityRoutine(Faction fac, Ability ability, Node node){
        //~ SetActionInProgress(true);
        //~ yield return CRoutine.Get().StartCoroutine(ability.HitTarget(target));
        //~ //yield return StartCoroutine(fac.UseAbilityRoutine(ability, node));
        //~ SetActionInProgress(false);
        //~ }


        public bool gameOver = false;

        //public int winFacIdx=-1;
        public Faction winningFac;

        void Awake()
        {
            instance = this;

            if (useGlobalSetting)
            {
                GlobalSettingDB db = GlobalSettingDB.Init();

                enableUnitDeployment = db.enableUnitDeployment;

                autoEndTurn = db.autoEndTurn;
                endMoveAfterAttack = db.endMoveAfterAttack;
                enableCounterAttack = db.enableCounterAttack;
                restoreAPOnTurn = db.restoreAPOnTurn;
                useAPToMove = db.useAPToMove;
                useAPToAttack = db.useAPToAttack;

                apPerMove = db.apPerMove;
                //apPerNode=db.apPerNode;
                apPerAttack = db.apPerAttack;

                enableSideStepping = db.enableSideStepping;
                enableFogOfWar = db.enableFogOfWar;
                enableCoverSystem = db.enableCoverSystem;
                coverCritBonus = db.coverCritBonus;
                coverDodgeBonus = db.coverDodgeBonus;
            }

            ObjectPoolManager.Init();
            GridManager.Init();
            UnitManager.Init();
            TurnControl.Init(useGlobalSetting);
            AbilityManager.Init();
            CollectibleManager.Init();

            //gameObject.AddComponent<RoutineManager>();
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            if (UnitManager.RequireManualDeployment())
            {
                while (UnitManager.DeployingUnit()) yield return null;
            }

            Debug.Log("Start Game");
            UnitManager.StartGame();
            TurnControl.StartGame();

            GridManager.SetupFogOfWar();

            TBTK.OnGameStart(); //inform UI to start game
        }

        public static bool EnableUnitDeployment()
        {
            return instance.enableUnitDeployment;
        }

        public static bool AutoEndTurn()
        {
            return instance.autoEndTurn;
        }

        public static bool EndMoveAfterAttack()
        {
            return instance.endMoveAfterAttack;
        }

        public static bool EnableCounterAttack()
        {
            return instance.enableCounterAttack;
        }

        public static bool RestoreAPOnTurn()
        {
            return instance.restoreAPOnTurn;
        }

        public static bool UseAPToMove()
        {
            return instance.useAPToMove;
        }

        public static bool UseAPToAttack()
        {
            return instance.useAPToAttack;
        }

        public static int GetAPPerMove()
        {
            return instance.apPerMove;
        }

        //public static int GetAPPerNode(){ return instance.apPerNode; }
        public static int GetAPPerAttack()
        {
            return instance.apPerAttack;
        }

        public static bool EnableCoverSystem()
        {
            return instance.enableCoverSystem;
        }

        public static float GetCoverCritBonus()
        {
            return EnableCoverSystem() ? instance.coverCritBonus : 0;
        }

        public static float GetCoverDodgeBonus()
        {
            return EnableCoverSystem() ? instance.coverDodgeBonus : 0;
        }

        public static bool EnableSideStepping()
        {
            return instance != null ? instance.enableSideStepping : false;
        }

        public static bool EnableFogOfWar()
        {
            return instance != null ? instance.enableFogOfWar : false;
        }

        public static bool EndTurn()
        {
            if (IsGameOver()) return false;
            if (ActionInProgress()) return false;
            if (AI.ActionInProgress()) return false;

            UnitManager.TBSelectUnit(null);

            instance.StartCoroutine(_EndTurn());

            return true;
        }

        public static IEnumerator _EndTurn()
        {
            yield return null; //wait a frame for the UI to update after end turn button is press
            GridIndicator.HideAll();
            TurnControl.EndTurn();
        }

        public static bool ActionInProgress()
        {
            return instance.actionInProgress || AI.ActionInProgress();
        }

        public void SetActionInProgress(bool flag)
        {
            actionInProgress = flag;
            TBTK.OnActionInProgress(flag);
        }

        /// <summary>
        /// unit物体移动
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="node"></param>
        public static void UnitMove(Unit unit, Node node)
        {
            Debug.Log($"开始移动，{node}");
            GridManager.ClearSelectUnit();
            instance.StartCoroutine(instance.UnitMoveRoutine(unit, node));
        }

        public IEnumerator UnitMoveRoutine(Unit unit, Node node)
        {
            Debug.Log($"StartCoroutine 开始移动，{node}");

            SetActionInProgress(true);
            yield return StartCoroutine(unit.MoveRoutine(node));
            SetActionInProgress(false);
            if (unit.IsAllActionCompleted() && autoEndTurn) UnitManager.SelectNextUnit();
            else UnitManager.TBSelectUnit(unit);
        }

        public static void UnitAttack(Unit unit, Node node)
        {
            Debug.Log($" 开始UnitAttack,{unit}，{node}");

            GridManager.ClearSelectUnit();
            instance.StartCoroutine(instance.UnitAttackRoutine(unit, node));
        }

        public IEnumerator UnitAttackRoutine(Unit unit, Node node)
        {
            Debug.Log($"StartCoroutine UnitAttackRoutine 开始UnitAttack,{unit}，{node}");

            SetActionInProgress(true);
            yield return StartCoroutine(unit.AttackRoutine(node));
            SetActionInProgress(false);
            if (unit.IsAllActionCompleted() && autoEndTurn) UnitManager.SelectNextUnit();
            else UnitManager.TBSelectUnit(unit);
        }

        public static void UnitUseAbility(Unit unit, Ability ability, Node node)
        {
            GridManager.ClearSelectUnit();
            instance.StartCoroutine(instance.UnitAbilityRoutine(unit, ability, node));
        }

        public IEnumerator UnitAbilityRoutine(Unit unit, Ability ability, Node node)
        {
            SetActionInProgress(true);
            yield return StartCoroutine(unit.UseAbilityRoutine(ability, node));
            SetActionInProgress(false);
            if (unit.IsAllActionCompleted() && autoEndTurn) UnitManager.SelectNextUnit();
            else UnitManager.TBSelectUnit(unit);
        }

        public static void FactionUseAbility(Faction fac, Ability ability, Node node)
        {
            instance.StartCoroutine(instance.FacAbilityRoutine(fac, ability, node, UnitManager.GetSelectedUnit()));
            GridManager.ClearSelectUnit();
        }

        public IEnumerator FacAbilityRoutine(Faction fac, Ability ability, Node node, Unit unit)
        {
            SetActionInProgress(true);
            yield return StartCoroutine(fac.UseAbilityRoutine(ability, node));
            SetActionInProgress(false);
            UnitManager.TBSelectUnit(unit);
            TBTK.OnSelectFaction(fac);
        }

        public static bool IsGameOver()
        {
            return instance.gameOver;
        }

        public static int winningFacIdx()
        {
            return instance.winningFac.factionID;
        }

        public static void GameOver(Faction fac)
        {
            //bool playableFaction, int winFacIdx){
            Debug.Log("Game Over. fac-" + fac.factionID + " won!");
            instance.gameOver = true;
            instance.winningFac = fac;
            TBTK.OnGameOver(fac.playableFaction);

            factionWon_ID = fac.factionID;

            UnitManager.GameOver();
        }
    }
}