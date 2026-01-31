using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
	[SerializeField] private Animator _animator;
	[SerializeField] private TimelineAnimatorSwitch _timelineAnimatorSwitch;

	public Animator Animator => _animator;

	public void PlayWalkAnimation(Vector3 velocity)
	{
		if (_timelineAnimatorSwitch.IsPlayDirector)
		{
			return;
		}

		var vel = velocity;
		vel.y = 0f;

		var speed = vel.magnitude;

		// 微小速度カット
		if (speed < 0.05f)
		{
			speed = 0f;
		}

		_animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
		_animator.speed = 1f;

		var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.IsName("JumpStart") && stateInfo.normalizedTime >= 1f)
		{
			SetIsJump(false);
		}
	}

	public void SetIsGrounded(bool isGrounded)
	{
		_animator.SetBool("Grounded", isGrounded);
	}

	public void SetIsJump(bool isJump)
	{
		_animator.SetBool("Jump", isJump);
	}
}
