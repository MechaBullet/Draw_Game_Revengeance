private var timer = 0.0; 
var bobbingSpeed = 0.18; 
var bobbingAmount = 0.2; 
var midpoint = 1.0; 
 
function Update () 
{ 
	if (this.transform.root.networkView.isMine) {
		HeadBob();
	}
}

function HeadBob() {
	waveslice = 0.0; 
   horizontal = Input.GetAxis("Horizontal"); 
   vertical = Input.GetAxis("Vertical"); 
    
   if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) 
   { 
      timer = 0.0; 
   } 
    
   else 
   { 
   	  waveslice = Mathf.Sin(timer); 
      timer = timer + bobbingSpeed; 
       
      if (timer > Mathf.PI * 2) 
      { 
         timer = timer - (Mathf.PI * 2); 
      } 
    } 
    
    if (waveslice != 0) 
    { 
       translateChange = waveslice * bobbingAmount; 
       totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical); 
       totalAxes = Mathf.Clamp (totalAxes, 0.0, 1.0); 
       translateChange = totalAxes * translateChange; 
       transform.localPosition.x = midpoint + translateChange; 
    } 
    
    else 
    { 
       transform.localPosition.x = midpoint; 
    } 
}