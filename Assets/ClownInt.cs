using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using UnityEngine;
public class ClownInt: MonoBehaviour
{


    private string FirstPos = "standing";

    public GameObject SecondClown;

    private List<Tuple<Action, double>> avaliableActions = new List<Tuple<Action, double>>();
    private List<Tuple<Object, double>> avaliableObjects = new List<Tuple<Object, double>>();
    private List<Tuple<string, int>> repeatedActions = new List<Tuple<string, int>>();

    private List<Action> ActionsDataBase = new List<Action>();
    private List<Object> ObjectsDataBase = new List<Object>();

    private double[] appraisals = new double[3];
    private double[] feelings = new double[3];
    private double[] appraisalsSecondClown = new double[3];
    private double[] feelingsSecondClown = new double[3];

    public const double OBJECT_DISTANCE = 10f;
    public const double SEMANTIC_DISTANCE = 10;
    public const double DISTANCE_TURN_ON_MORAL_SCHEMA = 1;
    public const double DISTANCE_BETWEEN_APPRAISALS_AND_FEELINGS = 0.71;
    public const double r = 0.1;
    public const double r1 = 0.3;
    public const double beta = 1.1;

    private Dictionary<string, FeelingState> feelingsStates = new Dictionary<string, FeelingState>();

    private string secondClownCharacteristic = "NAN";
    private string firstClownCharacteristic = "NAN";

    private string firstHandle = "-";
    private string JSON_PATH_INDEPENDENT_FEELINGS_STATES = Directory.GetCurrentDirectory() + "\\Assets\\ConstantStates.json";

    bool moralSchemaIsActive = false;

    public bool NoAction = true;
    public GameObject Target;

    private float TimeForAction = 0f;

    Action doingAction = null;
    WhiteClownStates WhiteStates;
    public Transform Window;
    public Transform OutOfWindow;
    public bool isDied = false;

    public Transform ObjectPlace;
    public Transform DropPlace;
    private Object obj;
    Rigidbody rb;

