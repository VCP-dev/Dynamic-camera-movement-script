using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameramovementscript : MonoBehaviour
{

    public Vector3 offset;

    GameObject player;    
    
    public Camera cam;
    Vector3 velocity;
    List<Transform> targets;
    float smoothTime;
    Vector3 origpos;

    float minZoom;
    float maxZoom;
    float zoomlimiter;

    // ------------- ratio for getting point between actual center and the player ------------
    float m; 
    float n;
    // ------------- ratio for getting point between actual center and the player ------------

    [HideInInspector]
    public static bool work;


    void Awake()
    {
        targets = new List<Transform>();
        origpos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        targets.Add(player.transform);
    }

    // Start is called before the first frame update
    void Start()
    {        

        // preset values, change as required
        minZoom = 61.12f;        //  minimum FOV
        maxZoom = 57f;          //  max FOV
        zoomlimiter = 0.23f;
        smoothTime = 0.23f;//0.2f;

        m=1.2f;            
        n=1.7f;            
        //                                    m                            n
        // (actual center generated) ------------------- (new pos) ----------------- (player position)


        // -------- toggle for cameramovement ----------
        work=true;
        // -------- toggle for cameramovement ----------
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {

        if(Input.GetKeyDown(KeyCode.P)){
            work = (work)?false:true;
        }

        if(!work){
            if(Vector3.Distance(transform.position,origpos)>0.01f){
                transform.position = Vector3.Lerp(transform.position,origpos,smoothTime);
            }
            return;    
        }

        if(targets.Count==0 || player==null)
            return;

        cameramovement();
        zoom();
    }

    void zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom,minZoom,GetGreatestDistance()/zoomlimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,newZoom,Time.deltaTime * 2.87f);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position,Vector3.zero);
        for(int i=0;i<targets.Count;i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }

    void cameramovement()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position,newPosition,ref velocity,smoothTime);
    }

    Vector3 GetCenterPoint()
    {
        if(targets.Count == 1)
        {
            return targets[0].position;
        }
            
        var bounds = new Bounds(targets[0].position,Vector3.zero);
        for(int i=0;i<targets.Count;i++){
            bounds.Encapsulate(targets[i].position);
        }

        // ----------------- gets point between actual center and the player. Change ratio as required or just comment out completely -----------------
        Vector3 pos = new Vector3((m*targets[0].position.x+n*bounds.center.x)/(m+n),bounds.center.y,(m*targets[0].position.z+n*bounds.center.z)/(m+n));//(bounds.center+targets[0].position)/2;
        // ----------------- gets point between actual center and the player. Change ratio as required or just comment out completely -----------------

        return pos;//bounds.center;    
    }

    public void addobject(GameObject obj)
    {
        targets.Add(obj.transform);
    }

    public void removeobject(GameObject obj)
    {
        targets.Remove(obj.transform);
    }
}
