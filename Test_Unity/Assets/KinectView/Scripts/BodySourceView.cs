using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Joint = Windows.Kinect.Joint;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    public GameObject mFaceObject;
    public Sprite peeze;
    public Sprite happyFace;
    public Sprite sadFace;

    public Material BoneMaterial;
    public Material BoneMaterial2;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();

    private Vector3 leftHandPos = new Vector3();
    private Vector3 rightHandPos = new Vector3();
    private bool sitTest = false;

    //list of joints that prefab gets connected to
    private List<Kinect.JointType> _joints = new List<Kinect.JointType> {
        Kinect.JointType.HandLeft,
        Kinect.JointType.HandRight,
        Kinect.JointType.Head,
        //Kinect.JointType.Head, -> can add this in to track head also!
    };

    //used to determine sitting position
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {

        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },


    };


    private Vector3 testHip = new Vector3(-0.8f, -2.6f, 0);
    private Vector3 testKnee = new Vector3(-3.5f, -4.4f, 0);

    


    void Update () 
    {
        FindHipAngle(testHip, testKnee);
        Kinect.Body[] data = mBodySourceManager.GetData();

        //check to see if kenect is returning data
        if (data == null)
            return;

        //all tracking ids that the kinect can see
        List<ulong> trackedIds = new List<ulong>();

        foreach (var body in data) {
            

            //check to see if data isnt a body
            if (body == null) 
                continue;

            //if body is currently being tracked add it to trackedIds list
            
            if (body.IsTracked) {
                trackedIds.Add(body.TrackingId);
            }
                    
            
            
        }

        //the keys of the bodies dictionary
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);

        foreach (ulong trackingId in knownIds) {

            //if trackedIds list does not contain the tracking id from know ids delete from scene
            if (!trackedIds.Contains(trackingId)) {

                Destroy(mBodies[trackingId]);

                mBodies.Remove(trackingId);
            }
        }

        int counter = 0;

        //create bodies
        foreach (var body in data) {

            //if no body, skip
            if (body == null)
                continue;

            //counter is ghetto fix to only have one body at a time would need fixing
            if (body.IsTracked && counter < 1) {
                
                //if body doesnt exist, create it
                if (!mBodies.ContainsKey(body.TrackingId)) {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                //update position
                UpdateBodyObject(body, mBodies[body.TrackingId]);
                counter++;
            }
        }
    
        
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        //create empty game object
        GameObject body = new GameObject("Body:" + id);

        
        foreach (Kinect.JointType joint in _joints)
        {



            //deals with singular joint
            if (joint.ToString() != "Head")
            {
                //create game object
                GameObject newJoint = Instantiate(mJointObject);
                newJoint.name = joint.ToString();
                //mJointObject.GetComponent<SpriteRenderer>().sprite = normalSprite;

                //parent body
                newJoint.transform.parent = body.transform;
            }
            if (joint.ToString() == "Head")
            {
                //create game object
                GameObject newJoint = Instantiate(mFaceObject);
                newJoint.name = joint.ToString();
                //mJointObject.GetComponent<SpriteRenderer>().sprite = normalSprite;

                //parent body
                newJoint.transform.parent = body.transform;
                
                
            }

        }

        //deals with bone
        for(Kinect.JointType jt = Kinect.JointType.HipLeft; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = BoneMaterial;
            lr.startWidth = (0.5f);
            lr.endWidth = (0.5f);
            lr.startColor = Color.blue;

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }
    
    private void UpdateBodyObject(Kinect.Body body, GameObject bodyObject)
    {

        for (Kinect.JointType jt = Kinect.JointType.HipLeft; jt <= Kinect.JointType.ThumbRight; jt++)
        {

            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            //IF JOINT IS IN BONEMAP ADD IT TO TARGET JOINT 
            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
                Debug.Log(sourceJoint.ToString());
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.startColor = Color.white; //(Color.grey,Color.grey);
                lr.endColor = Color.grey;

                float legAngle = FindHipAngle(GetVector3FromJoint(sourceJoint), GetVector3FromJoint(targetJoint.Value));
                //Debug.Log(legAngle);

                //-5 <= x <= 5 -> tollerence TOTEST
                if (legAngle >= -5 && legAngle <= 5) {
                    Debug.Log("sitting");
                    sitTest = true;
                    LineRenderer lr2 = GameObject.Find("KneeLeft").GetComponent<LineRenderer>();
                    lr2.material = BoneMaterial2;

                    //ghetto solution -> couldnt get the face to be not visible at start --> set face after sit
                    GameObject.Find("Head").GetComponentInChildren<SpriteRenderer>().sprite = sadFace;
                }
            }
            else
            {
                lr.enabled = false;
            }
        }
        if (sitTest) {
            //for face and hands (single joints)
            foreach (Kinect.JointType _joint in _joints)
            {

                //get new target position
                Joint sourceJoint = body.Joints[_joint];

                Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
                //all play happens on same plane
                targetPosition.z = 0;

                //get joint, set new postion
                Transform jointObject = bodyObject.transform.Find(_joint.ToString());
                jointObject.position = targetPosition;

                Joint selectedJoint = body.Joints[_joint];

                if (_joint.ToString() != "Head")
                {
                    if (_joint.ToString() == "HandLeft")
                    {
                        leftHandPos = GetVector3FromJoint(selectedJoint);
                        Debug.Log(leftHandPos);
                    }
                    else
                    {
                        rightHandPos = GetVector3FromJoint(selectedJoint);
                        Debug.Log(rightHandPos);
                    }
                }


            }
            if (Vector3.Distance(leftHandPos, rightHandPos) < 1)
            {
                //Debug.Log("-----------------");
                //Debug.Log(body);

                //Debug.Log(GameObject.Find("HandLeft"));
                //Debug.Log(GameObject.Find("HandRight"));

                GameObject.Find("HandLeft").GetComponent<SpriteRenderer>().sprite = peeze;
                GameObject.Find("HandRight").GetComponent<SpriteRenderer>().sprite = peeze;
                GameObject.Find("Head").GetComponentInChildren<SpriteRenderer>().sprite = happyFace;
                // I BLOODY DID IT!!!


                //turns off old hands
                //bodyObject.SetActive(false);

                //Sprite hands = bodyObject.GetComponentInChildren<SpriteRenderer>().sprite;

                //Debug.Log(bodyObject.GetComponentInChildren<SpriteRenderer>().sprite);

                //bodyObject.GetComponent<SpriteRenderer>().sprite = togetherSprite;
            }
        }
        
    }
        
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    private static float FindHipAngle(Vector3 hip, Vector3 knee) {

        //would this be x and y if person is sitting 90 degrees to connect?? TOTEST
        float rise = knee.x - hip.x;
        float run = knee.y - hip.y;

        float angle = Mathf.Atan(run / rise);

        return RadToDegrees(angle);
    }

    private static float RadToDegrees(float rad) {
        return rad * (180 / Mathf.PI);
    }
}