    Logger logger;
    void Start()
    {
        feelingsStates = JsonConvert.DeserializeObject<Dictionary<string, FeelingState>>(File.ReadAllText(JSON_PATH_INDEPENDENT_FEELINGS_STATES));
        ActionsDataBase = gameObject.GetComponent<Clown>().ActionsDataBase;
        ObjectsDataBase = gameObject.GetComponent<Clown>().ObjectsDataBase;
        WhiteStates = SecondClown.GetComponent<WhiteClownStates>();
        rb = GetComponent<Rigidbody>();
        logger = new Logger();
        logger.initLoggingFile();
    }
    void Update()
    {
        if (!isDied)
        {
            if (NoAction)
            {
                doingAction = getResponseAction();
                doAction();
                NoAction = false;
            }
            if (Target != null)
            {

                if (Vector3.Distance(transform.position, Target.transform.position) <= 2 && Target.name.Equals("window"))
                {
                    transform.position = OutOfWindow.position;
                    isDied = true;
                }
                if(doingAction.name.Equals("shot"))
                {
                    ActionAnimation();
                    NoAction = true;
                    Target = null;
                    logger.updateLogs("Redhead", doingAction.target.type == "agent" ? "Whitehair" : doingAction.target.type,
                    doingAction.id, "Redhead " + replaceMocks(doingAction, doingAction.message, firstHandle),
                    appraisals, feelings, firstClownCharacteristic);
                }
                if (Vector3.Distance(transform.position, Target.transform.position) <= 2)
                {
                    if (doingAction.target.pos2.Equals("trash"))
                    {
                        WhiteStates.isTrash = true;
                        WhiteStates.isStanding = false;
                        SecondClown.GetComponent<ClownInfluence>().SetTrash();
                    }
                    if (TimeForAction > 0)
                    {
                        TimeForAction -= Time.deltaTime;
                    }
                    else
                    {
                        if(doingAction.name.Equals("take"))
                        {
                            Target.transform.position = ObjectPlace.position;
                            Target.transform.parent = ObjectPlace;

                        }
                        if (doingAction.name.Equals("rip"))
                        {
                            Target.transform.position = DropPlace.position;
                            Target.transform.parent = null;

                        }
                        ActionAnimation();
                        NoAction = true;
                        Target = null;
                        logger.updateLogs("Redhead", doingAction.target.type == "agent" ? "Whitehair" : doingAction.target.type,
                        doingAction.id, "Redhead " + replaceMocks(doingAction, doingAction.message, firstHandle),
                        appraisals, feelings, firstClownCharacteristic);
                    }
                }
                else
                {

                    transform.LookAt(Target.transform);
                    Vector3 direction = Target.transform.position - transform.position;
                    direction.y = 0;
                    direction.Normalize();
                    rb.velocity = 2 * direction;

                }
            }
            else
            {
                if (TimeForAction > 0)
                {
                    TimeForAction -= Time.deltaTime;
                }
                else
                {
                    ActionAnimation();
                    NoAction = true;
                    Target = null;
                    logger.updateLogs("Redhead", doingAction.target.type == "agent" ? "Whitehair" : doingAction.target.type,
doingAction.id, "Redhead " + replaceMocks(doingAction, doingAction.message, firstHandle),
appraisals, feelings, firstClownCharacteristic);
                }
            }
        }
    }
    void ActionAnimation()
    {
        TimeForAction = 1f;
        //Debug.Log(doingAction.id);
        //Debug.Log("First clown " + replaceMocks(doingAction, doingAction.message, firstHandle));
        //Debug.Log(appraisals[0] + " " + appraisals[1] + " " + appraisals[2] + " ");
        //Debug.Log(feelings[0] + " " + feelings[1] + " " + feelings[2] + " ");
        //Debug.Log(firstClownCharacteristic);
        rebuildClownsStates(doingAction, FirstPos);
    }
    void doAction()
    {
        if(doingAction.type.Equals("other-direct") || doingAction.type.Equals("prompted") 
            || doingAction.type.Equals("self-directed") || doingAction.type.Equals("tool-object"))
        {
            if (!doingAction.target.type.Equals("-"))
            {
                if (doingAction.target.type.Equals("agent"))
                {
                    Target = SecondClown;
                }
                else
                {
                    Target = GameObject.Find(doingAction.target.type);
                }
            }
        }
        if (doingAction.name.Equals("drop"))
        {
            ObjectPlace.GetChild(0).parent = null;
        }
        if (doingAction.name.Equals("take") || doingAction.name.Equals("walkto"))
        {
            Target = GameObject.Find(obj.name);
        }
        if (doingAction.name.Equals("rip"))
        {
            Target = GameObject.Find(obj.name);
        }

    }
    public void addRepeated(Action act)
    {
        for (int j = 0; j < repeatedActions.Count; ++j)
        {
            if (repeatedActions[j].Item1.Equals(act.name))
            {
                repeatedActions[j] = new Tuple<string, int>(repeatedActions[j].Item1, repeatedActions[j].Item2 + 2);
                return;
            }
        }
        repeatedActions.Add(new Tuple<string, int>((string)act.name.Clone(), 2));
    }
    Action getResponseAction () {
        Action responseAction;
        avaliableActions.Clear();
        getActionsForThisPos(FirstPos, firstHandle, appraisals, feelings);
        getLikelihoods(appraisals, feelings);
        responseAction = getResponseActionByLikelihood();
        if(responseAction.name.Equals("rip"))
            obj = getResponseObj(responseAction.name, appraisals, feelings, true);
        else
            obj = getResponseObj(responseAction.name, appraisals, feelings, false);
        Debug.Log(responseAction.name);
        //Debug.Log(obj.name);
        addRepeated(responseAction);
        return responseAction;
        }

    public string replaceMocks(Action act, string message, string handle)
    {
        message = message.Replace("<target>", doingAction.target.type);
        message = message.Replace("<object>", handle);
        message = message.Replace(",", "");
        message = message.Replace("<pos>", act.thing.pos2);
        return message;
    }

    public void rebuildClownsStates(Action action, string pos) {
        appraisals = getAppraisalsAfterAction(appraisals, action.vad.authorAppraisals, action.vad.w1);
        appraisalsSecondClown = getAppraisalsAfterAction(appraisalsSecondClown, action.vad.targetAppraisals, action.vad.w2);

        feelings = moralSchema(feelings, appraisals);
        feelingsSecondClown = moralSchema(feelingsSecondClown, appraisalsSecondClown);

        if (!action.author.pos2.Equals("-"))
        {
            pos = action.author.pos2;
        }
    }

