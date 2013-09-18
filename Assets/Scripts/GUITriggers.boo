import UnityEngine

class GUITriggers (MonoBehaviour): 
	# Name of the GUI text to be modified
	public gui_to_disappear as string = "GUI Text"
	# New string value
	public new_text as string = ""
	public passed as int = 0

	def Start ():
		pass
	
	def Update ():
		pass

	def OnTriggerEnter(other as Collider) as void:	
		gui_object = GameObject.Find(gui_to_disappear)
		if new_text == "I remember rocks that need smashing...":
			Destroy(GameObject.Find("GUI Trigger 2"))	
		gui_object.guiText.text = new_text
		
