using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
public class Clown : MonoBehaviour
{
    [SerializeField]
    public List<Action> ActionsDataBase = new List<Action>();
    public List<Object> ObjectsDataBase = new List<Object>();
    public Action BlankAction;
    void LoadObjectsData()
    {
        ObjectsDataBase.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read("Objects");
        for (var i = 0; i < data.Count; i++)
        {
            int id = int.Parse(data[i]["id"].ToString(), System.Globalization.NumberStyles.Integer);
            string name = data[i]["Object"].ToString();
            float w1 = float.Parse(data[i]["Weight"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float v1 = float.Parse(data[i]["Valence"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float a1 = float.Parse(data[i]["Activity"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float d1 = float.Parse(data[i]["Dominance"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            string stateA = data[i]["StateA"].ToString();
            string stateB = data[i]["StateB"].ToString();
            string stateC = data[i]["StateC"].ToString();

            ObjectsDataBase.Add(new Object(id, name,w1, new double[3] {v1,a1,d1 }, new string[3] { stateA, stateB, stateC }));
        }
    }    
    void LoadActionData()
    {
        ActionsDataBase.Clear();
        List<Dictionary<string, object>> data = CSVReader.Read("schema_library");
        for(var i = 0; i < data.Count; i++)
        {

            int id = int.Parse(data[i]["id"].ToString(), System.Globalization.NumberStyles.Integer);
            string name = data[i]["name"].ToString();
            string type = data[i]["type"].ToString();
            string message = data[i]["message"].ToString();
            string assoc = data[i]["assoc"].ToString();

            float mult = float.Parse(data[i]["mult"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float prior = float.Parse(data[i]["prior"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

            float w1 = float.Parse(data[i]["w1"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float v1 = float.Parse(data[i]["v1"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float a1 = float.Parse(data[i]["a1"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float d1 = float.Parse(data[i]["d1"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float w2 = float.Parse(data[i]["w2"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float v2 = float.Parse(data[i]["v2"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float a2 = float.Parse(data[i]["a2"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            float d2 = float.Parse(data[i]["d2"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            //Debug.Log(111);

            string atype = data[i]["author"].ToString();
            string astate1 = data[i]["astate1"].ToString();
            string apos1 = data[i]["apos1"].ToString();
            string apre = data[i]["apre"].ToString();
            string astate2 = data[i]["astate2"].ToString();
            string apos2 = data[i]["apos2"].ToString();
            string apost = data[i]["apost"].ToString();

            string otype = data[i]["object"].ToString();
            string ostate1 = data[i]["ostate1"].ToString();
            string opos1 = data[i]["opos1"].ToString();
            string opre = data[i]["opre"].ToString();
            string ostate2 = data[i]["ostate2"].ToString();
            string opos2 = data[i]["opos2"].ToString();
            string opost = data[i]["opost"].ToString();

            string ttype = data[i]["target"].ToString();
            string tstate1 = data[i]["tstate1"].ToString();
            string tpos1 = data[i]["tpos1"].ToString();
            string tpre = data[i]["tpre"].ToString();
            string tstate2 = data[i]["tstate2"].ToString();
            string tpos2 = data[i]["tpos2"].ToString();
            string tpost = data[i]["tpost"].ToString();

            AddAction(id, name, type, message, assoc, mult, prior, w1, v1, a1, d1, w2, v2, a2, d2, atype, astate1,
        apos1, apre, astate2, apos2, apost, otype, ostate1,
        opos1, opre, ostate2, opos2, opost, ttype, tstate1,
        tpos1, tpre, tstate2, tpos2, tpost);
        }
    }
    void AddAction(int id, string name, string type, string message, string assoc, float mult, float prior, float w1, float v1, float a1,
        float d1, float w2, float v2, float a2, float d2, string atype, string astate1,
        string apos1, string apre, string astate2, string apos2, string apost, string otype, string ostate1,
        string opos1, string opre, string ostate2, string opos2, string opost, string ttype, string tstate1,
        string tpos1, string tpre, string tstate2, string tpos2, string tpost)
    {
        Action tempAction = new Action(id, name, type, message, assoc,new VAD(
            mult, prior, w1,w2, new double[3] { v1, a1, d2 }, new double[3] { v2, a2, d2 }), 
            new ActionActor(atype, astate1,
        apos1, apre, astate2, apos2, apost), new ActionActor(otype, ostate1,
        opos1, opre, ostate2, opos2, opost), new ActionActor(ttype, tstate1,
        tpos1, tpre, tstate2, tpos2, tpost));

        ActionsDataBase.Add(tempAction);

    }
    void Awake()
    {
        LoadActionData();
        LoadObjectsData();
    }
    
    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(0.1f * move,0,0));
    }
}
public class Action
{

    public int id;
    public string name;
    public string type;
    public string message;
    public string assoc;
    public VAD vad;
    public ActionActor author;
    public ActionActor thing;
    public ActionActor target;
    public Action()
    {

    }
    public Action(int id, string name, string type, string message, string assoc, VAD vad, ActionActor author, ActionActor thing, ActionActor target)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.message = message;
        this.assoc = assoc;
        this.vad = vad;
        this.author = author;
        this.thing = thing;
        this.target = target;
    }
}

public class VAD
{
    public double mult;
    public double prior;
    public double w1;
    public double w2;
    public double[] authorAppraisals = new double[3];
    public double[] targetAppraisals = new double[3];

    public VAD(double mult, double prior, double w1, double w2, double[] author, double[] target)
    {
        this.mult = mult;
        this.prior = prior;
        this.w1 = w1;
        this.w2 = w2;
        this.authorAppraisals = author;
        this.targetAppraisals = target;
    }
}

public class ActionActor
{
    public string type;
    public string state1;
    public string pos1;
    public string pre;
    public string state2;
    public string pos2;
    public string post;

    public ActionActor(string type, string stateBefore, string posBefore, string preBefore, string stateAfter, string posAfter, string postAfter)
    {
        this.type = type;
        this.state1 = stateBefore;
        this.pos1 = posBefore;
        this.pre = preBefore;
        this.state2 = stateAfter;
        this.pos2 = posAfter;
        this.post = postAfter;
    }
}
public class Object
{
    public int id;
    public string name;
    public double p;
    public double w1;
    public double[] Appraisals = new double[3];
    public string[] Poses = new string[3];
    public Object()
    {

    }

    public Object(int id, string name, double w1, double[] appraisals,
        string[] poses)
    {
        this.id = id;
        this.name = name;
        this.w1 = w1;
        Appraisals = appraisals;
        Poses = poses;
    }
}