    public Action getResponseActionByLikelihood() {
        Action responseAction = new Action();
        avaliableActions.Sort((x1, y1) => x1.Item2.CompareTo(y1.Item2));
        System.Random x = new System.Random();
        double actionLikelihood = Convert.ToDouble(x.Next(0, 10000) / 10000.0);
        double currentLikelihood = 0;
            foreach (var el in avaliableActions)
            {
                    currentLikelihood += el.Item2;
                if (currentLikelihood >= actionLikelihood)
                {
                    responseAction = el.Item1;
                    break;
                }
            }
        return responseAction;
    }

    public void getLikelihoods(double[] appraisals, double[] feelings)
    {
        double firstNormSum = 0;
        double secondNormSum = 0;
        List<double> simplelist = new List<double>();
        for (int index = 0; index < avaliableActions.Count; ++index)
        {
            double distance =  distanceBetweenVectors(feelings, getAppraisalsAfterAction(appraisals,
            avaliableActions[index].Item1.vad.authorAppraisals, avaliableActions[index].Item1.vad.w1));
            simplelist.Add(distance);
            firstNormSum += distance;

        }
        for (int index = 0; index < avaliableActions.Count; ++index)
        {
            avaliableActions[index] = new Tuple<Action, double>(avaliableActions[index].Item1, 1 - simplelist[index] / firstNormSum);
            secondNormSum += avaliableActions[index].Item2;
        }
        secondNormSum = secondNormSum == 0 ? 0.001 : secondNormSum;
        if (repeatedActions.Count != 0)
        {
            double thirdNormSum = 0;
            for (int index = 0; index < avaliableActions.Count; ++index)
            {
                int pow = 0;
                if (checkRepeated(avaliableActions[index].Item1, pow))
                {
                        avaliableActions[index] = new Tuple<Action, double>(avaliableActions[index].Item1,
                            Math.Pow(avaliableActions[index].Item2,pow) / secondNormSum);
                        thirdNormSum += avaliableActions[index].Item2;
                }
                else
                {
                    avaliableActions[index] = new Tuple<Action, double>(avaliableActions[index].Item1, avaliableActions[index].Item2 / secondNormSum);
                    thirdNormSum += avaliableActions[index].Item2;
                }
            }
            for (int j = 0; j < repeatedActions.Count; ++j)
            {
                repeatedActions[j] = new Tuple<string, int>(repeatedActions[j].Item1, repeatedActions[j].Item2 - 1);
                if (repeatedActions[j].Item2 == 0)
                    repeatedActions.RemoveAt(j);
            }
            for (int index = 0; index < avaliableActions.Count; ++index)
            {
                avaliableActions[index] = new Tuple<Action, double>(avaliableActions[index].Item1, avaliableActions[index].Item2 / thirdNormSum);
            }
        }
        else
            for (int index = 0; index < avaliableActions.Count; ++index)
            {
                avaliableActions[index] = new Tuple<Action, double>(avaliableActions[index].Item1, avaliableActions[index].Item2 / secondNormSum);
            }
    }
    public bool checkRepeated(Action act,int pow)
    {
        for (int j = 0; j < repeatedActions.Count; ++j)
        {
            if (act.name.Equals(repeatedActions[j].Item1))
            {
                pow = repeatedActions[j].Item2;
                return true;
            }
        }
        return false;
    }
    public void getActionsForThisPos (string pos, string handle, double[] appraisalsClown, double[] feelings) {
        foreach (var el in ActionsDataBase)
        {
            if (!el.type.Equals("_unattended"))
            {

                if (actionInSphere(el, appraisalsClown))
                {
                    if (el.type.Equals("other-direct") || el.type.Equals("prompted") || el.type.Equals("self-directed"))
                    {

                        if (el.target.type.Equals("-"))
                        {
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                        }
                        else if (el.target.pos1.Equals("not trash") && !WhiteStates.isTrash)
                        {
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                        }
                        if (el.target.type.Equals("window"))
                        {
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                        }
                    }
                    if (el.type.Equals("auxiliary"))
                    {
                        if (el.name.Equals("drop") && ObjectPlace.childCount > 0)
                        {
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                            continue;
                        }
                        if (el.name.Equals("shot") && ObjectPlace.childCount > 0 )
                        {
                            if (ObjectPlace.GetChild(0).name.Equals("shotgun"))
                            {
                                //obj = getResponseObj(el.name, appraisalsClown, feelings, false);
                                avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                                continue;
                            }
                        }
                        if (el.name.Equals("take") && ObjectPlace.childCount == 0)
                        {
                            //obj = getResponseObj(el.name, appraisalsClown, feelings, false);
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                            continue;
                        }
                        if (el.name.Equals("rip"))
                        {
                            //    obj = getResponseObj(el.name, appraisalsClown, feelings, true);
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                            continue;
                        }
                    if (el.name.Equals("walkto"))
                        {
                            //obj = getResponseObj(el.name, appraisalsClown, feelings, true);
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                            continue;
                        }
                    }
                    if (el.type.Equals("tool-object") && ObjectPlace.childCount > 0)
                    {
                        if(el.thing.type.Equals(ObjectPlace.GetChild(0).name))
                        {
                            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                        }
                    }
                }

                //else if (el.name.Equals("drop") && !handle.Equals("-"))
                //{
                //    avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                //}
                //else
                //if (pos.Equals(el.author.pos1) && actionInSphere(el, appraisalsClown))
                //{
                //    if (el.thing.type.Equals("inhands"))
                //    {
                //        if (checkActionWithObject(el, handle))
                //            avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                //    }
                //    else
                //    {
                //        avaliableActions.Add(new Tuple<Action, double>(el, 0.0));
                //    }
                //}
            }
        }
    }
    public Object getResponseObj(string actionName, double[] appraisals, double[] feelings, bool rip)
    {
        avaliableObjects.Clear();
        foreach (var el in ObjectsDataBase)
        {

            if(objectInSphere(el, appraisals) && GameObject.Find(el.name))
            {
                if (actionName.Equals("shot"))
                {
                    if (GameObject.Find(el.name).CompareTag("Alive"))
                    {
                        avaliableObjects.Add(new Tuple<Object, double>(el, 0.0));
                        continue;
                    }
                }
                if (rip)
                {
                    if (GameObject.Find(el.name).transform.parent == GameObject.Find("hanger").transform)
                    {
                        avaliableObjects.Add(new Tuple<Object, double>(el, 0.0));
                        continue;
                    }
                }
                if (actionName.Equals("take") && GameObject.Find(el.name).CompareTag("Hangble"))
                {
                    avaliableObjects.Add(new Tuple<Object, double>(el, 0.0));
                    continue;
                }
                if(actionName.Equals("walkto"))
                {
                    avaliableObjects.Add(new Tuple<Object, double>(el, 0.0));
                    continue;
                }
            }
        }
        getLikelihoodsObj(appraisals, feelings);
        var obj = getResponseObjectByLikelihood();
            return obj;
    }
    public void getLikelihoodsObj(double[] appraisals, double[] feelings)
    {
        double firstNormSum = 0;
        double secondNormSum = 0;
        List<double> huita = new List<double>();
        for (int index = 0; index < avaliableObjects.Count; ++index)
        {
            double distance = distanceBetweenVectors(feelings, getAppraisalsAfterAction(appraisals,
            avaliableObjects[index].Item1.Appraisals, avaliableObjects[index].Item1.w1));
            huita.Add(distance);
            firstNormSum += distance;
        }
        for (int index = 0; index < avaliableObjects.Count; ++index)
        {
            avaliableObjects[index] = new Tuple<Object, double>(avaliableObjects[index].Item1, 1 - huita[index] / firstNormSum);
            secondNormSum += avaliableObjects[index].Item2;
        }
        for (int index = 0; index < avaliableObjects.Count; ++index)
        {
            Object obj = avaliableObjects[index].Item1;
            avaliableObjects[index] = new Tuple<Object, double>(avaliableObjects[index].Item1, avaliableObjects[index].Item2 / secondNormSum);
        }
    }
    public Object getResponseObjectByLikelihood()
    {
        Object responseObject = new Object();
        avaliableObjects.Sort((x1, y1) => x1.Item2.CompareTo(y1.Item2));
        System.Random x = new System.Random();
        double objectLikelihood = Convert.ToDouble(x.Next(0, 10000) / 10000.0);
        double currentLikelihood = 0;
        foreach (var el in avaliableObjects)
        {
            currentLikelihood += el.Item2;
            if (currentLikelihood >= objectLikelihood)
            {
                responseObject = el.Item1;
                break;
            }
        }
        return responseObject;
    }
    public bool checkActionWithObject(Action act, string handle)
    {
        {
            if (act.thing.type.Equals(handle))
                return true;
            else
                return false;
        }
    }
    public double [] moralSchema(double [] actorFeelings, double[] actorAppraisals) {
        moralSchemaIsActive = turnOnMoralSchema(actorFeelings);
        if (!moralSchemaIsActive) {
            return firstMoralSchemaMode(actorAppraisals);
        }
        double currentDistanceBetweenAppraisalsAndFeelings = distanceBetweenVectors(actorAppraisals, actorFeelings);
        if (currentDistanceBetweenAppraisalsAndFeelings < DISTANCE_BETWEEN_APPRAISALS_AND_FEELINGS)
        {
            return secondMoralSchemaMode(actorFeelings);
        }
        return thirdMoralSchemaMode (actorAppraisals, actorFeelings);
    }

