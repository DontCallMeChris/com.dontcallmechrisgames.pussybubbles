using UnityEngine;
using System.Collections;

public class Stars : MonoBehaviour {
	public void SetStars(int i){
		int j = 0;
		foreach (UISprite sprite in GetComponentsInChildren<UISprite>()) {
			if(!sprite.cachedGameObject.activeSelf)
				sprite.cachedGameObject.SetActive(true);
			if( j < i)
				sprite.spriteName = "StarOn";
			else
				sprite.spriteName = "StarOff";
			j++;
		}
	}
}
