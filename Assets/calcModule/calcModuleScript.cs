using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

public class calcModuleScript : MonoBehaviour {

	public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public KMAudio KMAudio;
	public KMSelectable upAns;
	public KMSelectable downAns;
	public KMSelectable submit;
	public TextMesh equation;
	public TextMesh ansField;
	bool started;
	bool solved;
	int batteryCount;
	int labelCount;
	int portCount;
	int[] terms;
	int degree;
	int secret;
	int secret2;
	int sPos;
	int sPos2;
	int sNum;
	int sNum2;
	int ans;
	int cAns;
	int type; //0 is Derivative. 1 is Integral.

    //necessary for logging
    static int moduleIdCounter = 1;
    int moduleId;

    void Start()
    {
        moduleId = moduleIdCounter++;
        started = false;
		solved = false;
        Init();
    }

    void Init()
    {
		ans = 0;
		terms = new int[Random.Range(2,4)];
		degree = Random.Range(1,4);
		type = Random.Range(0,2);
		for (int i = 0; i < terms.Length; i ++){
			terms[i] = Random.Range(-3,4);
		}
		secret = Random.Range(0,3);
		sPos = Random.Range(0,terms.Length);
		if (terms.Length == 2){
			sPos2 = -1;
			secret2 = -1;
		}else{
			sPos2 = Random.Range(0,terms.Length);
			while(sPos2 == sPos)sPos2 = Random.Range(0,terms.Length);
			secret2 = Random.Range(0,3);
			while(secret == secret2)secret2 = Random.Range(0,3);
		}
        GetComponent<KMBombModule>().OnActivate += OnActivate;
        GetComponent<KMSelectable>().OnCancel += OnCancel;
        GetComponent<KMSelectable>().OnLeft += OnLeft;
        GetComponent<KMSelectable>().OnRight += OnRight;
        GetComponent<KMSelectable>().OnSelect += OnSelect;
        GetComponent<KMSelectable>().OnDeselect += OnDeselect;
        GetComponent<KMSelectable>().OnHighlight += OnHighlight;
		upAns.OnInteract +=  delegate () { changeAnswer(true); return false;};
		downAns.OnInteract += delegate () { changeAnswer(false); return false;};
		submit.OnInteract += delegate () { submitAnser(); return false;};
    }
	private void OnDeselect()
    {
        //Debug.Log("ExampleModule2 OnDeselect.");
    }

    private void OnLeft()
    {
        //Debug.Log("ExampleModule2 OnLeft.");
    }

    private void OnRight()
    {
        //Debug.Log("ExampleModule2 OnRight.");
    }

    private void OnSelect()
    {
        //Debug.Log("ExampleModule2 OnSelect.");
    }

    private void OnHighlight()
    {
        //Debug.Log("ExampleModule2 OnHighlight.");
    }