    public double[] firstMoralSchemaMode(double [] actorAppraisals) {
        double[] feelings = new double[3];
        for (int index = 0; index < this.feelings.Length; ++index) {
            feelings[index] = beta * actorAppraisals[index];
        }
        return feelings;
    }

    public double [] secondMoralSchemaMode(double [] actorFeelings) {
        double[] feelings = new double[3];
        double minDistance = 20.0;
        string newSecondClownCharacteristic = "";
        foreach (var el in feelingsStates) {
            double currentDistance = distanceBetweenVectors(el.Value.feelingState, actorFeelings);
            if (currentDistance < minDistance) {
                minDistance = currentDistance;
                newSecondClownCharacteristic = el.Key;
            }
        }
        feelingsStates[newSecondClownCharacteristic].feelingState.CopyTo(feelings, 0);
        secondClownCharacteristic = newSecondClownCharacteristic;
        return feelings;
    }

    public double [] thirdMoralSchemaMode(double[] actorAppraisals, double[] actorFeelings) {
        double[] feelings = new double[3];
        for (int index = 0; index < this.feelings.Length; ++index) {
            feelings[index] = (1 - r1) * actorFeelings[index] + r1 * (actorAppraisals[index] - actorFeelings[index]);
        }
        return feelings;
    }

    public bool turnOnMoralSchema (double[] feelings) {
        return DISTANCE_TURN_ON_MORAL_SCHEMA < distanceBetweenVectors(feelings, new double[3] { 0, 0, 0 });
    }

    public bool actionInSphere (Action action, double [] appraisalsClown) {
        return distanceBetweenVectors(appraisalsClown, action.vad.authorAppraisals) < SEMANTIC_DISTANCE;
    }
    public bool objectInSphere(Object obj, double[] appraisalsClown)
    {
        return distanceBetweenVectors(appraisalsClown, obj.Appraisals) < OBJECT_DISTANCE;
    }
    public double distanceBetweenVectors (double [] v1, double [] v2) {
        double distance = 0.0;
        for (int index = 0; index < v1.Length; ++index){
            distance += Math.Pow(v1[index] - v2[index], 2);
        }
        return Math.Sqrt(distance);
    }

    public double [] getAppraisalsAfterAction (double[] actorAppraisals, double [] action, double w) {
        double[] appraisals = new double[3];
        for (int index = 0; index < actorAppraisals.Length; ++index) {
            appraisals[index] = (1.0 - r * w) * actorAppraisals[index] + r * w * action[index];
        }
        return appraisals;
    }
}


public class FeelingState
{
    public double[] feelingState;

    FeelingState()
    {
        feelingState = new double[3];
    }
}
