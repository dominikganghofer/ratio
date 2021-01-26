using System;
using System.Collections.Generic;
using System.Linq;
using PCAD.Backend;
using UnityEngine;

namespace PCAD.Logic
{
    /// <summary>
    /// Serializes the current app state and stores it for undo/redo operations.
    /// </summary>
    public class History
    {
        private FirebaseConnection _fb;

        public History(Action<SketchModel.Serialization> historyPositionChangedHandler)
        {
            _historyPositionChangedHandler = historyPositionChangedHandler;
            _fb = new FirebaseConnection();
            _fb.Initialize();

        }

        public void AddToHistory(SketchModel.Serialization currentModel)
        {
            //remove steps that lie ahead of current position
            _history = _history.Take(_historyPosition + 1).ToList();

            var json = JsonUtility.ToJson(currentModel);
                _fb.AddScore(json);
            _historyPosition = _history.Count;
            System.IO.File.WriteAllText($"{Application.persistentDataPath}/Serial{_historyPosition}.json", json);
            _history.Add(json);
        }

        public SketchModel.Serialization Undo()
        {
            if (_historyPosition > 0)
                _historyPosition--;
            return JsonUtility.FromJson<SketchModel.Serialization>(_history[_historyPosition]);
        }
    
        public SketchModel.Serialization Redo()
        {
            if (_historyPosition < _history.Count -1)
                _historyPosition++;
            return JsonUtility.FromJson<SketchModel.Serialization>(_history[_historyPosition]);
        }
    
        private List<string> _history = new List<string>();
        private int _historyPosition;
        private Action<SketchModel.Serialization> _historyPositionChangedHandler;
    }
}