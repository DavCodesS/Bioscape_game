using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEditor.Localization.Editor;

public class DialogBox : MonoBehaviour
{
    public List<string> _Phrases = new List<string>();
    public static DialogBox dialogbox;
    public string _Phrase;
    public string _NewPhrase; // helps show the phrase slowly
    public Text _Text; // The gameboject "Text" will show the phrase
    public int _Count; // its a counter
    public int _Count2; // its a counter of the list
    public bool _CanPlay; // Helps to play the dialog only a once
    public bool _StopPlaying; // skips the dialog
    public bool _IsPlaying = true;
    public string _Name;
    public AudioSource _characterVoice;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LocalizationA");
        _CanPlay = false;
        dialogbox = this;
        StartCoroutine(Dialog());
    }

    // Update is called once per frame
    void Update()
    {

        if (_IsPlaying)
        {
            if (_CanPlay) { StartCoroutine(Dialog()); _CanPlay = false; } // when the player come back again , the dialog plays again
        }
        if (Input.GetKeyDown(KeyCode.Space)) { _StopPlaying = true; } // if he press space skip the dialog
        if (Input.GetKeyDown(KeyCode.Space)) { ConcludeDialog(); } // if he press space skip the dialog
    }

    public IEnumerator Dialog() // synchronize the phrase with the audio , foreach letter the audio plays once
    {
        StringForName();
        _IsPlaying = false;
        yield return new WaitForSeconds(0.08f);
        PlayerController.playercontroller.SetCanMove(false);
        _Count2 = 0; // clean the variable  
        _Count = 0; // clean the variable  
        _NewPhrase = ""; // clean the variable  
        _Phrase = "";
        foreach (string phrase in _Phrases)
        {

            _Text.text = ""; // clean the variable 
            _Phrase = phrase;

            while (_Phrase.Length > _Count)
            {
                yield return new WaitForSeconds(0.08f); // wait 0.8 seconds to run again
                _NewPhrase = _NewPhrase + _Phrase[_Count]; //helps show the phrase slowly , putting letter by letter
                _Text.text = _NewPhrase; // shows the new phrase
                _characterVoice.Play();
                _Count++; // increase 1 to the counter
                if (_StopPlaying) { _Text.text = _Phrases[_Count2]; _StopPlaying = false; break; } // if he press space skip the dialog and reset the variable 

            }
            yield return new WaitForSeconds(0.5f); // wait 0.5 seconds to run again
            _Count2 += 1; // increase 1 to the counter
            _Count = 0; // clean the variable
            _NewPhrase = ""; // clean the variable
            _Phrase = "";  // clean the variable 

        }

        _StopPlaying = false; // reset the variable 
        _IsPlaying = true;

    }


    public void CanPlay()
    {
        _CanPlay = true;
    }
    public bool HeIsInDialog()
    {
        return _IsPlaying;
    }

    public void StringForName()
    {
        for (int i = 0; i < _Phrases.Count; i++)
        {
            _Phrases[i] = _Phrases[i].Replace("{nome}", _Name);
        }
    }
    private void ConcludeDialog()
    {
        if (_IsPlaying)
        {
            _Text.text = "";
            _Count2 = 0;
            _Count = 0;
            _NewPhrase = "";
            _IsPlaying = true;
            _StopPlaying = false;

            if (PlayerController.playercontroller != null)
            {
                PlayerController.playercontroller.SetCanMove(true);
            }
            if (transform.parent != null)
            {
                _CanPlay = true;
                transform.parent.gameObject.SetActive(false);
            }
        }

    }

    IEnumerator LocalizationA()
    {
        yield return LocalizationSettings.InitializationOperation;
        Debug.Log(LocalizationSettings.SelectedLocale);
    }

}