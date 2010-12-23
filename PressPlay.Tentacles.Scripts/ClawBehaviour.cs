using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class ClawBehaviour: MonoBehaviour {
	
        //public AudioWrapper sndBounce;
        //public AudioWrapper sndConnection;
		
		protected GameObject grabbedObject;
	    protected float grabTime = 0;

        protected OnClawBehaviourConnect doOnClawBehaviourConnect;
		
	    protected RaycastHit rh;
        protected Microsoft.Xna.Framework.Ray ray;
	    protected Lemmy lemmy;
		
		public float eatTime = 0.5f;
		public float eatSpeed = 15;
		private Vector3 dist;
		private Vector3 dir;
		
        //protected RipableObject connectedObjectRipScript;
	    protected Vector3 connectionPosition;
	    protected Vector3 connectionNormal;
	    protected float connectionTime;
		protected Collider connectedCollider;
	    protected GameObject connectionPointerObject;
		
		protected Vector3 accumulatedTraversal = Vector3.zero;
		
	    protected Vector3 lastPosition;
		protected Vector3 traversedVector;
		protected float lastDeltaTime;
		
        //public PoolableObject createAtConnect;
        //public PoolableObject createAtSlipperyConnect;
        //public PoolableObject createAtShieldConnect;
	
        //public PoolableObject visualSoundAtConnect;
        //public PoolableObject visualSoundAtSlipperyConnect;
        //public PoolableObject visualSoundAtShieldConnect;
        //public PoolableObject createAtDisconnect;
		
		
		
		public bool isClawIdle{
	        get { return clawState == ClawStates.idle; }
		}
	
	    public bool isClawAttacking
	    {
	        get { return clawState == ClawStates.attacking; }
		}
	
		public bool isClawGrabbing
	    {
	        get { return clawState == ClawStates.grabbing; }
		}
		
		public bool isEating{
			get{ return clawState == ClawBehaviour.ClawStates.eating;}
		}
		
	    public bool isClawConnected
	    {
			get{
				//Debug.Log("CHECKING IF CLAW IS CONNECTED "+(state == States.connected).ToString());
	            return clawState == ClawStates.connected;
			}
		}
		
		public bool isClawDormant{
			get{return clawState == ClawStates.dormant;}
		}
		
		public enum ClawStates{
			idle,
			attacking,
			connected,
			grabbing,
			eating,
			dormant
	
		}
	
	    private ClawStates _clawState = ClawStates.idle;
	
	    protected ClawStates clawState
	    {
	        get { return _clawState; }
		}
		
		//Use this for initialization
		public override void Start () {
			lastPosition = transform.position;
			
			connectionPointerObject = new GameObject();
			connectionPointerObject.name = "connectionPointerObject";
			connectionPointerObject.active = false;
		}
		
		protected virtual void ChangeClawState(ClawStates newClawState)
		{
			//Debug.Log("CLAW STATE CHANGED TO : "+newClawState);
			_clawState = newClawState;
			
			switch(newClawState)
			{
			case ClawStates.dormant:
				transform.position = lemmy.transform.position;
				
				break;
			}
		}
		
		public virtual void GoDormant()
		{
			ChangeClawState(ClawStates.dormant);
		}
		public virtual void ExitDormant()
		{
			if (clawState == ClawBehaviour.ClawStates.dormant)
			{
				ChangeClawState(ClawStates.idle);
			}
		}
		
	    //// Update is called once per frame
        public override void FixedUpdate()
	    {
	        DoFixedUpdate();
	    }
	
	    protected virtual void DoFixedUpdate()
	    {
			
	    }
	
	    public override void Update()
	    {		
	        DoUpdate();
	    }
	
	    protected virtual void DoUpdate()
	    {
			traversedVector = transform.position - lastPosition;
			
			accumulatedTraversal *= 0.5f;
			accumulatedTraversal += traversedVector/Time.deltaTime;
			
			if (isClawDormant)
			{
				transform.position = lemmy.transform.position;
			}
			
			if (isClawConnected)
			{
	        	HandleConnection();
			}
			
			if (isClawGrabbing)
			{
	        	HandleGrabbedObject();
			}
			
			if (isClawAttacking)
			{
	        	RaycastForEnemies();
			}
			
			if (isEating)
			{
				HandleEating();
			}
	
			lastDeltaTime = Time.deltaTime;
			lastPosition = transform.position;
	    }
	
	    // Use this for initialization
	    public void Initialize(Lemmy _lemmy)
	    {
	        lemmy = _lemmy;
	    }
	
	    void HandleGrabbedObject()
	    {
	        if (grabbedObject != null)
	        {
                ////Debug.Log("HandleGrabbedObject : " + grabbedObject.name+" grabTime "+grabTime);
                if (Time.time - grabTime > lemmy.stats.grabPickupTime)
                {
                    ReleaseGrabbedObject();
                }
	        }
	    }
		
		void HandleEating()
		{
            dist = (transform.position-lemmy.transform.position);
            dir = dist.normalized;
			
            if (dist.magnitude < eatSpeed*Time.deltaTime)
            {
                transform.position = lemmy.transform.position;
                ReleaseGrabbedObject();
                //connectedObjectRipScript.Eat(eatTime, lemmy.transform);
                lemmy.mainBody.Chew();
                ChangeClawState(ClawStates.idle);
				
            }else
            {
                transform.position = transform.position - dir*eatSpeed*Time.deltaTime;
            }
			
		}
	
	    protected void HandleConnection()
	    {
	 
		
            rigidbody.velocity = Vector3.zero;
	        
	
            if (connectionPointerObject != null && connectionPointerObject.active)
            {
                transform.position = connectionPointerObject.transform.position;
                transform.rotation = connectionPointerObject.transform.rotation;
            }else
            {
                transform.position = connectionPosition;
                transform.LookAt(transform.position - connectionNormal);
            }
			
            if(connectionPointerObject != null && (connectionPointerObject.transform.parent == null || !connectionPointerObject.transform.parent.gameObject.active))
            {
                BreakConnection();
            }
			
	    }
	
	    protected void RaycastForEnemies()
	    {
            ray.Position = lastPosition;
            ray.Direction = traversedVector;


            bool hitWallLayer = Physics.Raycast(ray, out rh, traversedVector.magnitude * 2, GlobalSettings.Instance.allWallsAndShields);
            float wallDist = rh.distance;
            bool hitEnemyLayer = Physics.Raycast(ray, out rh, traversedVector.magnitude * 2, GlobalSettings.Instance.enemyLayer);
            float enemyDist = rh.distance;

            if ((hitWallLayer && !hitEnemyLayer) || (hitWallLayer && hitEnemyLayer && wallDist < enemyDist))
            {

                HitWall(rh.point, rh.normal, rh.collider);

            }
            else if ((!hitWallLayer && hitEnemyLayer) || (hitWallLayer && hitEnemyLayer && wallDist > enemyDist))
            {
                //EnergyCell energyCell = rh.collider.gameObject.GetComponent<EnergyCell>();
                //if (energyCell)
                //{
                //    HitEnergyCell(energyCell, rh.point, rh.normal);
                //    return;
                //}
                //else
                //{
                //    Debug.Log("NO ENERGY CELL FOUND!!! THIS IS BAD");
                //}


                //BasicBullet bulletScript = (BasicBullet)(rh.collider.gameObject.GetComponent(typeof(BasicBullet)));
                //if (bulletScript)
                //{
                //    HitBullet(bulletScript, rh.point);
                //    return;
                //}

                //RipableObject ripObj = (RipableObject)(rh.collider.gameObject.GetComponent(typeof(RipableObject)));
                //if (ripObj)
                //{
                //    ConnectToRipableObject(rh, ripObj);
                //    return;
                //}


            }
	    }
		
		void HitWall(Vector3 hitPosition, Vector3 hitNormal, Collider hitCollider)
		{
	        //Debug.Log("HIT WALL!!!");

            if (hitCollider != null && hitCollider.gameObject.layer == GlobalSettings.Instance.shieldLayer)
            {
                // Add bounce feedback here
                //if (createAtShieldConnect != null)
                //{
                //    ObjectPool.Instance.Draw(createAtShieldConnect, transform.position, transform.rotation);
                //}
            }
	
            //sndBounce.PlaySound();

            transform.position = rh.point;
            rigidbody.velocity = -rigidbody.velocity * 0.2f;
	        lastPosition = transform.position;
	        ChangeClawState(ClawStates.idle);
		}
	
        //void HitEnergyCell(EnergyCell cell, Vector3 hitPosition, Vector3 hitNormal)
        //{
	
        //    //Debug.Log("HitEnergyCell");
	
        //    rigidbody.velocity = Vector3.zero;
        //    transform.position = hitPosition;
	
        //    BasicEnemyHitLump lump = cell.GetClosestHitlump(hitPosition);
	
        //    transform.position = lump.transform.position;
	
        //    ConnectToRipableObject(lump.transform.position, hitNormal, lump);
	
        //    if (collider && cell.collider)
        //    {
        //        Physics.IgnoreCollision(collider, cell.collider);
        //    }
	
        //    //DoOnHitEnemy(cell, hitPosition, hitNormal);
        //}
	
        //void HitBullet(BasicBullet bullet, Vector3 hitPosition)
        //{
        //    rigidbody.velocity = Vector3.zero;
        //    transform.position = hitPosition;
        //    ChangeClawState(ClawStates.idle);
	
        //    //BasicEnemy enemyScript = (BasicEnemy)(enemy.GetComponent(typeof(BasicEnemy)));
        //    bullet.DoHitByClaw(this);
	
        //    //Debug.Log("CLAW HIT BULLET!!");
	
        //    DoOnHitBullet(bullet, hitPosition);
        //}
	
        //protected virtual void DoOnHitBullet(BasicBullet bullet, Vector3 hitPosition)
        //{
	
        //}
	
		
		protected void HandleRipableObject()
		{
            //if (connectedObjectRipScript == null)
            //{
            //    return;
            //}
		}
		
        //protected void ConnectToRipableObject(Vector3 _pos, Vector3 _normal, RipableObject ripObj)
        //{
        //    connectedObjectRipScript = ripObj;
        //    ripObj.ConnectToClaw(this);
        //    //state = States.attacking;
        //    ConnectToAtPosition(_pos, _normal, ripObj.gameObject);
        //}
	
        //protected void ConnectToRipableObject(RaycastHit rh, RipableObject ripObj)
        //{
        //    connectedObjectRipScript = ripObj;
        //    ripObj.ConnectToClaw(this);
        //    //state = States.attacking;
        //    ConnectToAtPosition(rh.point, rh.normal, ripObj.gameObject);
        //}
		
	    protected void ConnectToAtPosition(Vector3 hitPosition, Vector3 hitNormal, GameObject connectTo)
	    {

            connectedCollider = connectTo.collider;

            connectionNormal = (-rigidbody.velocity.normalized * 0 + hitNormal * 1).normalized;

            rigidbody.velocity = Vector3.zero;
            transform.position = hitPosition;
            connectionPosition = hitPosition;


            ChangeClawState(ClawStates.connected);
            connectionTime = Time.time;

            if (!connectionPointerObject)
            {
                connectionPointerObject = new GameObject();
                connectionPointerObject.name = "connectionPointerObject";
            }

            connectionPointerObject.active = true;
            connectionPointerObject.transform.position = hitPosition;
            //connectionPointerObject.transform.rotation = Quaternion.LookRotation(-hitNormal);
            connectionPointerObject.transform.parent = connectTo.transform;
			
            //sndConnection.PlaySound();
			
            //if (createAtConnect != null)
            //{
            //    ObjectPool.Instance.Draw(createAtConnect, transform.position, Quaternion.LookRotation(-connectionNormal));
				
            //}
	
            //if (visualSoundAtConnect != null)
            //{
            //    ObjectPool.Instance.Draw(visualSoundAtConnect, transform.position+(transform.forward/2), transform.rotation);
            //}

            doOnClawBehaviourConnect = connectTo.GetComponent<OnClawBehaviourConnect>();
            if (doOnClawBehaviourConnect != null)
            {
                doOnClawBehaviourConnect.DoOnClawBehaviourConnect(this, hitNormal);
            }
	    }
		
	    protected void ConnectAtPosition(Vector3 hitPosition, Vector3 hitNormal)
	    {
            if (!isClawAttacking)
            {
                //Debug.Log("CLAW is allready connected, or is idle");
                return;
            }

            if (connectionPointerObject)
            {
                //Destroy(connectionPointerObject);
                connectionPointerObject.active = false;// = null;
            }
	
            ////Debug.Log("Connecting tip at "+hitPosition);

            connectionNormal = (-rigidbody.velocity.normalized * 0 + hitNormal * 1) * 0.5f;

            rigidbody.velocity = Vector3.zero;
            transform.position = hitPosition;
            connectionPosition = hitPosition;


            ChangeClawState(ClawStates.connected);
            connectionTime = Time.time;
	    }
		
		public void Push(Vector3 _force)
		{
			if (!isClawConnected)
			{
                rigidbody.AddForce(_force);
			}
		}
		
		public void BreakConnection(float pushOfForce)
	    {
			if (isClawConnected)
			{
				BreakConnection();

                Push(-transform.forward * pushOfForce);
			}
		}
		
	    public void BreakConnection()
	    {
	        if (!isClawConnected)
	        {
	            //Debug.Log("Trying to break CLAW connection, but claw isn't connected");
	            return;
	        }
			
	        ChangeClawState(ClawStates.idle);
            //connectedObjectRipScript = null;
	
            //if (connectionPointerObject)
            //{
            //    Destroy(connectionPointerObject);
            //    connectionPointerObject = null;
            //}
	    }
	
		public void Reset()
	    {
            rigidbody.velocity = Vector3.zero;

            if (grabbedObject)
            {
                ReleaseGrabbedObject();
            }

            ChangeClawState(ClawStates.idle);

            //connectedObjectRipScript = null;

            DoOnReset();
	    }
	
	    public virtual void DoOnReset()
	    {
	
	    }
	
        //public void Eat(RipableObject _rippedObject)
        //{
        //    //Debug.Log("   -------------EAT!!! : "+_rippedObject.name);
			
        //    lemmy.mainBody.OpenMouth();
        //    Grab(_rippedObject.gameObject);
        //    connectedObjectRipScript = _rippedObject;
        //    ChangeClawState(ClawStates.eating);
        //}
		
	    public void Grab(GameObject _obj)
	    {
			if (_obj == grabbedObject)
			{
				return;
			}
			
	        //Debug.Log("   GRAB : " + _obj.name);
			
	        if (grabbedObject)
	        {
	            ReleaseGrabbedObject();
	        }
			
	        grabbedObject = _obj;
			
	        grabbedObject.transform.parent = transform;
	        grabTime = Time.time;
	
	        DoOnGrab(grabbedObject);
			
			ChangeClawState(ClawStates.grabbing);
	    }
	
	    public virtual void DoOnGrab(GameObject _obj)
	    {
	 
	    }
	
	    public void ReleaseGrabbedObject()
	    {
			//Debug.Log("ReleaseGrabbedObject : "+grabbedObject.name);
            DoOnReleaseGrabbedObject(grabbedObject);

            if (grabbedObject.tag == GlobalSettings.Instance.pickupTag)
            {
                lemmy.GetPickupGrab(grabbedObject);
            }

            grabbedObject = null;
			
			ChangeClawState(ClawStates.idle);
	    }
		
		public virtual void DoOnReleaseGrabbedObject(GameObject _grabbedObject)
	    {
	 
	    }
	}
}