using UnityEngine;

namespace Slime
{
	public class Entity : MonoBehaviour
	{
		protected new Rigidbody2D rigidbody;

		protected void Awake()
		{
			rigidbody = GetComponent<Rigidbody2D>();
		}
	}
}