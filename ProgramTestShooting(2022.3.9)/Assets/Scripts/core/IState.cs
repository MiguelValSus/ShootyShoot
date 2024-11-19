using Player;

namespace States {
    public interface IState {
	    
	    #region Initialization
        void OnEnter(PlayerController player);
        void OnExit(PlayerController player);
		#endregion
        
        #region Update
        void GamePlay(PlayerController player);
        #endregion
		
        #region FixedUpdate
        void Execute(PlayerController player);
        #endregion
		
        #region Interact
        void Interaction(PlayerController player);
        #endregion
		
        #region Animation events
        void AnimationEvent(string interactionType = "");
        #endregion

        #region State check
        string State();
        #endregion
    }
}