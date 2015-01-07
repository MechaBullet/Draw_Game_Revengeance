// The speed the bullet moves 
   var Speed : float = .4; 
   
   // The number of seconds before the bullet is automatically destroyed 
   var SecondsUntilDestroy : float = 10; 
   var collided : boolean = false;
   
   private var startTime : float; 
   
   function Start () {
       startTime = Time.time;
   } 
   
   function FixedUpdate () {
       // Move forward 
       this.gameObject.transform.position += Speed * this.gameObject.transform.forward;
       
       // If the Bullet has existed as long as SecondsUntilDestroy, destroy it 
       if (Time.time - startTime >= SecondsUntilDestroy && !collided) {
           Destroy(this.gameObject);
       } 
   }
        
   function OnCollisionEnter(coll : Collision) {
        // Remove the Bullet from the world
        Destroy(this.gameObject);
       	//Destroy(this.gameObject);
       	collided = true;
   }