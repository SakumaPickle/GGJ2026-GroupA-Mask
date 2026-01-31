using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TimelineAnimatorSwitch : MonoBehaviour
{
	// [SerializeField] private PlayableDirector _playableDirector;
	[SerializeField] private Animator _animator;
	[SerializeField] private PlayerInput _inputActions;

	public bool IsPlayDirector;

	private void Start()
	{
		// タイムライン駆動中は各種入力を切る
		// _inputActions.actions.Disable();
		// _playableDirector.stopped += DirectorStopped;
	}

	// タイムラインが停止したらアニメーター駆動に切り替える
	private void DirectorStopped(PlayableDirector director)
	{
		// _animator.Play("Idle Walk Run Blend");
		// IsPlayDirector = false;
		// _inputActions.actions.Enable();
	}
}
