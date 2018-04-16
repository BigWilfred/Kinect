using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Joint = Windows.Kinect.Joint;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointLeft;
    public GameObject mJointRight;

    public Material BoneMaterial;

    //still need this to store trackingIds
    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();

    private Vector3 leftHandPos = new Vector3();
    private Vector3 rightHandPos = new Vector3();

    private bool sitTest = false;
    private bool skelTest = false;

    //HELP! should this be saved or just use the first item in tracked.
    private ulong trackedId1 = new ulong();

    //list of joints that prefab gets connected to
    private List<Kinect.JointType> _joints = new List<Kinect.JointType> {
        Kinect.JointType.HandLeft,
        Kinect.JointType.HandRight,

        //Kinect.JointType.Head, -> can add this in to track head also!
    };

    //used to determine sitting position
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {

        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },


    };


    void Update () 
    {
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

                //reset bool tests
                resetTest();
            }
        }

        int counter = 0;

        //create bodies
        foreach (var body in data) {

            //if no body, skip
            if (body == null)
                continue;

            //counter is ghetto fix to only have one body at a time would need fixing
            //body.TrackingId == trackedIds[0] ie/ if the bodys tracking ID is the first body that was tracked, creater and update it
            if (body.IsTracked && body.TrackingId == trackedIds[0]) {
                //if body doesnt exist, create it
                if (!mBodies.ContainsKey(body.TrackingId)) {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }


                UpdateBodyObject(body, mBodies[body.TrackingId]);
                //update position

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
            //create game object for hands
            if (joint.ToString() == "HandLeft") {
                GameObject newJoint = Instantiate(mJointLeft);
                newJoint.name = joint.ToString();
                newJoint.transform.parent = body.transform;
            }

            if(joint.ToString() == "HandRight") {
                GameObject newJoint = Instantiate(mJointRight);
                newJoint.name = joint.ToString();
                newJoint.transform.parent = body.transform;
            }
            //newJoint = Instantiate(mJointObject);
            //newJoint.name = joint.ToString();

            //parent body
            //newJoint.transform.parent = body.transform;
        }

        //GameObject.Find("HandRight").SetActive(false);

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
        if (!sitTest) {
            for (Kinect.JointType jt = Kinect.JointType.HipLeft; jt <= Kinect.JointType.ThumbRight; jt++)
            {

                Kinect.Joint sourceJoint = body.Joints[jt];
                Kinect.Joint? targetJoint = null;

                //IF JOINT IS IN BONEMAP ADD IT TO TARGET JOINT 
                if (_BoneMap.ContainsKey(jt))
                {
                    targetJoint = body.Joints[_BoneMap[jt]];
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
                    if (legAngle >= -5 && legAngle <= 5)
                    {
                        Debug.Log("sitting");
                        sitTest = true;
                        

                        //ghetto solution -> couldnt get the face to be not visible at start --> set face after sit

                    }
                }
                else
                {
                    lr.enabled = false;
                }
            }
        }
        
        if (sitTest) {
            
            if (!skelTest) {
                //iterate over all the joints that arent hands and remover them
                for (Kinect.JointType jt = Kinect.JointType.HipLeft; jt <= Kinect.JointType.ThumbRight; jt++)
                {
                    if (jt != Kinect.JointType.HandRight || jt != Kinect.JointType.HandLeft)
                    {
                        GameObject.Find(jt.ToString()).SetActive(false);
                    }

                }
                skelTest = true;
            }
            

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
                    }
                    else
                    {
                        rightHandPos = GetVector3FromJoint(selectedJoint);
                        
                        
                    }
                }

                //0.5 is radius
                if (leftHandPos.x <= -4.5 && leftHandPos.x >= -5.5) {
                    //Debug.Log(rightHandPos.y);
                }

            }
           
        }
        
    }
        
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, 0.0f);
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

    private void resetTest() {
        sitTest = false;
        skelTest = false;
    }
}
