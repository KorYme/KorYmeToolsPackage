using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem 
{
   public class DSM_GameDataExample : DataSaveManager<GameDataExample>
   {
       // Modify if you're willing to add some behaviour to the component
   }

   [System.Serializable]
   public class GameDataExample : GameDataTemplate
   {
       // Create the values you want to save here
   }
}