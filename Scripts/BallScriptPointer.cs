using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallScriptPointer
{
	GameObject gameobject;

	private SpriteRenderer spriteRenderer;
	private SpriteRenderer spriteRendererChild;

	BallScript ball;
	Transform transform;
	List<JointRef> joints;
	public BALLTYPE color { get; private set;}

	public BallScriptPointer (BallScript ball)
	{
		this.gameobject = ball.gameObject;
		this.ball = ball;
		this.transform = ball.gameObject.transform;
		this.joints = new List<JointRef> ();
	}
	
	BallScriptPointer (GameObject gameobject, BallScript ball, Transform transform)
	{
		this.gameobject = gameobject;
		this.ball = ball;
		this.transform = transform;
		this.joints = new List<JointRef> ();
	}


	public void Clear(){
		ClearChild ();
    }

	private SpriteRenderer GetSprite ()
	{
		if (spriteRenderer == null) {
			spriteRenderer = gameobject.GetComponent<SpriteRenderer> ();
		}
        return spriteRenderer;
    }

	private void AddChild ()
	{
		GameObject child = new GameObject ();
		child.transform.parent = this.transform;
		child.transform.localPosition = Vector3.zero;
		child.name = "Child" + child.transform.position.ToString ();
		spriteRendererChild = child.AddComponent<SpriteRenderer> ();
	}
	
	public SpriteRenderer GetChildrenSprite ()
	{
		if (spriteRendererChild == null)
			AddChild ();
		spriteRendererChild.enabled = true;
		return spriteRendererChild;
	}
	
	public void ClearChild ()
    {
		if (spriteRendererChild != null)
			spriteRendererChild.enabled = false;
    }

	BALLTYPE GetRandomBalltype(bool onlycolor = true)
	{
		if(onlycolor)
			return (BALLTYPE) Random.Range(0, (int)BALLTYPE.LASTCOLOR);
		else
			return (BALLTYPE) Random.Range(0, (int)BALLTYPE.LASTONE);
    }
    
    public void setColor(BALLTYPE color)
	{
		if (color == BALLTYPE.LASTCOLOR)
			color = GetRandomBalltype ();
		this.color = color;

		ClearChild ();

		if (ball.armor > 0)
			GetChildrenSprite ().sprite = GameSetting.Instance.spriteArmor;
		
		if (color == BALLTYPE.BOMB){
			GetSprite().sprite = GameSetting.Instance.spriteBomb;

			if(ToolTipManager.Instance.IsNeedToBeToolTiped(enumToolTipsList.Bomb)){
				ToolTipObject tt = gameobject.AddComponent<ToolTipObject>();
				tt.tt = enumToolTipsList.Bomb;
			}
		}
		else if (color == BALLTYPE.MOVEDOWN){
			GetSprite().sprite = GameSetting.Instance.spriteMoveDown;

			if(ToolTipManager.Instance.IsNeedToBeToolTiped(enumToolTipsList.MoveDown)){
				ToolTipObject tt = gameobject.AddComponent<ToolTipObject>();
				tt.tt = enumToolTipsList.MoveDown;
			}
		}
		else if (ball.isbasic)
			GetSprite().sprite = GameSetting.Instance.spriteBasic;
		else
			GetSprite().sprite = GameSetting.Instance.spriteMain;


		GetSprite().color = IGame.Instance.get_color (color);
    }

	public void setColor(int color)
	{
		setColor((BALLTYPE)color);
	}
	
	public void Move(Vector3 v3)
	{
		gameobject.GetComponent<Rigidbody2D>().MovePosition (getPosition() + v3);
	}
	
	public GameObject getGameObject()
	{
		return gameobject;
	}
	
	public Vector3 getPosition()
	{
		return transform.position;
	}

	public void setPosition(Vector3 pos)
	{
		transform.position = pos;
	}
	
	public BallScript getBall()
	{
		return ball;
	}

	public JointRef AddJoint(SpringJoint2D joint)
	{
		JointRef jointref = new JointRef (this, joint);
		joints.Add (jointref);
		if (this.GetJointsCount () == 6) {
			ball.GetComponent<Rigidbody2D>().Sleep();
		}
		return jointref;
	}

	public JointRef AddJoint(SpringJoint2D joint, BallScriptPointer connectedBall)
	{
		JointRef jointref = new JointRef (this, joint, connectedBall);
		joints.Add (jointref);
		return jointref;
	}

	public void RemoveAllJoints()
	{
		RemoveJoints ();
		joints.Clear ();
	}

	public void RemoveJoints()
	{
		for (int i = 0; i < joints.Count; i++) {
			if(joints[i].deletemyself){
				RemoveJoint(joints[i]);
				i = 0;
			}
		}
	}

	public void RemoveJoint(JointRef jointref)
	{
		joints.Remove (jointref);
	}

	public List<JointRef> GetJoints()
	{
		return joints;
	}

	public int GetJointsCount()
	{
		int result = 0;
		foreach (JointRef joint in joints)
			if (!joint.deletemyself)
				result++;
		return result;
    }
}

public class JointRef
{
	public BallScriptPointer ball, connectedBall;
	public SpringJoint2D joint;
	public bool deletemyself;

	public JointRef(BallScriptPointer ball, SpringJoint2D joint){
		this.deletemyself = false;
		this.ball = ball;
		this.joint = joint;
		this.connectedBall = (joint.connectedBody.gameObject.GetComponent<BallScript>()).BallScriptPtr;
	}

	public JointRef(BallScriptPointer ball, SpringJoint2D joint, BallScriptPointer connectedBall){
		this.deletemyself = false;
		this.ball = ball;
		this.joint = joint;
		this.connectedBall = connectedBall;
	}

	public void Remove(){
		SpringJoint2D.Destroy (joint);
		deletemyself = true;
	}
}