    void OnActivate()
    {
		started = true;
        batteryCount = 0;
		labelCount = 0;
		portCount = 0;
        List<string> responses = GetComponent<KMBombInfo>().QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        foreach (string response in responses)
        {
            Dictionary<string, int> responseDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(response);
            batteryCount += responseDict["numbatteries"];
        }
		responses = GetComponent<KMBombInfo>().QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        foreach (string response in responses)
        {
            labelCount ++;
        }
		responses = GetComponent<KMBombInfo>().QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS, null);
        foreach (string response in responses)
        {
            portCount ++;
        }
        batteryCount %= 10;
        labelCount %= 10;
        portCount %= 10;
        if(secret == 0)sNum = (int)(batteryCount);
		else if(secret == 1)sNum = (int)(labelCount);
		else sNum = (int)(portCount);
		if(secret2 == 0)sNum2 = (int)(batteryCount);
		else if(secret2 == 1)sNum2 = (int)(labelCount);
		else sNum2 = (int)(portCount);
        if (sPos2 != -1)
        {
            while (sNum + sNum2 >= 10)
            {
                float temp1 = (float)(sNum) / 2.0f;
                float temp2 = (float)(sNum2) / 2.0f;
                sNum = (int)(Mathf.Floor(temp1));
                sNum2 = (int)(Mathf.Floor(temp2));
            }
        }
		int total = 0;
		for (int i = 0; i < terms.Length; i ++){
			if (sPos == i){
				total += sNum;
			}else if (sPos2 == i){
				total += sNum2;
			}else{
				total += terms[i];
			}
		}
		if (type == 0){
			while ((total * degree) >= 10 || (total * degree) <= -10){
				total = 0;
                degree = Random.Range(1, 4);
                for (int i = 0; i < terms.Length; i ++){
					terms[i] = Random.Range(-3,4);
				}
				for (int i = 0; i < terms.Length; i ++){
					if (sPos == i){
						total += sNum;
					}else if (sPos2 == i){
						total += sNum2;
					}else{
						total += terms[i];
					}
				}
			}
			cAns = (total * degree);
		}else if (type == 1){
			while ((int)(Mathf.Floor((float)total / (float)(degree + 1))) >= 10 || (int)(Mathf.Floor((float)total / (float)(degree + 1))) <= -10 || (Mathf.Floor((float)total / (float)(degree + 1))) % 1.0f != 0.0f){
				total = 0;
                degree = Random.Range(1, 4);
                for (int i = 0; i < terms.Length; i ++){
					terms[i] = Random.Range(-3,4);
				}
				for (int i = 0; i < terms.Length; i ++){
					if (sPos == i){
						total += sNum;
					}else if (sPos2 == i){
						total += sNum2;
					}else{
						total += terms[i];
					}
				}
			}
			cAns = (int)(Mathf.Floor((float)total / (float)(degree + 1)));
		}
		for (int i = 0; i < terms.Length; i ++){
			if (i > 0)equation.text += " + ";
			if (sPos == i){
				string lett = "B";
				if (secret == 1)lett = "R";
				else if (secret == 2)lett = "K";
				equation.text += lett + "x^" + degree;
			}else if (sPos2 == i){
				string lett = "Z";
				if (secret2 == 1)lett = "F";
				else if (secret2 == 2)lett = "M";
				equation.text += lett + "x^" + degree;
			}else{
				equation.text += terms[i] + "x^" + degree;
			}
		}
		ansField.text = ans + "";
		if (type == 1)ansField.text += "x^"+ (degree + 1);
		else ansField.text += "x^"+ (degree - 1);
        //logging
        Debug.LogFormat("[Calculus #{0}] The equation is {1}", moduleId, equation.text);
        if(type == 1)
        {
            Debug.LogFormat("[Calculus #{0}] The answer's degree is {1}, meaning that an Integral must be taken", moduleId, degree+1);
            Debug.LogFormat("[Calculus #{0}] The correct answer to the calculus equation is {1}", moduleId, cAns + "x^" + (degree+1));
        }
        else
        {
            Debug.LogFormat("[Calculus #{0}] The answer's degree is {1}, meaning that a Derivative must be taken", moduleId, degree-1);
            Debug.LogFormat("[Calculus #{0}] The correct answer to the calculus equation is {1}", moduleId, cAns + "x^" + (degree-1));
        }
    }
	void changeAnswer(bool positive){
        if (positive)
        {
            upAns.AddInteractionPunch();
            KMAudio.PlaySoundAtTransform("tick", upAns.transform);
        }
        else
        {
            downAns.AddInteractionPunch();
            KMAudio.PlaySoundAtTransform("tick", downAns.transform);
        }
		if (started){
			if (positive && ans < 9){
				ans ++;
				ansField.text = ans + "";
				if (type == 1)ansField.text += "x^"+ (degree + 1);
				else ansField.text += "x^"+ (degree - 1);
			}else if (!positive && ans > -9){
				ans --;
				ansField.text = ans + "";
				if (type == 1)ansField.text += "x^"+ (degree + 1);
				else ansField.text += "x^"+ (degree - 1);
			}
		}
	}
	void submitAnser(){
		submit.AddInteractionPunch();
		if (started && !solved){
			if (cAns == ans){
				KMAudio.PlaySoundAtTransform("tick", submit.transform);
                Debug.LogFormat("[Calculus #{0}] Submitted answer: {1}, which is correct! Module disarmed!", moduleId, ans + "x^" + ansField.text.Substring(ansField.text.Length-1));
                BombModule.HandlePass();
				solved = true;
			}else{
                Debug.LogFormat("[Calculus #{0}] Submitted answer: {1}, which is incorrect! Strike!", moduleId, ans + "x^" + ansField.text.Substring(ansField.text.Length - 1));
                BombModule.HandleStrike();
			}
		}else KMAudio.PlaySoundAtTransform("tick", submit.transform);
	}
    bool OnCancel()
    {
        //Debug.Log("ExampleModule2 cancel.");

        return true;
    }

    //twitch plays
    private bool inputIsValid(string cmd)
    {
        string[] validstuff = { "-9", "-8", "-7", "-6", "-5", "-4", "-3", "-2", "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        if (!validstuff.Contains(cmd))
        {
            return false;
        }
        int temp = int.Parse(cmd);
        if(temp >= -9 && temp <= 9)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} submit <num> [Submits the answer with the coefficient of <num>] | Valid answers range from -9 to 9";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (inputIsValid(parameters[1]))
                {
                    int temp = 0;
                    int.TryParse(parameters[1], out temp);
                    if (ans == temp)
                    {
                        submit.OnInteract();
                    }
                    else if (ans < temp)
                    {
                        for(int i = ans; i < temp; i++)
                        {
                            upAns.OnInteract();
                            yield return new WaitForSeconds(0.1f);
                        }
                        submit.OnInteract();
                    }
                    else if (ans > temp)
                    {
                        for (int i = ans; i > temp; i--)
                        {
                            downAns.OnInteract();
                            yield return new WaitForSeconds(0.1f);
                        }
                        submit.OnInteract();
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    yield return "sendtochaterror The specified answer to submit '" + parameters[1] + "' is invalid!";
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify an answer to submit!";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return ProcessTwitchCommand("submit "+cAns);
    }
}
