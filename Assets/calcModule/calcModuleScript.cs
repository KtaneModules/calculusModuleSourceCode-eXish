using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

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
	
	 void Start()
    {
		started = false;
		solved = false;
        Init();
    }

    void Init()
    {
		ans = 0;
		terms = new int[UnityEngine.Random.Range(2,4)];
		degree = UnityEngine.Random.Range(1,4);
		type = UnityEngine.Random.Range(0,2);
		for (int i = 0; i < terms.Length; i ++){
			terms[i] = UnityEngine.Random.Range(-3,4);
		}
		secret = UnityEngine.Random.Range(0,3);
		sPos = UnityEngine.Random.RangeFsecr(0,terms.Length);
		if (terms.Length == 2){
			sPos2 = -1;
			secret2 = -1;
		}else{
			sPos2 = UnityEngine.Random.Range((0,terms.Length);
			while(sPos2 == sPos)sPos2 = UnityEngine.Random.Range((0,terms.Length);
			secret2 = UnityEngine.Random.Range((0,3);
			while(secret == secret2)secret2 = UnityEngine.Random.Range((0,3);
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
		if(secret == 0)sNum = (int)(batteryCount);
		else if(secret == 1)sNum = (int)(labelCount);
		else sNum = (int)(portCount);
		if(secret2 == 0)sNum2 = (int)(batteryCount);
		else if(secret2 == 1)sNum2 = (int)(labelCount);
		else sNum2 = (int)(portCount);
		while(sNum + sNum2 >= 10){
			float temp1 = (float)(sNum) / 2.0f;
			float temp2 = (float)(sNum2) / 2.0f;
			sNum = (int)(Mathf.Floor(temp1));
			sNum2 = (int)(Mathf.Floor(temp2));
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
				string lett = "b";
				if (secret == 1)lett = "r";
				else if (secret == 2)lett = "k";
				equation.text += lett + "x^" + degree;
			}else if (sPos2 == i){
				string lett = "z";
				if (secret2 == 1)lett = "f";
				else if (secret2 == 2)lett = "m";
				equation.text += lett + "x^" + degree;
			}else{
				equation.text += terms[i] + "x^" + degree;
			}
		}
		ansField.text = ans + "";
		if (type == 1)ansField.text += "x^"+ (degree + 1);
		else ansField.text += "x^"+ (degree - 1);
    }
	void changeAnswer(bool positive){
		KMAudio.PlaySoundAtTransform("tick", this.transform);
		if(positive)upAns.AddInteractionPunch();
		else downAns.AddInteractionPunch();
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
		if (started && ! solved){
			if (cAns == ans){
				KMAudio.PlaySoundAtTransform("tick", this.transform);
				BombModule.HandlePass();
				solved = true;
			}else{
				BombModule.HandleStrike();
			}
		}else KMAudio.PlaySoundAtTransform("tick", this.transform);
	}
    bool OnCancel()
    {
        //Debug.Log("ExampleModule2 cancel.");

        return true;
    }
}
