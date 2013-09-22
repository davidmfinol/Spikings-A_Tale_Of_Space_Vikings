#pragma strict

var speed = 5.0;

function Start () {

}

function Update () {
  var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
  var z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
  var controller : CharacterController = GetComponent(CharacterController);
  var movement : Vector3 = Vector3(x, 0, z);
  controller.Move(movement);
}