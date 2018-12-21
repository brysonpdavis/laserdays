using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelCompletionData {

    public bool completed;
    //public float[] rotation; //will do this later

        public LevelCompletionData(PuzzleCompletion obj)
    {
        if (obj.counter > 0)
            completed = true;
    }
}

