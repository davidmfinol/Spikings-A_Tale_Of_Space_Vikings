#pragma strict

var speed = 5.0;

function Start () {

}

function Update () {
  var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
  var z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
  var controller : CharacterController = GetComponent(CharacterController);
  var y = -10 * Time.deltaTime; var movement : Vector3 = Vector3(x, y, z);
  controller.Move(movement);
  //transform.Translate(x, y, 0);
}